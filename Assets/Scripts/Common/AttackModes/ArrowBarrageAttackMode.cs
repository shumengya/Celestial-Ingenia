using UnityEngine;
using System.Collections;

public class ArrowBarrageAttackMode : AttackModeBase
{
    [Header("万箭齐发设置")]
    public float fireRate = 3f;                // 射击间隔（秒）
    public GameObject arrowPrefab;             // 箭矢预制体
    public float arcHeight = 4f;               // 弹道高度
    public float minimumAttackRange = 10f;     // 最小攻击距离
    public float attackRange = 30f;            // 最大攻击范围
    
    [Header("箭雨设置")]
    public int arrowCount = 15;                // 每次发射的箭矢数量
    public float spreadRadius = 5f;            // 散布半径
    public float delayBetweenArrows = 0.05f;   // 箭矢发射间隔
    public bool randomizeArrowPositions = true; // 随机化箭矢落点
    
    [Header("发射角度设置")]
    public float baseShootingAngle = 45f;      // 基础发射角度
    public float angleVariation = 15f;         // 角度变化范围
    public bool useArcFormation = false;       // 使用弧形阵列发射
    public int arcRows = 3;                    // 弧形的行数
    
    [Header("瞄准预览")]
    public bool showAttackAreaPreview = true;  // 是否显示攻击区域预览
    public Color previewColor = new Color(1f, 0.5f, 0, 0.3f); // 预览颜色（橙色半透明）
    public GameObject areaTargetPrefab;        // 区域目标指示器预制体
    
    [Header("视觉效果")]
    public GameObject firingEffect;            // 发射效果
    public AudioClip firingSound;              // 发射声音
    
    private float nextFireTime = 0f;
    private bool isFiring = false;
    private Vector2 nextTargetPosition;
    private bool hasValidTarget = false;
    private GameObject areaMarker;              // 区域标记
    
    public override void Initialize(RemoteAttack attack, Transform bulletsParentTransform)
    {
        base.Initialize(attack, bulletsParentTransform);
        
        // 设置攻击范围
        AttackRange attackRangeComponent = GetComponent<AttackRange>();
        if (attackRangeComponent != null)
        {
            CircleCollider2D circleCollider = attackRangeComponent.GetComponent<CircleCollider2D>();
            if (circleCollider != null)
            {
                circleCollider.radius = attackRange;
            }
        }
        
        // 初始化区域预览
        if (showAttackAreaPreview && areaTargetPrefab == null)
        {
            // 创建默认的区域指示器
            GameObject defaultMarker = new GameObject("DefaultAreaMarker");
            SpriteRenderer renderer = defaultMarker.AddComponent<SpriteRenderer>();
            renderer.sprite = CreateCircleSprite(spreadRadius);
            renderer.color = previewColor;
            
            // 存储作为预制体
            areaTargetPrefab = defaultMarker;
            areaTargetPrefab.SetActive(false);
        }
    }
    
    private void OnDestroy()
    {
        // 清理区域标记
        if (areaMarker != null)
        {
            Destroy(areaMarker);
        }
    }
    
    // 创建圆形精灵用于默认的区域指示器
    private Sprite CreateCircleSprite(float radius)
    {
        int resolution = 128;
        Texture2D texture = new Texture2D(resolution, resolution);
        
        Color transparent = new Color(1, 1, 1, 0);
        Color circleColor = Color.white;
        
        // 填充纹理
        for (int x = 0; x < resolution; x++)
        {
            for (int y = 0; y < resolution; y++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), new Vector2(resolution / 2, resolution / 2));
                
                // 区域内设为白色，区域外设为透明
                if (distance <= resolution / 2)
                {
                    texture.SetPixel(x, y, circleColor);
                }
                else
                {
                    texture.SetPixel(x, y, transparent);
                }
            }
        }
        
        texture.Apply();
        
        // 创建精灵
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, resolution, resolution), new Vector2(0.5f, 0.5f));
        return sprite;
    }
    
    public override bool CanAttack()
    {
        // 检查发射冷却和是否正在发射
        return Time.time >= nextFireTime && !isFiring;
    }
    
    public override void Attack(Vector2 targetPosition)
    {
        // 检查目标是否在最小射程之外
        float distanceToTarget = Vector2.Distance(transform.position, targetPosition);
        if (distanceToTarget < minimumAttackRange)
        {
            // 目标太近，无法攻击
            return;
        }
        
        // 设置下次射击时间
        nextFireTime = Time.time + fireRate;
        
        // 保存目标位置
        nextTargetPosition = targetPosition;
        hasValidTarget = true;
        
        // 更新区域预览
        if (showAttackAreaPreview)
        {
            UpdateAreaPreview(targetPosition);
        }
        
        // 开始箭雨协程
        StartCoroutine(FireArrowBarrage(targetPosition));
    }
    
    // 更新区域预览
    private void UpdateAreaPreview(Vector2 targetPos)
    {
        if (areaTargetPrefab == null)
        {
            return;
        }
        
        // 如果区域标记不存在，创建一个
        if (areaMarker == null)
        {
            areaMarker = Instantiate(areaTargetPrefab);
            areaMarker.SetActive(true);
            
            // 设置大小匹配散布半径
            areaMarker.transform.localScale = new Vector3(
                spreadRadius * 2 / areaTargetPrefab.GetComponent<SpriteRenderer>().sprite.bounds.size.x,
                spreadRadius * 2 / areaTargetPrefab.GetComponent<SpriteRenderer>().sprite.bounds.size.y,
                1f
            );
        }
        
        // 更新位置
        areaMarker.transform.position = targetPos;
    }
    
    // 发射箭雨协程
    private IEnumerator FireArrowBarrage(Vector2 targetPosition)
    {
        isFiring = true;
        
        // 播放发射效果
        PlayFiringEffect();
        
        if (useArcFormation)
        {
            // 使用弧形阵列发射
            yield return StartCoroutine(FireArrowArcFormation(targetPosition));
        }
        else
        {
            // 使用常规方式发射
            // 发射多支箭
            for (int i = 0; i < arrowCount; i++)
            {
                // 计算随机位置偏移
                Vector2 offsetPos = targetPosition;
                if (randomizeArrowPositions)
                {
                    float angle = Random.Range(0f, 360f);
                    float distance = Random.Range(0f, spreadRadius);
                    Vector2 offset = new Vector2(
                        Mathf.Cos(angle * Mathf.Deg2Rad) * distance,
                        Mathf.Sin(angle * Mathf.Deg2Rad) * distance
                    );
                    offsetPos += offset;
                }
                else
                {
                    // 均匀分布在圆形区域内
                    float angle = (360f / arrowCount) * i;
                    float distance = spreadRadius * 0.8f; // 使用80%的半径让分布更均匀
                    Vector2 offset = new Vector2(
                        Mathf.Cos(angle * Mathf.Deg2Rad) * distance,
                        Mathf.Sin(angle * Mathf.Deg2Rad) * distance
                    );
                    offsetPos += offset;
                }
                
                // 发射单支箭，使用不同的高度
                float heightVariation = Random.Range(-0.5f, 0.5f);
                FireSingleArrow(offsetPos, arcHeight + heightVariation);
                
                // 等待指定间隔
                yield return new WaitForSeconds(delayBetweenArrows);
            }
        }
        
        // 等待一小段时间确保所有箭都发射完毕
        yield return new WaitForSeconds(0.5f);
        
        isFiring = false;
    }
    
    // 弧形阵列发射
    private IEnumerator FireArrowArcFormation(Vector2 targetPosition)
    {
        // 计算到目标的方向
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        
        // 计算垂直于方向的向量（用于构建弧形）
        Vector2 perpendicular = new Vector2(-direction.y, direction.x);
        
        // 每行的箭矢数量
        int arrowsPerRow = arrowCount / arcRows;
        if (arrowsPerRow < 1) arrowsPerRow = 1;
        
        // 计算弧形宽度
        float arcWidth = spreadRadius * 2;
        
        // 为每一行发射箭矢
        for (int row = 0; row < arcRows; row++)
        {
            // 计算当前行的弧形距离
            float rowDistance = targetPosition.magnitude - (spreadRadius / 2) + (spreadRadius * row / (arcRows - 1));
            if (arcRows == 1) rowDistance = targetPosition.magnitude;
            
            // 弧形的中心点
            Vector2 rowCenter = (Vector2)transform.position + direction * rowDistance;
            
            // 发射当前行的箭矢
            for (int i = 0; i < arrowsPerRow; i++)
            {
                // 计算在弧上的位置
                float arcPosition = -arcWidth / 2 + arcWidth * i / (arrowsPerRow - 1);
                if (arrowsPerRow == 1) arcPosition = 0;
                
                Vector2 arrowPos = rowCenter + perpendicular * arcPosition;
                
                // 增加一些随机变化
                if (randomizeArrowPositions)
                {
                    arrowPos += new Vector2(
                        Random.Range(-spreadRadius * 0.1f, spreadRadius * 0.1f),
                        Random.Range(-spreadRadius * 0.1f, spreadRadius * 0.1f)
                    );
                }
                
                // 随机高度变化
                float heightVar = Random.Range(-0.3f, 0.3f);
                
                // 发射箭矢
                FireSingleArrow(arrowPos, arcHeight + heightVar);
                
                // 等待指定间隔
                yield return new WaitForSeconds(delayBetweenArrows);
            }
            
            // 行与行之间的额外延迟
            yield return new WaitForSeconds(delayBetweenArrows * 2);
        }
    }
    
    // 发射单支箭
    private void FireSingleArrow(Vector2 targetPosition, float height)
    {
        // 计算基础发射角度和添加随机变化
        float shootingAngle = baseShootingAngle + Random.Range(-angleVariation, angleVariation);
        
        // 添加随机角度偏差
        float deviation = Random.Range(-angleDeviation, angleDeviation);
        
        // 生成箭矢
        if (arrowPrefab != null)
        {
            // 使用自定义箭矢预制体
            GameObject arrow = Instantiate(arrowPrefab, transform.position, Quaternion.identity, bulletsParent);
            
            // 初始化抛物线子弹
            if (arrow.TryGetComponent(out ParabolicBullet parabolicBullet))
            {
                // 设置队伍为玩家
                parabolicBullet.team = 0;
                
                // 设置初始抛物线高度
                parabolicBullet.initialHeight = height;
                
                // 初始化目标位置
                parabolicBullet.Initialize(targetPosition);
            }
            else
            {
                Debug.LogError("箭矢预制体没有ParabolicBullet脚本!");
                Destroy(arrow);
            }
        }
        else
        {
            // 使用默认子弹预制体
            GameObject arrow = CreateBullet(transform.position, Vector2.up);
            
            // 添加抛物线组件
            ParabolicBullet parabolicBullet = arrow.AddComponent<ParabolicBullet>();
            if (parabolicBullet != null)
            {
                parabolicBullet.team = 0;
                parabolicBullet.initialHeight = height;
                parabolicBullet.Initialize(targetPosition);
            }
        }
    }
    
    public override void UpdateAttackState()
    {
        // 更新区域预览可见性
        if (areaMarker != null)
        {
            areaMarker.SetActive(hasValidTarget);
        }
    }
    
    // 播放发射效果
    private void PlayFiringEffect()
    {
        if (firingEffect != null)
        {
            Instantiate(firingEffect, transform.position, Quaternion.identity);
        }
        
        if (firingSound != null)
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.PlayOneShot(firingSound);
            }
        }
    }
    
    // 绘制攻击范围
    private void OnDrawGizmosSelected()
    {
        // 绘制最大攻击范围 (蓝色)
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        // 绘制最小攻击范围 (红色)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, minimumAttackRange);
        
        // 绘制散布半径示例 (橙色)
        if (hasValidTarget)
        {
            Gizmos.color = new Color(1f, 0.5f, 0, 0.5f);
            Gizmos.DrawWireSphere(nextTargetPosition, spreadRadius);
        }
    }
} 