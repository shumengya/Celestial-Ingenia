using UnityEngine;
using UnityEngine.EventSystems;

public class TimberMill : MonoBehaviour, IPointerClickHandler
{
    private HealthBar buildingHealthBar;
    public float damageAmount = 1f;
    private float timer = 0f;
    public string smyName = "伐木场";
    public string smyType = "建筑";
    public string smyDescription = "伐伐伐伐木工";

    // 控制是否可以被点击的开关
    public bool canBeClicked = true;
    public bool isSelected = false;

    void Start()
    {
        buildingHealthBar = GetComponentInChildren<HealthBar>();
    }

    void Update()
    {
        // 累加计时器
        timer += Time.deltaTime;

        // 当计时器达到 1 秒时
        if (timer >= 1f)
        {
            if (buildingHealthBar != null)
            {
                buildingHealthBar.TakeDamage(damageAmount);
            }
            //每秒增加一个木材
            PlayerConfig.Instance.woodNum += 1;
            timer = 0f;
        }


        if (buildingHealthBar.IsDead())
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