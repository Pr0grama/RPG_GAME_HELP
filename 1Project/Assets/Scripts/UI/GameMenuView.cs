using UnityEngine;
using UnityEngine.UI;

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