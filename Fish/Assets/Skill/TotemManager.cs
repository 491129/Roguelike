using System;
using System.Collections.Generic;
using UnityEngine;
using static TotemClickHandler;


public class TotemManager : MonoBehaviour
{
    public static TotemManager Instance { get; private set; }

    [Header("图腾点位（按顺序，5个）")]
    [SerializeField] private Transform[] totemPoints;
    [SerializeField] private TotemInfoPanel infoPanel;   // 拖入信息面板

    // 当前已激活的图腾数量（也对应下一个空闲点位的索引）
    private int activeCount = 0;            // 当前已放置图腾数量
    private int maxSlots = 4;               // 初始最多放4个
    // 存储每个已放置图腾的信息
    private List<TotemSlot> totemSlots = new List<TotemSlot>();
    public bool isPlace;
    // 全局加成属性（其他脚本读取）
    public float CostMultiplier { get; private set; } = 1f;     //省钱达人
    public bool get100 = false;

    // 记录“领低保了”图腾所在点位索引（-1表示未放置）
    private int lingDiBaoIndex = -1;
    private bool lingDiBaoTriggered = false;   // 防止重复触发

    public bool xijin = false;              //吸金海盗

    public bool canDebt = false;          // 负债
    public int debtLimit = 10000;

    public bool hasMerchantPirate = false;    // 商人海盗
    public bool hasPickyPirate = false;       // 挑剔海盗
    public bool hasTotemPirate = false;      //共享图腾
    public bool qianghua = false;             //强化管炮
    public bool kuaisu = false;               //快速装填
    public bool shengqian = false;            //省钱达人
    public bool chaopin = false;              //超频核心
    public bool huangjin=false;               //黄金渔网
    private int TotemNum = 0;

    public bool chuanzhang = false;             //我爸是船长
    public bool heixin = false;                 //黑心商人
    public bool hasJingbing = false;            //精兵
    public bool xuli = false;

    // 存储每个点位生成的特效实例，用于后续销毁
    private List<GameObject> activeEffects = new List<GameObject>();
    class TotemSlot
    {
        public Transform point;
        public SkillShopManager.ShopItemData itemData;
        public GameObject grayObj;   // 常驻灰色底图
        public int triggerCount;
        public TotemClickHandler clickHandler; // 用于右键交互
    }
    [System.Serializable]
    public class TotemInfo
    {
        public string itemName;
        public string description;
        public int triggerCount;
        public int index;
    }
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
                TotemManager.Instance?.TriggerEffectByName("领低保了");
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
        if (activeCount >= maxSlots)
        {
            Debug.Log("图腾点位已满，无法放置");
            return false;
        }
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
        
        if (hasTotemPirate)
        {
            TotemNum++;
            FishAttrbute.CatchRateMultiplier += 0.01f * TotemNum;//11111111111111111111111111111111111111111
        }
        Transform point = totemPoints[activeCount];
        GameObject gray = null;
        if (item.totemGrayPrefab != null)
        {
            gray = Instantiate(item.totemGrayPrefab, point.position, Quaternion.identity, point);
            TotemClickHandler handler = gray.GetComponent<TotemClickHandler>();
            if (handler == null) handler = gray.AddComponent<TotemClickHandler>();
            handler.Setup(activeCount, infoPanel);
        }
        TotemSlot slot = new TotemSlot
        {
            point = point,
            itemData = item,
            grayObj = gray,
             triggerCount = 0,
            clickHandler = gray?.GetComponent<TotemClickHandler>()
        };
        totemSlots.Add(slot);
        // 应用加成效果
        ApplyTotemEffect(item.itemName, activeCount);
        activeCount++;
        TriggerEffect(activeCount - 1);
        // 记录已购，防止再次出现（如果配置了skillID）
        if (!string.IsNullOrEmpty(item.skillID))
            SkillShopManager.Instance?.AddPurchasedSkill(item.skillID);

        return true;
    }
   
    public void TriggerEffect(int index)
    {
        if (index < 0 || index >= totemSlots.Count) return;
        TotemSlot slot = totemSlots[index];
        if (slot.itemData.totemEffectPrefab != null)
        {
            GameObject effect = Instantiate(slot.itemData.totemEffectPrefab, slot.point.position, Quaternion.identity);
            Destroy(effect, 2f);
            // 获取粒子系统总持续时间（若存在）
            ParticleSystem ps = effect.GetComponent<ParticleSystem>();
            float duration = (ps != null) ? ps.main.duration + ps.main.startLifetime.constantMax : 2f;
            Destroy(effect, duration);
        }
        slot.triggerCount++;
    }
    public void TriggerEffectByName(string itemName)
    {
        for (int i = 0; i < totemSlots.Count; i++)
        {
            if (totemSlots[i].itemData.itemName == itemName)
            {
                TriggerEffect(i);
                break;
            }
        }
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

    // 获取图腾信息供UI使用
    public TotemInfo GetTotemInfo(int index)
    {
        if (index < 0 || index >= totemSlots.Count) return null;
        TotemSlot slot = totemSlots[index];
        return new TotemInfo
        {
            itemName = slot.itemData.itemName,
            description = slot.itemData.description,
            triggerCount = slot.triggerCount,
            index = index
        };
    }

    // 删除图腾
    public bool RemoveTotem(int index)
    {
        if (index < 0 || index >= totemSlots.Count) return false;

        TotemSlot slot = totemSlots[index];
        if (slot.grayObj != null) Destroy(slot.grayObj);

        // 从已购技能中移除（允许重新购买）
        if (!string.IsNullOrEmpty(slot.itemData.skillID))
            SkillShopManager.Instance?.RemovePurchasedSkill(slot.itemData.skillID);

        totemSlots.RemoveAt(index);
        activeCount--;

        // 重新排布点位
        for (int i = 0; i < totemSlots.Count; i++)
        {
            totemSlots[i].point = totemPoints[i];
            if (totemSlots[i].grayObj != null)
                totemSlots[i].grayObj.transform.position = totemPoints[i].position;
            if (totemSlots[i].clickHandler != null)
                totemSlots[i].clickHandler.Setup(i, infoPanel);
        }

        RecalculateAllBonuses();
        return true;
    }

    void RecalculateAllBonuses()
    {
        // 重置所有加成
       
        // 重置其他bool...

        // 重新应用所有剩余图腾
        foreach (var slot in totemSlots)
            ApplyTotemEffect(slot.itemData.itemName);
    }

    public bool CanExpandSlots => maxSlots < totemPoints.Length;

    void ApplyTotemEffect(string itemName)
    {
        switch (itemName)
        {
            case "强化炮管"://2222222222222222222
                FishAttrbute.CatchRateMultiplier += 0.2f;//11111111111111111111111
                qianghua = true;
                Debug.Log(FishAttrbute.CatchRateMultiplier);
                break;
            case "快速填装"://222222222222222222
                Cannon.Instance.ApplyDebuffCAnnon();
                kuaisu = true;
                Debug.Log("kauisu");
                break;
            case "广域渔网"://2222222222222222222
                FishnetManager.Instance.UpgradeRange();
                Debug.Log("guangyu");
                break;
            case "省钱达人"://222222222222222222222
                shengqian = true;
                CostMultiplier *= 0.8f;   // 花费变为20%
                Debug.Log("shengqian");
                break;
            case "超频核心"://2222222222222
                chaopin = true;
                SkillButton.changeDuration *= 0.8f;
                Debug.Log("冷却时间："+SkillButton.changeDuration);
                break;
            case "黄金渔网"://22222222222
                huangjin=true;
                FishAttrbute.getgoldMore *= 1.25f;
                break;
            case "熟能生巧"://22222222222
                get100 = true;
                break;
            case "吸金海盗"://222222222222222222
                xijin = true;
                break;
            case "先欠着"://22222222222222222
                canDebt = true;
                Debug.Log("已启用负债功能，最大负债10000");
                break;
            case "商人海盗"://2222222222222
                hasMerchantPirate = true;
                break;
            case "挑剔海盗"://22222222222
                hasPickyPirate = true;
                break;
            case "共享图腾"://2222222222
                hasTotemPirate= true;
                FishAttrbute.CatchRateMultiplier += 0.01f * activeCount;//111111111111111111111111111111111
                break;
            case "我爸是船长"://2222222222
                chuanzhang = true;
                break;
            case "黑心商人"://22222222222222
                heixin = true;
                break;
            case "精兵"://22222222222
                hasJingbing= true;
                FishAttrbute.CatchRateMultiplier *= 1.5f;//11111111111111111111111111111111111111111
                FishAttrbute.getgoldMore *= 2f;
                break;
            case "蓄力"://222222222222
                xuli=true;
                break;
        }
    }
    void ApplyTotemEffect(string itemName, int index)
    {
        ApplyTotemEffect(itemName);
        switch (itemName)
        {
            case "领低保了":    // 一次性图腾
                lingDiBaoIndex = index;   // 记录点位索引
                lingDiBaoTriggered = false;
                Debug.Log("领低保了图腾已放置，等待触发");
                break;
        }
    }
}