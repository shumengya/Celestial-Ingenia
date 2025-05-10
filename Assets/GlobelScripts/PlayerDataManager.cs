using UnityEngine;
using System.IO;

/// <summary>
/// 玩家数据类，存储游戏中的玩家基本信息和资源数量
/// </summary>
[System.Serializable]
public class PlayerData
{
    public string playerName = "树萌芽";  // 玩家名称
    public int woodNum = 100;            // 木材数量
    public int stoneNum = 100;           // 石头数量
    public int ironNum = 100;            // 铁数量
    public int copperNum = 100;          // 铜数量
}

/// <summary>
/// 玩家数据管理器，负责玩家数据的存储与加载
/// </summary>
public static class PlayerDataManager
{
    private static string directoryName = "Files";        // 存储目录名
    private static string fileName = "player_config.json";  // 存储文件名

    /// <summary>
    /// 获取存储目录路径
    /// </summary>
    private static string GetDirectoryPath()
    {
#if UNITY_EDITOR
        // 编辑器环境下，存储在项目的Assets目录下的Files文件夹
        return Path.Combine(Application.dataPath, directoryName);
#else
        // 游戏发布环境下，存储在游戏可执行文件同级目录的Files文件夹
        string exePath = Path.GetDirectoryName(Application.dataPath);
        return Path.Combine(exePath, directoryName);
#endif
    }

    /// <summary>
    /// 获取完整文件路径
    /// </summary>
    private static string GetFilePath()
    {
        return Path.Combine(GetDirectoryPath(), fileName);
    }

    /// <summary>
    /// 保存玩家数据到JSON文件
    /// </summary>
    public static void SavePlayerData(PlayerData data)
    {
        // 检查并创建存储目录
        string dirPath = GetDirectoryPath();
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }

        // 将数据序列化为JSON格式并保存到文件
        string filePath = GetFilePath();
        string json = JsonUtility.ToJson(data, true); // 'true' 表示生成格式化的JSON文本
        File.WriteAllText(filePath, json);
        Debug.Log($"玩家数据已保存至: {filePath}");
    }

    /// <summary>
    /// 从JSON文件加载玩家数据
    /// </summary>
    public static PlayerData LoadPlayerData()
    {
        string filePath = GetFilePath();
        
        // 检查文件是否存在
        if (File.Exists(filePath))
        {
            // 读取文件内容并反序列化为PlayerData对象
            string json = File.ReadAllText(filePath);
            PlayerData data = JsonUtility.FromJson<PlayerData>(json);
            Debug.Log($"玩家数据已从 {filePath} 加载");
            return data;
        }
        else
        {
            // 文件不存在时返回默认数据
            Debug.LogWarning($"未在 {filePath} 找到存档文件，返回默认数据");
            return new PlayerData(); 
        }
    }
}