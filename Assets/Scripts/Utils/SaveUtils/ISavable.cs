namespace Utils.SaveUtils
{
    public interface ISavable<T> : ISavableBase
    {
        new T ToSave();
        void FromLoad(T data);

        object ISavableBase.ToSave()
        {
            return ToSave();
        }

        void ISavableBase.FromLoad(object data)
        {
            FromLoad((T) data);
        }
    }
}