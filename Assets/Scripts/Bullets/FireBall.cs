using UnityEngine;
using System.Collections;

public class FireBall : BulletBase
{
    [Header("爆炸设置")]
    // 爆炸范围半径
    public float explosionRadius = 0.5f;
    // 爆炸伤害(如果不设置，则使用基础伤害)
    public int explosionDamage = 0;
    // 爆炸效果预制体
    public GameObject explosionEffect;
    // 爆炸层级掩码，用于确定爆炸范围内的目标
    public LayerMask explosionLayerMask;

    // 标记是否已经爆炸过
    private bool hasExploded = false;

    protected override void Start()
    {
        base.Start();
        // 如果未设置爆炸伤害，则使用基础伤害
        if (explosionDamage <= 0)
        {
            explosionDamage = damage;
        }
    }

    // 重写碰撞响应，碰撞后触发爆炸
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        // 检查碰撞对象的队伍标签
        int otherTeam = GetTeamFromTag(other.tag);

        // 避免同队伍子弹造成伤害
        if (team != otherTeam && other is BoxCollider2D)
        {
            // 标记为已爆炸（防止重复爆炸）
            hasExploded = true;
            // 触发爆炸
            CreateExplosion(transform.position);
        }
    }

    // 重写销毁方法，在销毁时也触发爆炸
    protected override void OnDestroyBullet()
    {
        // 如果还没有爆炸过，则触发爆炸
        if (!hasExploded)
        {
            hasExploded = true;
            CreateExplosion(transform.position);
        }
        else
        {
            // 已经爆炸过，直接调用基类方法销毁子弹
            base.OnDestroyBullet();
        }
    }

    // 创建爆炸效果并对范围内敌人造成伤害
    protected void CreateExplosion(Vector2 position)
    {
        // 显示爆炸效果
        if (explosionEffect != null)
        {
            // 实例化爆炸效果并保存引用
            GameObject explosion = Instantiate(explosionEffect, position, Quaternion.identity);
            // 可以根据需要添加销毁爆炸效果的逻辑，例如：
            Destroy(explosion, 0.5f);
        }

        // 使用传统方法造成爆炸伤害
        TraditionalExplosionDamage(position);

        // 爆炸效果完成后销毁子弹
        base.OnDestroyBullet();
    }

    // 传统爆炸伤害方法（仅检测BoxCollider2D）
    private void TraditionalExplosionDamage(Vector2 position)
    {
        // 检测爆炸范围内的所有碰撞体
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(position, explosionRadius, explosionLayerMask);

        foreach (Collider2D hitCollider in hitColliders)
        {
            // 只处理BoxCollider2D类型的碰撞体
            if (hitCollider is BoxCollider2D)
            {
                // 检查是否是敌人
                int otherTeam = GetTeamFromTag(hitCollider.tag);
                // 只对敌方单位造成伤害
                if (team != otherTeam)
                {
                    ApplyDamage(hitCollider, explosionDamage);
                }
            }
        }
    }

    // 根据标签获取队伍编号
    private int GetTeamFromTag(string tag)
    {
        return tag == "Enemy" ? 1 : 0;
    }

    // 可视化爆炸范围（仅在编辑器中）
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}