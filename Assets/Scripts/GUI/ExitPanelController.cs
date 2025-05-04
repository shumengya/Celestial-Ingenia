using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitPanelController : MonoBehaviour
{
    public GameObject exitPanel;
     public CanvasGroup canvasGroup; 
    public void Start()
    {
        exitPanel.SetActive(false);
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
