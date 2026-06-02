using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SkillShopManager : MonoBehaviour
{
    public static SkillShopManager Instance;
    //补给券
    static public bool TTskill = false;
    public GameObject SureTT;//确认界面
    public Sprite[] sskillSprites;
    public Image ShopImages;
    public int sSkillNum;//总技能数
    private int sSkill;//技能
    public Text siname;
    public Text sitext;
    public Text sctext;
    //其他商品
    [System.Serializable]
    public class ShopItemData
    {
        public string itemName;         // 名字
        public int price;               // 价格
        public Sprite icon;             // 图片
        [TextArea] public string description;  // 介绍
        public UnityEvent onPurchase;   // 购买后要执行的激活事件
    }
    [Header("所有商品（在Inspector中配置）")]
    [SerializeField] private List<ShopItemData> allItems;

    [Header("五个商品槽位")]
    [SerializeField] private Image[] slotImages;
    [SerializeField] private Button[] slotButtons;
    [SerializeField] private Text[] slotPriceTexts;  

    [Header("确认弹窗")]
    [SerializeField] private ConfirmPanel confirmPanel;

    private ShopItemData[] currentSlots = new ShopItemData[5];

    private void Awake() => Instance = this;
    private void Start()
    {
        for (int i = 0; i < slotButtons.Length; i++)
        {
            int index = i;
            slotButtons[i].onClick.AddListener(() => OnSlotClicked(index));
        }
        RefreshAllSlots();
    }
    private void Update()
    {
       //tuteng
        sSkill = UnityEngine.Random.Range(0, sSkillNum);
        ShopImages.sprite = sskillSprites[sSkill];
        if (sSkill == 0)
        {
            siname.text = "图腾+1";
            sitext.text = "这是一个介绍（TT   ";
            sctext.text = Onemore.Skillcoin.ToString();

        }
        if (TTskill)
        {
            GetComponent<Onemore>().enabled = true;
        }
    }
    public void MakeSureTT()
    {
        SureTT.SetActive(true);
    }
   
    public void SureYesTT()
    {
        GameManager.CostCoin(Onemore.Skillcoin);
        TTskill = true;
        SureTT.SetActive(false);
    }
    public void DJskill()
    {
        if (!SkillButtonManager.Instance.CanPurchase)
        {
            Debug.Log("技能已满");
            return;
        }
        GameManager.CostCoin(CoolSkill.Skillcoin);
        SkillButtonManager.Instance.ActivateNextSkill(currentSlots[1].icon, () => { Debug.Log("Button"); GetComponent<CoolSkill>().enabled = true; });
    }
    public void RefreshAllSlots()
    {
        if (allItems.Count < 5)
        {
            Debug.LogError("商品总数不足5个！");
            return;
        }

        List<ShopItemData> pool = new List<ShopItemData>(allItems);
        for (int i = 0; i < 5; i++)
        {
            int rand = UnityEngine.Random.Range(0, pool.Count);
            currentSlots[i] = pool[rand];
            pool.RemoveAt(rand);
        }
        UpdateSlotUI();
    }

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
    }

    void OnSlotClicked(int index)
    {
        if (currentSlots[index] != null)
            confirmPanel.Show(currentSlots[index]);
    }

    /// <summary> 实际购买，由确认弹窗调用 </summary>
    public void TryPurchase(ShopItemData item)
    {
        if (GameManager.Coin < item.price)
        {
            Debug.Log("金币不足");
            return;
        }

        GameManager.SpendCoin(item.price);
        item.onPurchase.Invoke();   // 执行绑定的激活事件

        // 购买后清空该槽位（不可再买）
        for (int i = 0; i < 5; i++)
        {
            if (currentSlots[i] == item)
            {
                currentSlots[i] = null;
                break;
            }
        }
        UpdateSlotUI();
    }
}