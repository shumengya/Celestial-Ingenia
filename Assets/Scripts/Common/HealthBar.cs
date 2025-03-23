using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image healthBarImage;
    public Text text;

    public float maxHealth = 100f;
    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        healthBarImage.fillAmount = currentHealth / maxHealth;
        // 更新 Text 组件的文本内容，按照 "当前生命值/最大生命值" 的格式显示
        text.text = Mathf.FloorToInt(currentHealth) + "/" + Mathf.FloorToInt(maxHealth);
    }

    public void Heal(float healAmount)
    {
        currentHealth += healAmount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        UpdateHealthBar();
    }

    public void SetMaxHealth(float maxHealth)
    {
        this.maxHealth = maxHealth;
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public bool IsDead()
    {
        return currentHealth <= 0;
    }

    public void SetHealth(float health)
    {
        currentHealth = health;
        UpdateHealthBar();
    }


}