using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hyperbyte;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public List<QuestionSO> questionList = new List<QuestionSO>();
    public List<CalculatorSO> calculatorList = new List<CalculatorSO>();
    public GameObject mainPage, levelPage, inGamePage, shopPage;
    public int dailyScore;
    public int targetLevel;
    public bool isDaily;
    public bool locked;
    GameObject activePage;
    void Awake()
    {
        instance = this;
        InitPlayerPrefData();
    }
    void Start()
    {
        activePage = mainPage;
        mainPage.SetActive(true);
    }
    public void EquipCalculator(int calcIndex)
    {
        for (int i = 0; i < calculatorList.Count; i++)
        {
            if (PlayerPrefs.GetInt($"Calc {calculatorList[i].title}") == 2)
            {
                PlayerPrefs.SetInt($"Calc {calculatorList[i].title}", 1);
            }
        }
        PlayerPrefs.SetInt($"Calc {calculatorList[calcIndex].title}", 2);
    }
    public int GetScoreAtDifficulty(QuestionSO.Difficulty difficulty)
    {
        try
        {
            int sum = 0;
            for (int i = 0; i < questionList.Count; i++)
            {
                if (questionList[i].difficulty == difficulty)
                {
                    int score = int.Parse(PlayerPrefs.GetString("Level Score")[i].ToString());
                    sum += score;
                }
            }
            return sum;
        }
        catch (Exception e)
        {
            return 0;
        }
    }
    public int GetScoreAtIndex(int index)
    {
        try
        {
            return int.Parse(PlayerPrefs.GetString("Level Score")[index].ToString());
        }
        catch (Exception e)
        {
            return 0;
        }
    }
    public int GetUnsolvedIndex()
    {
        try
        {
            for (int i = 0; i < questionList.Count; i++)
            {
                int score = int.Parse(PlayerPrefs.GetString("Level Score")[i].ToString());
                if (score == 0)
                {
                    return i;
                }
            }
            return questionList.Count;
        }
        catch (Exception e)
        {
            return 0;
        }
    }
    public int GetDailyScore()
    {
        return PlayerPrefs.GetInt("Daily Score");
    }
    public int GetTotalScore()
    {
        int sum = 0;
        for (int i = 0; i < questionList.Count; i++)
        {
            sum += int.Parse(PlayerPrefs.GetString("Level Score")[i].ToString());
        }
        return sum + GetDailyScore();
    }
    public void SwitchPage(string targetPage)
    {
        activePage.SetActive(false);
        switch (targetPage)
        {
            case "Main":
                activePage = mainPage;
                break;
            case "Level":
                activePage = levelPage;
                break;
            case "InGame":
                activePage = inGamePage;
                break;
            case "Shop":
                activePage = shopPage;
                break;
        }
        activePage.SetActive(true);
    }
    void InitPlayerPrefData()
    {
        // PlayerPrefs.DeleteAll();
        if (PlayerPrefs.GetInt("Initialize") != 1)
        {
            string levelScore = "";
            string levelHint = "";
            for (int i = 0; i < questionList.Count; i++)
            {
                levelScore += "0";
                levelHint += "0";
            }
            PlayerPrefs.SetString("Level Score", levelScore);
            PlayerPrefs.SetString("Level Hint", levelHint);
            PlayerPrefs.SetInt("Daily Score", 0);
            PlayerPrefs.SetInt("Daily Hint", 0);
            PlayerPrefs.SetInt("Calc Basic", 2);
            PlayerPrefs.SetInt("Calc Round", 1);
            PlayerPrefs.SetInt("Initialize", 1);
        }
    }

    public int GetHintLevel()
    {
        try
        {
            return int.Parse(PlayerPrefs.GetString("Level Hint")[targetLevel].ToString());
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return 0;
        }
    }

    public void IncrementHintLevel()
    {
        try
        {
            string hintString = PlayerPrefs.GetString("Level Hint");
            int newHintLevel = int.Parse(hintString[targetLevel].ToString()) + 1;
            PlayerPrefs.SetString("Level Hint", Helper.ReplaceCharAt(hintString, newHintLevel.ToString(), targetLevel));
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    public void ClearCurrentLevel(int score) // Level Clear Post Process
    {
        try
        {
            string scoreString = PlayerPrefs.GetString("Level Score");
            string hintString = PlayerPrefs.GetString("Level Hint");
            if (scoreString[targetLevel] == '0')
            {
                PlayerPrefs.SetString("Level Score", Helper.ReplaceCharAt(scoreString, score.ToString(), targetLevel));
            }
            PlayerPrefs.SetString("Level Hint", Helper.ReplaceCharAt(hintString, "0", targetLevel));
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    public string GetCurrentCalcName()
    {
        foreach (CalculatorSO calc in calculatorList)
        {
            if (calc.GetStatus() == CalculatorSO.Status.Active)
            {
                return calc.title;
            }
        }
        return "Basic";
    }

    public bool IsLevelAvailable(int level)
    {
        if (IsLevelLocked(level))
        {
            return false;
        }
        else if (level > GetUnsolvedIndex())
        {
            return false;
        }
        return true;
    }
    public bool IsLevelLocked(int level)
    {
        if (!ProfileManager.Instance.IsAppAdFree() && questionList[level].difficulty != QuestionSO.Difficulty.Easy)
        {
            return true;
        }
        return false;
    }
}
