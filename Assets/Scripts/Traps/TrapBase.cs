using UnityEngine;

public class TrapBase : MonoBehaviour
{
    [Header("陷阱信息")]
    public string smyName = "陷阱";
    public string smyType = "陷阱";
    public string smyDescription = "基础陷阱";
    public int cost_wood = 0;
    public int cost_stone = 0;
    public int cost_iron = 0;
    public int cost_copper = 0;
    public BoxCollider2D boxCollider2D;

    protected void Start()
    {
        if (boxCollider2D == null){
            boxCollider2D = GetComponent<BoxCollider2D>();
        }
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && collision is BoxCollider2D){
            Debug.Log("敌人触发陷阱");
            OnTrapTrigger(collision);
        }
    }

    protected virtual void  OnTrapTrigger(Collider2D enemyObject){
        return;
    }


} 