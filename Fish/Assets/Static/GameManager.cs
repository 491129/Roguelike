using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static int Coin { get; private set; } = 100;

    [SerializeField] private Text coinText;  

    public static void AddCoin(int amount)
    {
        Coin += amount;
        // 更新 UI：通过单例引用
        if (Instance != null && Instance.coinText != null)
            Instance.coinText.text = "Coin：" + Coin;
    }

    public static bool SpendCoin(int amount)
    {
        if (Coin >= amount)
        {
            Coin -= amount;
            if (Instance != null && Instance.coinText != null)
                Instance.coinText.text = "Coin：" + Coin;
            return true;
        }
        return false;
    }

    // 单例模式，方便静态方法访问实例组件
    private static GameManager Instance;

    void Awake()
    {
        Instance = this;
        // 初始显示
        if (coinText != null)
            coinText.text = "Coin：" + Coin;
    }
}