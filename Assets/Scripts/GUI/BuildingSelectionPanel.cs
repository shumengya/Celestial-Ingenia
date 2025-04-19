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
//-------------------------建筑预制体注册----------------------------------//

    [Header("其他设置")]
    public Transform buildingParent;
    public CanvasGroup canvasGroup; 
    private GameObject selectedBuildingPrefab;
    private GameObject previewBuilding;
    private bool isPlacingBuilding;
    private bool isCollisionDetected; // 新增：用于标记是否发生碰撞

    void Start()
    {
        Debug.Log("建筑选择面板：Start方法已调用。");
        // 初始化时隐藏面板（透明度设为0，禁用交互）
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

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
//----------------------------------------注册按钮点击事件-------------------------------------------------//

        isPlacingBuilding = false;
        isCollisionDetected = false; // 初始化碰撞标记
    }

    void OnBuildingButtonClick(GameObject buildingPrefab)
    {
        Debug.Log($"建筑选择面板：OnBuildingButtonClick方法被调用，传入预制体：{buildingPrefab?.name}");
        if (buildingPrefab == null)
        {
            Debug.LogError("建筑预制体为空！");
            return;
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
        buildingBase.tag = "Default";
        
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
            Debug.Log("建筑选择面板：Update方法 - 正在放置建筑。");
            if (previewBuilding != null)
            {
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePosition.z = 0;
                previewBuilding.transform.position = mousePosition;
                Debug.Log($"建筑选择面板：预览建筑位置已设置为：{mousePosition}");

                // 检测碰撞
                isCollisionDetected = CheckCollision();
                Debug.Log($"建筑选择面板：碰撞检测结果：{isCollisionDetected}");
            }

            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("建筑选择面板：鼠标左键被点击。");
                if (!isCollisionDetected)
                {
                    PlaceBuilding();
                }
                else
                {
                    // 显示提示信息，例如使用之前封装的 Toast 弹窗
                    Debug.Log("建筑选择面板：由于碰撞，无法放置建筑。");
                    ToastManager.Instance.ShowToast("该位置已有建筑，无法放置！", 2f);
                }
            }
            else if (Input.GetMouseButtonDown(1))
            {
                Debug.Log("建筑选择面板：鼠标右键被点击。");
                CancelBuildingPlacement();
            }
        }
    }

    bool CheckCollision()
    {
        Debug.Log("建筑选择面板：CheckCollision方法已调用。");
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

    void PlaceBuilding()
    {
        Debug.Log("建筑选择面板：PlaceBuilding方法已调用。");
        if (selectedBuildingPrefab == null || previewBuilding == null)
        {
            Debug.LogError("无法放置建筑。所选预制体或预览建筑为空！");
            return;
        }
        
        // 实例化实际建筑
        GameObject newBuilding = Instantiate(selectedBuildingPrefab, previewBuilding.transform.position, Quaternion.identity, buildingParent);
        
        // 初始化建筑的建造状态
        BuildingBase buildingBase = newBuilding.GetComponent<BuildingBase>();
        if (buildingBase != null)
        {
            buildingBase.isUnderConstruction = true;
        }
        
        Destroy(previewBuilding);
        isPlacingBuilding = false;
    }

    void CancelBuildingPlacement()
    {
        Debug.Log("建筑选择面板：CancelBuildingPlacement方法已调用。");
        if (previewBuilding != null)
        {
            Destroy(previewBuilding);
        }
        ShowPanel(); // 取消放置时显示面板
        isPlacingBuilding = false;
        isCollisionDetected = false; // 重置碰撞标记
    }

    // 新增：通过Canvas Group控制显示隐藏
    public void ShowPanel()
    {
        Debug.Log("建筑选择面板：ShowPanel方法已调用。");
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void HidePanel()
    {
        Debug.Log("建筑选择面板：HidePanel方法已调用。");
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
}