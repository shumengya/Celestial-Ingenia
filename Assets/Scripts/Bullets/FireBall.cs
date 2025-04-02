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
    
    // 爆炸碰撞器
    private CircleCollider2D explosionCollider;
    
    // 已经造成伤害的目标列表
    private System.Collections.Generic.HashSet<Collider2D> damagedTargets = new System.Collections.Generic.HashSet<Collider2D>();

    protected override void Start()
    {
        base.Start();
        
        // 获取CircleCollider2D组件（假设在初始状态下是禁用的）
        explosionCollider = GetComponent<CircleCollider2D>();
        if (explosionCollider != null)
        {
            explosionCollider.enabled = false;
            // 设置为触发器
            explosionCollider.isTrigger = true;
            // 设置半径与爆炸半径一致
            explosionCollider.radius = explosionRadius;
        }
        
        // 如果未设置爆炸伤害，则使用基础伤害
        if (explosionDamage <= 0)
        {
            explosionDamage = damage;
        }
    }

    // 重写碰撞响应，碰撞后触发爆炸
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        // 判断是否是爆炸碰撞器触发的事件
        if (explosionCollider != null && explosionCollider.enabled && other != explosionCollider)
        {
            // 是爆炸碰撞器触发的，处理爆炸伤害逻辑
            HandleExplosionDamage(other);
            return;
        }
        
        // 正常子弹碰撞逻辑
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
            // 标记为已爆炸（防止重复爆炸）
            hasExploded = true;
            
            // 触发爆炸
            CreateExplosion(transform.position);
            
            // 销毁子弹会由爆炸效果结束后调用
        }
    }
    
    // 处理爆炸伤害
    private void HandleExplosionDamage(Collider2D other)
    {
        // 避免对同一目标重复造成伤害
        if (damagedTargets.Contains(other))
        {
            return;
        }
        
        // 检查是否是敌人
        int otherTeam = 0;
        if (other.CompareTag("Enemy"))
        {
            otherTeam = 1;
        }
        else if (other.CompareTag("Player"))
        {
            otherTeam = 0;
        }
        
        // 只对敌方单位造成伤害
        if (team != otherTeam)
        {
            ApplyDamage(other, explosionDamage);
            damagedTargets.Add(other);
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
            
            // 根据explosionRadius缩放爆炸效果
            // 假设原始预制体设计的半径为1.0
            float originalRadius = 1.0f;
            float scaleFactor = explosionRadius / originalRadius;
            explosion.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
        }

        // 激活爆炸碰撞器
        if (explosionCollider != null)
        {
            explosionCollider.enabled = true;
            // 确保半径与爆炸半径一致
            explosionCollider.radius = explosionRadius;
            
            // 清空已造成伤害的目标列表
            damagedTargets.Clear();
            
            // 爆炸持续时间（基于视觉效果）
            StartCoroutine(DisableExplosionCollider(0.1f));
        }
        else
        {
            // 如果没有爆炸碰撞器，使用传统方法
            TraditionalExplosionDamage(position);
            
            // 爆炸效果完成后销毁子弹
            OnDestroyBullet();
        }
    }
    
    // 传统爆炸伤害方法（作为备用）
    private void TraditionalExplosionDamage(Vector2 position)
    {
        // 检测爆炸范围内的所有碰撞体
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(position, explosionRadius, explosionLayerMask);
        
        foreach (Collider2D hitCollider in hitColliders)
        {
            // 检查是否是敌人
            int otherTeam = 0;
            if (hitCollider.CompareTag("Enemy"))
            {
                otherTeam = 1;
            }
            else if (hitCollider.CompareTag("Player"))
            {
                otherTeam = 0;
            }
            
            // 只对敌方单位造成伤害
            if (team != otherTeam)
            {
                ApplyDamage(hitCollider, explosionDamage);
            }
        }
    }
    
    // 延迟禁用爆炸碰撞器
    private IEnumerator DisableExplosionCollider(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (explosionCollider != null)
        {
            explosionCollider.enabled = false;
        }
        
        // 爆炸效果完成后销毁子弹
        base.OnDestroyBullet();
    }

    // 可视化爆炸范围（仅在编辑器中）
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}