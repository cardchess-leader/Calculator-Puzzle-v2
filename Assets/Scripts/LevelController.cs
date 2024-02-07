using System;
using System.Collections;
using System.Collections.Generic;
using Hyperbyte;
using UnityEngine;
using UnityEngine.UIElements;
using Kamgam.UIToolkitScrollViewPro;

public class LevelController : MonoBehaviour
{
    public GameObject mainPage;
    public GameObject noAdsPurchasePopup;
    VisualElement root;
    float time;
    int pageIndex = 0;
    void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        InitializeQuestionRows();
        InitializeHandler();
        Helper.SetHapticToBtn(root, "ui-btn", false, GameManager.instance.uiBtnClickSound);
    }

    void Update()
    {
        UpdatePageIndex();
        UpdatePanelDots();
        UpdatePageTitle();
    }

    void InitializeHandler()
    {
        root.Q<Button>(className: "back-button").clicked += () =>
        {
            GameManager.instance.SwitchPage("Main");
        };
        root.Q<Button>(className: "back-button").AddToClassList("ui-btn");
        root.Q<ScrollViewPro>().RegisterCallback<ClickEvent>(evt => OpenLevel(evt));
    }

    void OpenLevel(ClickEvent evt)
    {
        if (evt.target is VisualElement element && element.userData is int targetLevel)
        {
            if (GameManager.instance.IsLevelLocked(targetLevel))
            {
                noAdsPurchasePopup.Activate();
            }
            else if (GameManager.instance.IsLevelAvailable(targetLevel))
            {
                GameManager.instance.targetLevel = targetLevel;
                GameManager.instance.SwitchPage("InGame");
            }
        }
    }

    void InitializeQuestionRows()
    {
        VisualElement questionRow = null;
        int unsolvedIndex = GameManager.instance.GetUnsolvedIndex();
        for (int i = 0; i < GameManager.instance.questionList.Count; i++)
        {
            VisualElement cellElement;
            if (i % 5 == 0)
            {
                questionRow = new VisualElement();
                questionRow.AddToClassList("QuestionRow");
            }
            // locked or not
            if (GameManager.instance.IsLevelLocked(i))
            {
                cellElement = Resources.Load<VisualTreeAsset>($"Level Cell/cell_lock").CloneTree().Q("Cell");
            }
            else if (GameManager.instance.GetScoreAtIndex(i) == 0)
            {
                cellElement = Resources.Load<VisualTreeAsset>($"Level Cell/cell_number").CloneTree().Q("Cell");
                (cellElement as Button).text = (i + 1).ToString();
                if (i == unsolvedIndex)
                {
                    cellElement.Q("Triangle").style.visibility = Visibility.Visible;
                }
            }
            else
            {
                cellElement = Resources.Load<VisualTreeAsset>($"Level Cell/cell_star").CloneTree().Q("Cell");
                int score = GameManager.instance.GetScoreAtIndex(i);
                switch (score)
                {
                    case 1:
                        cellElement.Q("Star2").style.display = DisplayStyle.None;
                        cellElement.Q("Star3").style.display = DisplayStyle.None;
                        break;
                    case 2:
                        cellElement.Q("Star2").style.display = DisplayStyle.None;
                        break;
                }
            }
            // colorize
            if (i % 5 == 4)
            {
                cellElement.AddToClassList("color");
                switch (GameManager.instance.questionList[i].difficulty)
                {
                    case QuestionSO.Difficulty.Easy:
                        cellElement.AddToClassList("green");
                        break;
                    case QuestionSO.Difficulty.Medium:
                        cellElement.AddToClassList("orange");
                        break;
                    case QuestionSO.Difficulty.Hard:
                        cellElement.AddToClassList("red");
                        break;
                    case QuestionSO.Difficulty.Expert:
                        cellElement.AddToClassList("purple");
                        break;
                }
            }
            cellElement.Q<VisualElement>("Cell").userData = i;
            cellElement.AddToClassList("ui-btn");
            questionRow.Add(cellElement);

            if (i % 5 == 4)
            {
                root.Q<ScrollViewPro>().Add(questionRow);
            }
        }
    }

    void UpdatePageTitle()
    {
        Label title1 = root.Q<VisualElement>("PageTitle").ElementAt(0) as Label;
        Label title2 = root.Q<VisualElement>("PageTitle").ElementAt(1) as Label;
        switch (pageIndex)
        {
            case 0:
                title1.text = "Easy + SCORE";
                title2.text = $"= {GameManager.instance.GetScoreAtDifficulty(QuestionSO.Difficulty.Easy)}";
                title2.style.color = new StyleColor(new Color(0.435f, 1, 0, 1));
                break;
            case 1:
                title1.text = "Medium + SCORE";
                title2.text = $"= {GameManager.instance.GetScoreAtDifficulty(QuestionSO.Difficulty.Medium)}";
                title2.style.color = new StyleColor(new Color(1, 0.616f, 0, 1));
                break;
            case 2:
                title1.text = "Hard + SCORE";
                title2.text = $"= {GameManager.instance.GetScoreAtDifficulty(QuestionSO.Difficulty.Hard)}";
                title2.style.color = new StyleColor(new Color(1, 0, 0.333f, 1));
                break;
            case 3:
                title1.text = "Expert + SCORE";
                title2.text = $"= {GameManager.instance.GetScoreAtDifficulty(QuestionSO.Difficulty.Expert)}";
                title2.style.color = new StyleColor(new Color(0.333f, 0, 1, 1));
                break;
        }
    }

    void UpdatePanelDots()
    {
        for (int i = 0; i < 4; i++)
        {
            root.Q<VisualElement>($"Dot_{i + 1}").Q<VisualElement>("InnerDot").style.visibility = Visibility.Hidden;
        }
        root.Q<VisualElement>($"Dot_{pageIndex + 1}").Q<VisualElement>("InnerDot").style.visibility = Visibility.Visible;
    }

    void UpdatePageIndex()
    {
        ScrollViewPro scrollView = root.Q<ScrollViewPro>();
        float scrollAmount = scrollView.verticalScroller.value;
        float easyHeight = 200 / 5 * GameManager.instance.GetNumQuestionsWithDifficulty(QuestionSO.Difficulty.Easy);
        float mediumHeight = 200 / 5 * GameManager.instance.GetNumQuestionsWithDifficulty(QuestionSO.Difficulty.Medium);
        float hardHeight = 200 / 5 * GameManager.instance.GetNumQuestionsWithDifficulty(QuestionSO.Difficulty.Hard);
        float expertHeight = 200 / 5 * GameManager.instance.GetNumQuestionsWithDifficulty(QuestionSO.Difficulty.Expert);
        if (scrollAmount < easyHeight)
        {
            pageIndex = 0;
        }
        else if (scrollAmount < easyHeight + mediumHeight)
        {
            pageIndex = 1;
        }
        else if (scrollAmount > easyHeight + mediumHeight + hardHeight + expertHeight / 2 - scrollView.resolvedStyle.height)
        {
            pageIndex = 3;
        }
        else
        {
            pageIndex = 2;
        }
    }
}
