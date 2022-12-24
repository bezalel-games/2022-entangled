using UnityEngine;
using Utils;
using Utils.SaveUtils;

namespace Player
{
    public partial class PlayerController : ISavable<PlayerController.PlayerData>
    {

        [System.Serializable]
        public class PlayerData
        {
            #region Fields

            public float YoyoSize;

            #endregion

            #region Constructors

            public PlayerData(PlayerController player)
            {
                var yoyo = player.Yoyo;

                YoyoSize = yoyo.Size;
            }

            #endregion
        }

        #region ISavable Methods
        
        public SaveSystem.DataType GetDataType()
        {
            return SaveSystem.DataType.PLAYER;
        }

        public PlayerData ToSave()
        {
            return new PlayerData(this);
        }

        public void FromLoad(PlayerData data)
        {
            var yoyo = Yoyo;
            Yoyo.Size = data.YoyoSize;
        }

        #endregion
    }
}

