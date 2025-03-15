using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AsyncSceneLoader : MonoBehaviour
{
    public string sceneName;
    public Slider progressSlider;

    private AsyncOperation asyncOperation;

    private void Start()
    {
        // ��ʼ�첽���س���
        StartCoroutine(LoadSceneAsync());
    }

    private System.Collections.IEnumerator LoadSceneAsync()
    {
        // �첽����ָ�����Ƶĳ���
        asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        // ��ֹ����������ɺ��Զ�����
        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
            // ��ȡ���ؽ��ȣ���Χ�� 0 �� 0.9
            float progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);
            // ���½�������ʾ
            if (progressSlider != null)
            {
                progressSlider.value = progress;
            }

            // �����ؽ��ȴﵽ 0.9 ʱ����������������
            if (progress >= 0.9f)
            {
                if (Input.anyKeyDown)
                {
                    asyncOperation.allowSceneActivation = true;
                }
            }

            yield return null;
        }
    }
}