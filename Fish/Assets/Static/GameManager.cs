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
        if (TotemManager.Instance.heixin && Boss.Instance.bossDefeated) { amount *= 2; TotemManager.Instance?.TriggerEffectByName("黑心商人"); }
        ;
        Coin += amount;
        if (Instance != null && Instance.coinText != null)
            Instance.coinText.text = Coin.ToString();
        //Instance.coinText1.text = Coin.ToString();
        // 播放金币音效
        AudioManager.PlayCoin();
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
            if (newCoin <= 0)
            {
                TotemManager.Instance?.TriggerEffectByName("先欠着");
            }
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
            coinText.text = Coin.ToString();
        if (coinText1 != null)
            coinText1.text = Coin.ToString();
    }
    private static void UpdateUI()
    {
        if (Instance != null && Instance.coinText != null)
            Instance.coinText.text = Coin.ToString();
        if (Instance != null && Instance.coinText1 != null)
            Instance.coinText1.text = Coin.ToString(); 
    }

}