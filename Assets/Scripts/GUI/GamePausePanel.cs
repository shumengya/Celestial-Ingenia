using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePausePanel : MonoBehaviour
{
    public Button continueButton;
    public Button ToMainSceneButton;   
    public CanvasGroup canvasGroup;
    public bool isPauseing = false;

    void Start()
    {
        HidePanel();
        continueButton.onClick.AddListener(ContinueGame);
        ToMainSceneButton.onClick.AddListener(ToMainScene);
    }
    void ContinueGame(){

        HidePanel();
    }

    void ToMainScene(){
        SceneManager.LoadScene("MainMenu");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) )
        {
            if( isPauseing == false){
                ShowPanel();
            }else{
                HidePanel();
            }

        }
    }

    public void ShowPanel()
    {
        isPauseing = true;
        Time.timeScale = 0;
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void HidePanel()
    {
        isPauseing = false;
        Time.timeScale = 1;
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

}
