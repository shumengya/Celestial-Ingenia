using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

// 该类用于控制昼夜循环
public class DayNightCycle : MonoBehaviour
{
    // 序列化字段，用于指定平行光
    [SerializeField] private Light2D directionalLight;
    // 序列化字段，白天持续时间（秒），这里默认是5分钟
    [SerializeField] private float dayDuration = 300f; 
    // 序列化字段，夜晚持续时间（秒），这里默认是5分钟
    [SerializeField] private float nightDuration = 300f; 

    // 序列化字段，白天的最大光照强度
    [SerializeField] private float maxIntensity = 2f; 
    // 序列化字段，夜晚的最小光照强度
    [SerializeField] private float minIntensity = 0f; 
    
    // 序列化字段，白天的光照颜色
    [SerializeField] private Color dayColor = Color.white;
    // 序列化字段，夜晚的光照颜色，这里使用深蓝色
    [SerializeField] private Color nightColor = new Color(0.1f, 0.1f, 0.3f); 
    
    // 当前时间，范围从0到一个完整的昼夜循环周期
    private float timeOfDay = 0f;
    // 一个完整的昼夜循环周期的持续时间
    private float cycleDuration;
    
    // 在第一帧更新之前调用此方法
    void Start()
    {
        // 计算一个完整的昼夜循环周期的持续时间
        cycleDuration = dayDuration + nightDuration;
        
        // 如果没有指定平行光，则尝试在场景中查找全局2D光
        if (directionalLight == null)
        {
            // 尝试在场景中查找所有的2D光
            Light2D[] lights = FindObjectsOfType<Light2D>();
            foreach (Light2D light in lights)
            {
                // 如果找到全局光，则将其赋值给directionalLight
                if (light.lightType == Light2D.LightType.Global)
                {
                    directionalLight = light;
                    break;
                }
            }
            
            // 如果仍然没有找到全局光，则记录错误信息
            if (directionalLight == null)
            {
                Debug.LogError("场景中未找到全局2D光，请在检查器中指定一个。");
            }
        }
    }

    // 每帧调用此方法
    void Update()
    {
        // 如果没有指定平行光，则直接返回
        if (directionalLight == null) return;
        
        // 增加当前时间
        timeOfDay += Time.deltaTime;
        // 如果当前时间超过了一个完整的昼夜循环周期，则重置为0
        if (timeOfDay > cycleDuration)
        {
            timeOfDay = 0f;
        }
        
        // 更新光照设置
        UpdateLightSettings();
    }
    
    // 更新光照设置的方法
    void UpdateLightSettings()
    {
        // 光照强度
        float intensity;
        // 光照颜色
        Color color;
        
        // 计算昼夜过渡
        if (timeOfDay < dayDuration)
        {
            // 白天
            // 计算白天的进度，范围从0到1
            float dayProgress = timeOfDay / dayDuration;
            
            // 白天的前半段（早上）：增加光照强度
            if (dayProgress < 0.5f)
            {
                // 根据白天进度线性插值计算光照强度
                intensity = Mathf.Lerp(minIntensity, maxIntensity, dayProgress * 2f);
                // 根据白天进度线性插值计算光照颜色
                color = Color.Lerp(nightColor, dayColor, dayProgress * 2f);
            }
            // 白天的后半段（下午）：保持最大光照强度
            else
            {
                intensity = maxIntensity;
                color = dayColor;
            }
        }
        else
        {
            // 夜晚
            // 计算夜晚的进度，范围从0到1
            float nightProgress = (timeOfDay - dayDuration) / nightDuration;
            
            // 夜晚的前半段（傍晚）：降低光照强度
            if (nightProgress < 0.5f)
            {
                // 根据夜晚进度线性插值计算光照强度
                intensity = Mathf.Lerp(maxIntensity, minIntensity, nightProgress * 2f);
                // 根据夜晚进度线性插值计算光照颜色
                color = Color.Lerp(dayColor, nightColor, nightProgress * 2f);
            }
            // 夜晚的后半段（深夜）：保持最小光照强度
            else
            {
                intensity = minIntensity;
                color = nightColor;
            }
        }
        
        // 将计算得到的光照强度和颜色应用到平行光上
        directionalLight.intensity = intensity;
        directionalLight.color = color;
    }
}