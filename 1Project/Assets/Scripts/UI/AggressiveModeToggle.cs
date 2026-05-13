using UnityEngine;
using UnityEngine.UI;

public class AggressiveModeToggle : MonoBehaviour
{
    [Header("UI элементы")]
    public Button toggleButton;
    public Text buttonText;

    private bool isAggressive = false;

    private void Start()
    {
        if (toggleButton != null)
            toggleButton.onClick.AddListener(ToggleMode);

        UpdateButtonText();
    }

    private void ToggleMode()
    {
        isAggressive = !isAggressive;
        EnemyStateMachine.SetGlobalAggressiveMode(isAggressive);
        UpdateButtonText();
    }

    private void UpdateButtonText()
    {
        if (buttonText != null)
        {
            buttonText.text = isAggressive ? "🐺 Агрессивные мобы" : "🐑 Мирные мобы";
        }
    }
}