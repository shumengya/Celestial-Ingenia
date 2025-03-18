using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bug : MonoBehaviour
{
    private HealthBar healthBar;

    void Start()
    {
        healthBar = GetComponentInChildren<HealthBar>();
    }


    void Update()
    {
        if (healthBar.IsDead())
        {
            Destroy(gameObject);
        }
    }
}
