using UnityEngine;
using UnityEngine.EventSystems;

public class SlotHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private DisDialogue merchantDialogue;   // 商人对话框实例
    private SkillShopManager.ShopItemData currentItem;      // 当前槽位商品
    private SkillShopManager.ShopItemData currentItem00;
    private string description;   // 当前槽位的商品描述


    //public void SetItem(SkillShopManager.ShopItemData item)
    //{
    //    currentItem = item;
    //}
    //public void OnPointerEnter(PointerEventData eventData)
    //{
    //    if (currentItem == null)
    //    {
    //        merchantDialogue.HideMessage();
    //        return;
    //    }
    //    if (merchantDialogue != null)
    //        merchantDialogue.ShowMessage(currentItem.description);
    //}

    //public void OnPointerExit(PointerEventData eventData)
    //{
    //    if (merchantDialogue != null)
    //        merchantDialogue.HideMessage();
    //}
    public void SetItem(SkillShopManager.ShopItemData item)
    {
        description = (item != null) ? item.description : null;
    }

    // 补给券商品调用
    public void SetItem(SkillShopManager.ShopItemData00 item)
    {
        description = (item != null) ? item.description : null;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!string.IsNullOrEmpty(description) && merchantDialogue != null)
            merchantDialogue.ShowMessage(description);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (merchantDialogue != null)
            merchantDialogue.HideMessage();
    }

}