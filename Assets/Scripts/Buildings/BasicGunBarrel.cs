using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicGunBarrel : MonoBehaviour
{
    [SerializeField] // 强制序列化字段
    [Header("依赖设置")]
    private AttackRange attackRange;

    [Header("炮塔旋转设置")]
    public bool rotateWhenNoEnemy = false;
    public float rotationSpeed = 100f;

    private float targetAngle;

    private void Start()
    {
        // 自动获取组件（如果未手动赋值）
        if (attackRange == null)
        {
            attackRange = GetComponent<AttackRange>();
        }
        targetAngle = transform.eulerAngles.z;
    }

    private void Update()
    {
        // 获取第一个敌人
        GameObject target = GetFirstEnemyInRange();

        if (target != null)
        {
            // 计算炮塔指向敌人的方向
            Vector2 direction = (target.transform.position - transform.position).normalized;

            // 计算旋转角度
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // 调整角度，因为图片默认朝上
            angle += 270f;

            targetAngle = angle;
        }
        else if (rotateWhenNoEnemy)
        {
            // 没有敌人且允许旋转时，持续旋转
            targetAngle += rotationSpeed * Time.deltaTime;
        }

        // 平滑旋转到目标角度
        float currentAngle = transform.eulerAngles.z;
        float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, rotationSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0f, 0f, newAngle);
    }

    private GameObject GetFirstEnemyInRange()
    {
        // 遍历检测到的敌人列表，找到第一个有效的敌人
        foreach (GameObject enemy in attackRange.DetectedEnemies)
        {
            if (enemy != null)
            {
                return enemy;
            }
        }
        return null;
    }
}    