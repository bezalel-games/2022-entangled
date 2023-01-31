using System.Collections;
using Cards;
using Managers;
using Rooms;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Effects
{
    public class PostProcessingAgent : MonoBehaviour
    {
        #region Serialized Fields

        [Header("Vignette")]
        [SerializeField] private float _vignetteTransitionSpeed = 3f;

        [Header("Blur")]
        [SerializeField] private float _blurTransitionSpeed = 3f;

        [SerializeField] private float _focusDistanceAtMaxBlur = 1.5f;

        [Header("Intensity")]
        [SerializeField] private float _intensityTransitionSpeed = 3f;

        [SerializeField] private AnimationCurve _intensityEffectGraph;
        [SerializeField] private float _maxFilmGrainIntensity = 1;
        [SerializeField] private float _minSplitToningBalance = -100;
        [SerializeField] private float _maxChannelMixerRedOutGreenIn = 200;
        [SerializeField] private bool _intensityChangesChannelMixer;

        #endregion

        #region Non-Serialized Fields

        private Vignette _vignette;
        private float _baseSmoothness;
        private Coroutine _vignetteEffectCoroutine;

        private DepthOfField _depthOfField;
        private float _baseFocusDistance;
        private Coroutine _blurEffectCoroutine;

        private FilmGrain _filmGrain;
        private float _baseFilmGrainIntensity;
        private SplitToning _splitToning;
        private float _baseSplitToningBalance;
        private ChannelMixer _channelMixer;
        private float _baseChannelMixerRedOutGreenIn;
        private Coroutine _intensityEffectCoroutine;
        private float _intensity;

        private float Intensity
        {
            get => _intensity;
            set
            {
                var t = _intensityEffectGraph.Evaluate(value);
                _filmGrain.intensity.value = Mathf.Lerp(_baseFilmGrainIntensity, _maxFilmGrainIntensity, t);
                _splitToning.balance.value = Mathf.Lerp(_baseSplitToningBalance, _minSplitToningBalance, t);
                if (_intensityChangesChannelMixer)
                    _channelMixer.redOutGreenIn.value = Mathf.Lerp(_baseChannelMixerRedOutGreenIn, _maxChannelMixerRedOutGreenIn, t);
            }
        }

        #endregion

        #region Function Events

        private void Awake()
        {
            var volume = GetComponent<Volume>();
            volume.profile.TryGet(out _vignette);
            _baseSmoothness = _vignette.smoothness.value;
            volume.profile.TryGet(out _depthOfField);
            _baseFocusDistance = _depthOfField.focusDistance.value;
            volume.profile.TryGet(out _filmGrain);
            _baseFilmGrainIntensity = _filmGrain.intensity.value;
            volume.profile.TryGet(out _splitToning);
            _baseSplitToningBalance = _splitToning.balance.value;
            volume.profile.TryGet(out _channelMixer);
            _baseChannelMixerRedOutGreenIn = _channelMixer.redOutGreenIn.value;
        }

        private void OnEnable()
        {
            GameManager.TimeScaleChanged += ChangeVignetteByTimeScale;
            CardManager.StartedChoosingCards += IncreaseBlur;
            CardManager.FinishedChoosingCards += DecreaseBlur;
            RoomManager.RoomChanged += ChangeRoomIntensity;
        }

        private void OnDisable()
        {
            GameManager.TimeScaleChanged -= ChangeVignetteByTimeScale;
            CardManager.StartedChoosingCards -= IncreaseBlur;
            CardManager.FinishedChoosingCards -= DecreaseBlur;
            RoomManager.RoomChanged -= ChangeRoomIntensity;
        }

        #endregion

        #region Private Methods

        private void ChangeRoomIntensity(float intensity)
        {
            if (_intensityEffectCoroutine != null)
                StopCoroutine(_intensityEffectCoroutine);
            intensity = Mathf.Clamp01(intensity);
            _intensityEffectCoroutine = StartCoroutine(intensity > Intensity
                ? DecreaseIntensity(intensity)
                : IncreaseIntensity(intensity));
        }

        private void IncreaseBlur(Card left, Card right, bool leftIsDeckCard)
        {
            if (_blurEffectCoroutine != null)
                StopCoroutine(_blurEffectCoroutine);
            _blurEffectCoroutine = StartCoroutine(DecreaseFocusDistanceCoroutine(_focusDistanceAtMaxBlur));
        }

        private void DecreaseBlur()
        {
            if (_blurEffectCoroutine != null)
                StopCoroutine(_blurEffectCoroutine);
            _blurEffectCoroutine = StartCoroutine(IncreaseFocusDistanceCoroutine(_baseFocusDistance));
        }

        private void ChangeVignetteByTimeScale(float timeScale)
        {
            var targetSmoothness = _baseSmoothness + (1 - timeScale) * (1 - _baseSmoothness);
            if (_vignetteEffectCoroutine != null)
                StopCoroutine(_vignetteEffectCoroutine);
            _vignetteEffectCoroutine = StartCoroutine(targetSmoothness > _baseSmoothness
                ? IncreaseVignetteSmoothness(targetSmoothness)
                : DecreaseVignetteSmoothness(targetSmoothness));
        }

        #endregion

        #region Coroutines

        private IEnumerator IncreaseIntensity(float target)
        {
            float diff = target - Intensity;
            while (Intensity < target)
            {
                yield return null;
                Intensity += Time.deltaTime * diff * _intensityTransitionSpeed;
            }

            Intensity = target;
            _intensityEffectCoroutine = null;
        }

        private IEnumerator DecreaseIntensity(float target)
        {
            float diff = Intensity - target;
            while (Intensity > target)
            {
                yield return null;
                Intensity -= Time.deltaTime * diff * _intensityTransitionSpeed;
            }

            Intensity = target;
            _intensityEffectCoroutine = null;
        }

        private IEnumerator IncreaseFocusDistanceCoroutine(float target)
        {
            float value;
            float diff = target - _depthOfField.focusDistance.value;
            while ((value = _depthOfField.focusDistance.value) < target)
            {
                yield return null;
                _depthOfField.focusDistance.value = value + Time.deltaTime * diff * _blurTransitionSpeed;
            }

            _depthOfField.focusDistance.value = target;
            _blurEffectCoroutine = null;
        }

        private IEnumerator DecreaseFocusDistanceCoroutine(float target)
        {
            target = Mathf.Max(0, target);
            float value;
            float diff = _depthOfField.focusDistance.value - target;
            while ((value = _depthOfField.focusDistance.value) < target)
            {
                yield return null;
                _depthOfField.focusDistance.value = value - Time.deltaTime * diff * _blurTransitionSpeed;
            }

            _depthOfField.focusDistance.value = target;
            _blurEffectCoroutine = null;
        }

        private IEnumerator IncreaseVignetteSmoothness(float target)
        {
            target = Mathf.Min(target, 1);
            float value;
            float diff = target - _baseSmoothness;
            while ((value = _vignette.smoothness.value) < target)
            {
                yield return null;
                _vignette.smoothness.value = value + Time.deltaTime * diff * _vignetteTransitionSpeed;
            }

            _vignette.smoothness.value = target;
            _vignetteEffectCoroutine = null;
        }

        private IEnumerator DecreaseVignetteSmoothness(float target)
        {
            float value;
            float diff = _vignette.smoothness.value - target;
            while ((value = _vignette.smoothness.value) > target)
            {
                yield return null;
                _vignette.smoothness.value = value - Time.deltaTime * diff * _vignetteTransitionSpeed;
            }

            _vignette.smoothness.value = target;
            _vignetteEffectCoroutine = null;
        }

        #endregion
    }
}