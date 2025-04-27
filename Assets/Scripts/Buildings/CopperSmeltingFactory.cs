using UnityEngine;

public class CopperSmeltingFactory : BuildingBase
{
    public float damageAmount = 1f;
    private float oneTimer = 0f;

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