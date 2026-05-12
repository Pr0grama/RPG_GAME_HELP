using UnityEngine;
using System.Collections;

public class Mana : MonoBehaviour
{
    public float maxMana = 100f;
    public float currentMana;
    public float manaRegenRate = 10f; // реген в секунду
    public float manaRegenDelay = 2f; // задержка после траты маны
    public System.Action onManaChanged;
    public System.Action onManaEmpty;

    private float lastManaUseTime;
    private Coroutine regenCoroutine;

    private void Start()
    {
        currentMana = maxMana;
        lastManaUseTime = -manaRegenDelay;
        StartManaRegen();
    }

    public bool SpendMana(float amount)
    {
        if (currentMana >= amount)
        {
            currentMana -= amount;
            lastManaUseTime = Time.time;
            onManaChanged?.Invoke();

            if (currentMana <= 0 && onManaEmpty != null)
                onManaEmpty?.Invoke();

            return true;
        }
        return false;
    }

    public void RestoreMana(float amount)
    {
        currentMana = Mathf.Min(currentMana + amount, maxMana);
        onManaChanged?.Invoke();
    }

    private void StartManaRegen()
    {
        if (regenCoroutine != null)
            StopCoroutine(regenCoroutine);
        regenCoroutine = StartCoroutine(ManaRegenRoutine());
    }

    private IEnumerator ManaRegenRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);

            if (Time.time >= lastManaUseTime + manaRegenDelay && currentMana < maxMana)
            {
                currentMana = Mathf.Min(currentMana + manaRegenRate * 0.1f, maxMana);
                onManaChanged?.Invoke();
            }
        }
    }
}