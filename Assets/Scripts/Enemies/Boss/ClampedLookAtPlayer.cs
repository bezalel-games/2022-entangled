using System;
using Managers;
using UnityEngine;

namespace Enemies.Boss
{
    public class ClampedLookAtPlayer : MonoBehaviour
    {
        #region Serialized Fields

        [Range(0, 30)] [SerializeField] private float _maxRotationDegree = 30f;

        #endregion

        #region Properties

        private Vector3 _direction;

        #endregion

        #region Function Events

        private void Awake()
        {
            _direction = transform.up;
        }

        private void LateUpdate()
        {
            var player = GameManager.PlayerTransform;
            var t = transform;
            var angleToPlayer = Vector2.SignedAngle(_direction, (player.position - t.position));
            var desiredAngle = Mathf.Clamp(angleToPlayer, -_maxRotationDegree, _maxRotationDegree);
            var desiredRotation = Quaternion.Euler(0, 0, desiredAngle);
            t.localRotation = desiredRotation * Quaternion.LookRotation(Vector3.back);
        }

        #endregion
    }
}