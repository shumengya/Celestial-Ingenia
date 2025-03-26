using UnityEngine;

public class FireBall: MonoBehaviour
{
    // 子弹移动速度
    public float speed = 10f;
    // 子弹攻击伤害
    public int damage = 10;
    // 最大存活时间
    public float maxLifetime = 2f;
    // 最大移动距离
    public float maxDistance = 20f;
    // 子弹所属队伍，0 表示玩家，1 表示敌人
    public int team = 0;
    // 是否启用最大存活时间限制
    public bool useLifetimeLimit = true;
    // 是否启用最大移动距离限制
    public bool useDistanceLimit = true;
    // 子弹碰撞后的效果预制体
    public GameObject impactEffect;

    private Vector2 startPosition;
    private float elapsedTime = 0f;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // 子弹移动
        transform.Translate(Vector2.right * speed * Time.deltaTime);

        if (useLifetimeLimit)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= maxLifetime)
            {
                DestroyBullet();
            }
        }

        if (useDistanceLimit)
        {
            float distanceTraveled = Vector2.Distance(startPosition, transform.position);
            if (distanceTraveled >= maxDistance)
            {
                DestroyBullet();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
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
        //Debug.Log("发生碰撞！");
        // 避免同队伍子弹造成伤害
        if (team != otherTeam && other is BoxCollider2D)
        {
            HealthBar targetHealth = other.GetComponentInChildren<HealthBar>();
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(damage);
                
            }

            // 生成碰撞效果
            if (impactEffect != null)
            {
                Instantiate(impactEffect, transform.position, Quaternion.identity);
            }

            // 销毁子弹
            DestroyBullet();
        }
    }

    void DestroyBullet()
    {
        Destroy(gameObject);
    }
}