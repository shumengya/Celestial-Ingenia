using UnityEngine;

public class UnitMovement : MonoBehaviour
{
    public float moveSpeed = 3f; // 敌人的移动速度
    public float rotationSpeed = 200f; // 敌人的旋转速度
    public string playerTag = "Player"; // 玩家的标签

    // 避障相关参数
    public LayerMask obstacleLayerMask; // 障碍物图层掩码
    public float obstacleDetectionRange = 1.5f; // 障碍物检测范围
    public float avoidanceWeight = 2.0f; // 避障权重
    public int numberOfRays = 8; // 检测射线数量
    public float rayAngleRange = 120f; // 检测角度范围（度）

    // 添加调试参数
    public bool showDebugRays = true;
    public float steeringFactor = 0.5f; // 引导系数

    private AttackRange attackRange; // 引用攻击范围脚本
    private Rigidbody2D rb; // 敌人的刚体组件

    void Start()
    {
        // 获取攻击范围脚本
        attackRange = GetComponent<AttackRange>();
        // 获取刚体组件
        rb = GetComponent<Rigidbody2D>();
        
        // 修改：排除自身所在图层或使用特定图层
       // obstacleLayerMask = LayerMask.GetMask("Obstacles");
        // 如果需要检测玩家和敌人，可以单独添加
        // 避免敌人之间互相阻挡时可不添加Enemy层
    }

    void Update()
    {
        // 检查是否检测到玩家
        if (attackRange.DetectedEnemies.Count > 0)
        {
            // 假设只处理第一个检测到的玩家
            GameObject player = attackRange.DetectedEnemies[0];
            if (player != null)
            {
                // 计算敌人到玩家的原始方向
                Vector2 originalDirection = (player.transform.position - transform.position).normalized;
                
                // 计算避障方向
                Vector2 avoidanceDirection = CalculateAvoidanceDirection();
                
                // 结合原始方向和避障方向 - 改进混合方式
                Vector2 finalDirection;
                
                if (avoidanceDirection != Vector2.zero)
                {
                    // 当有障碍时使用改进的转向方法
                    finalDirection = Vector2.Lerp(originalDirection, avoidanceDirection, steeringFactor).normalized;
                    
                    // 绘制最终方向以便调试
                    if (showDebugRays)
                    {
                        Debug.DrawRay(transform.position, finalDirection * 2f, Color.blue);
                    }
                }
                else
                {
                    // 无障碍时直接使用原始方向
                    finalDirection = originalDirection;
                }

                // 计算目标角度
                float targetAngle = Mathf.Atan2(finalDirection.y, finalDirection.x) * Mathf.Rad2Deg - 90f;

                // 平滑旋转到目标角度
                float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.z, targetAngle, rotationSpeed * Time.deltaTime);
                transform.rotation = Quaternion.Euler(0f, 0f, angle);

                // 调整旋转判断条件，放宽容差
                if (Mathf.Abs(Mathf.DeltaAngle(angle, targetAngle)) < 15f)
                {
                    // 移动敌人
                    rb.velocity = finalDirection * moveSpeed;
                }
                else
                {
                    // 旋转时减缓移动，而非完全停止
                    rb.velocity = finalDirection * moveSpeed * 0.3f;
                }
            }
        }
        else
        {
            // 如果没有检测到玩家，停止移动
            rb.velocity = Vector2.zero;
        }
    }
    
    // 改进避障方向计算
    private Vector2 CalculateAvoidanceDirection()
    {
        Vector2 avoidanceDirection = Vector2.zero;
        float startAngle = -rayAngleRange / 2;
        float angleStep = rayAngleRange / (numberOfRays - 1);
        
        int hitCount = 0;
        
        // 找出最佳方向（无障碍的方向）
        Vector2 bestDirection = Vector2.zero;
        float bestWeight = -1f;
        
        for (int i = 0; i < numberOfRays; i++)
        {
            float angle = startAngle + i * angleStep;
            // 基于当前朝向计算射线方向
            Vector2 rayDirection = Quaternion.Euler(0, 0, angle) * transform.up;
            
            RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirection, obstacleDetectionRange, obstacleLayerMask);
            
            if (hit.collider != null && hit.collider.gameObject != gameObject)
            {
                // 记录有效的碰撞
                hitCount++;
                
                // 计算该方向的权重值
                float weight = 1.0f - (hit.distance / obstacleDetectionRange);
                
                // 累加反向力
                avoidanceDirection -= rayDirection.normalized * weight;
                
                // 绘制射线
                if (showDebugRays)
                    Debug.DrawRay(transform.position, rayDirection * hit.distance, Color.red);
            }
            else
            {
                // 找出可能的最佳方向（无障碍的地方）
                // 优先考虑接近正前方的无障碍方向
                float directionWeight = 1.0f - Mathf.Abs(angle) / rayAngleRange;
                
                if (directionWeight > bestWeight)
                {
                    bestWeight = directionWeight;
                    bestDirection = rayDirection;
                }
                
                // 绘制射线
                if (showDebugRays)
                    Debug.DrawRay(transform.position, rayDirection * obstacleDetectionRange, Color.green);
            }
        }
        
        // 如果找到了最佳无障碍方向，优先考虑这个方向
        if (hitCount > 0 && bestDirection != Vector2.zero)
        {
            // 将最佳方向添加到避障方向中，赋予较高权重
            avoidanceDirection = (avoidanceDirection + bestDirection * 2f).normalized;
            
            // 显示最佳方向
            if (showDebugRays)
                Debug.DrawRay(transform.position, bestDirection * 2.5f, Color.yellow);
        }
        else if (hitCount == 0)
        {
            // 无障碍时返回零向量
            return Vector2.zero;
        }
        
        return avoidanceDirection.normalized;
    }
}