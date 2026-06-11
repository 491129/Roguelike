using System.Collections.Generic;
using UnityEngine;

public class TotemManager : MonoBehaviour
{
    public static TotemManager Instance { get; private set; }

    [Header("图腾点位（按顺序，5个）")]
    [SerializeField] private Transform[] totemPoints;

    // 当前已激活的图腾数量（也对应下一个空闲点位的索引）
    private int activeCount = 0;            // 当前已放置图腾数量
    private int maxSlots = 4;               // 初始最多放4个
    public bool isPlace;
    // 全局加成属性（其他脚本读取）
    public float CatchRateBonus { get; private set; } = 1f;   // 捕鱼概率乘数，初始1
    public float AttackSpeedBonus { get; private set; } = 1f;  // 攻速乘数
    public float RangeBonus { get; private set; } = 1f;        // 范围乘数
    public int CostReduction { get; private set; } = 0;        // 消耗减免

    void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// 放置图腾（购买时调用），成功返回 true
    /// </summary>
    public bool PlaceTotem(SkillShopManager.ShopItemData item)
    {
        if (activeCount >= maxSlots)
        {
            Debug.Log("图腾点位已满，无法放置");
            return false;
        }

        // 在第一个空闲点位生成特效
        Transform point = totemPoints[activeCount];
        if (item.totemEffectPrefab != null)
        {
            Instantiate(item.totemEffectPrefab, point.position, Quaternion.identity, point);
        }

        // 根据图腾名称或 skillID 应用加成
        ApplyTotemEffect(item.itemName);

        activeCount++;
        return true;
    }
    public bool ExpandSlots()
    {
        if (maxSlots >= totemPoints.Length)
        {
            Debug.Log("图腾槽位已达最大");
            return false;
        }
        maxSlots++;
        // 如果需要激活第5个点位的视觉效果，可以在这里处理，比如显示底座等
        return true;
    }
    public bool CanExpandSlots => maxSlots < totemPoints.Length;

    void ApplyTotemEffect(string itemName)
    {
        switch (itemName)
        {
            case "强化炮管":
                CatchRateBonus += 0.1f;          // +10%
                Debug.Log("qianghua");
                break;
            case "快速填装":
                AttackSpeedBonus *= 1.2f;        // 攻速变为1.2倍
                Debug.Log("kauisu");
                break;
            case "广域渔网":
                RangeBonus *= 1.3f;              // 范围1.3倍
                Debug.Log("guangyu");
                break;
            case "省钱达人":
                CostReduction += 2;              // 每次发射少花2金币
                Debug.Log("shengqian");
                break;
        }
    }
}