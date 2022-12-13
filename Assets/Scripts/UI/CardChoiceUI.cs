using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class CardChoiceUI : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] private GameObject _firstSelectedCard;

        #endregion

        #region Non-Serialized Fields

        #endregion

        #region Properties

        #endregion

        #region Function Events

        private void OnEnable()
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(_firstSelectedCard);
        }

        #endregion

        #region Public Methods

        #endregion

        #region Private Methods

        #endregion
    }
}