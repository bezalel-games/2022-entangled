using HP_System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class FillMaterial : MonoBehaviour
    {
        #region Serialized Fields
        
          [SerializeField] private Image _hpImage;
          [SerializeField] private Image _mpImage;
          [SerializeField] LivingBehaviour _living;
          private static readonly int MaxValue = Shader.PropertyToID("_Max_Value");
          private static readonly int Value = Shader.PropertyToID("_Value");

          #endregion
        
          #region Non-Serialized Fields

          private Material _hpMaterial;
          private Material _mpMaterial;
          
          #endregion
        
          #region Properties
        
          #endregion
        
          #region Function Events
        
          private void Awake()
          {
            if (_hpImage != null)
              _hpMaterial = _hpImage.material;
            
            if (_mpImage != null)
              _mpMaterial = _mpImage.material;
            
            UpdateHPUI(_living.Hp, _living.MaxHp);
            UpdateMPUI(_living.Mp, _living.MaxMp);
            
            
            if (_living != null)
            {
              _living.OnHpChange += UpdateHPUI;
              _living.OnMpChange += UpdateMPUI;
            }
          }
        
          private void OnDestroy()
          {
            if (_living != null)
            {
              _living.OnHpChange -= UpdateHPUI;
              _living.OnMpChange -= UpdateMPUI;
            }
          }
        
          #endregion
        
          #region Public Methods
        
          #endregion
        
          #region Private Methods
          
          protected virtual void UpdateHPUI(float hp, float maxHp)
          {
            if (_hpMaterial == null) return;
        
            _hpMaterial.SetFloat(Value, hp);
            _hpMaterial.SetFloat(MaxValue, maxHp);
          }

          protected virtual void UpdateMPUI(float mp, float maxMp)
          {
            if (_mpMaterial == null) return;

            _mpMaterial.SetFloat(Value, mp);
            _mpMaterial.SetFloat(MaxValue, maxMp);
          }

          #endregion
    }
}