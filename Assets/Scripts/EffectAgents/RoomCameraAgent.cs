using System;
using System.Collections;
using Cinemachine;
using Managers;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace EffectAgents
{
    public class RoomCameraAgent : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] private float _transitionSpeed = 0.5f;
        [SerializeField] private float _maxOrthoSizeReduction = 0.2f;

        #endregion

        #region Non-Serialized Fields

        private CinemachineVirtualCamera _vCam;
        private float _baseOrthoSize;
        private Coroutine _effectCoroutine;

        #endregion

        #region Function Events

        private void Awake()
        {
            _vCam = GetComponent<CinemachineVirtualCamera>();
            _baseOrthoSize = _vCam.m_Lens.OrthographicSize;
        }

        private void OnEnable()
        {
            GameManager.TimeScaleChanged += ChangeOrthoSizeByTimeScale;
        }

        private void OnDisable()
        {
            GameManager.TimeScaleChanged -= ChangeOrthoSizeByTimeScale;
        }

        #endregion

        #region Private Methods

        private void ChangeOrthoSizeByTimeScale(float timeScale)
        {
            var targetOrthoSize = _baseOrthoSize - (1 - timeScale) * _maxOrthoSizeReduction;
            if (_effectCoroutine != null)
                StopCoroutine(_effectCoroutine);
            _effectCoroutine = StartCoroutine(targetOrthoSize > _baseOrthoSize
                ? IncreaseOrthoSize(targetOrthoSize)
                : DecreaseOrthoSize(targetOrthoSize));
        }

        private IEnumerator IncreaseOrthoSize(float target)
        {
            target = Mathf.Min(target, 1);
            float value;
            float diff = target - _vCam.m_Lens.OrthographicSize;
            while ((value = _vCam.m_Lens.OrthographicSize) < target)
            {
                yield return null;
                _vCam.m_Lens.OrthographicSize = value + Time.deltaTime * diff * _transitionSpeed;
            }

            _vCam.m_Lens.OrthographicSize = target;
            _effectCoroutine = null;
        }

        private IEnumerator DecreaseOrthoSize(float target)
        {
            float value;
            float diff = _baseOrthoSize - target;
            while ((value = _vCam.m_Lens.OrthographicSize) > target)
            {
                yield return null;
                _vCam.m_Lens.OrthographicSize = value - Time.deltaTime * diff * _transitionSpeed;
            }

            _vCam.m_Lens.OrthographicSize = target;
            _effectCoroutine = null;
        }

        #endregion
    }
}