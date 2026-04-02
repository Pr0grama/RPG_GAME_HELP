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