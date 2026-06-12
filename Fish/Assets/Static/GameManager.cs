using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class GameManager : MonoBehaviour
{
    public static int Coin { get; private set; } = 2000;

    [SerializeField] private Text coinText;
    [SerializeField] private Text coinText1;

    public static void AddCoin(int amount)
    {
        if (TotemManager.Instance.heixin && Boss.Instance.bossDefeated) { amount *= 2; };
        Coin += amount;
        if (Instance != null && Instance.coinText != null)
            Instance.coinText.text = "Coin：" + Coin;
            Instance.coinText1.text = "Coin：" + Coin;
    }
  
    public static bool SpendCoin(int amount)
    {
        if (Coin >= amount)
        {
            Coin -= amount;
            UpdateUI();
            return true;
        }
        // 负债模式：检查是否允许负债且负债后不超过上限
        else if (TotemManager.Instance != null && TotemManager.Instance.canDebt)
        {
            int newCoin = Coin - amount;
            if (newCoin >= -TotemManager.Instance.debtLimit)
            {
                Coin = newCoin;
                UpdateUI();
                return true;
            }
        }
        return false;
    }

    private static GameManager Instance;

    void Awake()
    {
        Instance = this;
        // 初始显示
        if (coinText != null)
            coinText.text = "Coin：" + Coin;
        if (coinText1 != null)
            coinText1.text = "Coin：" + Coin;
    }
    private static void UpdateUI()
    {
        if (Instance != null && Instance.coinText != null)
            Instance.coinText.text = "Coin：" + Coin;
        if (Instance != null && Instance.coinText1 != null)
            Instance.coinText1.text = "Coin：" + Coin;
    }

}