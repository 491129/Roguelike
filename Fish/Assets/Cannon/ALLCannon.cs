using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ALLCannon : MonoBehaviour
{
    [Header("按钮")]
    [SerializeField] private UnityEngine.UI.Button upgradeButton;
    [SerializeField] private UnityEngine.UI.Button downgradeButton;

    [Header("消耗显示")]
    [SerializeField] private Text costText;

    [Header("等级配置")]
    [SerializeField]static public int[] levelCosts = { 5, 10, 15 }; // C,B,A 对应消耗金币
    static public int currentLevel = 0;     // 0=C, 1=B, 2=A
    public GameObject[] Cannons;
    public int CurrentLevel => currentLevel;
    private void Start()
    {
        Cannons[currentLevel].SetActive(true);
        UpdateButtons();
    }
    private void Update()
    {
        UpdateCostText();
    }
    public void Upgrade()
    {
        if (currentLevel < levelCosts.Length - 1)
        {

            Cannons[currentLevel].SetActive(false);
            currentLevel++;
            UpdateButtons();
            Cannons[currentLevel].SetActive(true);
        }
    }

    public void Downgrade()
    {
        if (currentLevel > 0)
        {
            Cannons[currentLevel].SetActive(false);
            currentLevel--;
            UpdateButtons();
            Cannons[currentLevel].SetActive(true);
        }
    }
    void UpdateButtons()
    {
        // 升级按钮：达到最高级时禁用
        if (upgradeButton != null)
            upgradeButton.interactable = (currentLevel < levelCosts.Length - 1);

        // 降级按钮：最低级时禁用
        if (downgradeButton != null)
            downgradeButton.interactable = (currentLevel > 0);
    }
    void UpdateCostText()
    {
        if (costText != null)
            costText.text = "消耗：" + levelCosts[currentLevel];
    }
}
