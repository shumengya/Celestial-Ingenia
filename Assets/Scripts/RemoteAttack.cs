using UnityEngine;

public class RemoteAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public GameObject bulletPrefab;
    public Transform BulletsParent;
    public float fireRate = 1f;
    public float angleDeviation = 10f; // 可设置的角度偏差
    private float lastFireTime;

    [Header("Dependencies")]
    [SerializeField] // 强制序列化字段
    private AttackRange attackRange;

    private GameObject currentTarget; // 当前攻击目标

    private void Start()
    {
        // 自动获取组件（如果未手动赋值）
        if (attackRange == null)
        {
            attackRange = GetComponent<AttackRange>();
        }
    }

    private void Update()
    {
        // 检查当前目标是否还存在
        if (currentTarget == null || !attackRange.DetectedEnemies.Contains(currentTarget))
        {
            // 当前目标不存在或者不在攻击范围内，选择新的目标
            currentTarget = GetFirstEnemyInRange();
        }

        // 检查是否达到射击间隔时间且有目标
        if (Time.time - lastFireTime >= fireRate && currentTarget != null)
        {
            ShootAtTarget(currentTarget.transform.position);
            lastFireTime = Time.time;
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

    private void ShootAtTarget(Vector2 targetPosition)
    {
        // 计算射击方向
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;

        // 计算随机角度偏差
        float randomDeviation = Random.Range(-angleDeviation, angleDeviation);
        Quaternion rotation = Quaternion.Euler(0, 0, randomDeviation);
        direction = rotation * direction;

        // 实例化子弹并将其作为 BulletsParent 的子对象
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity, BulletsParent);
        // 设置子弹的朝向
        bullet.transform.right = direction;

        // 获取子弹脚本并设置团队信息
        if (bullet.TryGetComponent(out Bullet bulletScript))
        {
            bulletScript.team = 0;
        }
        else
        {
            Debug.LogError($"子弹预制体 {bullet.name} 没有一个脚本文件.");
        }
    }
}