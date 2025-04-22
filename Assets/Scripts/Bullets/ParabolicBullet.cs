using UnityEngine;

public class ParabolicBullet : BulletBase
{
    [Header("抛物线设置")]
    public float gravity = 9.8f;          // 重力加速度
    public float initialHeight = 2.0f;     // 初始高度，影响弹道高度
    
    [Header("层级设置")]
    public string flyingLayer = "Projectile";     // 飞行时的层级
    public string landingLayer = "PlayerBullet";  // 落地时的层级
    public float landingThreshold = 0.9f;         // 接近目标的阈值，达到此进度时切换层级

    private Vector2 initialVelocity;       // 初始速度
    private float flightTime = 0f;         // 飞行时间
    private Vector2 targetPosition;        // 目标位置
    new private Vector2 startPosition;     // 起始位置，使用new关键字表明我们有意隐藏继承的成员
    private SpriteRenderer spriteRenderer; // 子弹的渲染器，用于旋转图像
    
    // 抛物线运动需要的参数
    private float distanceToTarget;        // 到目标的水平距离
    private bool hasChangedToLandingLayer = false; // 是否已切换到着陆层
    
    // 初始化子弹
    public void Initialize(Vector2 targetPos, float bulletSpeed)
    {
        targetPosition = targetPos;
        startPosition = transform.position;
        distanceToTarget = Vector2.Distance(new Vector2(startPosition.x, startPosition.y), 
                                           new Vector2(targetPosition.x, targetPosition.y));
        
        // 设置初始速度，计算出合适的发射角度
        // 使用抛物线公式计算初始速度
        float angle = CalculateOptimalLaunchAngle(distanceToTarget, initialHeight, bulletSpeed);
        
        // 将角度转换为弧度
        float angleRad = angle * Mathf.Deg2Rad;
        
        // 计算水平和垂直方向的初速度分量
        Vector2 direction = (targetPosition - startPosition).normalized;
        initialVelocity = new Vector2(
            direction.x * bulletSpeed * Mathf.Cos(angleRad),
            direction.y * bulletSpeed * Mathf.Cos(angleRad)
        );
        
        // 向上的初速度
        float upVelocity = bulletSpeed * Mathf.Sin(angleRad);
        
        // 最终的初始速度
        initialVelocity = new Vector2(initialVelocity.x, initialVelocity.y + upVelocity);
        
        // 设置初始层级为飞行层
        SetBulletLayer(flyingLayer);
    }
    
    // 设置子弹层级
    protected void SetBulletLayer(string layerName)
    {
        int layer = LayerMask.NameToLayer(layerName);
        if (layer != -1)
        {
            gameObject.layer = layer;
            
            // 同时设置所有子对象的层级
            foreach (Transform child in transform)
            {
                child.gameObject.layer = layer;
            }
        }
        else
        {
            Debug.LogWarning($"未找到层级: {layerName}，请确保在项目设置中已定义此层级");
        }
    }
    
    // 计算最佳发射角度
    private float CalculateOptimalLaunchAngle(float distance, float height, float speed)
    {
        // 简单情况下，使用45度角（π/4弧度）是最佳的
        // 但这可以根据具体需求进行调整
        return 45f;
    }
    
    protected override void Start()
    {
        base.Start();
        startPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
        
        // 设置初始层级为飞行层
        SetBulletLayer(flyingLayer);
    }
    
    protected override void Update()
    {
        // 不使用基类的直线移动逻辑
        // base.Update();
        
        flightTime += Time.deltaTime;
        
        // 计算当前位置 (基于抛物线方程)
        float progress = flightTime * speed / distanceToTarget;
        
        // 如果到达或超过目标，销毁子弹
        if (progress >= 1.0f)
        {
            OnDestroyBullet();
            return;
        }
        
        // 检查是否应该切换到着陆层
        if (!hasChangedToLandingLayer && progress >= landingThreshold)
        {
            SetBulletLayer(landingLayer);
            hasChangedToLandingLayer = true;
        }
        
        // 水平位置随时间线性变化
        Vector2 horizontalPosition = Vector2.Lerp(startPosition, targetPosition, progress);
        
        // 垂直位置遵循抛物线：h = h0 + v0t - 0.5gt²
        // 这里我们使用sin曲线来模拟抛物线，以简化计算
        float height = initialHeight * Mathf.Sin(progress * Mathf.PI);
        
        // 如果高度接近0且进度超过0.5（下降阶段），也切换到着陆层
        if (!hasChangedToLandingLayer && progress > 0.5f && height < initialHeight * 0.2f)
        {
            SetBulletLayer(landingLayer);
            hasChangedToLandingLayer = true;
        }
        
        // 设置最终位置
        transform.position = new Vector3(horizontalPosition.x, horizontalPosition.y, 0) + new Vector3(0, height, 0);
        
        // 调整子弹旋转，使其沿着路径方向
        if (spriteRenderer != null)
        {
            // 计算当前运动方向的切线
            Vector2 tangent = Vector2.Lerp(initialVelocity.normalized, 
                                          new Vector2(initialVelocity.x, initialVelocity.y - gravity * flightTime).normalized, 
                                          progress);
            
            // 根据切线方向旋转子弹
            float angle = Mathf.Atan2(tangent.y, tangent.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        
        // 检查距离限制
        if (useDistanceLimit)
        {
            float distanceTraveled = Vector2.Distance(startPosition, transform.position);
            if (distanceTraveled >= maxDistance)
            {
                OnDestroyBullet();
            }
        }
        
        // 检查生命周期限制
        if (useLifetimeLimit)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= maxLifetime)
            {
                OnDestroyBullet();
            }
        }
    }
    
    // 在碰撞时不立即销毁，而是创建爆炸效果
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        // 检查碰撞对象的队伍标签
        int otherTeam = 0;

        if (other.CompareTag("Enemy"))
        {
            otherTeam = 1;
        }
        else if (other.CompareTag("Player"))
        {
            otherTeam = 0;
        }

        // 避免同队伍子弹造成伤害
        if (team != otherTeam && other is BoxCollider2D)
        {
            // 对目标造成伤害
            ApplyDamage(other, damage);

            // 生成碰撞效果
            if (impactEffect != null)
            {
                Instantiate(impactEffect, transform.position, Quaternion.identity);
            }

            // 销毁子弹
            OnDestroyBullet();
        }
    }
} 