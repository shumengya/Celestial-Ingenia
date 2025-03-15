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
        // 开始异步加载场景
        StartCoroutine(LoadSceneAsync());
    }

    private System.Collections.IEnumerator LoadSceneAsync()
    {
        // 异步加载指定名称的场景
        asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        // 阻止场景加载完成后自动激活
        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
            // 获取加载进度，范围是 0 到 0.9
            float progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);
            // 更新进度条显示
            if (progressSlider != null)
            {
                progressSlider.value = progress;
            }

            // 当加载进度达到 0.9 时，按下任意键激活场景
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