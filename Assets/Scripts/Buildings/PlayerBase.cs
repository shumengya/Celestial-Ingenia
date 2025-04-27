using UnityEngine;

public class PlayerBase : BuildingBase
{
    public float damageAmount = 1f;
    private float fiveTimer = 0f;


    protected override void Update()
    {
        fiveTimer += Time.deltaTime;
        if (fiveTimer >= 5f)
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