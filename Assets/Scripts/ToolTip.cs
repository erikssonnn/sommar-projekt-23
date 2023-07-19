using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string text;
    private float tooltipTimer = 0.5f;
    public void OnPointerEnter(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(StartTimer());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopAllCoroutines();
        UIManager.Instance.HideTooltip();
    }

    private void ShowTooltip(string text, Vector3 mousePos)
    {
        UIManager.Instance.ShowTooltip(text, mousePos);
    }

    private IEnumerator StartTimer()
    {
        yield return new WaitForSeconds(tooltipTimer);

        ShowTooltip(text, Input.mousePosition);
    }
}
