using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SkillShopManager : MonoBehaviour
{
    public static SkillShopManager Instance;
    //补给券
    //static public bool TTskill = false;
    //public GameObject SureTT;//确认界面
    //public Sprite[] sskillSprites;
    //public Image ShopImages;
    //public int sSkillNum;//总技能数
    //private int sSkill;//技能
    //public Text siname;
    //public Text sitext;
    //public Text sctext;
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

    private ShopItemData[] currentSlots = new ShopItemData[5];

    //补给券
    [System.Serializable]
    public class ShopItemData00
    {
        public string itemName;
        public int price;
        public Sprite icon;
        [TextArea] public string description;
        public UnityEvent onPurchase;

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
    private void Start()
    {
        for (int i = 0; i < slotButtons.Length; i++)
        {
            int index = i;
            slotButtons[i].onClick.AddListener(() => OnSlotClicked(index));
        }
        slotButtons00.onClick.AddListener(OnSlotClicked00);
        RefreshAllSlots();
        BJRefreshAllSlots();
    }
    private void Update()
    {
        //tuteng
        //sSkill = UnityEngine.Random.Range(0, sSkillNum);
        //ShopImages.sprite = sskillSprites[sSkill];
        //if (sSkill == 0)
        //{
        //    siname.text = "图腾+1";
        //    sitext.text = "这是一个介绍（TT   ";
        //    sctext.text = Onemore.Skillcoin.ToString();

        //}
        //if (TTskill)
        //{
        //    GetComponent<Onemore>().enabled = true;
        //}
    }
    //public void MakeSureTT()
    //{
    //    SureTT.SetActive(true);
    //}

    //public void SureYesTT()
    //{
    //    GameManager.SpendCoin(Onemore.Skillcoin);
    //    TTskill = true;
    //    SureTT.SetActive(false);
    //}
    public void TTskill00()
    {
        GetComponent<Onemore>().enabled = true;
    }
    public void DJskill()
    {
            Debug.Log($"CanPurchase: {SkillButtonManager.Instance.CanPurchase}, AllActivated: {SkillButtonManager.Instance.AllActivated}, nextActivateIndex: ..., maxSlotCount: ...");
        if (!SkillButtonManager.Instance.CanPurchase)
        {
            Debug.Log("技能已满");
            return;
        }
        CoolSkill.isUsed = true;
        GetComponent<CoolSkill>().enabled = true;
        SkillButtonManager.Instance.ActivateNextSkill(currentSlots[1].icon, () => { Debug.Log("Button"); CoolSkill.FreezeAllFish(); });
    }
    public void YFskill()
    {
        GetComponent<YFSkill>().enabled = true;
        YFSkill.isUsed = true;
    }
    public void RefreshAllSlots()
    {
        // 过滤出所有满足前置条件的商品
        List<ShopItemData> available = new List<ShopItemData>();
        foreach (var item in allItems)
        {
            bool canAppear = string.IsNullOrEmpty(item.requiredSkillID)
                             || purchasedSkills.Contains(item.requiredSkillID);
            if (canAppear)
                available.Add(item);
        }

        if (available.Count < 5)
        {
            Debug.LogError($"可用商品不足5个，当前只有 {available.Count} 个");
            return;
        }

        // 从可用商品中随机抽取5个不重复
        List<ShopItemData> pool = new List<ShopItemData>(available);
        for (int i = 0; i < 5; i++)
        {
            int rand = UnityEngine.Random.Range(0, pool.Count);
            currentSlots[i] = pool[rand];
            pool.RemoveAt(rand);
        }
     
        //商店界面加载
        UpdateSlotUI();
    }
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
        for (int i = 0; i < 5; i++)
        {
            if (currentSlots[i] != null)
            {
                slotImages[i].sprite = currentSlots[i].icon;
                if (slotPriceTexts != null && slotPriceTexts.Length > i)
                    slotPriceTexts[i].text = currentSlots[i].price.ToString();
                slotButtons[i].interactable = true;
            }
            else
            {
                slotImages[i].sprite = null;
                slotButtons[i].interactable = false;
            }
            
        }
        UpdateSlotUI00();
    }
    void UpdateSlotUI00()
    {
        if (currentSlots00 != null)
        {
            slotImages00.sprite = currentSlots00.icon;
            slotPriceTexts00.text = currentSlots00.price.ToString();
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
        confirmPanel.Show(currentSlots[index]);

    }
    void OnSlotClicked00()
    {
        if (currentSlots00 != null)
            confirmPanel00.Show(currentSlots00);
        confirmPanel00.gameObject.SetActive(true);
    }
    /// <summary> 实际购买，由确认弹窗调用 </summary>
    public void TryPurchase(ShopItemData item)
    {
        if (GameManager.Coin < item.price) { Debug.Log("金币不足"); return; }

        GameManager.SpendCoin(item.price);
        item.onPurchase.Invoke();

        // 记录已购买的技能ID（作为后续商品的前置条件）
        if (!string.IsNullOrEmpty(item.skillID))
            purchasedSkills.Add(item.skillID);

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
    }
    public void TryPurchase00(ShopItemData00 item)
    {
        if (GameManager.Coin < item.price) { Debug.Log("金币不足"); return; }
        GameManager.SpendCoin(item.price);
        item.onPurchase.Invoke();
        currentSlots00 = null;

        UpdateSlotUI00();
    }
    public void CoolChange00()
    {
        SkillButton.duration = 4f;
    }
    public void CoolChange01()
    {
        CoolSkill.duration = 6f;
    }
    public void OnRefreshButtonClicked()
    {
        RefreshAllSlots();
    }
}