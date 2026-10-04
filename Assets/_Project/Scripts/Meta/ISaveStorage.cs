namespace Game.Meta
{
    public interface ISaveStorage
    {
        SaveData Load();
        void Save(SaveData data);
    }
}
