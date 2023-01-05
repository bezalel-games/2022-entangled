using System.Collections;
using Managers;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace EffectAgents
{
    public class LightingAgent : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] private float _transitionSpeed = 3;
        [SerializeField] private float _maxIntensityAddition = 0.1f;
        [SerializeField] private float _maxRadiusAddition = 5f;

        #endregion

        #region Non-Serialized Fields

        private Light2D _light;
        private float _baseIntensity;
        private float _baseRadius;
        private Coroutine _effectCoroutine;

        #endregion

        #region Function Events

        private void Awake()
        {
            _light = GetComponent<Light2D>();
            _baseIntensity = _light.intensity;
            _baseRadius = _light.pointLightOuterRadius;
        }

        private void OnEnable()
        {
            GameManager.TimeScaleChanged += ChangeGlobalLightingByTimeScale;
        }

        private void OnDisable()
        {
            GameManager.TimeScaleChanged -= ChangeGlobalLightingByTimeScale;
        }

        #endregion

        #region Private Methods

        private void ChangeGlobalLightingByTimeScale(float timeScale)
        {
            var targetIntensity = _baseIntensity + (1 - timeScale) * _maxIntensityAddition;
            var targetRadius = _baseRadius + (1 - timeScale) * _maxRadiusAddition;
            if (_effectCoroutine != null)
                StopCoroutine(_effectCoroutine);
            _effectCoroutine = StartCoroutine(targetIntensity > _baseIntensity
                ? IncreaseLight(targetIntensity, targetRadius)
                : DecreaseLight(targetIntensity, targetRadius));
        }

        private IEnumerator IncreaseLight(float targetIntensity, float targetRadius)
        {
            float value;
            float intensityDiff = targetIntensity - _baseIntensity;
            float radiusDiff = targetRadius - _baseRadius;
            while ((value = _light.intensity) < targetIntensity)
            {
                yield return null;
                _light.intensity = value + Time.deltaTime * intensityDiff * _transitionSpeed;
                _light.pointLightOuterRadius += Time.deltaTime * radiusDiff * _transitionSpeed;
            }

            _light.intensity = targetIntensity;
            _light.pointLightOuterRadius = targetRadius;
            _effectCoroutine = null;
        }

        private IEnumerator DecreaseLight(float targetIntensity, float targetRadius)
        {
            float value;
            float intensityDiff = _light.intensity - targetIntensity;
            float radiusDiff = _light.pointLightOuterRadius - targetRadius;
            while ((value = _light.intensity) > targetIntensity)
            {
                yield return null;
                _light.intensity = value - Time.deltaTime * intensityDiff * _transitionSpeed;
                _light.pointLightOuterRadius -= Time.deltaTime * radiusDiff * _transitionSpeed;
            }

            _light.intensity = targetIntensity;
            _light.pointLightOuterRadius = targetRadius;
            _effectCoroutine = null;
        }

        #endregion
    }
}