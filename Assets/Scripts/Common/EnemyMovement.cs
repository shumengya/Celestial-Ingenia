using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float moveSpeed = 3f; // 敌人的移动速度
    public float rotationSpeed = 200f; // 敌人的旋转速度
    public string playerTag = "Player"; // 玩家的标签

    private AttackRange attackRange; // 引用攻击范围脚本
    private Rigidbody2D rb; // 敌人的刚体组件

    void Start()
    {
        // 获取攻击范围脚本
        attackRange = GetComponent<AttackRange>();
        // 获取刚体组件
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // 检查是否检测到玩家
        if (attackRange.DetectedEnemies.Count > 0)
        {
            // 假设只处理第一个检测到的玩家
            GameObject player = attackRange.DetectedEnemies[0];
            if (player != null)
            {
                // 计算敌人到玩家的方向
                Vector2 direction = (player.transform.position - transform.position).normalized;

                // 计算目标角度
                float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

                // 平滑旋转到目标角度
                float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.z, targetAngle, rotationSpeed * Time.deltaTime);
                transform.rotation = Quaternion.Euler(0f, 0f, angle);

                // 当旋转接近目标角度时开始移动
                if (Mathf.Abs(angle - targetAngle) < 5f)
                {
                    // 移动敌人
                    rb.velocity = direction * moveSpeed;
                }
                else
                {
                    // 旋转时停止移动
                    rb.velocity = Vector2.zero;
                }
            }
        }
        else
        {
            // 如果没有检测到玩家，停止移动
            rb.velocity = Vector2.zero;
        }
    }
}