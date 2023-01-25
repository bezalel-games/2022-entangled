using System.Collections;
using Cards;
using Managers;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Effects
{
    public class PostProcessingAgent : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] private float _vignetteTransitionSpeed = 3f;
        [SerializeField] private float _blurTransitionSpeed = 3f;
        [SerializeField] private float _focusDistanceAtMaxBlur = 1.5f;

        #endregion

        #region Non-Serialized Fields

        private Vignette _vignette;
        private float _baseSmoothness;
        private Coroutine _vignetteEffectCoroutine;

        private DepthOfField _depthOfField;
        private float _baseFocusDistance;
        private Coroutine _blurEffectCoroutine;

        #endregion

        #region Function Events

        private void Awake()
        {
            var volume = GetComponent<Volume>();
            volume.profile.TryGet(out _vignette);
            _baseSmoothness = _vignette.smoothness.value;
            volume.profile.TryGet(out _depthOfField);
            _baseFocusDistance = _depthOfField.focusDistance.value;
        }

        private void OnEnable()
        {
            GameManager.TimeScaleChanged += ChangeVignetteByTimeScale;
            CardManager.StartedChoosingCards += IncreaseBlur;
            CardManager.FinishedChoosingCards += DecreaseBlur;
        }

        private void OnDisable()
        {
            GameManager.TimeScaleChanged -= ChangeVignetteByTimeScale;
            CardManager.StartedChoosingCards -= IncreaseBlur;
            CardManager.FinishedChoosingCards -= DecreaseBlur;
        }

        #endregion

        #region Private Methods

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