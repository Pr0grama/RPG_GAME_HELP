using UnityEngine;

public interface IWeapon
{
    WeaponType WeaponType { get; }
    ElementType Element { get; set; }
    float Damage { get; }
    float Cooldown { get; }
    float Range { get; }

    void Initialize(ElementType element);
    void Attack(Transform attacker, Transform target);
    void ChangeElement(ElementType newElement);
    GameObject GetEffectPrefab();
    AudioClip GetAttackSound();
}