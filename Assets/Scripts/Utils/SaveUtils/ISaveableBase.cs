namespace Utils.SaveUtils
{
    public interface ISavableBase
    {
        SaveSystem.DataType GetDataType();
        object ToSave();
        void FromLoad(object data);
    }
}