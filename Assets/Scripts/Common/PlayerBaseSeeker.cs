using UnityEngine;

/// <summary>
/// 玩家基地寻路组件 - 当敌人没有目标时，会自动寻找玩家基地并移动向它
/// </summary>
public class PlayerBaseSeeker : MonoBehaviour
{
    // PlayerBase寻路相关参数
    public bool seekPlayerBaseWhenIdle = true; // 是否在空闲时寻找PlayerBase
    public System.Collections.Generic.List<string> playerBaseNames = new System.Collections.Generic.List<string> { "PlayerBase", "PlayerBase(Clone)" }; // PlayerBase的名称列表
    public float playerBaseSearchInterval = 2f; // 搜索PlayerBase的时间间隔

    // 引用其他组件
    private UnitMovement unitMovement;
    private AttackRange attackRange;
    private Rigidbody2D rb;
    
    // 内部状态
    private Transform targetPlayerBase;
    private float searchTimer;
    private bool wasSeekingBase = false;

    void Start()
    {
        // 获取必要组件
        unitMovement = GetComponent<UnitMovement>();
        attackRange = GetComponent<AttackRange>();
        rb = GetComponent<Rigidbody2D>();
        
        // 检查必要组件是否存在
        if (unitMovement == null || attackRange == null || rb == null)
        {
            Debug.LogError("PlayerBaseSeeker需要UnitMovement、AttackRange和Rigidbody2D组件！");
            enabled = false;
            return;
        }
        
        // 初始化搜索计时器，让不同单位错开搜索时间
        searchTimer = Random.Range(0f, playerBaseSearchInterval);
    }

    void Update()
    {
        // 只有当没有检测到直接目标时，寻找玩家基地
        if (attackRange.DetectedEnemies.Count == 0 && seekPlayerBaseWhenIdle)
        {
            // 更新搜索计时器
            searchTimer -= Time.deltaTime;
            if (searchTimer <= 0f || targetPlayerBase == null)
            {
                searchTimer = playerBaseSearchInterval;
                FindPlayerBase();
            }
            
            // 如果找到了PlayerBase，向它移动
            if (targetPlayerBase != null)
            {
                MoveTowardsPlayerBase();
                wasSeekingBase = true;
                return;
            }
        }
        else if (wasSeekingBase && attackRange.DetectedEnemies.Count > 0)
        {
            // 如果之前在寻找基地，但现在发现了敌人，则重置状态
            wasSeekingBase = false;
            // 让UnitMovement组件接管移动
        }
    }
    
    // 查找场景中的PlayerBase
    private void FindPlayerBase()
    {
        GameObject playerBaseObj = null;
        foreach (string name in playerBaseNames)
        {
            playerBaseObj = GameObject.Find(name);
            if (playerBaseObj != null)
            {
                break; // 找到一个就停止搜索
            }
        }
        
        // 更新目标引用
        targetPlayerBase = playerBaseObj != null ? playerBaseObj.transform : null;
    }
    
    // 向玩家基地移动
    private void MoveTowardsPlayerBase()
    {
        if (targetPlayerBase == null) return;
        
        // 计算敌人到目标的原始方向
        Vector2 originalDirection = (targetPlayerBase.position - transform.position).normalized;
        
        // 计算避障方向（使用UnitMovement中的避障逻辑）
        Vector2 avoidanceDirection = CalculateAvoidanceDirection();
        
        // 结合原始方向和避障方向 - 与UnitMovement相同的混合方式
        Vector2 finalDirection;
        
        if (avoidanceDirection != Vector2.zero)
        {
            // 当有障碍时使用转向方法
            finalDirection = Vector2.Lerp(originalDirection, avoidanceDirection, unitMovement.steeringFactor).normalized;
            
            // 绘制最终方向以便调试
            if (unitMovement.showDebugRays)
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
        float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.z, targetAngle, unitMovement.rotationSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // 调整旋转判断条件，放宽容差
        if (Mathf.Abs(Mathf.DeltaAngle(angle, targetAngle)) < 15f)
        {
            // 移动敌人
            rb.velocity = finalDirection * unitMovement.moveSpeed;
        }
        else
        {
            // 旋转时减缓移动，而非完全停止
            rb.velocity = finalDirection * unitMovement.moveSpeed * 0.3f;
        }
    }
    
    // 从UnitMovement复制的避障方向计算方法
    private Vector2 CalculateAvoidanceDirection()
    {
        Vector2 avoidanceDirection = Vector2.zero;
        float startAngle = -unitMovement.rayAngleRange / 2;
        float angleStep = unitMovement.rayAngleRange / (unitMovement.numberOfRays - 1);
        
        int hitCount = 0;
        
        // 找出最佳方向（无障碍的方向）
        Vector2 bestDirection = Vector2.zero;
        float bestWeight = -1f;
        
        for (int i = 0; i < unitMovement.numberOfRays; i++)
        {
            float angle = startAngle + i * angleStep;
            // 基于当前朝向计算射线方向
            Vector2 rayDirection = Quaternion.Euler(0, 0, angle) * transform.up;
            
            RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirection, unitMovement.obstacleDetectionRange, unitMovement.obstacleLayerMask);
            
            if (hit.collider != null && hit.collider.gameObject != gameObject)
            {
                // 记录有效的碰撞
                hitCount++;
                
                // 计算该方向的权重值
                float weight = 1.0f - (hit.distance / unitMovement.obstacleDetectionRange);
                
                // 累加反向力
                avoidanceDirection -= rayDirection.normalized * weight;
                
                // 绘制射线
                if (unitMovement.showDebugRays)
                    Debug.DrawRay(transform.position, rayDirection * hit.distance, Color.red);
            }
            else
            {
                // 找出可能的最佳方向（无障碍的地方）
                // 优先考虑接近正前方的无障碍方向
                float directionWeight = 1.0f - Mathf.Abs(angle) / unitMovement.rayAngleRange;
                
                if (directionWeight > bestWeight)
                {
                    bestWeight = directionWeight;
                    bestDirection = rayDirection;
                }
                
                // 绘制射线
                if (unitMovement.showDebugRays)
                    Debug.DrawRay(transform.position, rayDirection * unitMovement.obstacleDetectionRange, Color.green);
            }
        }
        
        // 如果找到了最佳无障碍方向，优先考虑这个方向
        if (hitCount > 0 && bestDirection != Vector2.zero)
        {
            // 将最佳方向添加到避障方向中，赋予较高权重
            avoidanceDirection = (avoidanceDirection + bestDirection * 2f).normalized;
            
            // 显示最佳方向
            if (unitMovement.showDebugRays)
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