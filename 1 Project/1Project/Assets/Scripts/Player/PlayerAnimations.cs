using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    private Animator animator;
    private PlayerController playerController;

    private void Start()
    {
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        // Получаем скорость из Input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Вычисляем общую скорость движения
        float speed = new Vector2(horizontal, vertical).magnitude;



        // Передаём значение в аниматор
        animator.SetFloat("speed", speed);

        // Атака
        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("attack");
        }


    }

    // Публичный метод для вызова из другого скрипта (например, Health)
    public void TriggerHitAnimation()
    {
        animator.SetTrigger("hit");
        Debug.Log("🤕 Анимация получения урона!");
    }
}