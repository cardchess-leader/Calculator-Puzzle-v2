using System.Collections;
using System.Collections.Generic;
using Hyperbyte;
using UnityEngine;
using UnityEngine.UIElements;

public class MainController : MonoBehaviour
{
    public static MainController instance;
    VisualElement root;
    public GameObject noAdsPurchasePopup;
    public GameObject settingsPopup;
    public GameObject reviewPopup;
    void Awake()
    {
        instance = this;
    }
    void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        InitializeHandler();
        InitializeScore();
        if (!LeaderBoardController.instance.initialized)
        {
            root.Q<Button>("RankBtn").SetEnabled(false);
        }
        Helper.SetHapticToBtn(root);
    }
    void InitializeHandler()
    {
        root.Q<Button>("PlayBtn").clicked += () => GameManager.instance.SwitchPage("Level");
        root.Q<Button>("DailyBtn").clicked += () =>
        {
            if (GameManager.instance.IsDailySolved())
            {
                UIController.Instance.ShowMessage("Out of Questions", "Come Back Tomorrow!");
            }
            else
            {
                GameManager.instance.SwitchPage("Daily");
            }
        };
        root.Q<Button>("RankBtn").clicked += () => GameManager.instance.SwitchPage("Rank");
        root.Q<Button>("ShopBtn").clicked += () => GameManager.instance.SwitchPage("Shop");
        root.Q<Button>("NoAdsBtn").clicked += () => noAdsPurchasePopup.Activate();
        root.Q<Button>("SettingsBtn").clicked += () => settingsPopup.Activate();
        root.Q<Button>("RatingBtn").clicked += () => reviewPopup.Activate();
    }
    void InitializeScore()
    {
        root.Q<Label>("Score").text = $"SCORE: {GameManager.instance.GetTotalScore()}";
    }
    public void EnableRankBtn()
    {
        if (gameObject.activeSelf)
        {
            root.Q<Button>("RankBtn").SetEnabled(true);
        }
    }
}
