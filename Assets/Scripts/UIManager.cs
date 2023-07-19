using System;
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
        #region CheckReferences
        // Check if all references are set
        if (!pauseMenu)
        {
            throw new Exception("PauseMenu was not set in UIManager");
        }
        if (!hud)
        {
            throw new Exception("HUD was not set in UIManager");
        }
        if (!citizenText)
        {
            throw new Exception("CitizenText was not set in UIManager");
        }
        if (!foodText)
        {
            throw new Exception("FoodText was not set in UIManager");
        }
        if (!woodText)
        {
            throw new Exception("WoodText was not set in UIManager");
        }
        if (!stoneText)
        {
            throw new Exception("StoneText was not set in UIManager");
        }
        if (!tinText)
        {
            throw new Exception("TinText was not set in UIManager");
        }
        if (!copperText)
        {
            throw new Exception("CopperText was not set in UIManager");
        }
        if (!tooltipWindow)
        {
            throw new Exception("TooltipWindow was not set in UIManager");
        }
        if (!tooltipText)
        {
            throw new Exception("TooltipText was not set in UIManager");
        }
        #endregion
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

    public void TogglePauseMenu()
    {
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        hud.SetActive(!hud.activeSelf);
    }

    public void ChangeResources(ResourceClass CurrentResources)
    {
        foodText.text = CurrentResources.food.ToString();
        woodText.text = CurrentResources.wood.ToString();
        stoneText.text = CurrentResources.stone.ToString();
        tinText.text = CurrentResources.tin.ToString();
        copperText.text = CurrentResources.copper.ToString();
    }
}
