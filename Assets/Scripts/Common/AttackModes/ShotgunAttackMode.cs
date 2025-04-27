using UnityEngine;
using System.Collections;

public class ShotgunAttackMode : AttackModeBase
{
    [Header("散弹设置")]
    public int rows = 3;                    // 子弹行数
    public int columns = 5;                 // 子弹列数
    public float horizontalSpread = 5f;     // 水平方向上的角度偏移
    public float verticalSpread = 3f;       // 垂直方向上的角度偏移
    public float rowDelay = 0.1f;           // 行与行之间的发射延迟
    public float cooldownTime = 1f;         // 攻击冷却时间
    
    private float nextAttackTime = 0f;
    private bool isAttacking = false;
    
    public override bool CanAttack()
    {
        return !isAttacking && Time.time >= nextAttackTime;
    }
    
    public override void Attack(Vector2 targetPosition)
    {
        if (!isAttacking)
        {
            StartCoroutine(FireShotgunPattern(targetPosition));
        }
    }
    
    private IEnumerator FireShotgunPattern(Vector2 targetPosition)
    {
        isAttacking = true;
        
        // 计算基础射击方向
        Vector2 baseDirection = GetFiringDirection(targetPosition);
        
        // 计算散弹阵列的中心点
        float centerRow = (rows - 1) / 2f;
        float centerCol = (columns - 1) / 2f;
        
        // 按行依次发射
        for (int row = 0; row < rows; row++)
        {
            // 发射当前行的所有子弹
            for (int col = 0; col < columns; col++)
            {
                // 计算当前子弹相对于中心的角度偏移
                float horizontalOffset = (col - centerCol) * horizontalSpread;
                float verticalOffset = (row - centerRow) * verticalSpread;
                
                // 应用角度偏移
                Quaternion rotation = Quaternion.Euler(0, 0, horizontalOffset + verticalOffset);
                Vector2 bulletDirection = rotation * baseDirection;
                
                // 创建子弹
                CreateBullet(transform.position, bulletDirection);
            }
            
            // 等待指定时间后发射下一行
            if (row < rows - 1)
            {
                yield return new WaitForSeconds(rowDelay);
            }
        }
        
        // 设置冷却时间
        nextAttackTime = Time.time + cooldownTime;
        isAttacking = false;
    }
    
    public override void UpdateAttackState()
    {
        // 此模式不需要额外的状态更新，因为使用了协程
    }
} 