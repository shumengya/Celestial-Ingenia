using UnityEngine;

public class MortarProjectile : ParabolicBullet
{
    [Header("迫击炮设置")]
    public float mortarSpeed = 5f;          // 迫击炮子弹速度
    
    [Header("迫击炮爆炸设置")]
    public float explosionRadius = 2.5f;       // 爆炸范围
    public int explosionDamage = 20;           // 爆炸伤害
    public GameObject explosionEffect;          // 爆炸效果预制体
    public LayerMask explosionLayerMask = -1;    // 爆炸层级掩码，默认为全部层级
    public float explosionForce = 500f;         // 爆炸力
    
    [Header("音效")]
    public AudioClip explosionSound;           // 爆炸音效
    
    [Header("层级控制")]
    public bool useLayerBasedCollision = true; // 是否使用基于层级的碰撞控制
    
    
    [Header("调试")]
    public bool showDebugInfo = true;         // 是否显示调试信息
    
    // 静态调试开关，可以在游戏中全局控制
    public static bool globalDebug = true;
    
    private bool hasExploded = false;          // 是否已经爆炸
    
    protected override void Start()
    {
        // 调用父类的Start方法
        base.Start();
        
        // 如果没有设置爆炸效果，使用默认的爆炸效果
        if (explosionEffect == null)
        {
            // 尝试查找项目中的爆炸效果
            //explosionEffect = Resources.Load<GameObject>("Effects/Explosion");
        }
        
        // 确保炮弹初始使用飞行层
        if (useLayerBasedCollision)
        {
            SetBulletLayer(flyingLayer);
        }
        
        // 如果没有设置爆炸层级掩码，设置为包含敌人层的默认值
        if (explosionLayerMask.value == -1 || explosionLayerMask.value == 0)
        {
            // 获取敌人层的索引
            int enemyLayer = LayerMask.NameToLayer("Enemy");
            if (enemyLayer != -1)
            {
                // 如果找到敌人层，创建掩码
                explosionLayerMask = 1 << enemyLayer; // 只包含敌人层
            }
            else
            {
                // 包含所有可能包含敌人的层
                explosionLayerMask = LayerMask.GetMask("Default", "Enemy", "Player");
            }
            
            if (showDebugInfo || globalDebug)
            {
                Debug.Log($"已设置爆炸层级掩码为: {explosionLayerMask.value}，对应层: {LayerMaskToString(explosionLayerMask)}");
            }
        }
        
    }
    
    protected override void Update()
    {
        base.Update();
        
        // 如果已经启用了层级控制且未切换到着陆层，检查是否需要手动切换层级
        if (useLayerBasedCollision && gameObject.layer == LayerMask.NameToLayer(flyingLayer))
        {
            // 使用射线检测检查下方是否有地面
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.5f, LayerMask.GetMask("Ground"));
            if (hit.collider != null)
            {
                // 如果下方有地面，切换层级
                if (showDebugInfo || globalDebug) Debug.Log("接近地面，切换层级");
                SetBulletLayer(landingLayer);
            }
        }
    }
    
    // 在销毁子弹时清理资源
    protected override void OnDestroyBullet()
    {
        if (!hasExploded)
        {
            Explode();
        }
        else
        {   // 已经爆炸过，直接调用基类方法销毁子弹
            base.OnDestroyBullet();
        }
    }
    
    // 将LayerMask转换为字符串表示，用于调试
    private string LayerMaskToString(LayerMask mask)
    {
        string result = "";
        for (int i = 0; i < 32; i++)
        {
            // 检查第i位是否为1
            if ((mask.value & (1 << i)) != 0)
            {
                result += LayerMask.LayerToName(i) + ", ";
            }
        }
        if (result.Length > 0)
        {
            result = result.Substring(0, result.Length - 2); // 移除最后的逗号和空格
        }
        return result;
    }
    
    // 重写碰撞响应，碰撞后触发爆炸
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (showDebugInfo || globalDebug)
        {
            Debug.Log($"炮弹碰撞到: {other.gameObject.name}，层级: {LayerMask.LayerToName(other.gameObject.layer)}，标签: {other.tag}");
        }
        
        // 如果此时炮弹层级是Projectile，则不进行碰撞检测（仍在空中飞行）
        if (useLayerBasedCollision && gameObject.layer == LayerMask.NameToLayer(flyingLayer))
        {
            // 检查是否与地面碰撞
            if (other.gameObject.CompareTag("Ground"))
            {
                if (showDebugInfo || globalDebug) Debug.Log("碰到地面，切换层级并爆炸");
                SetBulletLayer(landingLayer);
                Explode();
            }
            return;
        }
        
        // 检查碰撞对象的队伍标签
        int otherTeam = 0;

        if (other.CompareTag("Enemy"))
        {
            otherTeam = 1;
            if (showDebugInfo || globalDebug) Debug.Log("碰到敌人，准备爆炸");
        }
        else if (other.CompareTag("Player"))
        {
            otherTeam = 0;
        }

        // 避免同队伍子弹造成伤害
        if (team != otherTeam)
        {
            // 触发爆炸
            Explode();
        }
    }
    
    // 爆炸方法
    void Explode()
    {
        if (hasExploded) return; // 防止重复爆炸
        
        hasExploded = true;
        

        // 生成爆炸效果
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }
        
        // 播放爆炸音效
        if (explosionSound != null)
        {
            AudioSource.PlayClipAtPoint(explosionSound, transform.position);
        }
        
        if (showDebugInfo || globalDebug) 
        {
            Debug.Log($"爆炸位置: {transform.position}，爆炸半径: {explosionRadius}");
            Debug.Log($"爆炸层级掩码: {explosionLayerMask.value}，对应层: {LayerMaskToString(explosionLayerMask)}");
        }
        
        // 检测爆炸范围内的物体
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius, explosionLayerMask);
        
        if (showDebugInfo || globalDebug) Debug.Log($"检测到爆炸范围内的碰撞体数量: {colliders.Length}");
        
        
        foreach (Collider2D hit in colliders)
        {
            if (showDebugInfo || globalDebug) 
            {
                Debug.Log($"碰撞体: {hit.gameObject.name}，层级: {LayerMask.LayerToName(hit.gameObject.layer)}，标签: {hit.tag}");
            }
            
            // 检查队伍标签，避免伤害友方单位
            int hitTeam = -1;
            if (hit.CompareTag("Player") || hit.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                hitTeam = 0; // 玩家队伍
            }
            else if (hit.CompareTag("Enemy") || hit.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                hitTeam = 1; // 敌人队伍
            }
            
            // 如果是友方单位，跳过伤害计算
            if (hitTeam == team)
            {
                if (showDebugInfo || globalDebug) Debug.Log($"跳过友方单位: {hit.gameObject.name}");
                continue;
            }
            
            // 尝试多种方式获取健康值组件
            HealthBar healthBar = FindHealthBarInObject(hit.gameObject);
            
            // 对范围内的目标造成伤害
            if (healthBar != null)
            {
                int actualDamage = explosionDamage; 

                if (showDebugInfo || globalDebug) 
                {
                    Debug.Log($"对 {hit.gameObject.name} 造成 {actualDamage} 点伤害");
                }
                // 应用伤害
                healthBar.TakeDamage(actualDamage);
            }
        }      
        // 销毁炮弹
        DestroyBullet();
    }
    
    // 辅助方法：查找对象及其子对象中的HealthBar组件
    private HealthBar FindHealthBarInObject(GameObject obj)
    {
        // 直接获取
        HealthBar healthBar = obj.GetComponent<HealthBar>();
        if (healthBar != null) return healthBar;
        
        // 在子对象中查找
        healthBar = obj.GetComponentInChildren<HealthBar>();
        if (healthBar != null) return healthBar;
        
        // 在父对象中查找
        healthBar = obj.GetComponentInParent<HealthBar>();
        if (healthBar != null) return healthBar;   
        return null;
    }
    
    // 绘制爆炸范围辅助线
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
    
    // 辅助接口定义
    private interface IDamageable
    {
        void TakeDamage(int damage);
    }
} 