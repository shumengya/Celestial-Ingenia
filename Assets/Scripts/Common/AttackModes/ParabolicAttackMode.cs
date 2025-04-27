using UnityEngine;
using System.Collections;

public class ParabolicAttackMode : AttackModeBase
{
    [Header("抛物线射击设置")]
    public float fireRate = 2f;               // 射击间隔（秒）
    public float bulletSpeed = 8f;            // 子弹速度
    public float arcHeight = 3f;              // 弹道高度
    public GameObject parabolicBulletPrefab;  // 抛物线子弹预制体
    
    [Header("瞄准预览")]
    public bool showTrajectoryPreview = true; // 是否显示弹道预览
    public int trajectoryPoints = 10;         // 预览点数量
    public GameObject trajectoryPointPrefab;  // 预览点预制体
    
    [Header("多重发射设置")]
    public int burstCount = 1;                // 一次射击发射的子弹数量
    public float burstInterval = 0.1f;        // 连发间隔时间（秒）
    public float positionOffset = 0.2f;       // 每颗子弹位置偏移量
    public float angleOffset = 5f;            // 每颗子弹角度偏移量
    
    private float nextFireTime = 0f;
    private LineRenderer trajectoryLine;      // 预览线渲染器
    private MortarWeapon mortarWeapon;        // 迫击炮武器组件引用
    private bool isFiring = false;            // 是否正在连发中
    
    private void Awake()
    {
        // 初始化弹道预览
        if (showTrajectoryPreview)
        {
            InitializeTrajectoryPreview();
        }
        
        // 获取迫击炮组件
        mortarWeapon = GetComponent<MortarWeapon>();
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
        trajectoryLine.startColor = Color.yellow;
        trajectoryLine.endColor = new Color(1, 1, 0, 0.2f); // 透明黄色
    }
    
    public override bool CanAttack()
    {
        return Time.time >= nextFireTime && !isFiring;
    }
    
    public override void Attack(Vector2 targetPosition)
    {
        // 设置下次射击时间
        nextFireTime = Time.time + fireRate;
        
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
        
        // 播放迫击炮发射效果
        if (mortarWeapon != null)
        {
            mortarWeapon.PlayFiringEffect();
        }
        
        // 生成抛物线子弹
        if (parabolicBulletPrefab != null)
        {
            // 使用自定义子弹预制体
            GameObject bullet = Instantiate(parabolicBulletPrefab, transform.position, Quaternion.identity, bulletsParent);
            
            // 初始化抛物线子弹
            if (bullet.TryGetComponent(out ParabolicBullet parabolicBullet))
            {
                parabolicBullet.team = 0; // 确保设置为玩家队伍(0)
                parabolicBullet.speed = bulletSpeed;
                parabolicBullet.initialHeight = arcHeight;
                parabolicBullet.Initialize(targetPosition, bulletSpeed);
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
                parabolicBullet.speed = bulletSpeed;
                parabolicBullet.initialHeight = arcHeight;
                parabolicBullet.Initialize(targetPosition, bulletSpeed);
            }
        }
    }
    
    public override void UpdateAttackState()
    {
        // 更新瞄准预览
        if (showTrajectoryPreview && trajectoryLine != null)
        {
            // 获取鼠标位置作为目标点
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            UpdateTrajectoryPreview(mousePos);
        }
    }
    
    // 更新弹道预览
    private void UpdateTrajectoryPreview(Vector2 targetPos)
    {
        // 计算开始和结束点
        Vector2 startPos = transform.position;
        Vector2 endPos = targetPos;
        float distance = Vector2.Distance(startPos, endPos);
        
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