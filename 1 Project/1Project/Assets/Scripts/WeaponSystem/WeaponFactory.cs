using System;
using System.Collections.Generic;

public static class WeaponFactory
{
    private static Dictionary<WeaponType, Func<BaseWeapon>> weaponCreators = new Dictionary<WeaponType, Func<BaseWeapon>>
    {
        { WeaponType.Melee, () => new MeleeWeapon() },
        { WeaponType.Ranged, () => new RangedWeapon() }
    };

    public static BaseWeapon CreateWeapon(WeaponType type)
    {
        if (weaponCreators.TryGetValue(type, out var creator))
        {
            return creator();
        }
        return new MeleeWeapon(); // По умолчанию
    }

    public static BaseWeapon CreateRandomWeapon()
    {
        Array values = Enum.GetValues(typeof(WeaponType));
        WeaponType randomType = (WeaponType)values.GetValue(UnityEngine.Random.Range(0, values.Length));
        return CreateWeapon(randomType);
    }

    public static ElementType GetRandomElement()
    {
        Array values = Enum.GetValues(typeof(ElementType));
        return (ElementType)values.GetValue(UnityEngine.Random.Range(0, values.Length));
    }
}