using System;
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
    public float RangeBonus { get; private set; } = 1f;        // 范围乘数
    public float CostMultiplier { get; private set; } = 1f;     //省钱达人

    public bool get100 = false;

    // 存储每个点位生成的特效实例，用于后续销毁
    private List<GameObject> activeEffects = new List<GameObject>();
    // 记录“领低保了”图腾所在点位索引（-1表示未放置）
    private int lingDiBaoIndex = -1;
    private bool lingDiBaoTriggered = false;   // 防止重复触发

    public bool xijin = false;              //吸金海盗

    public bool canDebt = false;          // 负债
    public int debtLimit = 10000;

    public bool hasMerchantPirate = false;   // 商人海盗
    public bool hasPickyPirate = false;      // 挑剔海盗
    private bool hasTotemPirate = false;      //共享图腾
    private int TotemNum = 0;

    public bool chuanzhang = false;
    public bool heixin = false;
    public bool hasJingbing = false;
    public bool xuli = false;
    void Awake()
    {
        Instance = this;
    }
    private void Update()
    {
        if (!lingDiBaoTriggered && lingDiBaoIndex >= 0)
        {
            if (GameManager.Coin < 100)
            {
                // 触发：给予金币，销毁特效，效果作废
                GameManager.AddCoin(10000);
                Debug.Log("领低保了触发：金币+10000");

                // 销毁对应特效
                if (lingDiBaoIndex < activeEffects.Count && activeEffects[lingDiBaoIndex] != null)
                {
                    Destroy(activeEffects[lingDiBaoIndex]);
                    activeEffects[lingDiBaoIndex] = null;
                }

                // 标记已触发，后续不再检测
                lingDiBaoTriggered = true;
                // 注意：activeCount 不减少，点位仍被占用（特效已消失）
            }
        }
    }
    /// <summary>
    /// 放置图腾（购买时调用），成功返回 true
    /// </summary>

    public bool PlaceTotem(SkillShopManager.ShopItemData item)
    {
            if (hasJingbing)
            {
                if (maxSlots <= 0)
                {
                Debug.Log("没有可牺牲的图腾坑位");
                return false;
                }

                hasJingbing = true;
                maxSlots--;                    // 牺牲一个坑位
                ApplyTotemEffect(item.itemName, -1);   // 不占用点位
                Debug.Log($"精兵生效：图腾坑位减少1，当前最大容量 {maxSlots}");
                return true;
            }
        

        if (activeCount >= maxSlots)
        {
            Debug.Log("图腾点位已满，无法放置");
            return false;
        }
        if (hasTotemPirate)
        {
            TotemNum++;
            FishAttrbute.escapeChance -= 0.01f * TotemNum;
        }
        Transform point = totemPoints[activeCount];
        GameObject effect = null;
        if (item.totemEffectPrefab != null)
        {
            effect = Instantiate(item.totemEffectPrefab, point.position, Quaternion.identity, point);
        }
        activeEffects.Add(effect);   // 保存实例（可能为null）

        // 应用加成效果
        ApplyTotemEffect(item.itemName, activeCount);  // 传入当前索引，便于“领低保了”记录

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

    void ApplyTotemEffect(string itemName, int index)
    {
        switch (itemName)
        {
            case "强化炮管":
                FishAttrbute.escapeChance -= 0.2f;
                Debug.Log("FishAttrbute.escapeChance");
                break;
            case "快速填装":
                Cannon.Instance.ApplyDebuffCAnnon(); 
                Debug.Log("kauisu");
                break;
            case "广域渔网":
                RangeBonus *= 1.3f;              // 范围1.3倍
                Debug.Log("guangyu");
                break;
            case "省钱达人":
                CostMultiplier *= 0.8f;   // 花费变为20%
                Debug.Log("shengqian");
                break;
            case "超频核心":
                SkillButton.changeDuration *= 0.8f;
                Debug.Log("冷却时间："+SkillButton.changeDuration);
                break;
            case "黄金渔网":
                FishAttrbute.getgoldMore = 1.2f;
                break;
            case "熟能生巧":
                get100 = true;
                break;
            case "领低保了":    // 一次性图腾
                lingDiBaoIndex = index;   // 记录点位索引
                lingDiBaoTriggered = false;
                Debug.Log("领低保了图腾已放置，等待触发");
                break;
            case "吸金海盗":
                xijin = true;
                break;
            case "先欠着":
                canDebt = true;
                Debug.Log("已启用负债功能，最大负债10000");
                break;
            case "商人海盗":
                hasMerchantPirate = true;
                break;
            case "挑剔海盗":
                hasPickyPirate = true;
                break;
            case "共享图腾":
                hasTotemPirate= true;
                FishAttrbute.escapeChance -= 0.01f * activeCount;
                break;
            case "我爸是船长":
                chuanzhang = true;
                break;
            case "黑心商人":
                heixin = true;
                break;
            case "精兵":
                hasJingbing= true;
                FishAttrbute.escapeChance *= 1.5f;
                FishAttrbute.getgoldMore *= 2f;
                break;
            case "蓄力":
                xuli=true;
                break;
        }
    }
}