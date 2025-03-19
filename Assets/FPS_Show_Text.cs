using UnityEngine;
using UnityEngine.UI;

public class FPS_Show_Text : MonoBehaviour
{
    public Text fpsText; // 用于显示帧率的 Text 组件
    private float deltaTime = 0.0f;
    private float timer = 0.0f;
    private const float updateInterval = 0.5f;

    void Start()
    {
        // 确保 fpsText 已正确赋值
        if (fpsText == null)
        {
            fpsText = GetComponent<Text>();
        }
    }

    void Update()
    {
        // 计算每帧的时间间隔
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        // 累加计时器
        timer += Time.unscaledDeltaTime;

        // 当计时器达到更新间隔时
        if (timer >= updateInterval)
        {
            // 计算帧率
            float fps = 1.0f / deltaTime;

            // 更新文本显示
            if (fpsText != null)
            {
                fpsText.text = string.Format("FPS: {0:0.}", fps);
            }

            // 重置计时器
            timer = 0;
        }
    }
}