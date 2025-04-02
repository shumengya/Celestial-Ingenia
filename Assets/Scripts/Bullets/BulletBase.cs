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

    protected Vector2 startPosition;
    protected float elapsedTime = 0f;

    protected virtual void Start()
    {
        startPosition = transform.position;
    }

    protected virtual void Update()
    {
        // 子弹移动
        transform.Translate(Vector2.right * speed * Time.deltaTime);

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
        DestroyBullet();
    }

    // 真正销毁子弹的方法
    protected void DestroyBullet()
    {
        Destroy(gameObject);
    }
} 