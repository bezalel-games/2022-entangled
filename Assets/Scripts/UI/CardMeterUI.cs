using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CardMeterUI : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] [Range(0, 1)] private float _min = 0;
        [SerializeField] [Range(0, 1)] private float _max = 1;
        [SerializeField] private float _fillSpeed = 3;
        [SerializeField] private string _fillPropertyName = "_Fill";

        #endregion

        #region Non-Serialized Fields

        private Material _material;
        private float _targetFill;
        private float _currentFill;

        #endregion

        #region Function Events

        private void Awake()
        {
            _material = GetComponent<Image>().material;
            _material.SetFloat(_fillPropertyName, _min);
            GameManager.NextCardProgressionUpdated += UpdateFill;
        }

        private void OnDestroy()
        {
            _material.SetFloat(_fillPropertyName, 0);
            GameManager.NextCardProgressionUpdated -= UpdateFill;
        }

        private void Update()
        {
            if (_targetFill == 0)
            {
                _currentFill = Mathf.Max(_currentFill - Time.deltaTime * _fillSpeed, 0);
                _material.SetFloat(_fillPropertyName, _currentFill);
            }
            if (_currentFill >= _targetFill) return;
            _currentFill = Mathf.Min(_currentFill + Time.deltaTime * _fillSpeed, _targetFill);
            _material.SetFloat(_fillPropertyName, _currentFill);
        }

        #endregion

        #region Private Methods

        private void UpdateFill(float fill)
        {
            print(fill);
            _targetFill = _min + (_max - _min) * fill;
        }

        #endregion
    }
}