using UnityEngine;
using System.Collections;

public class KatyushaAttackMode : AttackModeBase
{
    [Header("喀秋莎设置")]
    public int rocketCount = 5; // 一次发射的火箭数量
    public float rocketDelay = 0.2f; // 火箭之间的发射延迟
    public float salvoInterval = 4f; // 齐射之间的间隔
    public float areaRadius = 2f; // 目标区域半径
    
    private bool isLaunching = false;
    private float nextSalvoTime = 0f;
    
    public override bool CanAttack()
    {
        return !isLaunching && Time.time >= nextSalvoTime;
    }
    
    public override void Attack(Vector2 targetPosition)
    {
        if (!isLaunching)
        {
            StartCoroutine(LaunchRockets(targetPosition));
        }
    }
    
    private IEnumerator LaunchRockets(Vector2 targetPosition)
    {
        isLaunching = true;
        
        for (int i = 0; i < rocketCount; i++)
        {
            // 在目标区域内选择一个随机点
            Vector2 randomOffset = Random.insideUnitCircle * areaRadius;
            Vector2 rocketTarget = targetPosition + randomOffset;
            
            // 计算射击方向
            Vector2 direction = GetFiringDirection(targetPosition);
            
            // 添加随机角度偏差
            float randomDeviation = Random.Range(-angleDeviation, angleDeviation);
            Quaternion rotation = Quaternion.Euler(0, 0, randomDeviation);
            direction = rotation * direction;
            
            // 生成火箭
            CreateBullet(transform.position, direction);
            
            // 等待下一枚火箭发射
            yield return new WaitForSeconds(rocketDelay);
        }
        
        // 设置下次齐射时间
        nextSalvoTime = Time.time + salvoInterval;
        isLaunching = false;
    }
    
    public override void UpdateAttackState()
    {
        // 此模式不需要额外的状态更新，因为使用了协程
    }
} 