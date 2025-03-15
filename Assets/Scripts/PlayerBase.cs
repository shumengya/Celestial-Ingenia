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
        // 累加计时器
        timer += Time.deltaTime;

        // 当计时器达到 1 秒时
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
        // 当有物体与基地发生碰撞时，在控制台输出信息
        Debug.Log("发生了碰撞!");
    }

    void OnObjectClicked()
    {
        Debug.Log("这个物体被点击了");
    }
}
