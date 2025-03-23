using UnityEngine;
using UnityEngine.UI;

public class BuildingSelectionPanel : MonoBehaviour
{
    public Button PlayerBase;
    public Button TimberMill;
    public Button StoneQuarry;
    public Button IronFactory;

    public GameObject playerBasePrefab;
    public GameObject timberMillPrefab;
    public GameObject stoneQuarryPrefab;
    public GameObject IronFactoryPrefab;

    public Transform buildingParent;
    public CanvasGroup canvasGroup; // 新增：通过Canvas Group控制显示

    private GameObject selectedBuildingPrefab;
    private GameObject previewBuilding;
    private bool isPlacingBuilding;

    void Start()
    {
        // 初始化时隐藏面板（透明度设为0，禁用交互）
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        PlayerBase.onClick.AddListener(() => OnBuildingButtonClick(playerBasePrefab));
        TimberMill.onClick.AddListener(() => OnBuildingButtonClick(timberMillPrefab));
        StoneQuarry.onClick.AddListener(() => OnBuildingButtonClick(stoneQuarryPrefab));
        IronFactory.onClick.AddListener(() => OnBuildingButtonClick(IronFactoryPrefab));
        isPlacingBuilding = false;
    }

    void OnBuildingButtonClick(GameObject buildingPrefab)
    {
        if (buildingPrefab == null)
        {
            Debug.LogError("Building prefab is null!");
            return;
        }
        selectedBuildingPrefab = buildingPrefab;
        HidePanel(); // 隐藏面板
        isPlacingBuilding = true;
        CreatePreviewBuilding();
    }

    void CreatePreviewBuilding()
    {
        if (selectedBuildingPrefab == null)
        {
            Debug.LogError("Selected building prefab is null when creating preview!");
            return;
        }
        previewBuilding = Instantiate(selectedBuildingPrefab);
        SpriteRenderer spriteRenderer = previewBuilding.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("Preview building does not have a SpriteRenderer component!");
            Destroy(previewBuilding);
            isPlacingBuilding = false;
            return;
        }
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
            }

            if (Input.GetMouseButtonDown(0))
            {
                PlaceBuilding();
            }
            else if (Input.GetMouseButtonDown(1))
            {
                CancelBuildingPlacement();
            }
        }
    }

    void PlaceBuilding()
    {
        if (selectedBuildingPrefab == null || previewBuilding == null)
        {
            Debug.LogError("Cannot place building. Selected prefab or preview building is null!");
            return;
        }
        Instantiate(selectedBuildingPrefab, previewBuilding.transform.position, Quaternion.identity, buildingParent);
        Destroy(previewBuilding);
        isPlacingBuilding = false;
        // 放置后保持面板隐藏
    }

    void CancelBuildingPlacement()
    {
        if (previewBuilding != null)
        {
            Destroy(previewBuilding);
        }
        ShowPanel(); // 取消放置时显示面板
        isPlacingBuilding = false;
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
}