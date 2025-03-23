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
    [SerializeField] // 序列化字段便于调试
    private List<GameObject> detectedEnemies = new List<GameObject>();

    // 通过属性暴露敌人列表（只读）
    public List<GameObject> DetectedEnemies => detectedEnemies;

    private void Update()
    {
        // 动态同步碰撞器范围
        GetComponent<CircleCollider2D>().radius = attackRange;
        //Debug.Log($"当前检测到敌人数量: {detectedEnemies.Count}");
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
