using UnityEngine;

public class PickupWeapon : MonoBehaviour
{
    private WeaponType weaponType;
    private MeshRenderer meshRenderer;

    [Header("Визуал")]
    public Color meleeColor = Color.red;
    public Color rangedColor = Color.blue;

    public void Initialize(WeaponType type)
    {
        weaponType = type;
        meshRenderer = GetComponent<MeshRenderer>();

        if (meshRenderer != null)
        {
            meshRenderer.material.color = weaponType == WeaponType.Melee ? meleeColor : rangedColor;
        }

        // Добавляем вращение
        StartCoroutine(RotateWeapon());
    }

    private System.Collections.IEnumerator RotateWeapon()
    {
        while (true)
        {
            transform.Rotate(Vector3.up, 90f * Time.deltaTime);
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Передаём оружие игроку
            PlayerCombat playerCombat = other.GetComponent<PlayerCombat>();
            if (playerCombat != null)
            {
                if (weaponType == WeaponType.Melee)
                    playerCombat.EquipMeleeWeapon();
                else
                    playerCombat.EquipRangedWeapon();

                Debug.Log($"🎮 Игрок подобрал {weaponType} оружие!");
                Destroy(gameObject);
            }
        }
    }
}