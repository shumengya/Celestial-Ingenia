using UnityEngine;

public class PlayerBase : BuildingBase
{
    public float damageAmount = 1f;
    private float fiveTimer = 0f;

    protected override void Start()
    {
        // 设置铁工厂特有的属性
        smyName = "大本营";
        smyType = "建筑";
        smyDescription = "玩家的核心中的核心";
        
        // 调用基类的Start方法
        base.Start();
    }

    protected override void Update()
    {
        fiveTimer += Time.deltaTime;
        if (fiveTimer >= 1f)
        {
            PlayerConfig.Instance.ironNum += 1;
            PlayerConfig.Instance.copperNum += 1;
            PlayerConfig.Instance.woodNum += 1;
            PlayerConfig.Instance.stoneNum += 1;

            fiveTimer = 0f;

        }
        base.Update();
    }
}    