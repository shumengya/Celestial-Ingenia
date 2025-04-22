using UnityEngine;

public class MortarWeapon : MonoBehaviour
{
    [Header("迫击炮设置")]
    public GameObject mortarPrefab;          // 抛射物预制体
    public ParabolicAttackMode attackMode;   // 抛物线攻击模式
    public float attackRange = 15f;          // 攻击范围
    
    [Header("视觉效果")]
    public GameObject firingEffect;          // 发射效果
    public AudioClip firingSound;            // 发射声音
    
    private RemoteAttack remoteAttack;       // 远程攻击组件
    private Transform bulletsParent;         // 子弹的父物体
    
    private void Start()
    {
        // 初始化组件
        remoteAttack = GetComponent<RemoteAttack>();
        if (remoteAttack == null)
        {
            remoteAttack = gameObject.AddComponent<RemoteAttack>();
        }
        
        // 设置远程攻击组件的子弹预制体
        if (mortarPrefab != null)
        {
            remoteAttack.bulletPrefab = mortarPrefab;
        }
        
        // 保险获取子弹的父物体
        if (bulletsParent == null)
        {
            GameObject bulletsParentObj = GameObject.Find("PlayerBullets");
            bulletsParent = bulletsParentObj.transform;
            remoteAttack.BulletsParent = bulletsParent;
        }
        
        // 初始化攻击模式
        if (attackMode == null)
        {
            attackMode = gameObject.AddComponent<ParabolicAttackMode>();
        }
        
        // 设置抛物线攻击模式
        attackMode.arcHeight = 3f;              // 设置弹道高度
        attackMode.fireRate = 2f;               // 设置射击间隔
        attackMode.bulletSpeed = 5f;            // 设置子弹速度
        attackMode.showTrajectoryPreview = true; // 启用弹道预览
        
        // 设置远程攻击组件的攻击模式
        remoteAttack.SetAttackMode(attackMode);
        
        // 确保攻击范围组件存在
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
    
    // 播放发射效果
    public void PlayFiringEffect()
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
} 