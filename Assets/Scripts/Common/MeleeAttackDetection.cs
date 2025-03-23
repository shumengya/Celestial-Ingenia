using UnityEngine;
using System.Collections.Generic;

public class MeleeAttackDetection : MonoBehaviour
{
    [Header("Detection Settings")]
    public string targetTag = "Player"; // 合并后的目标标签
    public int meleeDamage = 3;
    public float attackAngle = 60f; // 正前方攻击角度范围
    public float attackRate = 1f; // 攻击间隔时间（秒）
    public bool enableSelfDestruct; // 是否启用自毁功能

    private float attackTimer = 0f;
    public List<GameObject> detectedEnemies = new List<GameObject>();

    // 通过属性暴露敌人列表（只读）
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
                        Debug.Log("近战攻击，对目标 " + target.name + " 造成 " + meleeDamage + " 点伤害");

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