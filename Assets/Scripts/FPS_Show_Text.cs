using UnityEngine;
using UnityEngine.UI;

public class FPS_Show_Text : MonoBehaviour
{
    public Text fpsText; // ������ʾ֡�ʵ� Text ���
    private float deltaTime = 0.0f;
    private float timer = 0.0f;
    private const float updateInterval = 0.5f;

    void Start()
    {
        // ȷ�� fpsText ����ȷ��ֵ
        if (fpsText == null)
        {
            fpsText = GetComponent<Text>();
        }
    }

    void Update()
    {
        // ����ÿ֡��ʱ����
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        // �ۼӼ�ʱ��
        timer += Time.unscaledDeltaTime;

        // ����ʱ���ﵽ���¼��ʱ
        if (timer >= updateInterval)
        {
            // ����֡��
            float fps = 1.0f / deltaTime;

            // �����ı���ʾ
            if (fpsText != null)
            {
                fpsText.text = string.Format("FPS: {0:0.}", fps);
            }

            // ���ü�ʱ��
            timer = 0;
        }
    }
}