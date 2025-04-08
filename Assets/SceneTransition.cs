using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneTransition : MonoBehaviour {
    [SerializeField] private Image transitionImage; // 绑定自身 Image 组件
    [SerializeField] private float fadeDuration = 0.5f; // 过渡动画时长（秒）
    private Canvas parentCanvas; // 父级 Canvas（自动获取 GUI Canvas）

    private void Awake() {
        // 确保父节点为已有的 GUI Canvas（需提前将预制体拖入该 Canvas 下）
        parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas == null) {
            Debug.LogError("SceneTransition 必须作为 GUI Canvas 的子节点！");
        }
        // 初始状态：禁用（不显示且不阻挡点击）
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 触发场景过渡动画并加载场景
    /// </summary>
    /// <param name="sceneName">目标场景名称</param>
    public void FadeToScene(string sceneName) {
        if (!gameObject.activeSelf) {
            gameObject.SetActive(true); // 激活预制体
            StartCoroutine(ExecuteTransition(sceneName));
        }
    }

    private IEnumerator ExecuteTransition(string sceneName) {
        // 淡入动画（Alpha 0→1，遮罩显示）
        yield return Fade(1);

        // 异步加载场景（可在此处添加加载进度逻辑）
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName);
        while (!loadOperation.isDone) {
            yield return null;
        }

        // 淡出动画（Alpha 1→0，遮罩隐藏）
        yield return Fade(0);

        // 动画结束后禁用并销毁（避免残留）
        gameObject.SetActive(false);
        Destroy(gameObject, fadeDuration); // 延迟销毁，确保淡出完成
    }

    /// <summary>
    /// 淡入/淡出插值协程
    /// </summary>
    /// <param name="targetAlpha">目标透明度（0-1）</param>
    private IEnumerator Fade(float targetAlpha) {
        float startAlpha = transitionImage.color.a;
        float elapsed = 0;

        while (elapsed < fadeDuration) {
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
            transitionImage.color = new Color(0, 0, 0, alpha); // 遮罩颜色可自定义
            elapsed += Time.deltaTime;
            yield return null;
        }
        transitionImage.color = new Color(0, 0, 0, targetAlpha); // 确保最终值准确
    }
}