using UnityEngine;

public abstract class AttackModeBase : MonoBehaviour
{
    [Header("基础设置")]
    public float angleDeviation = 10f; // 射击角度偏差
    
    [Header("后坐力设置")]
    public bool enableRecoil = false; // 是否启用后坐力
    public float recoilDistance = 0.2f; // 后坐力移动距离
    public float recoilRecoveryTime = 0.1f; // 恢复原位所需时间
    
    protected Transform bulletsParent;
    protected RemoteAttack remoteAttack;
    private Vector3 originalPosition; // 原始位置
    private bool isRecoiling = false; // 是否正在后坐力状态
    private float recoilTimer = 0f; // 后坐力计时器
    private AudioSource audioSource; // 音频源组件
    private float lastPlayTime = 0f;
    private const float minInterval = 0.2f;
    
    public virtual void Initialize(RemoteAttack attack, Transform bulletsParent)
    {
        this.remoteAttack = attack;
        this.bulletsParent = bulletsParent;
        this.originalPosition = transform.localPosition;
        
        // 获取AudioSource组件
        audioSource = GetComponent<AudioSource>();
    }
    
    // 每个攻击模式必须实现的方法
    public abstract bool CanAttack(); // 是否可以攻击
    public abstract void Attack(Vector2 targetPosition); // 执行攻击
    public abstract void UpdateAttackState(); // 更新攻击状态
    
    // 获取发射方向
    protected Vector2 GetFiringDirection(Vector2 targetPosition)
    {
        // 检查是否有炮管组件
        BasicGunBarrel gunBarrel = GetComponent<BasicGunBarrel>();
        if (gunBarrel != null)
        {
            // 使用炮管的当前朝向作为发射方向
            float angle = gunBarrel.transform.eulerAngles.z - 270f; // 调整为正确的角度
            return new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
        }
        else
        {
            // 默认行为：朝向目标方向
            return ((Vector3)targetPosition - transform.position).normalized;
        }
    }
    
    // 用于生成子弹的通用方法
    protected GameObject CreateBullet(Vector2 position, Vector2 direction)
    {
        GameObject bullet = Instantiate(remoteAttack.bulletPrefab, position, Quaternion.identity, bulletsParent);
        bullet.transform.right = direction;
        
        if (bullet.TryGetComponent(out BulletBase bulletScript))
        {
            bulletScript.team = 0;
        }
        else
        {
            Debug.LogError($"子弹预制体 {bullet.name} 没有BulletBase脚本!");
        }
        
        // 播放开火音效
        PlayFireSound();
        
        // 应用后坐力
        if (enableRecoil && !isRecoiling)
        {
            ApplyRecoil(direction);
        }
        
        return bullet;
    }
    
    // 应用后坐力效果
    protected void ApplyRecoil(Vector2 direction)
    {
        isRecoiling = true;
        recoilTimer = 0f;
        
        // 计算后坐力方向（与射击方向相反）
        Vector3 recoilDirection = -new Vector3(direction.x, direction.y, 0).normalized;
        
        // 设置后坐力位置
        transform.localPosition = originalPosition + recoilDirection * recoilDistance;
    }
    
    // 播放开火音效
    protected void PlayFireSound()
    {
        if (audioSource != null && Time.time - lastPlayTime >= minInterval)
        {
            audioSource.Play();
            lastPlayTime = Time.time;
        }
    }
    
    protected virtual void Update()
    {
        // 处理后坐力恢复
        if (isRecoiling)
        {
            recoilTimer += Time.deltaTime;
            float t = Mathf.Clamp01(recoilTimer / recoilRecoveryTime);
            transform.localPosition = Vector3.Lerp(
                originalPosition + (transform.localPosition - originalPosition).normalized * recoilDistance, 
                originalPosition, 
                t
            );
            
            if (recoilTimer >= recoilRecoveryTime)
            {
                transform.localPosition = originalPosition;
                isRecoiling = false;
            }
        }
    }
} 