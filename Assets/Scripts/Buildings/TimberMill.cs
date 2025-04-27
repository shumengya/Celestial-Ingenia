using UnityEngine;

public class TimberMill : BuildingBase
{
    public float damageAmount = 1f;
    private float oneTimer = 0f;


    protected override void Update()
    {

        oneTimer += Time.deltaTime;
        if (oneTimer >= 1f)
        {
            PlayerConfig.Instance.woodNum += 1;
            oneTimer = 0f;
        }

        base.Update();
    }
}    