using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public Button BuildStructuresBtn;
    public Button TrainUnitsBtn;
    public BuildingSelectionPanel buildingSelectionPanel; // 引用建造面板

    private bool isBuildingPanelOpen = false;

    void Start()
    {
        BuildStructuresBtn.onClick.AddListener(() => OnBuildStructuresBtnClick());
        TrainUnitsBtn.onClick.AddListener(() => OnTrainUnitsBtnClick());
    }

    void Update()
    {
        // 可根据需求添加其他更新逻辑
    }

    public void OnBuildStructuresBtnClick()
    {
        if (isBuildingPanelOpen)
        {
            // 关闭建造面板
            buildingSelectionPanel.HidePanel();
        }
        else
        {
            // 打开建造面板
            buildingSelectionPanel.ShowPanel();
        }
        isBuildingPanelOpen = !isBuildingPanelOpen;
    }

    public void OnTrainUnitsBtnClick()
    {
        // 可添加训练单位按钮点击后的逻辑
    }
}