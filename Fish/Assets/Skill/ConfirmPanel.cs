using UnityEngine;
using UnityEngine.UI;
using static SkillShopManager;

public class ConfirmPanel : MonoBehaviour
{
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    private ShopItemData currentItem;

    private void Awake()
    {
        confirmButton.onClick.AddListener(OnConfirm);
        cancelButton.onClick.AddListener(Hide);
        gameObject.SetActive(false);
    }

    public void Show(ShopItemData item)
    {
        currentItem = item;
        gameObject.SetActive(true);
    }

    public void Hide() => gameObject.SetActive(false);

    private void OnConfirm()
    {
        if (currentItem == null) return;
       // SkillShopManager.Instance.TryPurchase(currentItem);
        Hide();
    }
}