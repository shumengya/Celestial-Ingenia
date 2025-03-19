using UnityEngine;
using System.Collections.Generic;

public class MeleeAttackDetection : MonoBehaviour
{
    [Header("Detection Settings")]
    public string enemyTag = "Enemy";
    public string playerTag = "Player"; // 新增玩家标签
    public int meleeDamage = 10; // 新增近战攻击伤害值
    public List<GameObject> detectedEnemies = new List<GameObject>();

    // 通过属性暴露敌人列表（只读）
    public List<GameObject> DetectedEnemies => detectedEnemies;

    // 当碰撞发生时调用
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(playerTag)) // 检查是否碰撞到玩家
        {
            // 获取玩家的 HealthBar 组件
            HealthBar playerHealthBar = collision.gameObject.GetComponentInChildren<HealthBar>();
            if (playerHealthBar != null)
            {
                // 对玩家造成近战伤害
                playerHealthBar.TakeDamage(meleeDamage);
                Debug.Log("敌人近战攻击，对玩家造成 " + meleeDamage + " 点伤害");
            }
        }
        else if (collision.gameObject.CompareTag(enemyTag) && !detectedEnemies.Contains(collision.gameObject))
        {
            detectedEnemies.Add(collision.gameObject);
            // 在这里可以添加近战攻击触发的逻辑，例如播放攻击动画、造成伤害等
            Debug.Log("触发近战攻击，攻击到了: " + collision.gameObject.name);
        }
    }

    // 当碰撞结束时调用
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(enemyTag) && detectedEnemies.Contains(collision.gameObject))
        {
            detectedEnemies.Remove(collision.gameObject);
        }
    }
}