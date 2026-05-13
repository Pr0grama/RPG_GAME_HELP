using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MagicCooldownUI : MonoBehaviour
{
    public Image cooldownOverlay;
    public float cooldownDuration = 2f;

    private bool isOnCooldown = false;

    private void Start()
    {
        if (cooldownOverlay == null)
            cooldownOverlay = GetComponentInChildren<Image>();

        cooldownOverlay.fillAmount = 0;
    }

    public void StartCooldown()
    {
        if (!isOnCooldown)
            StartCoroutine(CooldownRoutine());
    }

    private IEnumerator CooldownRoutine()
    {
        isOnCooldown = true;
        float elapsedTime = 0f;

        while (elapsedTime < cooldownDuration)
        {
            elapsedTime += Time.deltaTime;
            cooldownOverlay.fillAmount = 1 - (elapsedTime / cooldownDuration);
            yield return null;
        }

        cooldownOverlay.fillAmount = 0;
        isOnCooldown = false;
    }

    public bool CanCast()
    {
        return !isOnCooldown;
    }
}