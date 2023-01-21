using System.Collections;
using Managers;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Effects
{
    public class PostProcessingAgent : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] private float _transitionSpeed = 0.5f;

        #endregion

        #region Non-Serialized Fields

        private Vignette _vignette;
        private float _baseSmoothness;
        private Coroutine _effectCoroutine;

        #endregion

        #region Function Events

        private void Awake()
        {
            GetComponent<Volume>()?.profile.TryGet<Vignette>(out _vignette);
            _baseSmoothness = _vignette.smoothness.value;
        }

        private void OnEnable()
        {
            GameManager.TimeScaleChanged += ChangeVignetteByTimeScale;
        }

        private void OnDisable()
        {
            GameManager.TimeScaleChanged -= ChangeVignetteByTimeScale;
        }

        #endregion

        #region Private Methods

        private void ChangeVignetteByTimeScale(float timeScale)
        {
            var targetSmoothness = _baseSmoothness + (1 - timeScale) * (1 - _baseSmoothness);
            if (_effectCoroutine != null)
                StopCoroutine(_effectCoroutine);
            _effectCoroutine = StartCoroutine(targetSmoothness > _baseSmoothness
                ? IncreaseVignetteSmoothness(targetSmoothness)
                : DecreaseVignetteSmoothness(targetSmoothness));
        }

        private IEnumerator IncreaseVignetteSmoothness(float target)
        {
            target = Mathf.Min(target, 1);
            float value;
            float diff = target - _baseSmoothness;
            while ((value = _vignette.smoothness.value) < target)
            {
                yield return null;
                _vignette.smoothness.value = value + Time.deltaTime * diff * _transitionSpeed;
            }

            _vignette.smoothness.value = target;
            _effectCoroutine = null;
        }

        private IEnumerator DecreaseVignetteSmoothness(float target)
        {
            float value;
            float diff = _vignette.smoothness.value - target;
            while ((value = _vignette.smoothness.value) > target)
            {
                yield return null;
                _vignette.smoothness.value = value - Time.deltaTime * diff * _transitionSpeed;
            }

            _vignette.smoothness.value = target;
            _effectCoroutine = null;
        }

        #endregion
    }
}