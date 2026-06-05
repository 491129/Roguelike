using UnityEngine;
using UnityEngine.UI;
using static SkillShopManager;

public class ConfirmPanel : MonoBehaviour
{
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    private ShopItemData currentItem;
    private ShopItemData00 currentItem00;
    private bool isBossItem = false;

    private void Awake()
    {
        confirmButton.onClick.AddListener(OnConfirm);
        cancelButton.onClick.AddListener(Hide);
        gameObject.SetActive(false);
    }

    public void Show(ShopItemData item)
    {
        currentItem = item;
        isBossItem = false;
        gameObject.SetActive(true);
    }
    public void Show(ShopItemData00 item)
    {
        currentItem00 = item;
        isBossItem = true;
        gameObject.SetActive(true);
    }
    public void Hide() => gameObject.SetActive(false);

    private void OnConfirm()
    {
        if (isBossItem)
        {
            if (currentItem00 != null)
                SkillShopManager.Instance.TryPurchase00(currentItem00);
        }
        else
        {
            if (currentItem != null)
                SkillShopManager.Instance.TryPurchase(currentItem);
        }
        Hide();
    }
}