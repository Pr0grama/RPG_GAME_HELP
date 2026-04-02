using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuView : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Button closeSettingsButton;

    public Button PlayButton => playButton;
    public Button SettingsButton => settingsButton;
    public GameObject SettingsPanel => settingsPanel;
    public Slider VolumeSlider => volumeSlider;
    public Button CloseSettingsButton => closeSettingsButton;
}

public class GameMenuView : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button loadButton;
    [SerializeField] private Button resumeButton;

    public GameObject MenuPanel => menuPanel;
    public Button MainMenuButton => mainMenuButton;
    public Button SaveButton => saveButton;
    public Button LoadButton => loadButton;
    public Button ResumeButton => resumeButton;
}