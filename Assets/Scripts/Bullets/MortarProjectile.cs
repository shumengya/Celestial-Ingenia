using UnityEngine;

public class MortarProjectile : ParabolicBullet
{
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
    
    [Header("拖尾效果")]
    public bool useTrailEffect = true;         // 是否使用拖尾效果
    public GameObject trailEffectPrefab;       // 拖尾效果预制体
    
    [Header("调试")]
    public bool showDebugInfo = true;         // 是否显示调试信息
    
    // 静态调试开关，可以在游戏中全局控制
    public static bool globalDebug = true;
    
    private bool hasExploded = false;          // 是否已经爆炸
    private GameObject trailInstance;          // 拖尾实例
    private ParticleSystem trailParticleSystem; // 拖尾粒子系统引用
    
    protected override void Start()
    {
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
        
        // 初始化拖尾效果
        if (useTrailEffect && trailEffectPrefab != null)
        {
            // 实例化拖尾预制体并设置为子对象
            trailInstance = Instantiate(trailEffectPrefab, transform.position, Quaternion.identity, transform);
            // 获取粒子系统引用以便后续控制
            trailParticleSystem = trailInstance.GetComponent<ParticleSystem>();
            
            if (trailParticleSystem == null)
            {
                // 尝试在子对象中查找粒子系统
                trailParticleSystem = trailInstance.GetComponentInChildren<ParticleSystem>();
            }
            
            if (trailParticleSystem == null && showDebugInfo)
            {
                Debug.LogWarning("拖尾预制体中未找到粒子系统组件");
            }
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
    
    // 重写Update方法，以便在接近地面时手动切换层级
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
    
    // 重写销毁方法，在销毁时也触发爆炸
    protected override void OnDestroyBullet()
    {
        if (!hasExploded)
        {
            Explode();
        }
        else
        {
            // 已经爆炸过，直接调用基类方法销毁子弹
            base.OnDestroyBullet();
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
        
        // 如果没有检测到任何碰撞体，尝试使用另一种方式进行检测
        if (colliders.Length == 0 && globalDebug)
        {
            Debug.LogWarning("未检测到任何碰撞体，尝试使用更大范围和全部层级进行检测");
            // 使用更大的爆炸半径和所有层进行二次检测
            colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius * 1.5f, -1);
            Debug.Log($"二次检测结果: {colliders.Length} 个碰撞体");
        }
        
        bool causedAnyDamage = false;
        
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
                causedAnyDamage = true;
            }
            else
            {
                // 尝试查找更通用的伤害接口或组件
                var damageable = hit.GetComponent<IDamageable>();
                if (damageable != null && hitTeam != team)
                {
                    int actualDamage = explosionDamage;
                    if (showDebugInfo || globalDebug) Debug.Log($"通过接口对 {hit.gameObject.name} 造成伤害: {actualDamage}");
                    damageable.TakeDamage(actualDamage);
                    causedAnyDamage = true;
                }
                else if (showDebugInfo || globalDebug)
                {
                    Debug.LogWarning($"无法在 {hit.gameObject.name} 上找到健康值组件");
                    
                    // 列出所有组件，帮助诊断
                    Component[] components = hit.GetComponents<Component>();
                    Debug.Log($"{hit.gameObject.name} 上的组件: {components.Length}个");
                    foreach (var component in components)
                    {
                        Debug.Log($"  - {component.GetType().Name}");
                    }
                }
            }
            
            // 应用爆炸力 - 只对敌方应用爆炸力
            if (hitTeam != team)
            {
                Rigidbody2D rb = hit.attachedRigidbody;
                if (rb != null)
                {
                    Vector2 direction = (hit.transform.position - transform.position).normalized;
                    rb.AddForce(direction * explosionForce);
                    if (showDebugInfo || globalDebug) Debug.Log($"对 {hit.gameObject.name} 施加爆炸力: {explosionForce}");
                }
            }
        }
        
        if (!causedAnyDamage && globalDebug)
        {
            Debug.LogWarning("爆炸没有造成任何伤害！");
            
            // 尝试直接查找所有Enemy标签的对象
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            Debug.Log($"场景中有 {enemies.Length} 个Enemy标签对象");
            
            foreach (var enemy in enemies)
            {
                float distance = Vector2.Distance(transform.position, enemy.transform.position);
                Debug.Log($"Enemy: {enemy.name}，距离: {distance}，层级: {LayerMask.LayerToName(enemy.layer)}");
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
        
        // 特殊处理：如果对象有ExplodeBug组件，尝试在其子对象中查找
        ExplodeBug bug = obj.GetComponent<ExplodeBug>();
        if (bug != null)
        {
            foreach (Transform child in obj.transform)
            {
                healthBar = child.GetComponent<HealthBar>();
                if (healthBar != null) return healthBar;
            }
        }
        
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