using System;
using UnityEngine;

[Serializable]
public class PlayerModel
{
    [Header("Health")]
    public float health;
    public float maxHealth;

    [Header("Mana")]
    public float mana;
    public float maxMana;

    [Header("Position & Rotation")]
    public Vector3 position;
    public Quaternion rotation;  // ? днаюбхкх онбнпнр

    [Header("Stats")]
    public int killCount;
    public float playTime;
    public int currentWave;

    [Header("Cooldowns")]
    public float nextMagicTime;  // ? днаюбхкх йскдюсм люцхх
    public float nextAttackTime; // ? днаюбхкх йскдюсм юрюйх

    public PlayerModel()
    {
        health = 100f;
        maxHealth = 100f;
        mana = 100f;
        maxMana = 100f;
        position = Vector3.zero;
        rotation = Quaternion.identity;
        killCount = 0;
        playTime = 0f;
        currentWave = 0;
        nextMagicTime = 0f;
        nextAttackTime = 0f;
    }

    public void CopyFrom(PlayerModel other)
    {
        health = other.health;
        maxHealth = other.maxHealth;
        mana = other.mana;
        maxMana = other.maxMana;
        position = other.position;
        rotation = other.rotation;
        killCount = other.killCount;
        playTime = other.playTime;
        currentWave = other.currentWave;
        nextMagicTime = other.nextMagicTime;
        nextAttackTime = other.nextAttackTime;
    }
}