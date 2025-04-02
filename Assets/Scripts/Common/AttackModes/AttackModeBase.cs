using UnityEngine;

public abstract class AttackModeBase : MonoBehaviour
{
    [Header("基础设置")]
    public float angleDeviation = 10f; // 射击角度偏差
    
    protected Transform bulletsParent;
    protected RemoteAttack remoteAttack;
    
    public virtual void Initialize(RemoteAttack attack, Transform bulletsParent)
    {
        this.remoteAttack = attack;
        this.bulletsParent = bulletsParent;
    }
    
    // 每个攻击模式必须实现的方法
    public abstract bool CanAttack(); // 是否可以攻击
    public abstract void Attack(Vector2 targetPosition); // 执行攻击
    public abstract void UpdateAttackState(); // 更新攻击状态
    
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
        
        return bullet;
    }
} 