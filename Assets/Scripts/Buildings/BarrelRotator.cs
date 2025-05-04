using UnityEngine;

public class BarrelRotator : MonoBehaviour
{
    [Header("旋转设置")]
    public float maxRotationAngle = 60f;       // 最大旋转角度（左右各60度）
    public float rotationSpeed = 5f;           // 旋转速度
    public bool invertRotation = false;        // 是否反转旋转方向
    
    [Header("依赖组件")]
    public AttackRange attackRange;            // 攻击范围组件
    public RemoteAttack remoteAttack;          // 远程攻击组件
    
    private Quaternion defaultRotation;        // 默认旋转
    private Quaternion targetRotation;         // 目标旋转
    private GameObject currentTarget;          // 当前目标
    private float initialAngle;                // 炮管的初始角度
    
    private void Start()
    {
        // 保存默认旋转
        defaultRotation = transform.rotation;
        targetRotation = defaultRotation;
        
        // 自动获取组件
        if (attackRange == null)
        {
            attackRange = GetComponentInParent<AttackRange>();
        }
        
        if (remoteAttack == null)
        {
            remoteAttack = GetComponentInParent<RemoteAttack>();
        }
    }
    
    private void Update()
    {
        // 查找目标
        if (attackRange != null && attackRange.DetectedEnemies.Count > 0)
        {
            currentTarget = GetNearestEnemy();
            
            if (currentTarget != null)
            {
                // 计算敌人相对炮台的位置
                Vector2 relativePos = currentTarget.transform.position - transform.position;
                
                // 计算敌人与炮管之间的角度（以向上为0度）
                float targetAngle = Mathf.Atan2(relativePos.x, relativePos.y) * Mathf.Rad2Deg;
                
                // 偏转方向与敌人位置相反（反转角度）
                targetAngle = -targetAngle;
                
                // 限制在最大旋转角度范围内
                targetAngle = Mathf.Clamp(targetAngle, -maxRotationAngle, maxRotationAngle);
                
                // 应用额外的反转选项（如果需要）
                if (invertRotation)
                {
                    targetAngle = -targetAngle;
                }
                
                // 将偏移角度应用到默认向上的旋转
                Quaternion offsetRotation = Quaternion.Euler(0, 0, targetAngle);
                
                // 设置目标旋转（以向上为基准的旋转）
                targetRotation = offsetRotation;
            }
            else
            {
                // 没有目标时回到默认旋转
                targetRotation = defaultRotation;
            }
        }
        else
        {
            // 没有目标时回到默认旋转
            targetRotation = defaultRotation;
            currentTarget = null;
        }
        
        // 平滑旋转炮管
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
    
    // 获取最近的敌人
    private GameObject GetNearestEnemy()
    {
        GameObject nearest = null;
        float minDistance = float.MaxValue;
        
        foreach (GameObject enemy in attackRange.DetectedEnemies)
        {
            if (enemy != null)
            {
                // 检查距离
                float distance = Vector2.Distance(transform.position, enemy.transform.position);
                
                // 检查是否在有效范围内（如果使用抛物线攻击模式有最小射程）
                bool isInValidRange = true;
                if (remoteAttack != null && remoteAttack.attackMode is ParabolicAttackMode parabolicMode)
                {
                    isInValidRange = distance >= parabolicMode.minimumAttackRange;
                }
                
                if (isInValidRange && distance < minDistance)
                {
                    minDistance = distance;
                    nearest = enemy;
                }
            }
        }
        
        return nearest;
    }
    
    // 绘制辅助线，显示旋转范围
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 upDirection = Vector3.up;
        Vector3 leftLimit = Quaternion.Euler(0, 0, -maxRotationAngle) * upDirection;
        Vector3 rightLimit = Quaternion.Euler(0, 0, maxRotationAngle) * upDirection;
        
        Gizmos.DrawRay(transform.position, leftLimit * 2);
        Gizmos.DrawRay(transform.position, upDirection * 2);
        Gizmos.DrawRay(transform.position, rightLimit * 2);
    }
} 