using UnityEngine;
using UnityEngine.EventSystems;

public class IronFactory : MonoBehaviour, IPointerClickHandler
{
    private HealthBar IronFactorHealthBar;
    public float damageAmount = -1f;
    private float oneTimer = 0f;
    public string smyName = "铁矿厂";
    public string smyType = "建筑";
    public string smyDescription = "生产铁的矿场";

    // 控制是否可以被点击的开关
    public bool canBeClicked = true;
    public bool isSelected = false;

    void Start()
    {
        IronFactorHealthBar = GetComponentInChildren<HealthBar>();
    }

    void Update()
    {
        // 一秒计时器
        oneTimer += Time.deltaTime;
        // 当计时器达到 1 秒时
        if (oneTimer >= 1f)
        {
            if (IronFactorHealthBar != null)
            {
                IronFactorHealthBar.TakeDamage(damageAmount);
                PlayerConfig.Instance.ironNum++;
                
            }
            oneTimer = 0f;
        }


        if (IronFactorHealthBar.IsDead())
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
