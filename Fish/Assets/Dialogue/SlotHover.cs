using UnityEngine;
using UnityEngine.EventSystems;

public class SlotHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private DisDialogue merchantDialogue;   // 商人对话框实例
    private SkillShopManager.ShopItemData currentItem;      // 当前槽位商品

    public void SetItem(SkillShopManager.ShopItemData item)
    {
        currentItem = item;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentItem != null && merchantDialogue != null)
            merchantDialogue.ShowMessage(currentItem.description);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (merchantDialogue != null)
            merchantDialogue.HideMessage();
    }
}