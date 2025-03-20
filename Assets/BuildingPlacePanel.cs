using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingPlacePanel : MonoBehaviour
{
    public Image PlayerBase;
    public Image TimberMill;
    public Image StoneQuarry;

    public GameObject playerBasePrefab;
    public GameObject timberMillPrefab;
    public GameObject stoneQuarryPrefab;

    private GameObject currentBuildingToPlace;
    private bool isPlacingBuilding = false;

    void Start()
    {
        // 为每个 Image 添加点击事件监听器
        PlayerBase.GetComponent<Button>().onClick.AddListener(() => StartPlacingBuilding(playerBasePrefab));
        TimberMill.GetComponent<Button>().onClick.AddListener(() => StartPlacingBuilding(timberMillPrefab));
        StoneQuarry.GetComponent<Button>().onClick.AddListener(() => StartPlacingBuilding(stoneQuarryPrefab));
    }

    void Update()
    {
        if (isPlacingBuilding)
        {
            // 获取鼠标位置并转换为世界坐标
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;

            // 可以在这里添加预览建筑的逻辑，例如创建一个半透明的建筑模型跟随鼠标

            if (Input.GetMouseButtonDown(0))
            {
                // 鼠标左键单击，放置建筑
                Instantiate(currentBuildingToPlace, mousePosition, Quaternion.identity);
                StopPlacingBuilding();
            }
        }
    }

    private void StartPlacingBuilding(GameObject buildingPrefab)
    {
        // 隐藏当前面板
        gameObject.SetActive(false);

        // 记录要放置的建筑类型
        currentBuildingToPlace = buildingPrefab;
        isPlacingBuilding = true;
    }

    private void StopPlacingBuilding()
    {
        // 显示面板
        gameObject.SetActive(true);

        // 重置状态
        currentBuildingToPlace = null;
        isPlacingBuilding = false;
    }
}