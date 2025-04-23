using UnityEngine;

public class BombTrap : TrapBase
{
    protected override void OnTrapTrigger(Collider2D target){
        HealthBar targetHealth = target.GetComponentInChildren<HealthBar>();
        if (targetHealth != null)
        {
            targetHealth.TakeDamage(100);
            Destroy(gameObject);
        }
    }
} 