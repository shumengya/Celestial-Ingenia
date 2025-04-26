using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitPanelController : MonoBehaviour
{
    public GameObject exitPanel;
    public void Start()
    {
        exitPanel.SetActive(false);
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            exitPanel.SetActive(!exitPanel.activeSelf);
        }
        if(exitPanel.activeSelf)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale =1f;

        }
    }
}
