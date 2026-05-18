using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Cannon : MonoBehaviour
{
    [Header("炮台设置")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private float rotationSpeed = 360f;

    [Header("等级配置")]
    [SerializeField] private int[] levelCosts = { 5, 10, 15 }; // C,B,A 对应消耗金币
    private int currentLevel = 0;     // 0=C, 1=B, 2=A
    public int CurrentLevel => currentLevel;

    private float nextFireTime;
    private Camera mainCam;
    private ObjectPool pool;
    private float currentAngle;

    // UI按钮引用（在Inspector中拖入）
    [Header("按钮")]
    [SerializeField] private UnityEngine.UI.Button upgradeButton;
    [SerializeField] private UnityEngine.UI.Button downgradeButton;

    [Header("消耗显示")]
    [SerializeField] private Text costText;   // 拖入炮台正下方的 Text

    void Start()
    {
        mainCam = Camera.main;
        pool = ObjectPool.Instance;
        currentAngle = transform.eulerAngles.z;
        UpdateButtons();
       
    }

    void Update()
    {
        AimAtMouse();
        // 发射条件：按下鼠标左键、冷却时间结束、且鼠标没有悬停在 UI 上
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime
            && !EventSystem.current.IsPointerOverGameObject())
        {
            Fire();
            nextFireTime = Time.time + fireRate;
        }
        UpdateCostText();
    }

    // 简单的一维鼠标映射
    void AimAtMouse()
    {
        float mouseXPercent = Mathf.Clamp01(mainCam.ScreenToViewportPoint(Input.mousePosition).x);
        float angle = Mathf.Lerp(80f, -80f, mouseXPercent);
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void Fire()
    {
        // 检查金币是否足够
        int cost = levelCosts[currentLevel];
        if (!GameManager.SpendCoin(cost))
        {
            // 金币不足，可加提示
            Debug.Log("金币不足！");
            return;
        }

        GameObject bullet = pool.GetBullet();
        if (bullet != null)
        {
            bullet.transform.position = firePoint.position;
            bullet.transform.rotation = firePoint.rotation;
            bullet.SetActive(true);
        }
    }

    // 由按钮调用
    public void Upgrade()
    {
        if (currentLevel < levelCosts.Length - 1)
        {
            currentLevel++;
            UpdateButtons();
        }
    }

    public void Downgrade()
    {
        if (currentLevel > 0)
        {
            currentLevel--;
            UpdateButtons();
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