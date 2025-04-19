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
    private AttackModeBase attackMode; // 当前使用的攻击模式

    private GameObject currentTarget; // 当前攻击目标

    private void Start()
    {
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
            currentTarget = GetFirstEnemyInRange();
        }

        // 检查是否可以攻击且有目标
        if (attackMode != null && attackMode.CanAttack() && currentTarget != null)
        {
            attackMode.Attack(currentTarget.transform.position);
        }
    }

    private GameObject GetFirstEnemyInRange()
    {
        // 遍历检测到的敌人列表，找到第一个有效的敌人
        foreach (GameObject enemy in attackRange.DetectedEnemies)
        {
            if (enemy != null)
            {
                return enemy;
            }
        }
        return null;
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