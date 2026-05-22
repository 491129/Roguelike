using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Cannon : MonoBehaviour
{
    [Header("炮台设置")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private float rotationSpeed = 360f;

    private float nextFireTime;
    private Camera mainCam;
    private ObjectPool pool;
    private float currentAngle;

    
    void Start()
    {
        mainCam = Camera.main;
        pool = ObjectPool.Instance;
        currentAngle = transform.eulerAngles.z;
       // UpdateButtons();
       
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
       // UpdateCostText();
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
        int cost = ALLCannon.levelCosts[ALLCannon.currentLevel];
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

 
}