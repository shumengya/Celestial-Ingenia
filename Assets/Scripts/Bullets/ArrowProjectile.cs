using UnityEngine;

public class ArrowProjectile : ParabolicBullet
{
    [Header("箭矢设置")]
    public float arrowSpeed = 7f;          // 箭矢速度
    public int arrowDamage = 15;           // 箭矢伤害
    
    [Header("箭矢效果")]
    public bool stickToTarget = true;      // 是否插在目标上
    public float stickDuration = 2f;       // 插在目标上的持续时间
    public GameObject stickEffect;         // 箭矢插入效果
    
    [Header("视觉效果")]
    public bool adjustRotationToTrajectory = true; // 是否根据轨迹调整旋转
    
    [Header("音效")]
    public AudioClip hitSound;             // 击中音效
    public AudioClip missSoundGround;      // 射偏落地音效
    
    [Header("层级控制")]
    public bool useLayerBasedCollision = true; // 是否使用基于层级的碰撞控制
    
    private bool hasHit = false;           // 是否已经击中目标
    private Transform hitTransform;        // 击中的目标变换
    private Vector3 hitLocalPosition;      // 击中位置的本地坐标
    private float hitTime = 0f;            // 击中时间
    private Vector3 lastPosition;          // 上一帧位置
    
    protected override void Start()
    {
        // 调用父类的Start方法
        base.Start();
        
        // 设置箭矢速度和伤害
        bulletSpeed = arrowSpeed;
        damage = arrowDamage;
        
        // 初始位置记录
        lastPosition = transform.position;
        
        // 确保箭矢初始使用飞行层
        if (useLayerBasedCollision)
        {
            SetBulletLayer(flyingLayer);
        }
    }
    
    protected override void Update()
    {
        // 如果已经击中并且设置了粘附
        if (hasHit && stickToTarget)
        {
            // 如果目标仍然存在，跟随目标
            if (hitTransform != null)
            {
                transform.position = hitTransform.TransformPoint(hitLocalPosition);
                
                // 检查是否超过粘附持续时间
                if (Time.time - hitTime > stickDuration)
                {
                    // 时间到，销毁箭矢
                    OnDestroyBullet();
                }
            }
            else
            {
                // 目标已经不存在，直接销毁
                OnDestroyBullet();
            }
        }
        else
        {
            // 保存当前位置，用于下一帧计算方向
            Vector3 currentPosition = transform.position;
            
            // 如果已经启用了层级控制且未切换到着陆层，检查是否需要手动切换层级
            if (useLayerBasedCollision && gameObject.layer == LayerMask.NameToLayer(flyingLayer))
            {
                // 使用射线检测检查下方是否有地面
                RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.5f, LayerMask.GetMask("Ground"));
                if (hit.collider != null)
                {
                    // 如果下方有地面，切换层级
                    SetBulletLayer(landingLayer);
                }
            }
            
            // 调用基类的Update方法，更新位置
            base.Update();
            
            // 根据飞行轨迹调整箭矢旋转
            if (adjustRotationToTrajectory)
            {
                // 计算移动方向
                Vector3 moveDirection = transform.position - lastPosition;
                if (moveDirection.magnitude > 0.01f) // 如果有足够的移动
                {
                    // 计算角度（朝向移动方向）
                    float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.Euler(0, 0, angle);
                }
            }
            
            // 更新上一帧位置
            lastPosition = currentPosition;
        }
    }
    
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        // 如果已经击中，不再处理
        if (hasHit) return;
        
        // 如果此时箭矢层级是飞行层，则不进行碰撞检测（仍在空中飞行）
        if (useLayerBasedCollision && gameObject.layer == LayerMask.NameToLayer(flyingLayer))
        {
            // 只检查是否与地面层级的物体碰撞
            if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                // 如果碰到地面层级的物体，切换层级并处理地面碰撞
                SetBulletLayer(landingLayer);
                HandleGroundHit(other);
            }
            return;
        }
        
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
            HandleEnemyHit(other);
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            HandleGroundHit(other);
        }
    }
    
    // 处理击中敌人
    private void HandleEnemyHit(Collider2D other)
    {
        // 标记为已击中
        hasHit = true;
        hitTime = Time.time;
        
        // 记录击中位置
        if (stickToTarget)
        {
            hitTransform = other.transform;
            // 保存本地坐标，使箭矢相对于目标保持固定位置
            hitLocalPosition = hitTransform.InverseTransformPoint(transform.position);
        }
        
        // 对目标造成伤害
        ApplyDamage(other, damage);
        
        // 播放击中音效
        if (hitSound != null)
        {
            AudioSource.PlayClipAtPoint(hitSound, transform.position);
        }
        
        // 创建击中效果
        if (stickEffect != null)
        {
            Instantiate(stickEffect, transform.position, transform.rotation);
        }
        
        // 如果不需要粘附，直接销毁
        if (!stickToTarget)
        {
            OnDestroyBullet();
        }
    }
    
    // 处理击中地面
    private void HandleGroundHit(Collider2D other)
    {
        // 击中地面
        hasHit = true;
        hitTime = Time.time;
        
        // 播放落地音效
        if (missSoundGround != null)
        {
            AudioSource.PlayClipAtPoint(missSoundGround, transform.position);
        }
        
        // 如果设置了粘附到地面
        if (stickToTarget)
        {
            hitTransform = other.transform;
            hitLocalPosition = hitTransform.InverseTransformPoint(transform.position);
        }
        else
        {
            // 不需要粘附，直接销毁
            OnDestroyBullet();
        }
    }
    
    // 在销毁前清理资源
    protected override void OnDestroyBullet()
    {
        // 先调用基类方法，确保清理干净
        base.OnDestroyBullet();
    }
} 