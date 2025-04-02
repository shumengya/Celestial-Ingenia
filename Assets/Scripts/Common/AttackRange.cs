using UnityEngine;
using System.Collections.Generic;

public class AttackRange : MonoBehaviour
{
    [Header("检测设置")]
    public string enemyTag = "Enemy";

    [Header("Debug")]
    [SerializeField] // 序列化字段便于调试
    private List<GameObject> detectedEnemies = new List<GameObject>();

    // 通过属性暴露敌人列表（只读）
    public List<GameObject> DetectedEnemies => detectedEnemies;

    [Header("依赖组件")]
    public CircleCollider2D circleCollider;

    private void Awake()
    {
        if (circleCollider == null)
        {
            circleCollider = GetComponent<CircleCollider2D>();
        }
    }

    //这个才是真正的检测范围 
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other is BoxCollider2D){
            if (other.CompareTag(enemyTag) && !detectedEnemies.Contains(other.gameObject))
            {
                detectedEnemies.Add(other.gameObject);
            }
        }

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other is BoxCollider2D){
            if (other.CompareTag(enemyTag) && detectedEnemies.Contains(other.gameObject))
            {
                detectedEnemies.Remove(other.gameObject);
            }
        }
    }
}