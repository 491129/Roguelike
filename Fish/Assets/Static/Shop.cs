using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Shop : MonoBehaviour
{
    public GameObject shopPanel;
    public GameObject[] dialoguePanels;
    [SerializeField] private DisDialogue merchantDialogue;   // 拖入商人对话框物体
    public static bool BossDef=true;
    public void LoadShop()
    {
        shopPanel.SetActive(true);
        DisDialogue dialogue = FindObjectOfType<DisDialogue>(); // 或直接引用
        merchantDialogue.ShowTemporary("欢迎光临！挑选你需要的商品吧~", 2.5f);
        Time.timeScale = 0f;

    }
    public void closeShop()
    {
        shopPanel.SetActive(false);
        Time.timeScale = 1f;
        if (TotemManager.Instance.chuanzhang)
        {
            TotemManager.Instance?.TriggerEffectByName("我爸是船长");
        }
    }
}
