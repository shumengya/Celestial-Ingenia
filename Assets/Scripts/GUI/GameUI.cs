using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public Button BuildStructuresBtn;
    public Button TrainUnitsBtn;
    public BuildingSelectionPanel buildingSelectionPanel; // ���ý������

    private bool isBuildingPanelOpen = false;

    void Start()
    {
        BuildStructuresBtn.onClick.AddListener(() => OnBuildStructuresBtnClick());
        TrainUnitsBtn.onClick.AddListener(() => OnTrainUnitsBtnClick());
    }

    void Update()
    {
        // �ɸ�������������������߼�
    }

    public void OnBuildStructuresBtnClick()
    {
        if (isBuildingPanelOpen)
        {
            // �رս������
            buildingSelectionPanel.HidePanel();
        }
        else
        {
            // �򿪽������
            buildingSelectionPanel.ShowPanel();
        }
        isBuildingPanelOpen = !isBuildingPanelOpen;
    }

    public void OnTrainUnitsBtnClick()
    {
        // �����ѵ����λ��ť�������߼�
    }
}