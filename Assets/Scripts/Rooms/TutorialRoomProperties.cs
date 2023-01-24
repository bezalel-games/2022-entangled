using System;
using Enemies;
using UnityEngine;

namespace Rooms
{
    [CreateAssetMenu(fileName = "TutorialRoomData", menuName = "Entangled/Tutorial Room Data")]
    public class TutorialRoomProperties : ScriptableObject
    {
        public EnemyBarrierPair[] _enemies;
        public string _text;
    }
    
    [Serializable]
    public struct EnemyBarrierPair
    {
        public Enemy _enemy;
        public bool _hasBarrier;
    }
}