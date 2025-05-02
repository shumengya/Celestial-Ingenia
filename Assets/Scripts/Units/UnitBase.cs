using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBase : MonoBehaviour
{
    protected HealthBar healthBar;
    public bool isHideHealthBar = false;
    public string smyName = "单位";
    public string smyType = "单位";
    public string smyDescription = "这是一个单位";

    protected virtual void Start()
    {
        healthBar = GetComponentInChildren<HealthBar>();
        // 根据 isHideHealthBar 的初始值设置血量条的显示状态
        SetHealthBarVisibility();
    }

    protected virtual void Update()
    {
        if (healthBar.IsDead())
        {
            Destroy(gameObject);
        }
        // 动态检查是否需要隐藏血量条
        //SetHealthBarVisibility();
    }

    protected void SetHealthBarVisibility()
    {
        if (healthBar != null)
        {
            // 根据 isHideHealthBar 的值设置血量条对象的激活状态
            healthBar.gameObject.SetActive(!isHideHealthBar);
        }
    }
} 