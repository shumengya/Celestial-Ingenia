using UnityEngine;
using UnityEngine.UI;

public class GameSettingPanel : MonoBehaviour
{

    public InputField woodInputField;
    public InputField stoneInputField;
    public InputField ironInputField;
    public InputField copperInputField;
    public InputField playerNameInputField;
    public Button sureButton;
    public Button sureButton2;
    public Button quitButton;
    public CanvasGroup canvasGroup;


    void Start()
    {
        HidePanel();
        sureButton.onClick.AddListener(OnSureButtonClick);
        sureButton2.onClick.AddListener(OnSureButton2Click);
        quitButton.onClick.AddListener(OnQuitButtonClick);
        //读取本地玩家现有数据
        PlayerData loadedData = PlayerDataManager.LoadPlayerData();
        woodInputField.text = loadedData.woodNum.ToString();
        stoneInputField.text = loadedData.stoneNum.ToString();
        ironInputField.text = loadedData.ironNum.ToString();
        copperInputField.text = loadedData.copperNum.ToString();
        playerNameInputField.text = loadedData.playerName;
    }

    void OnSureButtonClick()
    {
        //保存玩家设置到本地文件
        PlayerData playerData = PlayerDataManager.LoadPlayerData();
        playerData.woodNum = int.Parse(woodInputField.text);
        playerData.stoneNum = int.Parse(stoneInputField.text);
        playerData.ironNum = int.Parse(ironInputField.text);
        playerData.copperNum = int.Parse(copperInputField.text);
        PlayerDataManager.SavePlayerData(playerData);
    }

    void OnSureButton2Click()
    {
        //保存玩家设置到本地文件
        PlayerData playerData = PlayerDataManager.LoadPlayerData();
        playerData.playerName = playerNameInputField.text;
        PlayerDataManager.SavePlayerData(playerData);
    }


    void OnQuitButtonClick()
    {
        HidePanel();
    }

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