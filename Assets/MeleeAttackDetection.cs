using UnityEngine;
using System.Collections.Generic;

public class MeleeAttackDetection : MonoBehaviour
{
    [Header("Detection Settings")]
    public string enemyTag = "Enemy";
    public string playerTag = "Player"; // ������ұ�ǩ
    public int meleeDamage = 10; // ������ս�����˺�ֵ
    public List<GameObject> detectedEnemies = new List<GameObject>();

    // ͨ�����Ա�¶�����б�ֻ����
    public List<GameObject> DetectedEnemies => detectedEnemies;

    // ����ײ����ʱ����
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(playerTag)) // ����Ƿ���ײ�����
        {
            // ��ȡ��ҵ� HealthBar ���
            HealthBar playerHealthBar = collision.gameObject.GetComponentInChildren<HealthBar>();
            if (playerHealthBar != null)
            {
                // �������ɽ�ս�˺�
                playerHealthBar.TakeDamage(meleeDamage);
                Debug.Log("���˽�ս�������������� " + meleeDamage + " ���˺�");
            }
        }
        else if (collision.gameObject.CompareTag(enemyTag) && !detectedEnemies.Contains(collision.gameObject))
        {
            detectedEnemies.Add(collision.gameObject);
            // �����������ӽ�ս�����������߼������粥�Ź�������������˺���
            Debug.Log("������ս��������������: " + collision.gameObject.name);
        }
    }

    // ����ײ����ʱ����
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(enemyTag) && detectedEnemies.Contains(collision.gameObject))
        {
            detectedEnemies.Remove(collision.gameObject);
        }
    }
}