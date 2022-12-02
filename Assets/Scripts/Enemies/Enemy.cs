using UnityEngine;

namespace Enemies
{
    public class Enemy : MonoBehaviour
    {
        #region Serialized Fields

        #endregion

        #region Non-Serialized Fields

        #endregion

        #region Properties

        [field: SerializeField] public int Rank { get; set; } = 1;

        #endregion

        #region Function Events

        private void OnTriggerEnter2D(Collider2D col)
        {
            gameObject.SetActive(false);
        }

        #endregion

        #region Public Methods

        #endregion

        #region Private Methods

        #endregion
    }
}