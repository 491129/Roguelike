using UnityEngine;
using UnityEngine.EventSystems;

public class TotemClickHandler : MonoBehaviour
{
    private int totemIndex;
    private TotemInfoPanel panel;

    public void Setup(int index, TotemInfoPanel infoPanel)
    {
        totemIndex = index;
        panel = infoPanel;
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1)) // ср╪Э
        {
            if (panel != null && TotemManager.Instance != null)
            {
                TotemManager.TotemInfo info = TotemManager.Instance.GetTotemInfo(totemIndex);
                if (info != null) panel.Show(info);
            }
        }
    }
}