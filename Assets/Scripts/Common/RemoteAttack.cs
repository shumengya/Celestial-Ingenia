using UnityEngine;

public class RemoteAttack : MonoBehaviour
{
    [Header("攻击设置")]
    public GameObject bulletPrefab;
    public Transform BulletsParent;
    
    [Header("依赖组件")]
    [SerializeField]
    private AttackRange attackRange;
    [SerializeField]
    public AttackModeBase attackMode; // 当前使用的攻击模式

    private GameObject currentTarget; // 当前攻击目标
    private Vector2 lastTargetPosition; // 上一次目标的位置
    private bool isUpdatingPreview = false; // 防止无限递归

    private void Start()
    {

        GameObject playerBullets = GameObject.FindWithTag("PlayerBullets");
        BulletsParent = playerBullets.transform;

        // 自动获取组件（如果未手动赋值）
        if (attackRange == null)
        {
            attackRange = GetComponent<AttackRange>();
        }
        
        // 初始化攻击模式
        if (attackMode != null)
        {
            attackMode.Initialize(this, BulletsParent);
        }
        else
        {
            Debug.LogError("没有设置攻击模式组件!");
        }
    }

    private void Update()
    {
        // 检查建筑是否可以攻击（如果在建造中则不能攻击）
        BuildingBase building = GetComponentInParent<BuildingBase>();
        if (building != null && !building.CanAttack())
        {
            return; // 建筑正在建造中，不能攻击
        }
        
        // 更新攻击模式状态
        if (attackMode != null)
        {
            attackMode.UpdateAttackState();
        }
        
        // 检查当前目标是否还存在
        if (currentTarget == null || !attackRange.DetectedEnemies.Contains(currentTarget))
        {
            // 当前目标不存在或者不在攻击范围内，选择新的目标
            currentTarget = GetValidEnemyInRange();
            
            // 如果找到了新目标，且是抛物线攻击模式，更新预览轨迹
            if (currentTarget != null && attackMode is ParabolicAttackMode parabolicMode && !isUpdatingPreview)
            {
                Vector2 targetPos = currentTarget.transform.position;
                lastTargetPosition = targetPos;
                
                // 更新预览轨迹，但不实际发射
                isUpdatingPreview = true;
                parabolicMode.UpdateTrajectoryPreview(targetPos);
                isUpdatingPreview = false;
            }
        }
        
        // 如果有目标且目标位置已经改变显著，更新预览轨迹
        if (currentTarget != null && !isUpdatingPreview)
        {
            Vector2 currentPos = currentTarget.transform.position;
            float positionDelta = Vector2.Distance(currentPos, lastTargetPosition);
            
            // 如果位置变化明显，更新预览
            if (positionDelta > 0.5f && attackMode is ParabolicAttackMode parabolicMode)
            {
                lastTargetPosition = currentPos;
                
                // 更新目标位置预览，但不攻击
                isUpdatingPreview = true;
                parabolicMode.UpdateTrajectoryPreview(currentPos);
                isUpdatingPreview = false;
            }
        }

        // 检查是否可以攻击且有目标，避免重复调用
        if (!isUpdatingPreview && attackMode != null && attackMode.CanAttack() && currentTarget != null)
        {
            attackMode.Attack(currentTarget.transform.position);
            lastTargetPosition = currentTarget.transform.position;
        }
    }

    private GameObject GetValidEnemyInRange()
    {
        // 检查是否使用抛物线攻击模式（需要考虑最小射程）
        float minimumRange = 0f;
        if (attackMode is ParabolicAttackMode parabolicMode)
        {
            minimumRange = parabolicMode.minimumAttackRange;
        }
        
        GameObject bestTarget = null;
        float bestDistance = float.MaxValue;
        
        // 遍历检测到的敌人列表，找到有效的敌人
        foreach (GameObject enemy in attackRange.DetectedEnemies)
        {
            if (enemy != null)
            {
                // 计算与敌人的距离
                float distance = Vector2.Distance(transform.position, enemy.transform.position);
                
                // 检查是否在有效攻击范围内（不小于最小射程）
                if (distance >= minimumRange)
                {
                    // 找距离最近的敌人（如果有最小射程限制）
                    if (distance < bestDistance)
                    {
                        bestTarget = enemy;
                        bestDistance = distance;
                    }
                }
            }
        }
        
        return bestTarget;
    }
    
    // 允许在运行时切换攻击模式
    public void SetAttackMode(AttackModeBase newMode)
    {
        if (newMode != null)
        {
            attackMode = newMode;
            attackMode.Initialize(this, BulletsParent);
        }
    }
}