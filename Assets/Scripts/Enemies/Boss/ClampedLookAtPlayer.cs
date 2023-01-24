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
        private Quaternion _rotation;

        #endregion

        #region Function Events

        private void Awake()
        {
            _direction = transform.up;
            _rotation = Quaternion.Euler(0,0,transform.rotation.eulerAngles.z);
        }

        private void LateUpdate()
        {
            var player = GameManager.PlayerTransform;
            var t = transform;
            var angleToPlayer = Vector2.SignedAngle(_direction, (player.position - t.position));
            var desiredAngle = Mathf.Clamp(angleToPlayer, -_maxRotationDegree, _maxRotationDegree);
            var desiredRotation = Quaternion.Euler(0, 0, desiredAngle);
            t.rotation = _rotation * desiredRotation;
        }

        #endregion
    }
}