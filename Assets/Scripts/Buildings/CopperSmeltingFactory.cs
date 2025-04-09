using UnityEngine;

public class CopperSmeltingFactory : BuildingBase
{
    public float damageAmount = 1f;
    private float oneTimer = 0f;

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
        oneTimer += Time.deltaTime;
        if (oneTimer >= 1f)
        {
            PlayerConfig.Instance.copperNum += 1;
            oneTimer = 0f;
        }

        // 调用基类的Update方法处理通用逻辑
        base.Update();
    }
}    