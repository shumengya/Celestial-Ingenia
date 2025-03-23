using UnityEngine;
using UnityEngine.EventSystems;

public class BasicTurret : MonoBehaviour, IPointerClickHandler
{
    private HealthBar playerHealthBar;
    public float damageAmount = 1f;
    private float oneTimer = 0f;
    public string smyName = "基础炮塔";
    public string smyType = "建筑";
    public string smyDescription = "就是一个普普通通的发射炮弹的炮塔罢了";

    // 控制是否可以被点击的开关
    public bool canBeClicked = true;
    public bool isSelected = false;

    void Start()
    {
        playerHealthBar = GetComponentInChildren<HealthBar>();
    }

    void Update()
    {
        // 累加计时器
        oneTimer += Time.deltaTime;

        // 当计时器达到 1 秒时
        if (oneTimer >= 1f)
        {
            if (playerHealthBar != null)
            {
                playerHealthBar.TakeDamage(damageAmount);
            }
            oneTimer = 0f;
        }
       
        if (playerHealthBar.IsDead())
        {
            Destroy(gameObject);
        }
    }



    // 实现 IPointerClickHandler 接口的方法
    public void OnPointerClick(PointerEventData eventData)
    {
        if (canBeClicked)
        {
            Debug.Log("该建筑被选中:" + eventData);
            isSelected = true;
        }
    }


}