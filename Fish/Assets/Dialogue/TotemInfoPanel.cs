using UnityEngine;
using UnityEngine.UI;

public class TotemInfoPanel : MonoBehaviour
{
    [SerializeField] private Text infoText;
    [SerializeField] private Button deleteButton;

    private int currentIndex = -1;

    void Start()
    {
        deleteButton.onClick.AddListener(OnDeleteClicked);
        gameObject.SetActive(false);
    }

    public void Show(TotemManager.TotemInfo info)
    {
        currentIndex = info.index;
        infoText.text = $"{info.itemName}\n{info.description}\n´¥·¢´ÎÊý£º{info.triggerCount}";
        gameObject.SetActive(true);
    }

    public void Hide() => gameObject.SetActive(false);

    void OnDeleteClicked()
    {
        TotemManager.Instance?.RemoveTotem(currentIndex);
        Hide();
    }
    void Cancel()
    {
        Hide();
    }
}