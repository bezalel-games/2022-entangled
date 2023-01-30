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
        #region Serialized Fields
        
        [field: SerializeField] public EventReference MusicEventReference { get; private set; }
        [SerializeField] private List<PlayerRefPair> _playerRefs = new();
        [SerializeField] private List<YoyoRefPair> _yoyoRefs = new();
        [SerializeField] private List<EnemyRefPair> _enemyRefs = new();
        [SerializeField] private List<SfxRefPair> _sfxRefs = new();
        
        // #region Add
        // [Header("Add Sounds")] [SerializeField]
        // private SoundType _type;
        //
        // [SerializeField] [HideInInspector] private PlayerSounds _playerSounds;
        // [SerializeField] [HideInInspector] private YoyoSounds _yoyoSounds;
        // [SerializeField] private EventReference _reference;
        //
        // #endregion
        
        #endregion

        #region None-Serialized Fields

        private Dictionary<SoundType, Object> _refsDict;

        #endregion

        public EventReference this[SoundType soundType, int sound]
        {
            get
            {
                return soundType switch
                {
                    SoundType.PLAYER => _playerRefs[sound]._reference,
                    SoundType.YOYO => _yoyoRefs[sound]._reference,
                    SoundType.ENEMY => _enemyRefs[sound]._reference,
                    SoundType.SFX => _sfxRefs[sound]._reference
                };
            }
        }

        private void OnEnable()
        {
            InitSoundDictionary();
        }

        private void OnValidate()
        {
            InitSoundDictionary();

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

        private void InitSoundDictionary()
        {
            if (_refsDict == null)
            {
                _refsDict = new Dictionary<SoundType, Object>()
                {
                    {SoundType.PLAYER, _playerRefs},
                    {SoundType.YOYO, _yoyoRefs},
                    {SoundType.ENEMY, _enemyRefs},
                    {SoundType.SFX, _sfxRefs}
                };
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
    
    [Serializable]
    public class SfxRefPair : SoundRefPair
    {
        public SfxSounds _type;
        public override int Type => (int) _type;
    }
    

    public enum SoundType
    {
        PLAYER = 0,
        YOYO,
        ENEMY,
        SFX,
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
        FUMER_ATTACK,
        SHOOTER_ATTACK,
        DEATH,
        BOSS_EXPLOSION,
        BOSS_ATTACK
    }

    public enum YoyoSounds
    {
        THROW = 0,
        PRECISION,
        WALL_HIT,
        EXPLOSION
    }
    
    public enum MusicSounds
    {
        MENU = 0,
        RUN,
        BOSS1,
        // BOSS2,
        TUTORIAL
    }

    public enum SfxSounds
    {
        FOUNTAIN,
        CHEST,
        CARDS_IN_OUT,
        BUTTON_MOVE,
        CLOSE_DOOR,
    }

    #endregion
}