using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceableBase : MonoBehaviour
{
    // 建筑信息
    [Header("基础信息")]
    public string smyName = "建筑";
    public string smyType = "建筑";
    public string smyDescription = "基础建筑";
    public int cost_wood = 0;
    public int cost_stone = 0;
    public int cost_iron = 0;
    public int cost_copper = 0;
    public bool isOnlyBePlacedOnGround = false; //是否只能放置在特定资源点上面
    public bool isOnlyBePlacedAdjacent = false; //是否只能放置在特定资源点旁边
}
