using UnityEngine;

public class StoneQuary : BuildingBase
{
    public float damageAmount = 1f;
    private float oneTimer = 0f;


    protected override void Update()
    {
        // 铁工厂特有的逻辑：每秒造成伤害
        oneTimer += Time.deltaTime;
        if (oneTimer >= 1f)
        {
            PlayerConfig.Instance.stoneNum += 1;
            oneTimer = 0f;
        }

        // 调用基类的Update方法处理通用逻辑
        base.Update();
    }
}    