using UnityEngine;
using System.Collections;

public class ParabolicAttackMode : AttackModeBase
{
    [Header("抛物线射击设置")]
    public float fireRate = 2f;               // 射击间隔（秒）
    public float arcHeight = 3f;              // 弹道高度
    public GameObject parabolicBulletPrefab;  // 抛物线子弹预制体
    public float minimumAttackRange = 15f;    // 最小攻击距离
    public float attackRange = 25f;           // 最大攻击范围
    
    [Header("瞄准预览")]
    public bool showTrajectoryPreview = true; // 是否显示弹道预览
    public Color trajectoryLineColor = new Color(0, 1, 0, 0.2f);
    public int trajectoryPoints = 10;         // 预览点数量
    public GameObject trajectoryPointPrefab;  // 落点预览预制体
    
    [Header("多重发射设置")]
    public int burstCount = 1;                // 一次射击发射的子弹数量
    public float burstInterval = 0.1f;        // 连发间隔时间（秒）
    public float positionOffset = 0.2f;       // 每颗子弹位置偏移量
    public float angleOffset = 5f;            // 每颗子弹角度偏移量
    
    [Header("视觉效果")]
    public GameObject firingEffect;           // 发射效果
    public AudioClip firingSound;             // 发射声音
    
    private float nextFireTime = 0f;
    private LineRenderer trajectoryLine;      // 预览线渲染器
    private bool isFiring = false;            // 是否正在连发中
    private Vector2 nextTargetPosition;       // 下一次射击的目标位置
    private bool hasValidTarget = false;      // 是否有有效的目标
    private GameObject landingPointMarker;    // 落点标记实例
    
    private void Awake()
    {
        // 初始化弹道预览
        if (showTrajectoryPreview)
        {
            InitializeTrajectoryPreview();
        }
    }
    
    // 初始化
    public override void Initialize(RemoteAttack attack, Transform bulletsParentTransform)
    {
        base.Initialize(attack, bulletsParentTransform);
        
        // 设置攻击范围
        AttackRange attackRangeComponent = GetComponent<AttackRange>();
        if (attackRangeComponent != null)
        {
            // 获取碰撞器并设置范围
            CircleCollider2D circleCollider = attackRangeComponent.GetComponent<CircleCollider2D>();
            if (circleCollider != null)
            {
                circleCollider.radius = attackRange;
            }
        }
    }
    
    private void OnDestroy()
    {
        // 清理落点标记
        if (landingPointMarker != null)
        {
            Destroy(landingPointMarker);
            landingPointMarker = null;
        }
    }
    
    // 初始化弹道预览
    private void InitializeTrajectoryPreview()
    {
        // 创建轨迹线渲染器
        GameObject trajectoryLineObj = new GameObject("TrajectoryLine");
        trajectoryLineObj.transform.SetParent(transform);
        trajectoryLine = trajectoryLineObj.AddComponent<LineRenderer>();
        trajectoryLine.startWidth = 0.05f;
        trajectoryLine.endWidth = 0.05f;
        trajectoryLine.positionCount = trajectoryPoints;
        trajectoryLine.material = new Material(Shader.Find("Sprites/Default"));
        trajectoryLine.startColor = trajectoryLineColor;
        trajectoryLine.endColor = trajectoryLineColor;
        trajectoryLine.enabled = false; // 初始时不显示
    }
    
    public override bool CanAttack()
    {
        // 检查发射冷却和是否正在发射
        if (Time.time < nextFireTime || isFiring)
        {
            return false;
        }
        
        return true;
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
        
        // 更新轨迹预览
        if (showTrajectoryPreview)
        {
            UpdateTrajectoryPreview(targetPosition);
        }
        
        // 开始连发协程
        StartCoroutine(FireBurst(targetPosition));
    }
    
    // 连发射击协程
    private IEnumerator FireBurst(Vector2 targetPosition)
    {
        isFiring = true;
        
        for (int i = 0; i < burstCount; i++)
        {
            // 计算当前子弹的目标位置（添加偏移）
            Vector2 offsetDirection = (targetPosition - (Vector2)transform.position).normalized;
            Vector2 perpendicularDirection = new Vector2(-offsetDirection.y, offsetDirection.x);
            
            // 交替左右偏移
            float currentPosOffset = positionOffset * ((i % 2 == 0) ? 1 : -1) * (i / 2 + 0.5f);
            Vector2 offsetTargetPos = targetPosition + perpendicularDirection * currentPosOffset;
            
            // 添加角度偏移
            float currentAngleOffset = angleOffset * ((i % 2 == 0) ? 1 : -1) * (i / 2 + 0.5f);
            
            // 发射单颗子弹
            FireSingleProjectile(offsetTargetPos, currentAngleOffset);
            
            // 等待指定间隔时间
            if (i < burstCount - 1)
            {
                yield return new WaitForSeconds(burstInterval);
            }
        }
        
        isFiring = false;
    }
    
    // 发射单颗子弹
    private void FireSingleProjectile(Vector2 targetPosition, float additionalAngle)
    {
        // 计算发射方向
        Vector2 direction = GetFiringDirection(targetPosition);
        
        // 添加随机角度偏差和指定的额外偏移
        float totalDeviation = Random.Range(-angleDeviation, angleDeviation) + additionalAngle;
        Quaternion rotation = Quaternion.Euler(0, 0, totalDeviation);
        direction = rotation * direction;
        
        // 播放发射效果
        PlayFiringEffect();
        
        // 生成抛物线子弹
        if (parabolicBulletPrefab != null)
        {
            // 使用自定义子弹预制体
            GameObject bullet = Instantiate(parabolicBulletPrefab, transform.position, Quaternion.identity, bulletsParent);
            
            // 初始化抛物线子弹
            if (bullet.TryGetComponent(out ParabolicBullet parabolicBullet))
            {
                parabolicBullet.team = 0; // 确保设置为玩家队伍(0)
                parabolicBullet.initialHeight = arcHeight; // 只设置抛物线高度
                parabolicBullet.Initialize(targetPosition); // 不再传递速度参数
            }
            else
            {
                Debug.LogError("抛物线子弹预制体没有ParabolicBullet脚本!");
                Destroy(bullet);
            }
        }
        else
        {
            // 使用默认子弹预制体
            GameObject bullet = CreateBullet(transform.position, direction);
            
            // 尝试添加ParabolicBullet组件
            ParabolicBullet parabolicBullet = bullet.AddComponent<ParabolicBullet>();
            if (parabolicBullet != null)
            {
                parabolicBullet.team = 0; // 确保设置为玩家队伍(0)
                parabolicBullet.initialHeight = arcHeight; // 只设置抛物线高度
                parabolicBullet.Initialize(targetPosition); // 不再传递速度参数
            }
        }
    }
    
    public override void UpdateAttackState()
    {
        // 更新预览显示
        if (showTrajectoryPreview)
        {
            // 检查是否有有效目标
            bool shouldShowTrajectory = hasValidTarget;
            
            // 更新轨迹线可见性
            if (trajectoryLine != null)
            {
                trajectoryLine.enabled = shouldShowTrajectory;
            }
            
            // 更新落点标记可见性
            if (landingPointMarker != null)
            {
                landingPointMarker.SetActive(shouldShowTrajectory);
            }
            
            // 如果没有有效目标但仍然显示轨迹，隐藏预览
            if (!shouldShowTrajectory)
            {
                HideTrajectoryPreview();
            }
        }
    }
    
    // 隐藏轨迹预览
    private void HideTrajectoryPreview()
    {
        if (trajectoryLine != null)
        {
            trajectoryLine.enabled = false;
        }
        
        if (landingPointMarker != null)
        {
            landingPointMarker.SetActive(false);
        }
    }
    
    // 更新弹道预览
    public void UpdateTrajectoryPreview(Vector2 targetPos)
    {
        // 计算开始和结束点
        Vector2 startPos = transform.position;
        Vector2 endPos = targetPos;
        float distance = Vector2.Distance(startPos, endPos);
        
        // 更新目标位置
        nextTargetPosition = targetPos;
        hasValidTarget = true;
        
        // 更新落点标记
        UpdateLandingPointMarker(endPos);
        
        // 确保轨迹线可见
        if (trajectoryLine != null)
        {
            trajectoryLine.enabled = true;
            
            // 设置预览点
            for (int i = 0; i < trajectoryPoints; i++)
            {
                // 计算水平插值进度
                float progress = (float)i / (trajectoryPoints - 1);
                
                // 水平位置
                Vector2 horizontalPos = Vector2.Lerp(startPos, endPos, progress);
                
                // 高度遵循抛物线：使用正弦函数模拟抛物线
                float height = arcHeight * Mathf.Sin(progress * Mathf.PI);
                
                // 设置点位置
                Vector3 pointPos = new Vector3(horizontalPos.x, horizontalPos.y, 0) + new Vector3(0, height, 0);
                trajectoryLine.SetPosition(i, pointPos);
            }
        }
    }
    
    // 更新落点标记
    private void UpdateLandingPointMarker(Vector2 landingPosition)
    {
        // 如果预制体不存在，不创建标记
        if (trajectoryPointPrefab == null)
        {
            return;
        }
        
        // 如果落点标记不存在，创建一个
        if (landingPointMarker == null)
        {
            landingPointMarker = Instantiate(trajectoryPointPrefab, landingPosition, Quaternion.identity);
            
            // 如果预制体有动画，不要摧毁它（让它循环播放）
            Animator animator = landingPointMarker.GetComponent<Animator>();
            if (animator != null)
            {
                animator.enabled = true;
            }
        }
        else
        {
            // 直接更新落点标记的位置
            landingPointMarker.transform.position = landingPosition;
            landingPointMarker.SetActive(true);
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
    
    // 绘制有效攻击范围
    private void OnDrawGizmosSelected()
    {
        // 绘制最大攻击范围 (蓝色)
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        // 绘制最小攻击范围 (红色)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, minimumAttackRange);
    }
} 