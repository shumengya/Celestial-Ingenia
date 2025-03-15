using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    public ClickResponder clickResponder;
    private HealthBar playerHealthBar;
    public float damageAmount = 1f;
    private float timer = 0f;

    void Start()
    {
        clickResponder.OnClicked += OnObjectClicked;
        playerHealthBar = GetComponentInChildren<HealthBar>();
    }


    void Update()
    {
        // �ۼӼ�ʱ��
        timer += Time.deltaTime;

        // ����ʱ���ﵽ 1 ��ʱ
        if (timer >= 1f)
        {
            if (playerHealthBar != null)
            {
                playerHealthBar.TakeDamage(damageAmount);
            }
            timer = 0f;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // ������������ط�����ײʱ���ڿ���̨�����Ϣ
        Debug.Log("��������ײ!");
    }

    void OnObjectClicked()
    {
        Debug.Log("������屻�����");
    }
}
