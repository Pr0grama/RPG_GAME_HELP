using System;

public interface IPlayerInteractor
{
    void SaveCurrentState();
    void LoadLastState();
    bool HasSave();
    PlayerModel GetCurrentModel();
    void DeleteSave();
    event Action<PlayerModel> OnGameSaved;
    event Action<PlayerModel> OnGameLoaded;
}