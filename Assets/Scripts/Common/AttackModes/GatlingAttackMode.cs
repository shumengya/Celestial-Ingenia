using UnityEngine;

public class GatlingAttackMode : AttackModeBase
{
    [Header("加特林设置")]
    public float burstDuration = 2f; // 连续射击持续时间
    public float cooldownDuration = 1.5f; // 冷却时间
    public float fireRate = 0.1f; // 射击间隔 (秒)
    
    private enum GatlingState { Ready, Firing, Cooling }
    private GatlingState currentState = GatlingState.Ready;
    
    private float burstTimer = 0f;
    private float cooldownTimer = 0f;
    private float nextFireTime = 0f;
    
    public override bool CanAttack()
    {
        return currentState != GatlingState.Cooling && Time.time >= nextFireTime;
    }
    
    public override void Attack(Vector2 targetPosition)
    {
        if (currentState == GatlingState.Ready)
        {
            currentState = GatlingState.Firing;
            burstTimer = burstDuration;
        }
        
        // 计算射击方向
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        
        // 添加随机角度偏差
        float randomDeviation = Random.Range(-angleDeviation, angleDeviation);
        Quaternion rotation = Quaternion.Euler(0, 0, randomDeviation);
        direction = rotation * direction;
        
        // 生成子弹
        CreateBullet(transform.position, direction);
        
        // 设置下次射击时间
        nextFireTime = Time.time + fireRate;
    }
    
    public override void UpdateAttackState()
    {
        switch (currentState)
        {
            case GatlingState.Firing:
                burstTimer -= Time.deltaTime;
                if (burstTimer <= 0)
                {
                    currentState = GatlingState.Cooling;
                    cooldownTimer = cooldownDuration;
                }
                break;
                
            case GatlingState.Cooling:
                cooldownTimer -= Time.deltaTime;
                if (cooldownTimer <= 0)
                {
                    currentState = GatlingState.Ready;
                }
                break;
        }
    }
} 