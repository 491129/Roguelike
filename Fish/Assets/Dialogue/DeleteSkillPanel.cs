using UnityEngine;
using UnityEngine.UI;

public class DeleteSkillPanel : MonoBehaviour
{
    public static DeleteSkillPanel Instance { get; private set; }

    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    private SkillButton currentTarget;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // 如果希望跨场景保持，可加 DontDestroyOnLoad(gameObject);

        confirmButton.onClick.AddListener(OnConfirm);
        if (cancelButton != null) cancelButton.onClick.AddListener(Hide);
        gameObject.SetActive(false);
    }

    public void Show(SkillButton target)
    {
        currentTarget = target;
        gameObject.SetActive(true);
        // 将面板移动到目标按钮附近（可选）
        transform.position = target.transform.position;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        currentTarget = null;
    }

    void OnConfirm()
    {
        if (currentTarget != null)
        {
            SkillButtonManager.Instance.RemoveSkill(currentTarget);
        }
        Hide();
    }
}