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
    }


    void Start()
    {
        PlayerData loadedData = PlayerDataManager.LoadPlayerData();
        playerName = loadedData.playerName;
        woodNum = loadedData.woodNum;
        stoneNum = loadedData.stoneNum;
        ironNum = loadedData.ironNum;
        copperNum = loadedData.copperNum;
    }


}