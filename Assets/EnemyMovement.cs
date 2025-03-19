using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float moveSpeed = 3f; // ���˵��ƶ��ٶ�
    public float rotationSpeed = 200f; // ���˵���ת�ٶ�
    public string playerTag = "Player"; // ��ҵı�ǩ

    private AttackRange attackRange; // ���ù�����Χ�ű�
    private Rigidbody2D rb; // ���˵ĸ������

    void Start()
    {
        // ��ȡ������Χ�ű�
        attackRange = GetComponent<AttackRange>();
        // ��ȡ�������
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // ����Ƿ��⵽���
        if (attackRange.DetectedEnemies.Count > 0)
        {
            // ����ֻ�����һ����⵽�����
            GameObject player = attackRange.DetectedEnemies[0];
            if (player != null)
            {
                // ������˵���ҵķ���
                Vector2 direction = (player.transform.position - transform.position).normalized;

                // ����Ŀ��Ƕ�
                float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

                // ƽ����ת��Ŀ��Ƕ�
                float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.z, targetAngle, rotationSpeed * Time.deltaTime);
                transform.rotation = Quaternion.Euler(0f, 0f, angle);

                // ����ת�ӽ�Ŀ��Ƕ�ʱ��ʼ�ƶ�
                if (Mathf.Abs(angle - targetAngle) < 5f)
                {
                    // �ƶ�����
                    rb.velocity = direction * moveSpeed;
                }
                else
                {
                    // ��תʱֹͣ�ƶ�
                    rb.velocity = Vector2.zero;
                }
            }
        }
        else
        {
            // ���û�м�⵽��ң�ֹͣ�ƶ�
            rb.velocity = Vector2.zero;
        }
    }
}