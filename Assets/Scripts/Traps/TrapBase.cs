using UnityEngine;

public class TrapBase : BuildingBase
{

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision is BoxCollider2D && collision.CompareTag("Enemy") && isUnderConstruction == false){
            Debug.Log("敌人触发陷阱");
            //Destroy(gameObject);
            OnTrapTrigger(collision);
        }

    }

    protected virtual void OnTrapTrigger(Collider2D enemyObject)
    {

    }
    
} 