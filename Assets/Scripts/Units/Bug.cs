using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bug : MonoBehaviour
{
    private HealthBar healthBar;
    public bool isHideHealthBar = false;
    public string smyName = "����";
    public string smyType = "��λ";
    public string smyDescription = "����һֻ����";

    void Start()
    {
        healthBar = GetComponentInChildren<HealthBar>();
        // ���� isHideHealthBar �ĳ�ʼֵ����Ѫ��������ʾ״̬
        SetHealthBarVisibility();
    }

    void Update()
    {
        if (healthBar.IsDead())
        {
            Destroy(gameObject);
        }
        // ��̬����Ƿ���Ҫ����Ѫ����
        //SetHealthBarVisibility();
    }

    private void SetHealthBarVisibility()
    {
        if (healthBar != null)
        {
            // ���� isHideHealthBar ��ֵ����Ѫ��������ļ���״̬
            healthBar.gameObject.SetActive(!isHideHealthBar);
        }
    }
}