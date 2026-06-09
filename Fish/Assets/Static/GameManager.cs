using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static int Coin { get; private set; } = 1000;

    [SerializeField] private Text coinText;
    [SerializeField] private Text coinText1;
    public static void AddCoin(int amount)
    {
        Coin += amount;
        if (Instance != null && Instance.coinText != null)
            Instance.coinText.text = "Coin£∫" + Coin;
            Instance.coinText1.text = "Coin£∫" + Coin;
    }
    public static void CostCoin(int amount)
    {
        Coin -= amount;
        if (Instance != null && Instance.coinText != null)
            Instance.coinText.text = "Coin£∫" + Coin;
        Instance.coinText1.text = "Coin£∫" + Coin;
    }

    public static bool SpendCoin(int amount)
    {
        if (Coin >= amount)
        {
            Coin -= amount;
            if (Instance != null && Instance.coinText != null)
                Instance.coinText.text = "Coin£∫" + Coin;
             Instance.coinText1.text = "Coin£∫" + Coin;
            return true;
        }
        return false;
    }

    private static GameManager Instance;

    void Awake()
    {
        Instance = this;
        // ≥ı ºœ‘ æ
        if (coinText != null)
            coinText.text = "Coin£∫" + Coin;
        if (coinText1 != null)
            coinText1.text = "Coin£∫" + Coin;
    }
}