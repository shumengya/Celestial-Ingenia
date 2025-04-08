using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;

public class UICycler : MonoBehaviour
{
    // 存储所有 Image 元素的列表
    public List<Image> images;
    // 存储所有 Text 元素的列表
    public List<Text> texts;
    // 存储原始文本内容
    private List<string> originalTexts = new List<string>();
    // 当前激活的 Image 索引
    private int currentImageIndex = 0;
    // 当前激活的 Text 索引
    private int currentTextIndex = 0;
    // 打字机效果的速度
    public float typingSpeed = 0.1f;
    // 图片过渡动画的时间
    public float transitionTime = 0.5f;
    
    // 用于直接淡入设置
    private bool isInitialized = false;
    // 用于追踪打字机效果是否正在进行
    private bool isTyping = false;
    // 用于保存当前的打字机协程
    private Coroutine currentTypingCoroutine = null;

    void Awake()
    {
        // 保存所有文本的原始内容
        SaveOriginalTexts();
       // DontDestroyOnLoad(gameObject);
        
        // 给对象添加标签，以便在场景切换时能够被识别和清理
        //gameObject.tag = "UICycler";
    }

    void SaveOriginalTexts()
    {
        originalTexts.Clear();
        foreach (Text text in texts)
        {
            if (text != null)
            {
                originalTexts.Add(text.text);
            }
            else
            {
                originalTexts.Add("");
            }
        }
        Debug.Log($"Saved {originalTexts.Count} original texts");
    }

    void OnEnable()
    {
        // 当启用时，确保初始化UI
        isInitialized = false;
    }
    
    void Start()
    {
        // 在Start中也尝试初始化，双重保险
        InitializeUI();
    }

    void Update()
    {
        // 确保UI正确初始化
        if (!isInitialized)
        {
            InitializeUI();
        }
    }

    void InitializeUI()
    {
        // 防止重复初始化
        if (isInitialized) return;
        
        Debug.Log("Initializing UI - Images count: " + images.Count);
        Debug.Log("Initializing UI - Texts count: " + texts.Count);
        
        // 重置索引
        currentImageIndex = 0;
        currentTextIndex = 0;

        // 检查引用是否有效
        if (CheckReferences())
        {
            // 直接显示第一个图片和文本 - 不使用动画，避免timing issues
            DirectlyShowFirstImageAndText();
            isInitialized = true;
            Debug.Log("UI successfully initialized");
        }
        else
        {
            Debug.LogError("Failed to initialize UI due to missing references");
        }
    }
    
    // 检查所有引用是否有效
    bool CheckReferences()
    {
        if (images == null || images.Count == 0)
        {
            Debug.LogWarning("No images found");
            return false;
        }
        
        if (texts == null || texts.Count == 0)
        {
            Debug.LogWarning("No texts found");
            return false;
        }
        
        // 检查每个图片引用
        for (int i = 0; i < images.Count; i++)
        {
            if (images[i] == null)
            {
                Debug.LogWarning($"Image at index {i} is null");
                return false;
            }
        }
        
        // 检查每个文本引用
        for (int i = 0; i < texts.Count; i++)
        {
            if (texts[i] == null)
            {
                Debug.LogWarning($"Text at index {i} is null");
                return false;
            }
        }
        
        return true;
    }
    
    // 直接显示第一个图片和文本，不使用渐变动画
    void DirectlyShowFirstImageAndText()
    {
        // 隐藏所有图片
        for (int i = 0; i < images.Count; i++)
        {
            CanvasGroup canvasGroup = images[i].GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = images[i].gameObject.AddComponent<CanvasGroup>();
            }
            
            if (i == currentImageIndex)
            {
                // 直接显示当前图片
                canvasGroup.alpha = 1f;
                canvasGroup.blocksRaycasts = true;
                Debug.Log($"Directly showing image {i}");
            }
            else
            {
                // 隐藏其他图片
                canvasGroup.alpha = 0f;
                canvasGroup.blocksRaycasts = false;
            }
        }
        
        // 启动打字机效果显示第一个文本
        if (currentTextIndex < texts.Count && currentTextIndex < originalTexts.Count)
        {
            currentTypingCoroutine = StartCoroutine(TypeText(texts[currentTextIndex], originalTexts[currentTextIndex]));
            Debug.Log($"Starting typing effect for text: {originalTexts[currentTextIndex]}");
        }
        
        // 为每个图片添加点击事件
        for (int i = 0; i < images.Count; i++)
        {
            // 先清除可能存在的事件触发器
            EventTrigger existingTrigger = images[i].GetComponent<EventTrigger>();
            if (existingTrigger != null)
            {
                existingTrigger.triggers.Clear();
            }
            
            int index = i;
            AddEventTrigger(images[i].gameObject, () => OnImageClick(index));
        }
    }

    // 为 UI 元素添加 EventTrigger 点击事件
    void AddEventTrigger(GameObject ui, System.Action onClick)
    {
        EventTrigger trigger = ui.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = ui.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((data) => onClick());
        trigger.triggers.Add(entry);
    }

    // 处理 Image 点击事件
    void OnImageClick(int clickedIndex)
    {
        Debug.Log($"Image clicked: {clickedIndex}");
        
        // 如果正在打字，先完成当前文本
        if (isTyping)
        {
            // 停止当前打字协程
            if (currentTypingCoroutine != null)
            {
                StopCoroutine(currentTypingCoroutine);
                currentTypingCoroutine = null;
            }
            
            // 直接显示完整文本
            if (currentTextIndex < texts.Count && currentTextIndex < originalTexts.Count)
            {
                texts[currentTextIndex].text = originalTexts[currentTextIndex];
                Debug.Log($"Completed typing for text: {texts[currentTextIndex].text}");
            }
            
            // 设置打字状态为完成
            isTyping = false;
            return;
        }
        
        // 计算下一个 Image 的索引
        int nextIndex = (clickedIndex + 1) % images.Count;
        if (nextIndex == 0)
        {
            // 翻到最后一页，使用场景切换动画跳转到 MainGame 场景
            Debug.Log("正在加载 MainGame 场景...");
            SceneManager.LoadScene("MainGame");
            Time.timeScale = 1f;
        }
        else
        {
            currentImageIndex = nextIndex;
            
            // 启动图片切换动画
            StartCoroutine(FadeImages(clickedIndex, nextIndex));
            
            // 显示下一个文本并启动打字机效果
            if (texts.Count > 0)
            {
                currentTextIndex = (currentTextIndex + 1) % texts.Count;
                if (currentTextIndex < originalTexts.Count)
                {
                    currentTypingCoroutine = StartCoroutine(TypeText(texts[currentTextIndex], originalTexts[currentTextIndex]));
                    Debug.Log($"Starting typing effect for next text: {originalTexts[currentTextIndex]}");
                }
            }
        }
    }

    // 图片淡入淡出动画协程
    IEnumerator FadeImages(int currentIndex, int nextIndex)
    {
        CanvasGroup currentCanvasGroup = images[currentIndex].GetComponent<CanvasGroup>();
        CanvasGroup nextCanvasGroup = images[nextIndex].GetComponent<CanvasGroup>();
        
        if (currentCanvasGroup == null || nextCanvasGroup == null)
        {
            yield break;
        }
        
        float elapsedTime = 0f;
        
        while (elapsedTime < transitionTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / transitionTime;
            
            // 当前图片淡出
            currentCanvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
            // 下一张图片淡入
            nextCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
            
            yield return null;
        }
        
        // 确保最终状态正确
        currentCanvasGroup.alpha = 0f;
        currentCanvasGroup.blocksRaycasts = false;
        nextCanvasGroup.alpha = 1f;
        nextCanvasGroup.blocksRaycasts = true;
    }

    // 打字机效果协程
    IEnumerator TypeText(Text textComponent, string fullText)
    {
        isTyping = true;
        textComponent.text = "";
        
        // 逐字显示
        for (int i = 0; i < fullText.Length; i++)
        {
            textComponent.text += fullText[i];
            yield return new WaitForSeconds(typingSpeed);
        }
        
        isTyping = false;
        currentTypingCoroutine = null;
    }
}    