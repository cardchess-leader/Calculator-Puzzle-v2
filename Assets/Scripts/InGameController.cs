using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Kamgam.UIToolkitScrollViewPro;
using Hyperbyte;
using Hyperbyte.Ads;
public class InGameController : MonoBehaviour
{
    public static InGameController instance;
    public static string hintTag = "Show Hint";
    public ParticleSystem confetti;
    VisualElement root;
    VisualElement calculator;
    Label screenLabel;
    string result;
    string tempResult;
    string optr;
    int inputCount = 0;
    bool isError = false;
    public bool adFree;
    QuestionSO question;
    Dictionary<string, string> symbolToValue = new Dictionary<string, string>
    {
        { "Zero", "0" },
        { "ZeroZero", "00" },
        { "Dot", "." },
        { "One", "1" },
        { "Two", "2" },
        { "Three", "3" },
        { "Four", "4" },
        { "Five", "5" },
        { "Six", "6" },
        { "Seven", "7" },
        { "Eight", "8" },
        { "Nine", "9" },
        { "Plus", "Plus" },
        { "Equal", "Equal" },
        { "Minus", "Minus" },
        { "Percent", "Percent" },
        { "Multiply", "Multiply" },
        { "Root", "Root" },
        { "Divide", "Divide" },
        { "AC", "AC" },
        { "Log", "Log" },
        { "Pow", "Pow" },
        { "Factorial", "Factorial" },
        { "Inverse", "Inverse" },
        { "Round", "Round" }
    };

    void OnEnable()
    {
        instance = this;
        root = GetComponent<UIDocument>().rootVisualElement;
        AdManager.OnRewardedAdRewardedEvent += OnRewardedAdRewarded;
        InitializeCalcSetting();
        InitializeHeader();
        InitializeHandler();
        InitializeCalc();
        ShowHintUI();
        UpdateCalcScreen();
    }

    void OnDisable()
    {
        instance = null;
        AdManager.OnRewardedAdRewardedEvent -= OnRewardedAdRewarded;
    }

    void OnHintBtnClick()
    {
        PopupManager.instance.ShowPopup("Hint");
    }

    void OnRewardedAdRewarded(string tag)
    {
        if (tag == hintTag)
        {
            RevealMoreHint();
        }
    }
    public void RevealMoreHint()
    {
        GameManager.instance.IncrementHintLevel();
        InitializeCalc();
        ShowHintUI();
    }

    void ShowHintUI()
    {
        List<QuestionSO.Symbol> hintList = new List<QuestionSO.Symbol>();
        int hintLevel = GameManager.instance.GetHintLevel();
        if (question.GetMaxHintLevel() <= hintLevel)
        {
            root.Q<Button>("Hint").SetEnabled(false);
        }
        if (hintLevel == 0)
        {
            return;
        }
        else
        {
            hintList = question.GetHintAtLevel(hintLevel);
        }
        ScrollViewPro inputLogContainer = root.Q<VisualElement>("InputLog").Q<ScrollViewPro>("LogContainer");
        foreach (QuestionSO.Symbol symbol in hintList)
        {
            VisualElement element = Resources.Load<VisualTreeAsset>($"Log Symbol/{symbol.ToString()}").CloneTree();
            element.AddToClassList("hint");
            inputLogContainer.Add(element);
        }
        if (inputLogContainer.Children().ToList().Count > 0)
        {
            inputLogContainer.schedule.Execute(() => inputLogContainer.ScrollTo(inputLogContainer.Children().LastOrDefault())).StartingIn(40);
        }
    }

    void InitializeCalcSetting()
    {
        root.Q("CalcContainer").Add(Resources.Load<VisualTreeAsset>($"Calculator/{GameManager.instance.GetCurrentCalcName()}/Calculator").CloneTree());
        question = GameManager.instance.isDaily ? GameManager.instance.GetDailyQuestion() : GameManager.instance.questionList[GameManager.instance.targetLevel];
        for (int i = 0; i < question.swapFromSymbols.Count; i++)
        {
            VisualElement elementToRemove = root.Q<VisualElement>("Calculator").Q<VisualElement>(question.swapFromSymbols[i].ToString());
            int index = elementToRemove.parent.IndexOf(elementToRemove);
            VisualElement elementToAdd = Resources.Load<VisualTreeAsset>($"Calculator/Operators/{question.swapToSymbols[i].ToString()}").CloneTree();
            elementToRemove.parent.Insert(index, elementToAdd);
            elementToRemove.RemoveFromHierarchy();
        }
        foreach (QuestionSO.Symbol symbol in question.lockedSymbols)
        {
            Button btn = root.Q<VisualElement>("Calculator").Q<Button>(symbol.ToString());
            if (btn != null)
            {
                btn.SetEnabled(false);
                btn.userData = "disabled";
            }
        }
    }

    void InitializeHeader()
    {
        if (GameManager.instance.isDaily)
        {
            root.Q<Label>("LevelTitle").text = DateTime.Today.ToString("M/d/yyyy");
        }
        else
        {
            root.Q<Label>("LevelTitle").text = $"Level {GameManager.instance.targetLevel + 1}";
        }
        root.Q<VisualElement>("Title").Q<Label>().text = $"Make {question.goalNum}";
        if (GameManager.instance.isDaily)
        {
            root.Q("GamePage").AddToClassList("dailyLevel");
            root.Q("StageClear").AddToClassList("dailyLevel");
        }
        else if (question.difficulty == QuestionSO.Difficulty.Easy)
        {
            root.Q("GamePage").AddToClassList("easyLevel");
            root.Q("StageClear").AddToClassList("easyLevel");
        }
        else if (question.difficulty == QuestionSO.Difficulty.Medium)
        {
            root.Q("GamePage").AddToClassList("mediumLevel");
            root.Q("StageClear").AddToClassList("mediumLevel");
        }
        else if (question.difficulty == QuestionSO.Difficulty.Hard)
        {
            root.Q("GamePage").AddToClassList("hardLevel");
            root.Q("StageClear").AddToClassList("hardLevel");
        }
        else
        {
            root.Q("GamePage").AddToClassList("expertLevel");
            root.Q("StageClear").AddToClassList("expertLevel");
        }
    }

    void InitializeHandler()
    {
        root.Q<Button>(className: "back-button").clicked += () =>
        {
            if (GameManager.instance.isDaily)
            {
                GameManager.instance.SwitchPage("Main");
            }
            else
            {
                GameManager.instance.SwitchPage("Level");
            }
        };
        root.Q<VisualElement>("Calculator").RegisterCallback<ClickEvent>(HandleCalcBtnClick);
        root.Q<Button>("Hint").clicked += OnHintBtnClick;
        root.Q<Button>("Continue").clicked += OnContinueBtnClick;
    }

    void OnContinueBtnClick()
    {
        if (!ProfileManager.Instance.IsAppAdFree() && AdManager.Instance.IsInterstitialAvailable())
        {
            AdManager.Instance.ShowInterstitial();
        }
        if (GameManager.instance.isDaily)
        {
            GameManager.instance.SwitchPage("Main");
        }
        else
        {
            GameManager.instance.targetLevel++;
            gameObject.SetActive(false);
            gameObject.SetActive(true);
        }
    }

    void InitializeCalc()
    {
        result = question.initialNum.ToString();
        tempResult = "";
        optr = "";
        isError = false;
        inputCount = 0;
        InitializeInputLog();
        UpdateCalcScreen();
    }

    void InitializeInputLog()
    {
        root.Q<VisualElement>("InputLog").Q<VisualElement>("LogContainer").Clear();
        root.Q<VisualElement>("InputLog").Q<Label>("Count").text = $"0/{question.GetMaxInputLimit()}";
    }

    void HandleCalcBtnClick(ClickEvent evt)
    {
        VisualElement element = evt.target as VisualElement;
        if (!(element is Button) || element.userData != null)
        {
            return;
        }
        string btnName = element.name, btnValue = symbolToValue[element.name];
        if (btnValue == "AC") // Reset
        {
            InitializeCalc();
            ShowHintUI();
        }
        else if (isError)
        {
            return;
        }
        else if (btnValue == "Equal")
        {
            EvaluateExpression("Equal");
        }
        else if (inputCount < question.GetMaxInputLimit())
        {
            AppendInput(btnName);
            EvaluateExpression(btnValue);
        }
    }

    void AppendInput(string btnName)
    {
        if (inputCount == 0)
        {
            InitializeInputLog();
        }
        inputCount++;
        root.Q<VisualElement>("InputLog").Q<Label>("Count").text = $"{inputCount}/{question.GetMaxInputLimit()}";
        VisualElement element = Resources.Load<VisualTreeAsset>($"Log Symbol/{btnName}").CloneTree();
        ScrollViewPro inputLogContainer = root.Q<VisualElement>("InputLog").Q<ScrollViewPro>("LogContainer");
        inputLogContainer.Add(element);
        inputLogContainer.schedule.Execute(() => inputLogContainer.ScrollTo(element)).StartingIn(40);
    }

    void EvaluateExpression(string symbol)
    {
        try
        {
            if (symbol == "Equal")
            {
                if (optr != "")
                {
                    if (tempResult != "")
                    {
                        result = GetBinaryOptrResult(float.Parse(result), float.Parse(tempResult), optr).ToString();
                        tempResult = "";
                        optr = "";
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
            }
            else if (IsOptr(symbol))
            {
                if (optr != "")
                {
                    if (tempResult != "")
                    {
                        if (IsBinaryOptr(symbol))
                        {
                            result = GetBinaryOptrResult(float.Parse(result), float.Parse(tempResult), optr).ToString();
                            tempResult = "";
                            optr = symbol;
                        }
                        else
                        {
                            tempResult = GetUnaryOptrResult(float.Parse(tempResult), symbol).ToString();
                        }
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                else
                {
                    if (IsBinaryOptr(symbol))
                    {
                        optr = symbol;
                    }
                    else
                    {
                        result = GetUnaryOptrResult(float.Parse(result), symbol).ToString();
                    }
                }
            }
            else // numbers
            {
                if (optr == "")
                {
                    if (float.TryParse(result + symbol, out float newValue))
                    {
                        result = symbol == "." ? result + symbol : newValue.ToString();
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                else
                {
                    if (float.TryParse(tempResult + symbol, out float newValue))
                    {
                        tempResult = symbol == "." ? tempResult + symbol : newValue.ToString();
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
            isError = true;
        }
        finally
        {
            CompareResultWithAnswer();
            UpdateCalcScreen();
        }
    }

    bool IsOptr(string symbol)
    {
        switch (symbol)
        {
            case "Plus":
            case "Minus":
            case "Multiply":
            case "Divide":
            case "Percent":
            case "Root":
            case "Log":
            case "Pow":
            case "Factorial":
            case "Inverse":
            case "Round":
                return true;
        }
        return false;
    }

    bool IsBinaryOptr(string symbol)
    {
        switch (symbol)
        {
            case "Plus":
            case "Minus":
            case "Multiply":
            case "Divide":
            case "Pow":
                return true;
        }
        return false;
    }

    void CompareResultWithAnswer()
    {
        if (question.goalNum.ToString() == result)
        {
            StartCoroutine(OnStageClearCoroutine());
        }
    }

    IEnumerator OnStageClearCoroutine()
    {
        root.Q("StageClear").RemoveFromClassList("hidden");
        if (GameManager.instance.isDaily)
        {
            root.Q("StageClear").Q<Label>("LevelTitle").text = DateTime.Today.ToString("M/d/yyyy");
            root.Q<Button>("Continue").text = "Main Menu";
        }
        else
        {
            root.Q("StageClear").Q<Label>("LevelTitle").text = $"Level {GameManager.instance.targetLevel + 1}";
        }
        root.Q<Label>("NumHintsUsed").text = $"# Hints Used: {GameManager.instance.GetHintLevel()}";
        int score = Mathf.Clamp(3 - GameManager.instance.GetHintLevel(), 1, 3);
        VisualElement scoreElement = Resources.Load<VisualTreeAsset>($"Score/Score {score}").CloneTree();
        root.Q("StageClear").Q("Section3").Add(scoreElement);
        GameManager.instance.ClearCurrentLevel(score);
        yield return new WaitForSeconds(0.5f);
        confetti.Play();
    }

    float GetBinaryOptrResult(float operand1, float operand2, string optr)
    {
        switch (optr)
        {
            case "Plus":
                return operand1 + operand2;
            case "Minus":
                return operand1 - operand2;
            case "Multiply":
                return operand1 * operand2;
            case "Divide":
                return operand1 / operand2;
            case "Pow":
                return Mathf.Pow(operand1, operand2);
        }
        throw new Exception();
    }

    float GetUnaryOptrResult(float operand, string optr)
    {
        switch (optr)
        {
            case "Percent":
                return operand / 100;
            case "Root":
                return Mathf.Sqrt(operand);
            case "Log":
                return Mathf.Log(operand, 10);
            case "Factorial":
                if (operand < 0 || operand != Mathf.Floor(operand))
                {
                    throw new Exception();
                }
                float result = 1;
                for (int i = 1; i <= operand; i++)
                {
                    result *= i;
                }
                return result;
            case "Inverse":
                return 1 / operand;
            case "Round":
                return Mathf.Floor(operand + 0.5f);
        }
        throw new Exception();
    }

    void UpdateCalcScreen()
    {
        // set the operator marker
        root.Q<VisualElement>("Plus_1").style.display = DisplayStyle.None;
        root.Q<VisualElement>("Minus_1").style.display = DisplayStyle.None;
        root.Q<VisualElement>("Multiply_1").style.display = DisplayStyle.None;
        root.Q<VisualElement>("Divide_1").style.display = DisplayStyle.None;
        root.Q<VisualElement>("Pow_1").style.display = DisplayStyle.None;
        if (isError)
        {
            root.Q<Label>("ScreenLabel").text = "ERROR!";
            return;
        }
        switch (optr)
        {
            case "Plus":
                root.Q<VisualElement>("Plus_1").style.display = DisplayStyle.Flex;
                break;
            case "Minus":
                root.Q<VisualElement>("Minus_1").style.display = DisplayStyle.Flex;
                break;
            case "Multiply":
                root.Q<VisualElement>("Multiply_1").style.display = DisplayStyle.Flex;
                break;
            case "Divide":
                root.Q<VisualElement>("Divide_1").style.display = DisplayStyle.Flex;
                break;
            case "Pow":
                root.Q<VisualElement>("Pow_1").style.display = DisplayStyle.Flex;
                break;
        }
        if (tempResult != "")
        {
            root.Q<Label>("ScreenLabel").text = tempResult;
        }
        else
        {
            root.Q<Label>("ScreenLabel").text = result;
        }
        Debug.Log($"result is: {result}, tempResult is: {tempResult}, optr is: {optr}, isError is: {isError}");
    }
}
