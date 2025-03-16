using UnityEngine;

public class RemoteAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public GameObject bulletPrefab;
    public float fireRate = 1f;
    private float lastFireTime;

    [Header("Dependencies")]
    [SerializeField] // ǿ�����л��ֶ�
    private AttackRange attackRange;

    private void Start()
    {
        // �Զ���ȡ��������δ�ֶ���ֵ��
        if (attackRange == null)
        {
            attackRange = GetComponent<AttackRange>();
        }
    }

    private void Update()
    {
        if (Time.time - lastFireTime >= fireRate)
        {
            // ʹ��AttackRange�ļ����
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
            if (enemy == null) continue; // ��ֹ���˱����ٺ��������

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
