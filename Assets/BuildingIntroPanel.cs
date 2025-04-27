using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingIntroPanel : MonoBehaviour
{
    public Text wood_text;
    public Text stone_text;
    public Text copper_text;
    public Text iron_text;
    public Text building_intro;
    public Text building_name;
    public Text building_type;
    public Text built_time;
    public Text building_health;
    public Button quit_button;
    public CanvasGroup canvasGroup; 

    public void Start()
    {
        quit_button.onClick.AddListener(()=> OnQuitButtonClick());
        HidePanel();
    }

    public void OnQuitButtonClick(){
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
