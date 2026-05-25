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
    [SerializeField]static public int[] levelCosts = { 5, 10, 15 }; 
    static public int currentLevel = 0;   

    public GameObject[] Cannons;
    public int CurrentLevel => currentLevel;
    private int buttonDisableCount = 0;

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
        if (buttonDisableCount > 0)
        {
            if (upgradeButton != null) upgradeButton.interactable = false;
            if (downgradeButton != null) downgradeButton.interactable = false;
            return;
        }
        if (upgradeButton != null)
            upgradeButton.interactable = (currentLevel < levelCosts.Length - 1);

        if (downgradeButton != null)
            downgradeButton.interactable = (currentLevel > 0);
    }
    void UpdateCostText()
    {
        if (costText != null)
            costText.text = "消耗：" + levelCosts[currentLevel];
    }
    public void DisableButtonsByBoss()
    {
        buttonDisableCount++;
        UpdateButtons();
    }

    public void EnableButtonsByBoss()
    {
        buttonDisableCount--;
        if (buttonDisableCount < 0) buttonDisableCount = 0;
        UpdateButtons();
    }

}
