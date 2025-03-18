using UnityEngine;

public class RemoteAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public GameObject bulletPrefab;
    public Transform BulletsParent;
    public float fireRate = 1f;
    private float lastFireTime;

    [Header("Dependencies")]
    [SerializeField] // 强制序列化字段
    private AttackRange attackRange;

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
        // 检查是否达到射击间隔时间
        if (Time.time - lastFireTime >= fireRate)
        {
            // 缓存检测到的敌人列表，避免多次访问
            var detectedEnemies = attackRange.DetectedEnemies;
            if (detectedEnemies.Count > 0)
            {
                ShootAtTarget(GetNearestEnemy().transform.position);
                lastFireTime = Time.time;
            }
        }
    }

    private GameObject GetNearestEnemy()
    {
        GameObject nearest = null;
        float minDistance = Mathf.Infinity;

        // 遍历检测到的敌人列表，找到距离最近的敌人
        foreach (GameObject enemy in attackRange.DetectedEnemies)
        {
            if (enemy == null) continue; // 防止敌人被销毁后残留引用

            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = enemy;
            }
        }
        return nearest;
    }

    private void ShootAtTarget(Vector2 targetPosition)
    {
        // 计算射击方向
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
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