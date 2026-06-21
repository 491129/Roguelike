using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
//using static UnityEditor.Progress;

public class SkillShopManager : MonoBehaviour
{
    public static SkillShopManager Instance;
    public bool hasExtraSlot = false;
    public static float priceMultiplier = 1.0f;
    private int shopRefreshCount00 = 0;
    private int shopRefreshCount01 = 0; 
    //其他商品
    [System.Serializable]
    public class ShopItemData
    {
        public string itemName;
        public int price;
        public Sprite icon;
        [TextArea] public string description;
        public UnityEvent onPurchase;

        // 新增：前置技能ID（若为空则无前置条件）
        public string skillID;            // 本商品对应的技能ID（例如 "Skill1", "Skill2"）
        public string requiredSkillID;      // 例如填 "Skill1"

        public bool isZZ;                    // 装置
        public bool isMarketTicket;          // 是否是市场券
        public bool isTotem;                 // 是否是图腾商品
        public bool isFore;

        public GameObject totemGrayPrefab;     // 灰色底图预制体（世界空间，常驻显示）
        public GameObject totemEffectPrefab;   // 图腾对应的粒子特效预制体

        [Header("刷新权重 (总比例10000)")]
        public int refreshWeight = 100;
    }
    private HashSet<string> purchasedSkills = new HashSet<string>();


    [Header("所有商品（在Inspector中配置）")]
    [SerializeField] private List<ShopItemData> allItems;

    [Header("五个商品槽位")]
    [SerializeField] private Image[] slotImages;
    [SerializeField] private Button[] slotButtons;
    [SerializeField] private Text[] slotPriceTexts;

    [Header("确认弹窗")]
    [SerializeField] private ConfirmPanel confirmPanel;

    private ShopItemData[] currentSlots = new ShopItemData[6];

    //补给券
    [System.Serializable]
    public class ShopItemData00
    {
        public string itemName;
        public int price;
        public Sprite icon;
        [TextArea] public string description;
        public UnityEvent onPurchase;

        public string skillID;
    }

    [Header("补给券商品（在Inspector中配置）")]
    [SerializeField] private List<ShopItemData00> allItems00;

    [Header("补给券商品槽位")]
    [SerializeField] private Image slotImages00;
    [SerializeField] private Button slotButtons00;
    [SerializeField] private Text slotPriceTexts00;

    [Header("确认弹窗")]
    [SerializeField] private ConfirmPanel confirmPanel00;

    private ShopItemData00 currentSlots00  ;
    //-----------
    private void Awake() => Instance = this;

    public static bool isUsed = false;
    //刷新所需金币
    //public static int refreshCost = 0;
    public Text refreshCostText;
    public static float refreshCostMultiplier = 1.0f;
    //权重
    public static bool doubleType23Weight = false;   // 装置大亨
    public static bool doubleMarketWeight = false;   // 市场商人
    public static bool doubleTotemWeight = false;    // 大法师
    //图腾功能
    private int marketTicketUsed = 0;//市场券+概率
    //private int shopRefreshCount = 0;//刷新+概率
    //装置（
    public int finalPriceZZ;
    public int finalPrice;
    public int finalPriceNol;
    //
    public HashSet<string> purchasedItemNames = new HashSet<string>();

    [Header("删除技能面板")]
    [SerializeField] private DeleteSkillPanel deleteSkillPanel;   // 拖入面板

    private void Start()
    {
        shopRefreshCount00 = 0;
        if (slotImages.Length >= 6) slotImages[5].gameObject.SetActive(false);
        if (slotButtons.Length >= 6) slotButtons[5].gameObject.SetActive(false);
        for (int i = 0; i < slotButtons.Length; i++)
        {
            int index = i;
            slotButtons[i].onClick.AddListener(() => OnSlotClicked(index));
        }
        slotButtons00.onClick.AddListener(OnSlotClicked00);
        RefreshAllSlots();
        BJRefreshAllSlots();
        UpdateRefreshCostText();
        SkillButtonManager.Instance.OnSkillRemoved += HandleSkillRemoved;

    }
    private void Update()
    {
        if (TotemManager.Instance.xijin)
        {
            float xijin = (float)GameManager.Coin / 1000f;
           // FishAttrbute.escapeChance -= 0.01f * xijin;//111111111111111111111111
        }
        
    }
    void HandleSkillRemoved(SkillButton btn)
    {
        string mainSkillID = btn.skillID;
        if (string.IsNullOrEmpty(mainSkillID)) return;

        // 1. 从 purchasedSkills 中移除主技能ID
        purchasedSkills.Remove(mainSkillID);

        // 2. 找到所有 requiredSkillID == mainSkillID 的加成商品，移除它们的购买记录
        List<string> toRemove = new List<string>();
        foreach (var item in allItems)
        {
            if (item.requiredSkillID == mainSkillID)
            {
                // 从 purchasedSkills 中移除（如果有）
                purchasedSkills.Remove(item.skillID);
                // 从 purchasedItemNames 中移除（如果记录为唯一购买）
                purchasedItemNames.Remove(item.itemName);
                // 如果该商品有对应的技能物体，尝试重置或禁用
                // （可扩展，此处先忽略）
            }
        }

        // 3. 禁用主技能物体并重置升级效果（可选）
        // 通过 skillID 找到对应的技能物体，并调用 ResetSkill 方法
        // 例如：冰冻技能可调用 CoolSkill.Instance.ResetUpgrades();
        // 这里建议在各个技能脚本中添加 ResetUpgrades() 静态方法，或直接禁用物体。
        // 如果没有，至少将技能物体 SetActive(false)
        ResetMainSkillObject(mainSkillID);
    }

    void ResetMainSkillObject(string skillID)
    {
        // 根据技能ID，禁用对应的技能物体（使技能完全失效）
        switch (skillID)
        {
            case "Skill1":
                if (CoolSkill.Instance != null)
                    CoolSkill.Instance.enabled = false;
                    //CoolSkill.ResetUpgrades();
                break;
            case "Skill2":
                if (DepthChargeSkill.Instance != null)
                    DepthChargeSkill.Instance.enabled = false;
                break;
            case "Skill3":
                if (VortexSkill.Instance != null)
                    VortexSkill.Instance.enabled = false;
                break;
            case "Skill4":
                if (StrengthenSkill.Instance != null)
                    StrengthenSkill.Instance.enabled = false;
                break;
            case "Skill5":
                if (LockSkill.Instance != null)
                    LockSkill.Instance.enabled = false;
                break;
            case "Skill6":
                if (LanserSkill.Instance != null)
                    LanserSkill.Instance.enabled=false;
                break;
                // 如果有其他技能，继续添加 case
        }
    }
    public void CSSkill()
    {
        hasExtraSlot = true;
        if (slotImages.Length >= 6) slotImages[5].gameObject.SetActive(true);
        if (slotButtons.Length >= 6) slotButtons[5].gameObject.SetActive(true);
        RefreshAllSlots();
    }
    public void TTskill00()
    {
        TotemManager.Instance.ExpandSlots();
    }

    #region //fish
    public void YFskill()
    {
        GetComponent<YFSkill>().enabled = true;
        YFSkill.isUsed = true;
    }
    public void CFskill()
    {
        GetComponent<CFSkill>().enabled = true;
        CFSkill.isUsed=true;
    }
    public void GBskill()
    {
        GetComponent<GBSkill>().enabled = true;
        GBSkill.isUsed=true;
    }
    public void GDskill()
    {
        GetComponent<GDSkill>().enabled = true;
        GDSkill.isUsed = true;
    }
    public void GFskill()
    {
        GetComponent<GFSkill>().enabled = true;
        GFSkill.isUsed = true;
    }
    public void GFskill00()
    {
        GetComponent<GF00Skill>().enabled = true;
        GF00Skill.isUsed = true;
    }
    public void MTkill()
    {
        GetComponent<MTSkill>().enabled = true;
        MTSkill.isUsed = true;
    }

    public void HDSkill()
    {
        priceMultiplier = 0.8f;
        int slotCount = hasExtraSlot ? 6 : 5;
        for (int i = 0; i < slotCount; i++)
        {
            if (i >= slotImages.Length) break;
            if (currentSlots[i] != null)
            {
                if (slotPriceTexts != null && slotPriceTexts.Length > i)
                    slotPriceTexts[i].text = (currentSlots[i].price * priceMultiplier).ToString("F0");
            }
        }
    }
    public void ZHSkill()
    {
        refreshCostMultiplier = 0.8f;
        UpdateRefreshCostText();
    }
    public void ZZSkill()
    {
        doubleType23Weight = true;
    }
    public void SCSkill()
    {
        doubleMarketWeight = true;
    }
    public void DFSSkill()
    {
        doubleTotemWeight = true;
    }
    public void MDSkill()
    {
        Boss.passes += 2;
    }
    #endregion  
    public void RefreshAllSlots()
    {
        // 过滤出所有满足前置条件的商品
        List<ShopItemData> available = new List<ShopItemData>();
        foreach (var item in allItems)
        {
            bool canAppear = string.IsNullOrEmpty(item.requiredSkillID)
                             || purchasedSkills.Contains(item.requiredSkillID);
            if (!canAppear) continue;
            if(purchasedItemNames.Contains(item.itemName))continue;
                available.Add(item);
        }


        // 从可用商品中随机抽取5个不重复

        int slotCount = hasExtraSlot ? 6 : 5;
        if (available.Count < slotCount)
        {
            Debug.LogError($"可用商品不足5个，当前只有 {available.Count} 个");
            return;
        }
        // ----- 2. 构建加权候选池（同一商品出现多次）-----
        // 构建加权池
        List<ShopItemData> weightedPool = new List<ShopItemData>();
        foreach (var item in available)
        {
            int weight = item.refreshWeight;   // 使用配置的基础权重

            // 装置大亨：装置类商品概率×2
            if (doubleType23Weight && item.isZZ)
                weight *= 2;
            // 市场商人：市场券概率×2
            if (doubleMarketWeight && item.isMarketTicket)
                weight *= 2;
            // 大法师：图腾概率×2
            if (doubleTotemWeight && item.isTotem)
                weight *= 2;

            for (int i = 0; i < weight; i++)
                weightedPool.Add(item);
        }
        Debug.Log($"加权池总元素数：{weightedPool.Count}，可用商品种类数：{available.Count}");
        List<ShopItemData> tempPool = new List<ShopItemData>(weightedPool);
        for (int i = 0; i < slotCount; i++)
        {
            int rand = UnityEngine.Random.Range(0, tempPool.Count);
            ShopItemData chosen = tempPool[rand];
            Debug.Log($"槽位{i}：{chosen.itemName} (type={chosen.isZZ}, market={chosen.isMarketTicket}, totem={chosen.isTotem})");
            currentSlots[i] = chosen;
            // 从临时池中移除所有与chosen相同的元素（保证不重复）
            tempPool.RemoveAll(item => item == chosen);
        }

        if (!hasExtraSlot && currentSlots.Length > 5)
            currentSlots[5] = null;
        //商店界面加载
        UpdateSlotUI();
    }
    //补给券随机
    public void BJRefreshAllSlots()
    {
        if (allItems00.Count == 0) return;
        int rand = UnityEngine.Random.Range(0, allItems00.Count);
        currentSlots00 = allItems00[rand];
        UpdateSlotUI00();   // 刷新补给券 UI
    }
    //商店界面加载
    void UpdateSlotUI()
    {
        int slotCount = hasExtraSlot ? 6 : 5;

            for (int i = 0; i < slotCount; i++)
        {
               if (i >= slotImages.Length) break;

                 if (currentSlots[i] != null)
                 {
                    if (currentSlots[i].isMarketTicket && TotemManager.Instance.chuanzhang)
                    {
                        slotImages[i].sprite = currentSlots[i].icon;
                        if (slotPriceTexts != null && slotPriceTexts.Length > i)
                        slotPriceTexts[i].text = "0";
                        slotButtons[i].interactable = true;
                        slotImages[i].gameObject.SetActive(true); // 确保显示
                    }
                     else
                     {
                        slotImages[i].sprite = currentSlots[i].icon;
                        if (slotPriceTexts != null && slotPriceTexts.Length > i)
                        {
                            float sqrtMult = Mathf.Sqrt(shopRefreshCount00 + 1);   // √(N+1)
                        int displayPrice = Mathf.RoundToInt(currentSlots[i].price * sqrtMult * priceMultiplier);
                        slotPriceTexts[i].text = displayPrice.ToString();
                        }
                        slotButtons[i].interactable = true;
                        slotImages[i].gameObject.SetActive(true); // 确保显示
                     }
            }
                 else
                 {
                    slotImages[i].sprite = null;
                    slotButtons[i].interactable = false;
                    slotPriceTexts[i].text = " ";
                if (i >= 5 && !hasExtraSlot) slotImages[i].gameObject.SetActive(false);
                 }
                    SlotHover hover = slotImages[i].GetComponent<SlotHover>();
                    if (hover == null) hover = slotButtons[i].GetComponent<SlotHover>();
                    if (hover != null) hover.SetItem(currentSlots[i]);
               }
    }
    //补给券加载
    void UpdateSlotUI00()
    {
        if (currentSlots00 != null)
        {
            slotImages00.sprite = currentSlots00.icon;
            slotPriceTexts00.text = (currentSlots00.price * priceMultiplier).ToString("F0");
            slotButtons00.interactable = true;
        }
        else
        {
            slotImages00.sprite = null;
            slotButtons00.interactable = false;
        }
    }


    //确认弹窗
    void OnSlotClicked(int index)
    {
        if (currentSlots[index] != null)
            confirmPanel.Show(currentSlots[index]);
        confirmPanel.gameObject.SetActive(true);
        //confirmPanel.Show(currentSlots[index]);

    }
    void OnSlotClicked00()
    {
        if (currentSlots00 != null)
            confirmPanel00.Show(currentSlots00);
        confirmPanel00.gameObject.SetActive(true);
    }
    //普通商品购买，确认弹窗调用
    public void TryPurchase(ShopItemData item)
    {
        if (item.isTotem)
        {
            if (!TotemManager.Instance.PlaceTotem(item))
            {
                Debug.Log("图腾购买失败（点位已满）");
                return;
            }
            finalPrice = Mathf.RoundToInt(item.price * Mathf.Sqrt(shopRefreshCount00 + 1) * priceMultiplier);
            // 扣钱并执行绑定事件
            if (!GameManager.SpendCoin(finalPrice)) { Debug.Log("金币不足"); return; }
            GameManager.SpendCoin(item.price);
            item.onPurchase.Invoke();
            purchasedItemNames.Add(item.itemName);

            // 清除该商品在普通槽位的显示（如果图腾也出现在随机槽位中）
            for (int i = 0; i < currentSlots.Length; i++)
            {
                if (currentSlots[i] == item)
                {
                    currentSlots[i] = null;
                    break;
                }
            }
            UpdateSlotUI();
            return;
        }
        if (item.isFore)
        {
            
            if (!SkillButtonManager.Instance.CanPurchase)
            {
                Debug.Log("技能已满");
                return;
            }
            finalPriceZZ = Mathf.RoundToInt(item.price * Mathf.Sqrt(shopRefreshCount00 + 1) * priceMultiplier);
            if (GameManager.Coin < finalPriceZZ) { Debug.Log("金币不足"); return; }

            // 扣钱
            GameManager.SpendCoin(finalPriceZZ);
            item.onPurchase.Invoke();   // 激活技能物体/效果
            purchasedItemNames.Add(item.itemName);

            // 根据技能ID，准备使用回调
            System.Action onUse = null;
        
            switch (item.itemName)
            {
                case "冰冻":
                    onUse = () => CoolSkill.Instance.FreezeAllFish();
                    break;
                case "深水炸弹":
                    onUse = () => DepthChargeSkill.Instance?.Use();
                    break;
                case "电磁漩涡":
                    onUse = () => VortexSkill.Instance?.Use();
                    break;
                case "强化":
                    onUse = () => StrengthenSkill.Instance?.Use();
                    break;
                case "锁定":
                    onUse = () => LockSkill.Instance?.Use();
                    break;
                case "激光":
                    onUse = () => LanserSkill.Instance?.Use();
                    break;
            }

            if (onUse != null)
            {
                SkillButtonManager.Instance.ActivateNextSkill(item.icon, onUse,item.skillID);

                // 绑定按钮给技能（需要绑定冷却控制的技能）
                SkillButton lastBtn = SkillButtonManager.Instance.GetLastActivatedButton();
                if (lastBtn != null)
                {
                    if (item.itemName == "电磁漩涡")
                        VortexSkill.Instance?.BindButton(lastBtn);
                    else if (item.itemName == "深水炸弹")
                        DepthChargeSkill.Instance?.BindButton(lastBtn);
                    else if (item.itemName == "冰冻")
                        CoolSkill.Instance?.BindButton(lastBtn);
                    else if (item.itemName == "强化")   // 新增
                        StrengthenSkill.Instance?.BindButton(lastBtn);
                    else if (item.itemName == "锁定")
                        LockSkill.Instance?.BindButton(lastBtn);
                    else if (item.itemName == "激光")
                        LanserSkill.Instance?.BindButton(lastBtn);
                }

            }
            // 记录已购技能ID
            if (!string.IsNullOrEmpty(item.skillID))
            {
                purchasedSkills.Add(item.skillID);
            }

            // 清除槽位
            for (int i = 0; i < currentSlots.Length; i++)
                if (currentSlots[i] == item) { currentSlots[i] = null; break; }
            UpdateSlotUI();
            return;
        }
        if (item.isMarketTicket&&TotemManager.Instance.hasMerchantPirate)
        {
            marketTicketUsed++;
            FishAttrbute.CatchRateMultiplier -= 0.07f * marketTicketUsed;//11111111111111111111111
            
        }
        if(item.isMarketTicket&&TotemManager.Instance.chuanzhang)
        {
            item.onPurchase.Invoke();

            // 记录已购买的技能ID（作为后续商品的前置条件）
            if (!string.IsNullOrEmpty(item.skillID))
            {
                purchasedSkills.Add(item.skillID);
            }

            // 清空购买槽位
            for (int i = 0; i < currentSlots.Length; i++)
            {
                if (currentSlots[i] == item)
                {
                    currentSlots[i] = null;
                    break;
                }
            }
            UpdateSlotUI();
            return;
        }
        //  if (item.skillID == "Skill1"&& SkillButtonManager.Instance.AllActivated) { Debug.Log("满了"); return; }
        //int finalPrice = Mathf.RoundToInt(item.price * priceMultiplier);
        finalPriceNol = Mathf.RoundToInt(item.price * Mathf.Sqrt(shopRefreshCount00 + 1) * priceMultiplier);
        if (!GameManager.SpendCoin(finalPrice)) { Debug.Log("金币不足"); return; }


        GameManager.SpendCoin(finalPrice);
        item.onPurchase.Invoke();
        purchasedItemNames.Add(item.itemName);

        // 记录已购买的技能ID（作为后续商品的前置条件）
        if (!string.IsNullOrEmpty(item.skillID))
        {
            purchasedSkills.Add(item.skillID);
        }

        // 清空购买槽位
        for (int i = 0; i < currentSlots.Length; i++)
        {
            if (currentSlots[i] == item)
            {
                currentSlots[i] = null;
                break;
            }
        }
        UpdateSlotUI();
        return;
    }
    //补给券购买
    public void TryPurchase00(ShopItemData00 item)
    {
        if (!GameManager.SpendCoin(item.price)) { Debug.Log("金币不足"); return; }
        if (item.skillID == "ExpandSlot")
        {
            if (!SkillButtonManager.Instance.CanExpandSlot)
            {
                Debug.Log("技能槽已满，无法再次扩容");
                return;
            }
            if (!GameManager.SpendCoin(item.price))
            {
                Debug.Log("金币不足");
                return;
            }
            GameManager.SpendCoin(item.price);
            SkillButtonManager.Instance.ExpandSlot();
            return;
        }
            GameManager.SpendCoin(item.price);
            item.onPurchase.Invoke();
            currentSlots00 = null;
        // 清空购买槽位
        for (int i = 0; i < currentSlots.Length; i++)
        {
            if (currentSlots00 == item)
            {
                currentSlots[i] = null;
                break;
            }
        }
        UpdateSlotUI00();
        return;
    }
    //冰冻技能冷却时间减少
    public void CoolChange00()
    {
        CoolSkill.Instance.ReduceCooldown(2f);
    }
    /// <summary>
    //冰冻技能延长时间
    /// </summary>
    public void CoolChange01()
    {
        CoolSkill.duration = 6f;
    }
    public void Bommchange00()
    {
        if (DepthChargeSkill.Instance != null)
            DepthChargeSkill.Instance.ReduceCooldown(2f);  // 每次减少2秒
    }
    public void BoomChange01()
    {
        DepthChargeSkill.Instance?.IncreaseRadius(1.3f);
    }
    public void BoomChange02()
    {
        DepthChargeSkill.Instance?.EnableDotZone();
    }
    public void VortexChange00()
    {
        VortexSkill.Instance?.ReduceCooldown(2f);
    }

    public void VortexChange01()
    {
        VortexSkill.Instance?.IncreaseRadius(1.3f);
    }

    public void VortexChange02()
    {
        VortexSkill.Instance?.IncreaseDuration(2f);
    }
    public void StrengthenChange00()
    {
        StrengthenSkill.Instance?.ReduceCooldown(2f);
    }

    public void StrengthenChange01()
    {
        StrengthenSkill.Instance?.IncreaseDuration(2f);
    }

    public void StrengthenChange02()
    {
        StrengthenSkill.Instance?.IncreaseBoostRate(0.2f);  // 例如 +0.2 倍
    }
    public void LockChange00()
    {
        LockSkill.Instance?.ReduceCooldown(2f);
    }

    public void LockChange01()
    {
        LockSkill.Instance?.IncreaseDuration(2f);
    }
    public void LaserChange00()
    {
        LanserSkill.Instance?.ReduceCooldown(2f);
    }
    public void LaserChange01()
    {
        LanserSkill.Instance?.IncreaseWidth(0.5f);
    }
    public void LaserChange02()
    {
        LanserSkill.Instance?.IncreaseDuration(2f);
    }
    //刷新按钮
    public void OnRefreshButtonClicked()
    {
        // 计算本次刷新费用（基于当前已刷新次数，公式：300×√(N+1)×折扣）
        float sqrtMult = Mathf.Sqrt(shopRefreshCount00 + 1);
        int refreshPrice = Mathf.RoundToInt(300 * sqrtMult * refreshCostMultiplier);

        if (!GameManager.SpendCoin(refreshPrice))
            return;

        // 扣钱成功后，次数+1
        shopRefreshCount00++;

        // 挑剔海盗图腾（如果需要捕鱼概率加成，就在这里直接使用 shopRefreshCount00）
        if (TotemManager.Instance.hasPickyPirate)
        {
            shopRefreshCount01++;
            FishAttrbute.CatchRateMultiplier += 1.1f* shopRefreshCount01;
        }

        RefreshAllSlots();
        UpdateRefreshCostText();   // 更新刷新费用显示
    }
    public void AddPurchasedSkill(string skillID)
    {
        if (!string.IsNullOrEmpty(skillID))
            purchasedSkills.Add(skillID);
    }

    public void RemovePurchasedSkill(string skillID)
    {
        if (!string.IsNullOrEmpty(skillID) && purchasedSkills.Contains(skillID))
            purchasedSkills.Remove(skillID);
    }
    void UpdateRefreshCostText()
    {
        float sqrtMult = Mathf.Sqrt(shopRefreshCount00 + 1);
        int price = Mathf.RoundToInt(300 * sqrtMult * refreshCostMultiplier);
        refreshCostText.text = price.ToString();
    }
}