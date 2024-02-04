using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Kamgam.UIToolkitScrollViewPro;
public class RankController : MonoBehaviour
{
    VisualElement root;
    void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        InitializeHandler();
        GenerateTopRankList();
    }

    void InitializeHandler()
    {
        root.Q<Button>(className: "back-button").clicked += () =>
        {
            GameManager.instance.SwitchPage("Main");
        };
    }

    async void GenerateTopRankList()
    {
        root.Q<ScrollView>().contentContainer.Clear();
        List<ScoreEntry> topScores = await LeaderBoardController.instance.GetTop100Rank();
        foreach (ScoreEntry score in topScores)
        {
            Debug.Log($"Player ID: {score.PlayerId}, Score: {score.Score}");
            VisualElement row = GenerateRankRow(score);
            root.Q<ScrollView>().contentContainer.Add(row);
        }
    }

    VisualElement GenerateRankRow(ScoreEntry score)
    {
        VisualElement row = Resources.Load<VisualTreeAsset>("Rank/Row").CloneTree();
        row.Q<Label>("Rank").text = score.Rank.ToString();

        // Optimized handling for nickname and country metadata
        string nickname = score.Metadata.TryGetValue("nickname", out string nickValue) ? nickValue : "TEMP";
        row.Q<Label>("Player").text = nickname;

        string country = score.Metadata.TryGetValue("country", out string countryValue) ? countryValue : "TEMP";
        row.Q<Label>("Country").text = country;

        row.Q<Label>("Score").text = score.Score.ToString();

        return row;
    }
}
