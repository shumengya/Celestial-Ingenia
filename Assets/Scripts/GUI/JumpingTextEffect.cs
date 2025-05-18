using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class JumpingTextEffect : MonoBehaviour
{
    public List<string> randomTexts = new List<string>()
    {
        "你好，世界!",
        "灵创，你好!",
        "传奇的人生始于平凡!"
    };

    public float jumpSpeed = 2f;
    public float jumpHeight = 0.1f;
    public float angle = 30f;

    // 新增的缩放相关参数
    public float scaleSpeed = 1f;
    public float minScale = 0.9f;
    public float maxScale = 1.1f;

    private Text textComponent;
    private Vector3 startPosition;
    private float timeOffset;

    void Start()
    {
        textComponent = GetComponent<Text>();
        startPosition = transform.position;
        timeOffset = Random.Range(0f, 10f);

        // 随机选择一个文本
        int randomIndex = Random.Range(0, randomTexts.Count);
        textComponent.text = randomTexts[randomIndex];
    }

    void Update()
    {
        float jumpOffset = Mathf.Sin((Time.time + timeOffset) * jumpSpeed) * jumpHeight;
        Vector3 newPosition = startPosition + Quaternion.Euler(0, 0, angle) * Vector3.up * jumpOffset;
        transform.position = newPosition;

        // 新增的缩放逻辑
        float scaleFactor = Mathf.Lerp(minScale, maxScale, (Mathf.Sin(Time.time * scaleSpeed) + 1) / 2);
        transform.localScale = new Vector3(scaleFactor, scaleFactor, 1);
    }
}    