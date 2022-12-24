using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Utils.SaveUtils
{
    public static class SaveSystem
    {

        #region Fields

        private static Dictionary<DataType, string> Filenames = new()
        {
            {DataType.PLAYER, "player.data"}
        };

        #endregion
        
        #region Public Methods

        //TODO: move filestream creation to SaveData
        public static void SaveData(Dictionary<DataType, ISavableBase> data)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            var pathPrefix = Application.persistentDataPath;
            string path = "";
            FileStream stream;
            
            foreach (var pair in data)
            {
                var key = pair.Key;
                var val = pair.Value;
                if (val.GetDataType() != key) 
                    throw new UnknownDataTypeException();

                if (val.GetDataType() != key)
                    throw new WrongDataTypeException();
                
                path = pathPrefix + "/" + Filenames[key];
                stream = new FileStream(path, FileMode.Create);
                formatter.Serialize(stream, val.ToSave());
                stream.Close();
            }
        }

        public static void LoadData(Dictionary<DataType, ISavableBase> data)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            var pathPrefix = Application.persistentDataPath;
            string path = "";
            FileStream stream;
            
            foreach (var pair in data)
            {
                var key = pair.Key;
                var val = pair.Value;
                if (val.GetDataType() != key) 
                    throw new UnknownDataTypeException();

                if (val.GetDataType() != key)
                    throw new WrongDataTypeException();
                
                path = pathPrefix + "/" + Filenames[key];
                stream = new FileStream(path, FileMode.Open);
                val.FromLoad(formatter.Deserialize(stream));
                stream.Close();
            }
        }

        #endregion
        
        #region Private Methods

        // private static void SavePlayer(PlayerController player, BinaryFormatter formatter=null)
        // {
        //     if (formatter == null)
        //         formatter = new BinaryFormatter();
        //
        //     string path = Application.persistentDataPath + "/" +  Filenames[DataType.PLAYER];
        //     FileStream stream = new FileStream(path, FileMode.Create);
        //
        //     PlayerController.PlayerData playerData = new PlayerController.PlayerData(player);
        //     
        //     formatter.Serialize(stream, playerData);
        //     stream.Close();
        // }
        //
        // private static PlayerController.PlayerData LoadPlayer(BinaryFormatter formatter=null)
        // {
        //     string path = Application.persistentDataPath + "/" + Filenames[DataType.PLAYER];
        //     if (File.Exists(path))
        //     {
        //         if(formatter == null)
        //             formatter = new BinaryFormatter();
        //
        //         FileStream stream = new FileStream(path, FileMode.Open);
        //         
        //         PlayerController.PlayerData data = formatter.Deserialize(stream) as PlayerController.PlayerData;
        //
        //         stream.Close();
        //         
        //         return data;
        //     }
        //
        //     return null;
        // }
        
        #endregion
        
        #region Classes & Enums
        
        public enum DataType
        {
            PLAYER
        }
        
        private class UnknownDataTypeException : Exception
        {
        }
        
        private class WrongDataTypeException : Exception
        {
        }
        
        #endregion
    }
}