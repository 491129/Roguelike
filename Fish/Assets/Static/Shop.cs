using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Shop : MonoBehaviour
{
    public GameObject shopPanel;
    public GameObject[] dialoguePanels;
    private int wel;
    public static bool BossDef=true;
    public void LoadShop()
    {
        shopPanel.SetActive(true);
        wel=Random.Range(0,dialoguePanels.Length);
        dialoguePanels[wel].SetActive(true);
        Time.timeScale = 0f;

    }
    public void closeShop()
    {
        dialoguePanels[wel].SetActive(false);
        shopPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}
