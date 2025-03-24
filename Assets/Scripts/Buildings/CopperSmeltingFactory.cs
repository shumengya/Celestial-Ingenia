using UnityEngine;
using UnityEngine.EventSystems;

public class CopperSmeltingFactory : MonoBehaviour, IPointerClickHandler
{
    private HealthBar buildingHealthBar;
    public float damageAmount = 1f;
    private float oneSecondTimer = 0f;
    private float twoSecondTimer = 0f;
    private float threeSecondTimer = 0f;
    public string smyName = "炼铜厂";
    public string smyType = "建筑";
    public string smyDescription = "炼铜，炼铜，还是炼铜！";

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
        oneSecondTimer += Time.deltaTime;
        twoSecondTimer += Time.deltaTime;
        threeSecondTimer += Time.deltaTime;

        // 当 1 秒计时器达到 1 秒时
        if (oneSecondTimer >= 1f)
        {
            if (buildingHealthBar != null)
            {
                buildingHealthBar.TakeDamage(damageAmount);
            }
            oneSecondTimer = 0f;
        }

        // 当 2 秒计时器达到 2 秒时
        if (twoSecondTimer >= 2f)
        {
            // 每 2 秒增加一个石头
            PlayerConfig.Instance.stoneNum += 1;
            twoSecondTimer = 0f;
        }

        // 当 3 秒计时器达到 3 秒时
        if (threeSecondTimer >= 3f)
        {
            // 这里可以添加 3 秒计时要执行的逻辑
            // Debug.Log("3 秒计时触发");
            threeSecondTimer = 0f;
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