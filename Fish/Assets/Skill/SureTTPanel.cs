using UnityEngine;
using UnityEngine.UI;

public class SureTTPanel : MonoBehaviour
{
    [SerializeField] private Button confirmButton;  // 确认按钮
    [SerializeField] private Button cancelButton;   // 取消按钮

    private SkillShopManager.ShopItemData currentItem;  // 当前要购买的商品

    private void Awake()
    {
        confirmButton.onClick.AddListener(OnConfirm);
        cancelButton.onClick.AddListener(Hide);
        gameObject.SetActive(false);
    }

    public void Show(SkillShopManager.ShopItemData item)
    {
        currentItem = item;
        gameObject.SetActive(true);
    }

    public void Hide() => gameObject.SetActive(false);

    private void OnConfirm()
    {
        if (currentItem != null)
            SkillShopManager.Instance.TryPurchase(currentItem);
        Hide();
    }
}