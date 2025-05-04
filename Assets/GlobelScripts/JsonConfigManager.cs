using UnityEngine;
using System.IO;

// 全局单例JSON配置管理类
public class JsonConfigManager : MonoBehaviour
{
    private static JsonConfigManager instance;
    public static JsonConfigManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<JsonConfigManager>();
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject("JsonConfigManager");
                    instance = singletonObject.AddComponent<JsonConfigManager>();
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (this != instance)
            {
                Destroy(gameObject);
            }
        }
    }

    // 读取JSON配置文件
    public T ReadConfig<T>(string filePath)
    {
        string fullPath = Path.Combine(Application.persistentDataPath, filePath);
        if (File.Exists(fullPath))
        {
            try
            {
                string json = File.ReadAllText(fullPath);
                return JsonUtility.FromJson<T>(json);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"读取配置文件时出错: {e.Message}");
            }
        }
        return default(T);
    }

    // 写入JSON配置文件
    public void WriteConfig<T>(string filePath, T configData)
    {
        string fullPath = Path.Combine(Application.persistentDataPath, filePath);
        try
        {
            string json = JsonUtility.ToJson(configData, true);
            File.WriteAllText(fullPath, json);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"写入配置文件时出错: {e.Message}");
        }
    }
}
    