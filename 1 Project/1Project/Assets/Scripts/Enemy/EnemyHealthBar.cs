using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public Health enemyHealth;
    public Slider healthSlider;
    public Vector3 offset = new Vector3(0, 2.5f, 0);

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;

        if (enemyHealth == null)
            enemyHealth = GetComponentInParent<Health>();

        if (healthSlider == null)
            healthSlider = GetComponent<Slider>();

        // ќбновл€ем позицию холста
        transform.SetParent(null); // открепл€ем от врага дл€ ручного управлени€
    }

    private void Update()
    {
        if (enemyHealth != null && healthSlider != null)
        {
            // ќбновл€ем значение полоски
            healthSlider.value = enemyHealth.currentHealth / enemyHealth.maxHealth;

            // —ледим за врагом
            if (enemyHealth.transform != null)
            {
                transform.position = enemyHealth.transform.position + offset;
                transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
                                mainCamera.transform.rotation * Vector3.up);
            }
        }
        else
        {
            Destroy(gameObject); // если врага нет Ч удал€ем UI
        }
    }
}