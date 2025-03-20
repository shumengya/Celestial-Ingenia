using UnityEngine;
using System.Collections.Generic;

public class MeleeAttackDetection : MonoBehaviour
{
    [Header("Detection Settings")]
    public string targetTag = "Player"; // �ϲ����Ŀ���ǩ
    public int meleeDamage = 3;
    public float attackAngle = 60f; // ��ǰ�������Ƕȷ�Χ
    public float attackRate = 1f; // �������ʱ�䣨�룩
    public bool enableSelfDestruct; // �Ƿ������Իٹ���

    private float attackTimer = 0f;
    public List<GameObject> detectedEnemies = new List<GameObject>();

    // ͨ�����Ա�¶�����б�ֻ����
    public List<GameObject> DetectedEnemies => detectedEnemies;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(targetTag))
        {
            if (!detectedEnemies.Contains(collision.gameObject))
            {
                detectedEnemies.Add(collision.gameObject);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(targetTag) && detectedEnemies.Contains(collision.gameObject))
        {
            detectedEnemies.Remove(collision.gameObject);
        }
    }

    private void Update()
    {
        attackTimer += Time.deltaTime;

        if (attackTimer >= attackRate)
        {
            attackTimer = 0f;
            foreach (GameObject target in new List<GameObject>(detectedEnemies))
            {
                if (target == null)
                {
                    detectedEnemies.Remove(target);
                    continue;
                }

                if (IsInFront(target.transform.position))
                {
                    HealthBar targetHealthBar = target.GetComponentInChildren<HealthBar>();
                    if (targetHealthBar != null)
                    {
                        targetHealthBar.TakeDamage(meleeDamage);
                        Debug.Log("��ս��������Ŀ�� " + target.name + " ��� " + meleeDamage + " ���˺�");

                        if(enableSelfDestruct == true)
                        {
                            Destroy(gameObject);
                        }
                    }
                }
            }
        }
    }

    private bool IsInFront(Vector3 targetPosition)
    {
        Vector3 directionToTarget = (targetPosition - transform.position).normalized;
        float dotProduct = Vector3.Dot(transform.up, directionToTarget);
        float angle = Mathf.Acos(dotProduct) * Mathf.Rad2Deg;

        return angle <= attackAngle / 2;
    }
}