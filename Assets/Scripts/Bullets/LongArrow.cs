using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongArrow : BulletBase
{
    // 最大穿透敌人数量
    public int maxPierceCount = 3;
    
    // 当前已穿透敌人数量
    private int currentPierceCount = 0;
    
    // 存储已经穿透的敌人，避免重复伤害
    private HashSet<Collider2D> hitEnemies = new HashSet<Collider2D>();

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        // 检查碰撞对象的队伍标签
        int otherTeam = 0;

        if (other.CompareTag("Enemy"))
        {
            otherTeam = 1;
        }
        else if (other.CompareTag("Player"))
        {
            otherTeam = 0;
        }

        // 避免同队伍子弹造成伤害 且 避免重复伤害同一个敌人
        if (team != otherTeam && other is BoxCollider2D && !hitEnemies.Contains(other))
        {
            // 对目标造成伤害
            ApplyDamage(other, damage);

            // 添加到已命中集合
            hitEnemies.Add(other);
            
            // 生成碰撞效果
            if (impactEffect != null)
            {
                Instantiate(impactEffect, transform.position, Quaternion.identity);
            }

            // 增加穿透计数
            currentPierceCount++;
            
            // 如果达到最大穿透数，销毁子弹
            if (currentPierceCount >= maxPierceCount)
            {
                OnDestroyBullet();
            }
        }
    }
}
