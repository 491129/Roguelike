using UnityEngine;
using UnityEngine.UI;

public class TimerUI : MonoBehaviour
{
    [SerializeField] private float totalTime = 1050f; 
    private Slider slider;
    private float timeRemaining;
    private bool isTimeUp;
    public float TimeElapsed => totalTime - timeRemaining;
    public float TotalTime => totalTime;

    void Start()
    {
        slider = GetComponent<Slider>();
        slider.maxValue = totalTime;
        slider.value = totalTime;
        timeRemaining = totalTime;
    }

    void Update()
    {
        if (isTimeUp) return;

        timeRemaining -= Time.deltaTime;
        slider.value = timeRemaining;

        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            slider.value = 0f;
            isTimeUp = true;
            OnTimeUp();
        }
    }

    void OnTimeUp()
    {
        Debug.Log("时间到！游戏结束");
    }
}