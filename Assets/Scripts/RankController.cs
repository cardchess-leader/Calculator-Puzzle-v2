using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Kamgam.UIToolkitScrollViewPro;
public class RankController : MonoBehaviour
{
    public static RankController instance;
    VisualElement root;
    void Awake()
    {
        instance = this;
    }
    void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        InitializeHandler();
        ShowRanks();
        if (!GameManager.instance.IsRankProfileSet())
        {
            PopupManager.instance.ShowPopup("RankProfileSetup");
        }
        Helper.SetHapticToBtn(root);
    }

    void InitializeHandler()
    {
        root.Q<Button>(className: "back-button").clicked += () =>
        {
            GameManager.instance.SwitchPage("Main");
        };
        root.Q("PlayerStat").RegisterCallback<ClickEvent>(evt => PopupManager.instance.ShowPopup("RankProfileSetup"));
    }

    public async void ShowRanks()
    {
        try
        {
            await LeaderBoardController.instance.UpdateScore();
            GenerateMyScore();
            GenerateTopRankList();
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    async void GenerateMyScore()
    {
        ScoreEntry playerScore = await LeaderBoardController.instance.GetPlayerScore();
        GenerateRankRow(playerScore, root.Q("PlayerStat").Q("Row"));
    }

    async void GenerateTopRankList()
    {
        root.Q<ScrollView>().contentContainer.Clear();
        List<ScoreEntry> topScores = await LeaderBoardController.instance.GetTop100Rank();
        foreach (ScoreEntry score in topScores)
        {
            VisualElement row = GenerateRankRow(score);
            root.Q<ScrollView>().contentContainer.Add(row);
        }
    }

    VisualElement GenerateRankRow(ScoreEntry score, VisualElement row = null)
    {
        if (row == null)
        {
            row = Resources.Load<VisualTreeAsset>("Rank/Row").CloneTree();
        }
        row.Q("Rank").Q<Label>().text = score.Rank.ToString();

        // Optimized handling for nickname and country metadata
        string nickname = score.Metadata.TryGetValue("nickname", out string nickValue) ? nickValue : "TEMP";
        row.Q("Player").Q<Label>().text = nickname;

        string country = score.Metadata.TryGetValue("country", out string countryValue) ? countryValue : "UN";
        Texture2D flagTexture = Resources.Load<Texture2D>($"Flags/{country}");
        if (flagTexture != null)
        {
            row.Q("Country").Q("Flag").style.backgroundImage = new StyleBackground(flagTexture);
        }

        row.Q("Score").Q<Label>().text = score.Score.ToString();

        return row;
    }
}
