using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class SkillShopManager : MonoBehaviour
{
    public static SkillShopManager Instance;
    public bool hasExtraSlot = false;
    public static float priceMultiplier = 1.0f;
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
    public static int refreshCost = 0;
    public Text refreshCostText;
    public static float refreshCostMultiplier = 1.0f;
    //权重
    public static bool doubleType23Weight = false;   // 装置大亨
    public static bool doubleMarketWeight = false;   // 市场商人
    public static bool doubleTotemWeight = false;    // 大法师
    //图腾功能
    private int marketTicketUsed = 0;//市场券+概率
    private int shopRefreshCount = 0;//刷新+概率
    //装置（
    public int finalPriceZZ;

    //
    public HashSet<string> purchasedItemNames = new HashSet<string>();
    private void Start()
    {
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
        refreshCostText.text=refreshCost.ToString();
        
    }
    private void Update()
    {
        if (TotemManager.Instance.xijin)
        {
            float xijin = (float)GameManager.Coin / 1000f;
           // FishAttrbute.escapeChance -= 0.01f * xijin;//111111111111111111111111
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

    //public void BommSkill()
    //{
    //    Debug.Log($"CanPurchase: {SkillButtonManager.Instance.CanPurchase}, AllActivated: {SkillButtonManager.Instance.AllActivated}, nextActivateIndex: ..., maxSlotCount: ...");
    //    if (!SkillButtonManager.Instance.CanPurchase)
    //    {
    //        Debug.Log("技能已满");
    //        return;
    //    }
    //    //DepthChargeSkill.isUsed = true;
    //    GetComponent<DepthChargeSkill>().enabled = true;
    //    SkillButtonManager.Instance.ActivateNextSkill(currentSlots[0].icon, () => { Debug.Log("Button"); DepthChargeSkill.Instance.Use(); });
    //}
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
        refreshCostText.text = (refreshCost*refreshCostMultiplier).ToString();
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
        Boss.skillDuration += 10;
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
        List<ShopItemData> weightedPool = new List<ShopItemData>();
        foreach (var item in available)
        {
            int weight = 1;
            // 装置大亨：类型2或3概率×2
            if (doubleType23Weight && item.isZZ)
                weight *= 2;
            // 市场商人：市场券概率×2
            if (doubleMarketWeight && item.isMarketTicket)
                weight *= 2;
            // 大法师：图腾概率×2
            if (doubleTotemWeight && item.isTotem)
                weight *= 2;
            Debug.Log($"商品：{item.itemName}，基础权重：{weight}");
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
                        slotPriceTexts[i].text = (currentSlots[i].price * priceMultiplier).ToString("F0");
                    slotButtons[i].interactable = true;
                    slotImages[i].gameObject.SetActive(true); // 确保显示
                 }
            }
                 else
                 {
                    slotImages[i].sprite = null;
                    slotButtons[i].interactable = false;
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

            // 扣钱并执行绑定事件
            if (!GameManager.SpendCoin(item.price)) { Debug.Log("金币不足"); return; }
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

            finalPriceZZ = Mathf.RoundToInt(item.price * priceMultiplier);
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
            }

            if (onUse != null)
            {
                SkillButtonManager.Instance.ActivateNextSkill(item.icon, onUse);

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
        int finalPrice = Mathf.RoundToInt(item.price * priceMultiplier);
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
    //刷新按钮
    public void OnRefreshButtonClicked()
    {
        if (TotemManager.Instance.hasPickyPirate)
        {
            shopRefreshCount++;
           //FishAttrbute.escapeChance -= 0.07f * shopRefreshCount;//111111111111111111111111111
        }
        int refreshPrice = Mathf.RoundToInt(refreshCost * refreshCostMultiplier);
        if (!GameManager.SpendCoin(refreshPrice))
            return;
        GameManager.SpendCoin(refreshPrice);
        RefreshAllSlots();

        refreshCost += 10;
        refreshCostText.text = (refreshCost * refreshCostMultiplier).ToString();
        //refreshCostText.text = refreshPrice.ToString();
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
}