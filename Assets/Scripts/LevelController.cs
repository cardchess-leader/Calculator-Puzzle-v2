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
    }

    void Update()
    {
        // UpdatePageIndex();
        // UpdatePanelDots();
        // UpdatePageTitle();
    }

    void InitializeHandler()
    {
        root.Q<Button>(className: "back-button").clicked += () =>
        {
            GameManager.instance.SwitchPage("Main");
        };
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
                Debug.Log(cellElement == null);
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
                }
            }
            cellElement.Q<VisualElement>("Cell").userData = i;
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
                title1.text = "BASIC + SCORE";
                title2.text = $"= {GameManager.instance.GetScoreAtDifficulty(QuestionSO.Difficulty.Easy)}";
                title2.style.color = new StyleColor(new Color(0.25f, 1, 0, 1));
                break;
            case 1:
                title1.text = "Hard + SCORE";
                title2.text = $"= {GameManager.instance.GetScoreAtDifficulty(QuestionSO.Difficulty.Hard)}";
                title2.style.color = new StyleColor(new Color(1, 0, 0, 1));
                break;
            case 2:
                title1.text = "MORE + PUZZLE";
                title2.text = "= ???";
                title2.style.color = new StyleColor(new Color(0, 1, 1, 1));
                break;
        }
    }

    void UpdatePanelDots()
    {
        for (int i = 0; i < 3; i++)
        {
            root.Q<VisualElement>($"Dot_{i + 1}").Q<VisualElement>("InnerDot").style.visibility = Visibility.Hidden;
        }
        root.Q<VisualElement>($"Dot_{pageIndex + 1}").Q<VisualElement>("InnerDot").style.visibility = Visibility.Visible;
    }

    void UpdatePageIndex()
    {
        ScrollViewPro horizontalScrollView = root.Q<ScrollViewPro>("HorizontalScrollView");
        int index = (int)Mathf.Round(horizontalScrollView.horizontalScroller.value / horizontalScrollView.resolvedStyle.width);
        if (index >= 0 && index < 3)
        {
            pageIndex = index;
        }
    }
}
