using UnityEngine;

public class ShotgunAttackMode : AttackModeBase
{
    [Header("散弹设置")]
    public int bulletCount = 5; // 每次射击的子弹数量
    public float spreadAngle = 30f; // 散布角度
    public float fireRate = 1.5f; // 射击间隔（秒）
    
    private float nextFireTime = 0f;
    
    public override bool CanAttack()
    {
        return Time.time >= nextFireTime;
    }
    
    public override void Attack(Vector2 targetPosition)
    {
        // 计算基础射击方向
        Vector2 baseDirection = GetFiringDirection(targetPosition);
        
        // 计算每颗子弹之间的角度间隔
        float angleStep = spreadAngle / (bulletCount - 1);
        float startAngle = -spreadAngle / 2;
        
        // 发射多个子弹
        for (int i = 0; i < bulletCount; i++)
        {
            // 计算当前子弹的角度
            float currentAngle = startAngle + (angleStep * i);
            
            // 添加基础角度偏差
            Quaternion spreadRotation = Quaternion.Euler(0, 0, currentAngle);
            Vector2 bulletDirection = spreadRotation * baseDirection;
            
            // 再添加随机角度偏差（使散布更自然）
            float randomDeviation = Random.Range(-angleDeviation/2, angleDeviation/2);
            Quaternion randomRotation = Quaternion.Euler(0, 0, randomDeviation);
            bulletDirection = randomRotation * bulletDirection;
            
            // 生成子弹
            CreateBullet(transform.position, bulletDirection);
        }
        
        // 设置下次射击时间
        nextFireTime = Time.time + fireRate;
    }
    
    public override void UpdateAttackState()
    {
        // 散弹模式不需要特殊的状态更新
    }
} 