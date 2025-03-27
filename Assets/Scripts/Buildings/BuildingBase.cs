using UnityEngine;

public class BuildingBase : MonoBehaviour
{
    protected HealthBar playerHealthBar;
    public string smyName = "建筑";
    public string smyType = "建筑";
    public string smyDescription = "基础建筑";

    public bool canBeInteracted = true;
    public bool isSelected = false;
    public bool canBeClicked = true;

    // 添加选中状态视觉反馈
    public Color selectedColor = new Color(1f, 1f, 1f, 1f); // 选中时的颜色
    public Color normalColor = new Color(1f, 1f, 1f, 1f);   // 正常时的颜色
    protected SpriteRenderer spriteRenderer;
    protected BoxCollider2D boxCollider;

    protected virtual void Start()
    {
        playerHealthBar = GetComponentInChildren<HealthBar>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // 获取BoxCollider2D引用
        boxCollider = GetComponent<BoxCollider2D>();
        
        // 确保BoxCollider2D存在且正确配置
        if (boxCollider != null && spriteRenderer != null)
        {
            // 可以调整BoxCollider2D以匹配Sprite
            boxCollider.size = spriteRenderer.sprite.bounds.size;
        }
        
        // 初始化颜色
        UpdateVisualState();
    }

    protected virtual void Update()
    {
        // 检测玩家是否死亡
        if (playerHealthBar != null && playerHealthBar.IsDead())
        {
            Destroy(gameObject);
        }

        // 检测鼠标点击
        if (Input.GetMouseButtonDown(0) && canBeClicked)
        {
            // 创建射线从相机到鼠标位置
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D[] hits = Physics2D.RaycastAll(ray.origin, ray.direction);

            bool clickedOnBox = false;
            
            // 遍历所有碰撞结果
            foreach (RaycastHit2D hit in hits)
            {
                // 只有当点击到本对象的BoxCollider2D时才处理
                if (hit.collider != null && hit.collider.gameObject == gameObject && hit.collider is BoxCollider2D)
                {
                    clickedOnBox = true;
                    HandleClick();
                    break;
                }
            }
            
            // 如果已选中，但点击了其他地方（且没点击到当前对象的BoxCollider2D）
            if (!clickedOnBox && isSelected)
            {
                isSelected = false;
                UpdateVisualState();
            }
        }
    }
    
    // 处理点击事件
    protected virtual void HandleClick()
    {
        if (canBeInteracted)
        {
            // 切换选中状态
            isSelected = !isSelected;
            UpdateVisualState();
            
            // 这里可以添加选中后的操作，比如显示UI、执行特定功能等
            Debug.Log($"建筑 {smyName} 被" + (isSelected ? "选中" : "取消选中"));
        }
    }
    
    // 更新视觉状态
    protected virtual void UpdateVisualState()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = isSelected ? selectedColor : normalColor;
        }
    }
    
    // 公开方法：外部代码可调用来选择或取消选择
    public virtual void SetSelected(bool selected)
    {
        isSelected = selected;
        UpdateVisualState();
    }
}