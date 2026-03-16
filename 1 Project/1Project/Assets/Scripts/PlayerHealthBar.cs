using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    public Health playerHealth;
    public Slider healthSlider;

    private void Start()
    {
        // Если не назначено вручную — ищем на игроке
        if (playerHealth == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerHealth = player.GetComponent<Health>();
        }

        if (healthSlider == null)
            healthSlider = GetComponent<Slider>();
    }

    private void Update()
    {
        if (playerHealth != null && healthSlider != null)
        {
            healthSlider.value = playerHealth.currentHealth / playerHealth.maxHealth;
        }
    }
}