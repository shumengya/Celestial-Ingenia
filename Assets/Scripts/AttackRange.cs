using UnityEngine;
using System.Collections.Generic;

public class AttackRange : MonoBehaviour
{
    [Header("Detection Settings")]
    public float attackRange = 5f;
    public string enemyTag = "Enemy";
    public Color rangeColor = new Color(1f, 0f, 0f, 0.2f);
    public bool showAttackRange = false;

    [Header("Debug")]
    [SerializeField] // ���л��ֶα��ڵ���
    private List<GameObject> detectedEnemies = new List<GameObject>();

    // ͨ�����Ա�¶�����б�ֻ����
    public List<GameObject> DetectedEnemies => detectedEnemies;

    private void Update()
    {
        // ��̬ͬ����ײ����Χ
        GetComponent<CircleCollider2D>().radius = attackRange;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(enemyTag) && !detectedEnemies.Contains(other.gameObject))
        {
            detectedEnemies.Add(other.gameObject);
        }


    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(enemyTag) && detectedEnemies.Contains(other.gameObject))
        {
            detectedEnemies.Remove(other.gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (showAttackRange)
        {
            Gizmos.color = rangeColor;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}
