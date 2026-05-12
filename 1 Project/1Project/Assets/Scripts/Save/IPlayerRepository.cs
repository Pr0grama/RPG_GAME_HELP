public interface IPlayerRepository
{
    void Save(PlayerModel model);
    PlayerModel Load();
    bool HasSave();
    void Delete();
    string GetSaveFilePath();
}