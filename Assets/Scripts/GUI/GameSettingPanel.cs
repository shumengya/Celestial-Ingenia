using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSettingPanel : MonoBehaviour
{

public InputField woodInputField;
public InputField stoneInputField;
public InputField ironInputField;
public InputField copperInputField;
public Button sureButton;
public Button quitButton;
public CanvasGroup canvasGroup;


void Start()
{
    HidePanel();
    sureButton.onClick.AddListener(OnSureButtonClick);
    quitButton.onClick.AddListener(OnQuitButtonClick);
    woodInputField.text = PlayerConfig.Instance.woodNum.ToString();
    stoneInputField.text = PlayerConfig.Instance.stoneNum.ToString();
    ironInputField.text = PlayerConfig.Instance.ironNum.ToString();
    copperInputField.text = PlayerConfig.Instance.copperNum.ToString();
}

void OnSureButtonClick()
{
    PlayerConfig.Instance.woodNum = int.Parse(woodInputField.text);
    PlayerConfig.Instance.stoneNum = int.Parse(stoneInputField.text);
    PlayerConfig.Instance.ironNum = int.Parse(ironInputField.text);
    PlayerConfig.Instance.copperNum = int.Parse(copperInputField.text);
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