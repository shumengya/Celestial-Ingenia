using UnityEngine;

public class BulletBase : MonoBehaviour
{
    [Header("基础属性")]
    // 子弹移动速度
    public float speed = 10f;
    // 子弹攻击伤害
    public int damage = 10;
    // 最大存活时间
    [Header("子弹限制")]
    public bool useLifetimeLimit = true;
    // 是否启用最大移动距离限制
    public bool useDistanceLimit = true;
    public float maxLifetime = 2f;
    // 最大移动距离
    public float maxDistance = 20f;
    // 子弹所属队伍，0 表示玩家，1 表示敌人
    [Header("其他")]
    public int team = 0;
    // 是否启用最大存活时间限制

    // 子弹碰撞后的效果预制体
    public GameObject impactEffect;

    [Header("拖尾效果")]
    public bool useTrailEffect = false;         // 是否使用拖尾效果
    public GameObject trailEffectPrefab;       // 拖尾效果预制体
    public bool detachTrailFromProjectile = true; // 是否将拖尾效果与子弹分离

    protected Vector2 startPosition;
    protected float elapsedTime = 0f;

    // 拖尾效果相关变量
    protected GameObject trailInstance;          // 拖尾实例
    protected ParticleSystem trailParticleSystem; // 拖尾粒子系统引用
    protected Transform independentTrailParent;   // 独立的拖尾父物体

    protected virtual void Start()
    {
        startPosition = transform.position;
        
        // 初始化拖尾效果
        if (useTrailEffect)
        {
            InitializeTrailEffect();
        }
    }

    // 初始化拖尾效果
    protected virtual void InitializeTrailEffect()
    {
        if (useTrailEffect && trailEffectPrefab != null)
        {
            if (detachTrailFromProjectile)
            {
                // 创建一个独立的父物体来保持拖尾不旋转
                independentTrailParent = new GameObject("TrailParent").transform;
                independentTrailParent.position = transform.position;
                
                // 实例化拖尾预制体并设置为独立父物体的子对象
                trailInstance = Instantiate(trailEffectPrefab, transform.position, Quaternion.identity, independentTrailParent);
            }
            else
            {
                // 传统方式：直接将拖尾附着到子弹上
                trailInstance = Instantiate(trailEffectPrefab, transform.position, Quaternion.identity, transform);
            }
            
            // 获取粒子系统引用以便后续控制
            trailParticleSystem = trailInstance.GetComponent<ParticleSystem>();
            
            if (trailParticleSystem == null)
            {
                // 尝试在子对象中查找粒子系统
                trailParticleSystem = trailInstance.GetComponentInChildren<ParticleSystem>();
            }
            
            // 设置粒子系统的模拟空间为世界空间，避免受父物体旋转影响
            if (trailParticleSystem != null)
            {
                var mainModule = trailParticleSystem.main;
                mainModule.simulationSpace = ParticleSystemSimulationSpace.World;
            }
        }
    }

    protected virtual void Update()
    {
        // 子弹移动
        transform.Translate(Vector2.right * speed * Time.deltaTime);
        
        // 更新独立拖尾的位置
        if (useTrailEffect && detachTrailFromProjectile && independentTrailParent != null && trailInstance != null)
        {
            // 只更新拖尾的位置，不更新旋转
            trailInstance.transform.position = transform.position;
        }

        if (useLifetimeLimit)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= maxLifetime)
            {
                OnDestroyBullet();
            }
        }

        if (useDistanceLimit)
        {
            float distanceTraveled = Vector2.Distance(startPosition, transform.position);
            if (distanceTraveled >= maxDistance)
            {
                OnDestroyBullet();
            }
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
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

    // 应用伤害的方法，可以被子类重写
    protected virtual void ApplyDamage(Collider2D target, int damageAmount)
    {
        HealthBar targetHealth = target.GetComponentInChildren<HealthBar>();
        if (targetHealth != null)
        {
            targetHealth.TakeDamage(damageAmount);
        }
    }

    // 在销毁前可能需要执行的额外逻辑
    protected virtual void OnDestroyBullet()
    {
        // 处理拖尾效果的销毁
        HandleTrailDestruction();
        
        // 实际销毁子弹
        DestroyBullet();
    }
    
    // 处理拖尾效果的销毁
    protected virtual void HandleTrailDestruction()
    {
        // 如果使用分离的拖尾，让拖尾效果持续存在一段时间
        if (useTrailEffect && detachTrailFromProjectile && trailInstance != null)
        {
            // 如果有粒子系统，等待粒子播放完毕后销毁
            if (trailParticleSystem != null)
            {
                // 停止发射新粒子
                trailParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                
                // 分离拖尾父物体，让其独立存在一段时间
                independentTrailParent.SetParent(null);
                
                // 一段时间后销毁独立拖尾
                float destroyDelay = trailParticleSystem.main.duration + trailParticleSystem.main.startLifetime.constantMax;
                Destroy(independentTrailParent.gameObject, destroyDelay);
            }
            else
            {
                // 如果没有粒子系统，直接销毁
                Destroy(independentTrailParent.gameObject);
            }
        }
    }

    // 真正销毁子弹的方法
    protected void DestroyBullet()
    {
        Destroy(gameObject);
    }
} 