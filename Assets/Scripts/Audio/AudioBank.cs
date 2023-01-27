using System;
using System.Collections.Generic;
using Cards.Factory;
using FMOD.Studio;
using UnityEngine;
using FMODUnity;
using Object = System.Object;

namespace Audio
{
    [CreateAssetMenu(fileName = "Audio Bank", menuName = "Entangled/Audio/Audio Bank", order = 0)]
    public class AudioBank : ScriptableObject
    {
        [field: SerializeField] public EventReference MusicEventReference { get; private set; }
        [SerializeField] private List<PlayerRefPair> _playerRefs = new();
        [SerializeField] private List<YoyoRefPair> _yoyoRefs = new();
        [SerializeField] private List<EnemyRefPair> _enemyRefs = new();

        private Dictionary<SoundType, Object> _refsDict;

        // #region Add
        // [Header("Add Sounds")] [SerializeField]
        // private SoundType _type;
        //
        // [SerializeField] [HideInInspector] private PlayerSounds _playerSounds;
        // [SerializeField] [HideInInspector] private YoyoSounds _yoyoSounds;
        // [SerializeField] private EventReference _reference;
        //
        // #endregion

        public EventReference this[SoundType soundType, int sound]
        {
            get
            {
                return soundType switch
                {
                    SoundType.PLAYER => _playerRefs[sound]._reference,
                    SoundType.YOYO => _yoyoRefs[sound]._reference,
                    SoundType.ENEMY => _enemyRefs[sound]._reference,
                };
            }
        }

        private void OnEnable()
        {
            if (_refsDict == null)
            {
                _refsDict = new Dictionary<SoundType, Object>()
                {
                    {SoundType.PLAYER, _playerRefs},
                    {SoundType.YOYO, _yoyoRefs},
                    {SoundType.ENEMY, _enemyRefs}
                };
            }
        }

        private void OnValidate()
        {
            if (_refsDict == null)
            {
                _refsDict = new Dictionary<SoundType, Object>()
                {
                    {SoundType.PLAYER, _playerRefs},
                    {SoundType.YOYO, _yoyoRefs},
                    {SoundType.ENEMY, _enemyRefs}
                };
            }
            
            foreach ((var key, var value) in _refsDict)
            {
                switch (key)
                {
                    case SoundType.PLAYER:
                        ((List<PlayerRefPair>) value).Sort(((pair1, pair2) => pair1._type - pair2._type));
                        break;
                    case SoundType.YOYO:
                        ((List<YoyoRefPair>) value).Sort(((pair1, pair2) => pair1._type - pair2._type));
                        break;
                    case SoundType.ENEMY:
                        ((List<EnemyRefPair>) value).Sort(((pair1, pair2) => pair1._type - pair2._type));
                        break;
                }    
            }
        }

        // public void AddSound(SoundType soundType, Enum enumVal, string referenceStringValue)
        // {
        //     var sr = new SoundRefPair();
        //     sr._type = enumVal.IntValue();
        //     sr._reference = EventReference.Find(referenceStringValue);
        //     _refsDict[soundType].Add(sr);
        // }
    }

    #region Classes

    [Serializable]
    public abstract class SoundRefPair
    {
        public abstract int Type { get; }
        public EventReference _reference;
    }

    [Serializable]
    public class PlayerRefPair : SoundRefPair
    {
        public PlayerSounds _type;
        public override int Type => (int) _type;
    }
    
    [Serializable]
    public class YoyoRefPair : SoundRefPair
    {
        public YoyoSounds _type;
        public override int Type => (int) _type;
    }
    
    [Serializable]
    public class EnemyRefPair : SoundRefPair
    {
        public EnemySounds _type;
        public override int Type => (int) _type;
    }
    

    public enum SoundType
    {
        PLAYER = 0,
        YOYO,
        ENEMY,
        MUSIC,
    }

    public enum PlayerSounds
    {
        ROLL = 0,
        HIT,
        TELEPORT,
    }
    
    public enum EnemySounds
    {
        HIT,
        GOOMBA_PREP,
        GOOMBA_ATTACK,
        // FUMER_ATTACK,
        DEATH,
    }

    public enum YoyoSounds
    {
        THROW = 0,
        PRECISION,
        WALL_HIT
    }
    
    public enum MusicSounds
    {
        MENU = 0,
        RUN,
        BOSS1,
        // BOSS2,
        TUTORIAL
    }

    #endregion
}