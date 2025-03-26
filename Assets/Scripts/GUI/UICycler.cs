using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEditor.Experimental.GraphView;

public class UICyclerWithEventTrigger : MonoBehaviour
{
    // 存储所有 UI 元素的列表
    public List<GameObject> uiElements;
    // 当前激活的 UI 索引
    private int currentIndex = 0;

    void Start()
    {
        // 确保至少有一个 UI 元素
        if (uiElements.Count > 0)
        {
            // 显示第一个 UI
            ShowUI(currentIndex);

            // 为每个 UI 元素添加点击事件监听器
            for (int i = 0; i < uiElements.Count; i++)
            {
                int index = i;
                AddEventTrigger(uiElements[i], () => OnUIClick(index));
            }
        }
    }

    // 为 UI 元素添加 EventTrigger 点击事件
    void AddEventTrigger(GameObject ui, System.Action onClick)
    {
        EventTrigger trigger = ui.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = ui.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((data) => onClick());
        trigger.triggers.Add(entry);
    }

    // 显示指定索引的 UI
    void ShowUI(int index)
    {
        // 隐藏所有 UI
        for (int i = 0; i < uiElements.Count; i++)
        {
            uiElements[i].SetActive(false);
        }
        // 显示指定索引的 UI
        uiElements[index].SetActive(true);
    }

    // 处理 UI 点击事件
    void OnUIClick(int clickedIndex)
    {
        // 计算下一个 UI 的索引
        currentIndex = (clickedIndex + 1) % uiElements.Count;
        // 显示下一个 UI
        if((clickedIndex + 1) % uiElements.Count!=0)
        {
             ShowUI(currentIndex);
        }
        else 
        {
            SceneManager.LoadScene("MainGame");
            Time.timeScale = 1f;
        }
    }
}