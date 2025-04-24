using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : TrapBase
{
    public float damagePerSecond = 10f;
    private List<GameObject> trappedEnemies = new List<GameObject>();
    private bool isActivated = false;
    private float initEnemySpeed = 0;

    private float oneTimer = 0;
    private float fiveTimer = 0;

    protected override void OnTrapTrigger(Collider2D enemyObject)
    {

        if (!isUnderConstruction )
        {
            isActivated = true;
            if (!trappedEnemies.Contains(enemyObject.gameObject))
            {
                trappedEnemies.Add(enemyObject.gameObject);
            }
            
            UnitMovement enemyMovement = enemyObject.GetComponent<UnitMovement>();
            if (enemyMovement != null)
            {
                initEnemySpeed = enemyMovement.moveSpeed;
                enemyMovement.moveSpeed = 0;
            }
        }
    }

    protected override void Update()
    {
        base.Update();

        if (!isUnderConstruction && isActivated)
        {
            oneTimer += Time.deltaTime;
            if (oneTimer >= 1f)
            { 
                oneTimer = 0;
                foreach (GameObject enemy in trappedEnemies)
                {
                    if (enemy != null)
                    {
                        HealthBar healthBar = enemy.GetComponentInChildren<HealthBar>();
                        if (healthBar != null)
                        {
                            healthBar.TakeDamage(damagePerSecond);
                        }
                    }
                }
            }
            
            fiveTimer += Time.deltaTime;
            if (fiveTimer >= 10f)
            {
                fiveTimer = 0;
                foreach (GameObject enemy in trappedEnemies)
                {
                    if (enemy != null)
                    {
                        UnitMovement enemyMovement = enemy.GetComponent<UnitMovement>();
                        if (enemyMovement != null)
                        {
                            enemyMovement.moveSpeed = initEnemySpeed;
                        }
                    }
                }
                Destroy(gameObject);
            }
        }
    }
}