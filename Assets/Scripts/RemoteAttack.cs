using UnityEngine;

public class RemoteAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public GameObject bulletPrefab;
    public Transform BulletsParent;
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
        // ����Ƿ�ﵽ������ʱ��
        if (Time.time - lastFireTime >= fireRate)
        {
            // �����⵽�ĵ����б������η���
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

        // ������⵽�ĵ����б��ҵ���������ĵ���
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
        // �����������
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        // ʵ�����ӵ���������Ϊ BulletsParent ���Ӷ���
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity, BulletsParent);
        // �����ӵ��ĳ���
        bullet.transform.right = direction;

        // ��ȡ�ӵ��ű��������Ŷ���Ϣ
        if (bullet.TryGetComponent(out Bullet bulletScript))
        {
            bulletScript.team = 0;
        }
        else
        {
            Debug.LogError($"�ӵ�Ԥ���� {bullet.name} û��һ���ű��ļ�.");
        }
    }
}