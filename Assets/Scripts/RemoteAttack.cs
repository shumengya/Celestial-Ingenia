using UnityEngine;

public class RemoteAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public GameObject bulletPrefab;
    public Transform BulletsParent;
    public float fireRate = 1f;
    public float angleDeviation = 10f; // �����õĽǶ�ƫ��
    private float lastFireTime;

    [Header("Dependencies")]
    [SerializeField] // ǿ�����л��ֶ�
    private AttackRange attackRange;

    private GameObject currentTarget; // ��ǰ����Ŀ��

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
        // ��鵱ǰĿ���Ƿ񻹴���
        if (currentTarget == null || !attackRange.DetectedEnemies.Contains(currentTarget))
        {
            // ��ǰĿ�겻���ڻ��߲��ڹ�����Χ�ڣ�ѡ���µ�Ŀ��
            currentTarget = GetFirstEnemyInRange();
        }

        // ����Ƿ�ﵽ������ʱ������Ŀ��
        if (Time.time - lastFireTime >= fireRate && currentTarget != null)
        {
            ShootAtTarget(currentTarget.transform.position);
            lastFireTime = Time.time;
        }
    }

    private GameObject GetFirstEnemyInRange()
    {
        // ������⵽�ĵ����б��ҵ���һ����Ч�ĵ���
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
        // �����������
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;

        // ��������Ƕ�ƫ��
        float randomDeviation = Random.Range(-angleDeviation, angleDeviation);
        Quaternion rotation = Quaternion.Euler(0, 0, randomDeviation);
        direction = rotation * direction;

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