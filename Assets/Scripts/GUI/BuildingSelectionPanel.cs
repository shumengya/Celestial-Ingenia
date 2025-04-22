using UnityEngine;
using UnityEngine.UI;

public class BuildingSelectionPanel : MonoBehaviour
{
    
//------------------------按钮注册------------------------------//
    [Header("建筑按钮")]
    public Button PlayerBase;
    public Button TimberMill;
    public Button StoneQuarry;
    public Button IronFactory;
    public Button CopperSmeltingFactory;

    public Button Turret;
    public Button Turret_Arrow;
    public Button Turret_Fire_Gun;
    public Button Turret_Multi_Arrow;
    public Button Turret_QinCrossbow;
    public Button Turret_ZhugeCrossbow;
    public Button Turret_ThrowStoneCannon;
    public Button Turret_HuDunCannon;
//------------------------按钮注册------------------------------//


//-------------------------建筑预制体注册----------------------------------//
    [Header("建筑预制体")]
    public GameObject playerBasePrefab; //大本营
    public GameObject timberMillPrefab; //伐木厂
    public GameObject stoneQuarryPrefab; //采石场
    public GameObject IronFactoryPrefab; //铁矿厂
    public GameObject CopperSmeltingFactoryPrefab; //铜矿厂

    public GameObject TurretPrefab; // 炮塔
    public GameObject Turret_Arrow_Prefab; // 三弓床弩
    public GameObject Turret_Fire_Gun_Prefab; // 火枪塔
    public GameObject Turret_Multi_Arrow_Prefab; //神火飞鸦
    public GameObject Turret_QinCrossbowPrefab; // 秦弩
    public GameObject Turret_ZhugeCrossbowPrefab; // 诸葛弩
    public GameObject Turret_ThrowStoneCannonPrefab; // 投石炮
    public GameObject Turret_HuDunCannonPrefab; // 虎墩炮
//-------------------------建筑预制体注册----------------------------------//

    [Header("其他设置")]
    public Transform buildingParent;
    public CanvasGroup canvasGroup; 
    public GameObject buildingPlaceEffect; // 建筑放置时的粒子效果
    private GameObject selectedBuildingPrefab;
    private GameObject previewBuilding;
    private bool isPlacingBuilding;
    private bool isCollisionDetected; // 用于标记是否发生碰撞
    private bool canPlaceOnResourcePoint; // 是否满足资源点放置条件
    private AudioSource audioSource; // 音频源组件


    void Start()
    {
        Debug.Log("建筑选择面板：Start方法已调用。");
        // 初始化时隐藏面板（透明度设为0，禁用交互）
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        // 获取AudioSource组件
        audioSource = GetComponent<AudioSource>();

//----------------------------------------注册按钮点击事件-------------------------------------------------//
        PlayerBase.onClick.AddListener(() => OnBuildingButtonClick(playerBasePrefab));
        TimberMill.onClick.AddListener(() => OnBuildingButtonClick(timberMillPrefab));
        StoneQuarry.onClick.AddListener(() => OnBuildingButtonClick(stoneQuarryPrefab));
        IronFactory.onClick.AddListener(() => OnBuildingButtonClick(IronFactoryPrefab));
        CopperSmeltingFactory.onClick.AddListener(() => OnBuildingButtonClick(CopperSmeltingFactoryPrefab));

        //这些是各种炮塔
        Turret.onClick.AddListener(() => OnBuildingButtonClick(TurretPrefab));
        Turret_Arrow.onClick.AddListener(() => OnBuildingButtonClick(Turret_Arrow_Prefab));
        Turret_Fire_Gun.onClick.AddListener(() => OnBuildingButtonClick(Turret_Fire_Gun_Prefab));
        Turret_Multi_Arrow.onClick.AddListener(() => OnBuildingButtonClick(Turret_Multi_Arrow_Prefab));
        Turret_QinCrossbow.onClick.AddListener(() => OnBuildingButtonClick(Turret_QinCrossbowPrefab));
        Turret_ZhugeCrossbow.onClick.AddListener(() => OnBuildingButtonClick(Turret_ZhugeCrossbowPrefab));
        Turret_ThrowStoneCannon.onClick.AddListener(() => OnBuildingButtonClick(Turret_ThrowStoneCannonPrefab));
        Turret_HuDunCannon.onClick.AddListener(() => OnBuildingButtonClick(Turret_HuDunCannonPrefab));       
//----------------------------------------注册按钮点击事件-------------------------------------------------//

        isPlacingBuilding = false;
        isCollisionDetected = false; // 初始化碰撞标记
        canPlaceOnResourcePoint = true; // 初始化资源点放置条件
    }

    void OnBuildingButtonClick(GameObject buildingPrefab)
    {
        
        if (buildingPrefab == null)
        {
            Debug.LogError("建筑预制体为空！");
            return;
        }
        
        // 检查资源是否足够
        BuildingBase buildingBase = buildingPrefab.GetComponent<BuildingBase>();
        if (buildingBase != null)
        {
            if (!HasEnoughResources(buildingBase))
            {
                ToastManager.Instance.ShowToast("资源不足，无法建造！", 2f);
                return;
            }
        }
        
        selectedBuildingPrefab = buildingPrefab;
        HidePanel(); // 隐藏面板
        isPlacingBuilding = true;
        CreatePreviewBuilding();
    }

    // 创建预览建筑
    void CreatePreviewBuilding()
    {
        if (selectedBuildingPrefab == null)
        {
            Debug.LogError("创建预览建筑时，所选建筑预制体为空！");
            return;
        }
        
        previewBuilding = Instantiate(selectedBuildingPrefab);
        
        // 禁用预览建筑上的所有脚本组件，保留碰撞检测
        BuildingBase buildingBase = previewBuilding.GetComponent<BuildingBase>();
        if (buildingBase != null)
        {
            buildingBase.enabled = false;
        }

        //设置预览建筑的标签为默认，防止被敌人锁定
        previewBuilding.tag = "Default";
        
        // 禁用自身对象可能影响功能的其他组件
        MonoBehaviour[] scripts = previewBuilding.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            if (script != null && script != buildingBase)
            {
                script.enabled = false;
            }
        }

        //禁用子对象可能影响功能的其他组件
        foreach (Transform child in previewBuilding.transform)
        {
            MonoBehaviour[] childScripts = child.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour script in childScripts)
            {
                if (script != null && script != buildingBase)
                {
                    script.enabled = false;
                }
            }
        }
        
        SpriteRenderer spriteRenderer = previewBuilding.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("预览建筑没有SpriteRenderer组件！");
            Destroy(previewBuilding);
            isPlacingBuilding = false;
            return;
        }
        
        // 设置半透明预览效果
        spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);
    }

    void Update()
    {
        if (isPlacingBuilding)
        {
            if (previewBuilding != null)
            {
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePosition.z = 0;
                previewBuilding.transform.position = mousePosition;
                Debug.Log($"建筑选择面板：预览建筑位置已设置为：{mousePosition}");

                // 检测碰撞和放置条件
                isCollisionDetected = CheckCollision();
                
                // 获取建筑基础组件
                BuildingBase buildingBase = selectedBuildingPrefab.GetComponent<BuildingBase>();
                canPlaceOnResourcePoint = true; // 默认可以放置
                
                // 检查特殊放置条件
                if (buildingBase != null)
                {
                    // 如果需要放在资源点上
                    if (buildingBase.isOnlyBePlacedOnGround)
                    {
                        canPlaceOnResourcePoint = IsOnResourcePoint("GroundResourcePoints");
                    }
                    // 如果需要放在资源点旁边
                    else if (buildingBase.isOnlyBePlacedAdjacent)
                    {
                        canPlaceOnResourcePoint = IsAdjacentToResourcePoint("AdjacentResourcePoints");
                    }
                }
                
                // 根据放置条件改变预览建筑颜色
                UpdatePreviewColor(isCollisionDetected || !canPlaceOnResourcePoint);
            }

            //左键单击放置建筑
            if (Input.GetMouseButtonDown(0))
            {          
                // 判断是否可以放置
                if (!isCollisionDetected && canPlaceOnResourcePoint)
                {
                    PlaceBuilding();
                }
                else
                {
                    // 显示提示信息
                    if (isCollisionDetected)
                    {
                        ToastManager.Instance.ShowToast("该位置已有建筑，无法放置！", 2f);
                    }
                    else if (!canPlaceOnResourcePoint)
                    {
                        BuildingBase buildingBase = selectedBuildingPrefab.GetComponent<BuildingBase>();
                        if (buildingBase != null)
                        {
                            if (buildingBase.isOnlyBePlacedOnGround)
                            {
                                ToastManager.Instance.ShowToast("该建筑只能放置在资源点上！", 2f);
                            }
                            else if (buildingBase.isOnlyBePlacedAdjacent)
                            {
                                ToastManager.Instance.ShowToast("该建筑只能放置在资源点旁边！", 2f);
                            }
                        }
                    }
                }
            }
            //右键单击取消放置建筑返回建造面板
            else if (Input.GetMouseButtonDown(1))
            {
                CancelBuildingPlacement();
            }
        }
    }

    // 更新预览颜色以显示是否可放置
    void UpdatePreviewColor(bool cannotPlace)
    {
        if (previewBuilding == null) return;
        
        SpriteRenderer spriteRenderer = previewBuilding.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) return;
        
        // 红色表示不可放置，绿色表示可以放置
        if (cannotPlace)
        {
            spriteRenderer.color = new Color(1f, 0.5f, 0.5f, 0.5f); // 半透明红色
        }
        else
        {
            spriteRenderer.color = new Color(0.5f, 1f, 0.5f, 0.5f); // 半透明绿色
        }
    }

    bool CheckCollision()
    {
        if (previewBuilding == null)
        {
            Debug.Log("建筑选择面板：在CheckCollision方法中，预览建筑为空。");
            return false;
        }

        BoxCollider2D previewCollider = previewBuilding.GetComponent<BoxCollider2D>();
        if (previewCollider == null)
        {
            Debug.Log("建筑选择面板：在CheckCollision方法中，预览建筑没有BoxCollider2D组件。");
            return false;
        }

        // 获取预览建筑的位置和大小
        Vector2 position = previewBuilding.transform.position;
        Vector2 size = previewCollider.size;
        Quaternion rotation = previewBuilding.transform.rotation;

        // 修改部分：只检测BoxCollider2D类型的碰撞体
        Collider2D[] hits = Physics2D.OverlapBoxAll(position, size, rotation.eulerAngles.z, LayerMask.GetMask("Player"));
        foreach (Collider2D hit in hits)
        {
            // 排除自身碰撞体
            if (hit.gameObject == previewBuilding) continue;
            
            // 新增类型判断
            if (hit is BoxCollider2D)
            {
                Debug.Log($"建筑选择面板：检测到有效碰撞体（BoxCollider2D）: {hit.gameObject.name}");
                return true;
            }
            else
            {
                Debug.Log($"建筑选择面板：忽略非BoxCollider2D碰撞体: {hit.GetType().Name}");
            }
        }
        return false;
    }

    // 检查是否放置在资源点上面（超过50%重叠）
    bool IsOnResourcePoint(string layerName)
    {
        Debug.Log($"检查是否放置在{layerName}上面");
        if (previewBuilding == null) return false;

        BoxCollider2D previewCollider = previewBuilding.GetComponent<BoxCollider2D>();
        if (previewCollider == null) return false;

        // 获取预览建筑的位置和大小
        Vector2 position = previewBuilding.transform.position;
        Vector2 size = previewCollider.size;
        Quaternion rotation = previewBuilding.transform.rotation;
        
        // 获取指定图层的索引
        int layerIndex = LayerMask.NameToLayer(layerName);
        if (layerIndex == -1)
        {
            Debug.LogError($"图层 {layerName} 不存在！");
            return false;
        }
        
        // 创建图层掩码
        int layerMask = 1 << layerIndex;
        
        // 检测与资源点的碰撞
        Collider2D[] resourcePoints = Physics2D.OverlapBoxAll(position, size, rotation.eulerAngles.z, layerMask);
        
        if (resourcePoints.Length == 0)
        {
            Debug.Log($"没有找到{layerName}资源点");
            return false;
        }
        
        // 检查重叠面积是否超过50%
        foreach (Collider2D resourcePoint in resourcePoints)
        {
            if (IsOverlappingByHalf(previewCollider, resourcePoint))
            {
                Debug.Log($"在{layerName}上放置成功，重叠超过50%");
                return true;
            }
        }
        
        Debug.Log($"在{layerName}上放置失败，重叠不足50%");
        return false;
    }
    
    // 检查是否与资源点重叠面积超过50%
    bool IsOverlappingByHalf(BoxCollider2D buildingCollider, Collider2D resourceCollider)
    {
        if (resourceCollider is BoxCollider2D)
        {
            BoxCollider2D resourceBoxCollider = resourceCollider as BoxCollider2D;
            
            // 计算两个碰撞体的边界
            Bounds buildingBounds = buildingCollider.bounds;
            Bounds resourceBounds = resourceBoxCollider.bounds;
            
            // 计算重叠区域
            float xOverlap = Mathf.Max(0, Mathf.Min(buildingBounds.max.x, resourceBounds.max.x) - Mathf.Max(buildingBounds.min.x, resourceBounds.min.x));
            float yOverlap = Mathf.Max(0, Mathf.Min(buildingBounds.max.y, resourceBounds.max.y) - Mathf.Max(buildingBounds.min.y, resourceBounds.min.y));
            
            // 计算重叠面积
            float overlapArea = xOverlap * yOverlap;
            
            // 计算建筑碰撞体的面积
            float buildingArea = buildingBounds.size.x * buildingBounds.size.y;
            
            // 计算重叠百分比
            float overlapPercentage = overlapArea / buildingArea;
            
            Debug.Log($"重叠百分比: {overlapPercentage * 100}%");
            
            // 如果重叠面积超过建筑面积的50%，则返回true
            return overlapPercentage >= 0.5f;
        }
        
        // 对于非BoxCollider2D类型的碰撞体，使用简单的碰撞检测
        return buildingCollider.IsTouching(resourceCollider);
    }
    
    // 检查是否放置在资源点旁边
    bool IsAdjacentToResourcePoint(string layerName)
    {
        Debug.Log($"检查是否放置在{layerName}旁边");
        if (previewBuilding == null) return false;
        
        BoxCollider2D previewCollider = previewBuilding.GetComponent<BoxCollider2D>();
        if (previewCollider == null) return false;
        
        // 获取预览建筑的位置
        Vector2 position = previewBuilding.transform.position;
        
        // 获取指定图层的索引
        int layerIndex = LayerMask.NameToLayer(layerName);
        if (layerIndex == -1)
        {
            Debug.LogError($"图层 {layerName} 不存在！");
            return false;
        }
        
        // 创建图层掩码
        int layerMask = 1 << layerIndex;
        
        // 定义两个检测范围
        // 外圈范围：用于检测是否在资源点附近
        Vector2 outerCheckSize = previewCollider.size * 1.8f; // 扩大100%检测范围
        
        // 内圈范围：用于检测是否与资源点直接接触
        Vector2 innerCheckSize = previewCollider.size * 1f; // 略大于建筑的范围
        
        // 检测在外圈范围内是否有资源点
        Collider2D[] outerResourcePoints = Physics2D.OverlapBoxAll(position, outerCheckSize, previewBuilding.transform.rotation.eulerAngles.z, layerMask);
        
        // 检测在内圈范围内是否有资源点
        Collider2D[] innerResourcePoints = Physics2D.OverlapBoxAll(position, innerCheckSize, previewBuilding.transform.rotation.eulerAngles.z, layerMask);
        
        // 条件1：外圈必须有资源点
        // 条件2：内圈不能有资源点（不直接接触）
        bool isNearby = outerResourcePoints.Length > 0;
        bool isTouching = innerResourcePoints.Length > 0;
        
        bool isCorrectlyAdjacent = isNearby && !isTouching;
        
        Debug.Log($"在{layerName}旁边放置: {isCorrectlyAdjacent}, 靠近资源点: {isNearby}, 直接接触: {isTouching}");
        Debug.Log($"检测到附近有{outerResourcePoints.Length}个资源点，直接接触有{innerResourcePoints.Length}个资源点");
        
        return isCorrectlyAdjacent;
    }

    void PlaceBuilding()
    {
        if (selectedBuildingPrefab == null || previewBuilding == null)
        {
            Debug.LogError("无法放置建筑。所选预制体或预览建筑为空！");
            return;
        }
        
        // 再次检查资源是否足够（以防玩家在选择建筑和放置之间资源发生变化）
        BuildingBase buildingBase = selectedBuildingPrefab.GetComponent<BuildingBase>();
        if (buildingBase != null)
        {
            if (!HasEnoughResources(buildingBase))
            {
                ToastManager.Instance.ShowToast("资源不足，无法建造！", 2f);
                CancelBuildingPlacement();
                return;
            }
            
            // 扣除资源
            ConsumeResources(buildingBase);
        }
        
        Vector3 buildingPosition = previewBuilding.transform.position;
        
        // 实例化实际建筑
        GameObject newBuilding = Instantiate(selectedBuildingPrefab, buildingPosition, Quaternion.identity, buildingParent);
        
        // 初始化建筑的建造状态
        BuildingBase newBuildingBase = newBuilding.GetComponent<BuildingBase>();
        if (newBuildingBase != null)
        {
            newBuildingBase.isUnderConstruction = true;
        }
        
        // 产生建筑放置效果
        if (buildingPlaceEffect != null)
        {
            GameObject effect = Instantiate(buildingPlaceEffect, buildingPosition, Quaternion.identity);
            Destroy(effect,2f);
        }
        
        // 播放建筑放置音效
        if (audioSource != null)
        {
            audioSource.Play();
        }
        
        Destroy(previewBuilding);
        isPlacingBuilding = false;
    }

    void CancelBuildingPlacement()
    {
        if (previewBuilding != null)
        {
            Destroy(previewBuilding);
        }
        ShowPanel(); // 取消放置时显示面板
        isPlacingBuilding = false;
        isCollisionDetected = false; // 重置碰撞标记
        canPlaceOnResourcePoint = true; // 重置资源点放置条件
    }

    // 新增：通过Canvas Group控制显示隐藏
    public void ShowPanel()
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void HidePanel()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    // 检查是否有足够的资源
    private bool HasEnoughResources(BuildingBase building)
    {
        PlayerConfig playerConfig = PlayerConfig.Instance;
        
        if (playerConfig == null)
        {
            Debug.LogError("无法找到PlayerConfig实例！");
            return false;
        }
        
        return playerConfig.woodNum >= building.cost_wood &&
               playerConfig.stoneNum >= building.cost_stone &&
               playerConfig.ironNum >= building.cost_iron &&
               playerConfig.copperNum >= building.cost_copper;
    }
    
    // 消耗资源
    private void ConsumeResources(BuildingBase building)
    {
        PlayerConfig playerConfig = PlayerConfig.Instance;
        
        if (playerConfig == null)
        {
            Debug.LogError("无法找到PlayerConfig实例！");
            return;
        }
        
        playerConfig.woodNum -= building.cost_wood;
        playerConfig.stoneNum -= building.cost_stone;
        playerConfig.ironNum -= building.cost_iron;
        playerConfig.copperNum -= building.cost_copper;
    }
}