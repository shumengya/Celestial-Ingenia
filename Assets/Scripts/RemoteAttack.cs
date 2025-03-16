using UnityEngine;

public class RemoteAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public GameObject bulletPrefab;
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
        if (Time.time - lastFireTime >= fireRate)
        {
            // 使用AttackRange的检测结果
            if (attackRange.DetectedEnemies.Count > 0)
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
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        bullet.transform.right = direction;

        if (bullet.TryGetComponent(out Bullet bulletScript))
        {
            bulletScript.team = 0;
        }
    }
}
