using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public Button BuildStructuresBtn;
    public Button TrainUnitsBtn;
    public BuildingSelectionPanel buildingSelectionPanel; // 引用建造面板


    //-----------游戏目前基本设置----------------------

    public Text woodText;
    public Text stoneText;
    public Text ironText;
    public Text copperText;

    private float timer = 0f;
    private const float updateInterval = 0.5f;

    private bool isBuildingPanelOpen = false;

    void Start()
    {
        BuildStructuresBtn.onClick.AddListener(() => OnBuildStructuresBtnClick());
        TrainUnitsBtn.onClick.AddListener(() => OnTrainUnitsBtnClick());
    }

    void Update()
    {
        // 累加计时器
        timer += Time.deltaTime;

        // 当计时器达到更新间隔时
        if (timer >= updateInterval)
        {
            // 更新文本显示
            if (woodText != null) woodText.text = "木材: " + PlayerConfig.Instance.woodNum.ToString();
            if (stoneText != null) stoneText.text = "石头: " + PlayerConfig.Instance.stoneNum.ToString();
            if (ironText != null) ironText.text = "铁矿: " + PlayerConfig.Instance.ironNum.ToString();
            if (copperText != null) copperText.text = "铜矿: " + PlayerConfig.Instance.copperNum.ToString();

            // 重置计时器
            timer = 0f;
        }
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
    }
}