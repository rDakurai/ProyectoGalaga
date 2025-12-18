using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Stats base")]
    public float damage = 1f;
    public float fireRate = 1f;
    public float moveSpeed = 5f;

    [Header("Tipo de ataque")]
    public AttackType attackType = AttackType.Bullet;

    public enum AttackType
    {
        Bullet,
        Laser
    }
}

