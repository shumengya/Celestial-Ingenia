using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerConfig : MonoBehaviour
{
    // 单例实例
    public static PlayerConfig Instance;

//-----------游戏目前基本设置----------------------
    public string playerName = "树萌芽";
    public int woodNum = 0;
    public int stoneNum = 0;
    public int ironNum = 0;
    public int copperNum = 0;
//-----------游戏目前基本设置----------------------

    public Text woodText;
    public Text stoneText;
    public Text ironText;
    public Text copperText;

    private float timer = 0f;
    private const float updateInterval = 0.5f;

    void Awake()
    {
        // 检查单例实例是否已经存在
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 在场景加载完成后重新获取文本组件引用
        FindTextComponents();
    }

    void FindTextComponents()
    {
        // 假设这些 Text 组件在场景中可以通过名称找到
        woodText = GameObject.Find("Wood_Text")?.GetComponent<Text>();
        stoneText = GameObject.Find("Stone_Text")?.GetComponent<Text>();
        ironText = GameObject.Find("Iron_Ore_Text")?.GetComponent<Text>();
        copperText = GameObject.Find("Copper_Ore_Text")?.GetComponent<Text>();
    }

    void Start()
    {
        PlayerData loadedData = PlayerDataManager.LoadPlayerData();
        playerName = loadedData.playerName;
        woodNum = loadedData.woodNum;
        stoneNum = loadedData.stoneNum;
        ironNum = loadedData.ironNum;
        copperNum = loadedData.copperNum;
        FindTextComponents();
    }

    void Update()
    {
        // 累加计时器
        timer += Time.deltaTime;

        // 当计时器达到更新间隔时
        if (timer >= updateInterval)
        {
            // 更新文本显示
            if (woodText != null) woodText.text = "木材: " + woodNum.ToString();
            if (stoneText != null) stoneText.text = "石头: " + stoneNum.ToString();
            if (ironText != null) ironText.text = "铁矿: " + ironNum.ToString();
            if (copperText != null) copperText.text = "铜矿: " + copperNum.ToString();

            // 重置计时器
            timer = 0f;
        }
    }
}