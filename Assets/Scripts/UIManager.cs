// using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("UI:")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject hud;
    [Header("RESOURCES:")]
    [SerializeField] private TMPro.TextMeshProUGUI citizenText;
    [SerializeField] private TMPro.TextMeshProUGUI foodText;
    [SerializeField] private TMPro.TextMeshProUGUI woodText;
    [SerializeField] private TMPro.TextMeshProUGUI stoneText;
    [SerializeField] private TMPro.TextMeshProUGUI tinText;
    [SerializeField] private TMPro.TextMeshProUGUI copperText;
    [Header("TOOLTIPS:")]
    [SerializeField] private RectTransform tooltipWindow;
    [SerializeField] private TMPro.TextMeshProUGUI tooltipText;

    // private Action<string, Vector2> showTooltipAction;
    // private Action hideTooltipAction;

    private void OnEnable()
    {
        // showTooltipAction += ShowTooltip;
        // hideTooltipAction += HideTooltip;
        //EventManager.Instance.OnResourcesChanged += ChangeResources;
    }
    private void onDisable()
    {
        // showTooltipAction -= ShowTooltip;
        // hideTooltipAction -= HideTooltip;
        //EventManager.Instance.OnResourcesChanged -= ChangeResources;
    }

    public static UIManager Instance { get; set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        HideTooltip();
    }

    public void ShowTooltip(string text, Vector3 mousePos)
    {
        tooltipText.text = text;
        tooltipWindow.sizeDelta = new Vector2(tooltipText.preferredWidth > 220 ? 220 : tooltipText.preferredWidth, tooltipText.preferredHeight);

        tooltipWindow.anchoredPosition = new Vector2(mousePos.x + tooltipWindow.sizeDelta.x * .5f, mousePos.y);

        tooltipWindow.gameObject.SetActive(true);
    }

    public void HideTooltip()
    {
        tooltipText.text = default;
        tooltipWindow.gameObject.SetActive(false);
    }

    public void ChangeResources()
    {
        
    }
}
