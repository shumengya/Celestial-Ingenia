using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class FloatingText : MonoBehaviour
{
    public Text textPrefab;
    public float defaultFloatSpeed = 1f;
    public float defaultLifeTime = 1f;
    public int poolSize = 10;

    private List<Text> objectPool = new List<Text>();

    private void Start()
    {
        InitializeObjectPool();
    }

    private void InitializeObjectPool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            Text newText = Instantiate(textPrefab, transform);
            newText.gameObject.SetActive(false);
            objectPool.Add(newText);
        }
    }

    public void ShowFloatingText(string text, float lifeTime = -1, float floatSpeed = -1)
    {
        if (lifeTime < 0) lifeTime = defaultLifeTime;
        if (floatSpeed < 0) floatSpeed = defaultFloatSpeed;

        Text availableText = GetAvailableText();
        if (availableText != null)
        {
            availableText.text = text;
            availableText.color = Random.ColorHSV();
            availableText.gameObject.SetActive(true);

            Material material = availableText.material;
            StartCoroutine(AnimateText(availableText, material, lifeTime, floatSpeed));
        }
    }

    private Text GetAvailableText()
    {
        foreach (Text text in objectPool)
        {
            if (!text.gameObject.activeSelf)
            {
                return text;
            }
        }
        // 如果对象池没有可用对象，创建一个新的
        Text newText = Instantiate(textPrefab, transform);
        objectPool.Add(newText);
        return newText;
    }

    private System.Collections.IEnumerator AnimateText(Text text, Material material, float lifeTime, float floatSpeed)
    {
        float timer = 0f;
        while (timer < lifeTime)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, timer / lifeTime);
            float offsetY = Mathf.Lerp(0f, 1f, timer / lifeTime) * floatSpeed;

            material.SetFloat("_Alpha", alpha);
            material.SetFloat("_OffsetY", offsetY);

            yield return null;
        }
        text.gameObject.SetActive(false);
    }
}    