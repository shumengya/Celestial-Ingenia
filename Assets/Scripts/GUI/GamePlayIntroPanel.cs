using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayIntroPanel : MonoBehaviour
{

public Button quitButton;
public CanvasGroup canvasGroup;

    void Start()
    {
        quitButton.onClick.AddListener(OnQuitButtonClick);
    }

    void OnQuitButtonClick()
    {
        //Application.Quit();
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
