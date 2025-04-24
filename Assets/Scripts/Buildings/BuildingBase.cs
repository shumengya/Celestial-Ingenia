using UnityEngine;

public class BuildingBase : PlaceableBase
{
    
// 健康条相关
    protected HealthBar playerHealthBar;



    // 交互状态
    [Header("交互状态")]
    public bool canBeInteracted = true;
    public bool isSelected = false;
    public bool canBeClicked = true;


    // 建造状态
    [Header("建造状态")]
    public bool isUnderConstruction = true;
    public float constructionTime = 5f;
    private float constructionTimeRemaining;
    private float maxHealth = 0f; // 最大生命值

    // 选中状态视觉反馈颜色
    [Header("选中状态视觉反馈颜色")]
    public Color selectedColor = new Color(1f, 1f, 1f, 1f); // 选中时的颜色

    // 新增：动态获取初始颜色
    private Color originalColor;
    private Color normalColor = new Color(1f, 1f, 1f, 1f);   // 正常时的颜色



    // 组件引用
    [Header("组件引用")]
    protected SpriteRenderer spriteRenderer;
    protected BoxCollider2D boxCollider2D;

    protected virtual void Start()
    {

        playerHealthBar = GetComponentInChildren<HealthBar>();
        maxHealth = playerHealthBar.GetMaxHealth();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // 获取BoxCollider2D引用
        boxCollider2D = GetComponent<BoxCollider2D>();
        

        // 获取初始颜色
        originalColor = spriteRenderer.color;
        normalColor = originalColor;  // 将normalColor设为初始颜色
        
        // 初始化建造状态
        if (isUnderConstruction)
        {
            constructionTimeRemaining = constructionTime;
            canBeInteracted = false; // 建造期间不可交互
            canBeClicked = false;    // 建造期间不可点击
            
            // 设置自身对象的初始视觉效果为全透明
            if (spriteRenderer != null)
            {
                Color startColor = normalColor;
                startColor.a = 0.0f;
                spriteRenderer.color = startColor;
            }

            //  设置子对象的初始视觉效果为全透明
            SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer renderer in spriteRenderers)
            {
                if (renderer != null && renderer != spriteRenderer) // 排除自身的SpriteRenderer
                {
                    Color startColor = normalColor;
                    startColor.a = 0.0f;
                    renderer.color = startColor;
                }
            }
            
            // 设置生命值为正常血量的一半
            if (playerHealthBar != null)
            {
                playerHealthBar.SetMaxHealth(maxHealth);
                playerHealthBar.SetHealth(maxHealth * 0.5f);
            }
        }
        else
        {
            // 初始化颜色
            UpdateVisualState();
            
            // 设置满血
            if (playerHealthBar != null)
            {
                playerHealthBar.SetMaxHealth(maxHealth);
                playerHealthBar.SetHealth(maxHealth);
            }
        }
    }

    protected virtual void Update()
    {
        // 处理建造时间
        if (isUnderConstruction)
        {
            constructionTimeRemaining -= Time.deltaTime;
            
            // 计算建造进度 (0-1)
            float constructionProgress = 1f - (constructionTimeRemaining / constructionTime);
            
            // 更新建造中的视觉效果 - 只更新透明度
            UpdateConstructionVisuals(constructionProgress);
            
            // 建造完成
            if (constructionTimeRemaining <= 0)
            {
                CompleteConstruction();
            }


            
            // 建造期间不执行其他操作
            return;
        }
        
        // 检测玩家是否死亡
        if (playerHealthBar != null && playerHealthBar.IsDead())
        {
            Destroy(gameObject);
            // 方法1：通过FindObjectOfType获取（适用于场景中只有一个CameraMovement组件的情况）
            CameraMovement cameraMovement = FindObjectOfType<CameraMovement>();
            if (cameraMovement != null)
            {
                cameraMovement.ShakeCamera(0.1f, 0.1f); // 使用默认参数
                // 或者自定义参数: cameraMovement.ShakeCamera(1.0f, 0.8f);
            }
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
    
    // 更新建造中的视觉效果 - 只更新透明度
    protected virtual void UpdateConstructionVisuals(float progress)
    {
        if (spriteRenderer != null)
        {
            // 透明度从0.0到1.0渐变
            float alpha = progress;
            Color progressColor = normalColor;
            progressColor.a = alpha;
            spriteRenderer.color = progressColor;
            
            // 更新子对象的透明度
            SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer renderer in spriteRenderers)
            {
                if (renderer != null && renderer != spriteRenderer) // 排除自身的SpriteRenderer
                {
                    Color childColor = renderer.color;
                    childColor.a = alpha;
                    renderer.color = childColor;
                }
            }
        }
    }
    
    // 完成建造
    protected virtual void CompleteConstruction()
    {
        isUnderConstruction = false;
        canBeInteracted = true;
        canBeClicked = true;
        
        // 设置满生命值
        if (playerHealthBar != null)
        {
            playerHealthBar.SetHealth(maxHealth);
        }
        
        // 恢复正常颜色
        UpdateVisualState();
        
        Debug.Log($"建筑 {smyName} 建造完成！");
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
            if (isUnderConstruction)
            {
                // 在建造过程中保持透明效果
                Color buildingColor = originalColor;  // 使用原始颜色
                float constructionProgress = 1f - (constructionTimeRemaining / constructionTime);
                buildingColor.a = 0.5f + (constructionProgress * 0.5f);
                spriteRenderer.color = buildingColor;
            }
            else
            {
                spriteRenderer.color = isSelected ? selectedColor : originalColor;  // 直接使用原始颜色
                //恢复子对象颜色为原色
                SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
                foreach (SpriteRenderer renderer in spriteRenderers)
                {
                    if (renderer != null && renderer != spriteRenderer) // 排除自身的SpriteRenderer
                    {
                        renderer.color = originalColor;  // 恢复为原始颜色
                    }
                }
                
            }
        }
    }
    
    
    // 公开方法：外部代码可调用来选择或取消选择
    public virtual void SetSelected(bool selected)
    {
        if (!isUnderConstruction)
        {
            isSelected = selected;
            UpdateVisualState();
        }
    }
    
    // 获取建造进度
    public float GetConstructionProgress()
    {
        if (!isUnderConstruction)
            return 1f;
            
        return 1f - (constructionTimeRemaining / constructionTime);
    }
    
    // 检查建筑是否可交互
    public bool IsInteractable()
    {
        return canBeInteracted && !isUnderConstruction;
    }

    // 检查建筑是否可攻击
    public bool CanAttack()
    {
        return !isUnderConstruction;
    }

}