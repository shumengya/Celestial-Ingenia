using UnityEngine;

public class NormalAttackMode : AttackModeBase
{
    [Header("普通射击设置")]
    public float fireRate = 1f; // 射击间隔（秒）
    
    private float nextFireTime = 0f;
    
    public override bool CanAttack()
    {
        return Time.time >= nextFireTime;
    }
    
    public override void Attack(Vector2 targetPosition)
    {
        // 计算射击方向
        Vector2 direction = GetFiringDirection(targetPosition);
        
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
        // 普通射击模式不需要特殊的状态更新
    }
} 