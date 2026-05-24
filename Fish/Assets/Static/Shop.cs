using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Shop : MonoBehaviour
{
    public GameObject shopPanel;
    public void LoadShop()
    {
        shopPanel.SetActive(true);
        Time.timeScale = 0f;
    }
    public void closeShop()
    {
        shopPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}
