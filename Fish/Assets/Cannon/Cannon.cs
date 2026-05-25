using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Cannon : MonoBehaviour
{
    [Header("炮台设置")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private float rotationSpeed = 360f;

    [Header("Debuff")]
    public float attackSpeedMultiplier = 1f;  
    private float baseFireRate;

    private float nextFireTime;
    private Camera mainCam;
    private ObjectPool pool;
    private float currentAngle;
    public Text NoCoin;

    
    void Start()
    {
        baseFireRate = fireRate;
        mainCam = Camera.main;
        pool = ObjectPool.Instance;
        currentAngle = transform.eulerAngles.z;
    }

    void Update()
    {
        AimAtMouse();
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime
            && !EventSystem.current.IsPointerOverGameObject())
        {
            Fire();
            nextFireTime = Time.time + fireRate;
        }

    }

    void AimAtMouse()
    {
        float mouseXPercent = Mathf.Clamp01(mainCam.ScreenToViewportPoint(Input.mousePosition).x);
        float angle = Mathf.Lerp(80f, -80f, mouseXPercent);
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void Fire()
    {
       
        int cost = ALLCannon.levelCosts[ALLCannon.currentLevel];
        if (!GameManager.SpendCoin(cost))
        {
            NoCoin.text = "金币不足!无法发射!";
            return;
        }
        else
        {
            NoCoin.text = " ";
        }
            GameObject bullet = pool.GetBullet();
        if (bullet != null)
        {
            bullet.transform.position = firePoint.position;
            bullet.transform.rotation = firePoint.rotation;
            bullet.SetActive(true);
        }

    }
    public void ApplyDebuff(string bossType)
    {
        switch (bossType)
        {
            case "Freeze":
                attackSpeedMultiplier *= 0.85f;
                break;
                // 其他类型如攻速不影响，但可保留扩展
        }
        fireRate = baseFireRate / attackSpeedMultiplier;
    }

    public void RemoveDebuff(string bossType)
    {
        switch (bossType)
        {
            case "Freeze":
                attackSpeedMultiplier /= 0.85f;
                break;
        }
        fireRate = baseFireRate / attackSpeedMultiplier;
    }

}