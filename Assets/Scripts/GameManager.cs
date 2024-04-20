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
    public List<QuestionSO> dailyQuestionList = new List<QuestionSO>();
    public List<CalculatorSO> calculatorList = new List<CalculatorSO>();
    public GameObject mainPage, levelPage, inGamePage, shopPage, rankPage;
    public int dailyScore;
    public int targetLevel;
    public bool isDaily;
    public AudioClip uiBtnClickSound;
    public GameObject connectionLostOverlay;
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
        // StartCoroutine(CheckForInternetConnection(1));
    }
    IEnumerator ShowConsent()
    {
        // yield return new WaitForSeconds(0.75f);
        yield return null;
        UIController.Instance.ShowConsentDialogue();
    }
    IEnumerator CheckForInternetConnection(float intervalInSeconds)
    {
        // Check the internet connectivity
        switch (Application.internetReachability)
        {
            case NetworkReachability.NotReachable:
                connectionLostOverlay.SetActive(true);
                break;
            case NetworkReachability.ReachableViaCarrierDataNetwork:
            case NetworkReachability.ReachableViaLocalAreaNetwork:
                connectionLostOverlay.SetActive(false);
                break;
        }
        yield return new WaitForSeconds(intervalInSeconds);
        StartCoroutine(CheckForInternetConnection(intervalInSeconds));
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
        StartCoroutine(SwitchPageCoroutine(targetPage));
    }
    IEnumerator SwitchPageCoroutine(string targetPage)
    {
        yield return null;
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
                isDaily = false;
                break;
            case "Shop":
                activePage = shopPage;
                break;
            case "Daily":
                activePage = inGamePage;
                isDaily = true;
                break;
            case "Rank":
                activePage = rankPage;
                break;
        }
        activePage.SetActive(true);
    }
    void InitPlayerPrefData()
    {
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
            PlayerPrefs.SetInt("Daily Score", 0); // Accumulate Score
            PlayerPrefs.SetInt("Daily Today Score", 0); // Today's Daily Score
            PlayerPrefs.SetInt("Daily Hint", 0); // Today's Daily Hint
            PlayerPrefs.SetString("Daily Date", ""); // Last Daily Access Date
            PlayerPrefs.SetInt("Calc Basic", 2);
            PlayerPrefs.SetString("nickname", "Anonymous");
            PlayerPrefs.SetString("country", "UN");
            PlayerPrefs.SetInt("Initialize", 1);
            PlayerPrefs.SetInt("VersionCode", 1);
            // StartCoroutine(ShowConsent());
        }
        if (PlayerPrefs.GetInt("VersionCode") < 1) // update for the version code 1
        {
            PlayerPrefs.SetInt("VersionCode", 1);
            string levelScore = "";
            string levelHint = "";
            for (int i = 0; i < 10; i++) // for 10 additional questions
            {
                levelScore += "0";
                levelHint += "0";
            }
            PlayerPrefs.SetString("Level Score", PlayerPrefs.GetString("Level Score") + levelScore);
            PlayerPrefs.SetString("Level Hint", PlayerPrefs.GetString("Level Hint") + levelHint);
        }
    }

    public int GetHintLevel()
    {
        try
        {
            if (isDaily)
            {
                return PlayerPrefs.GetInt("Daily Hint");
            }
            else
            {
                return int.Parse(PlayerPrefs.GetString("Level Hint")[targetLevel].ToString());
            }
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
            if (isDaily)
            {
                PlayerPrefs.SetInt("Daily Hint", PlayerPrefs.GetInt("Daily Hint") + 1);
            }
            else
            {
                string hintString = PlayerPrefs.GetString("Level Hint");
                int newHintLevel = int.Parse(hintString[targetLevel].ToString()) + 1;
                PlayerPrefs.SetString("Level Hint", Helper.ReplaceCharAt(hintString, newHintLevel.ToString(), targetLevel));
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    public bool ClearCurrentLevel(int score) // Level Clear Post Process
    {
        try
        {
            if (isDaily)
            {
                PlayerPrefs.SetInt("Daily Hint", 0);
                PlayerPrefs.SetInt("Daily Today Score", score);
                PlayerPrefs.SetInt("Daily Score", PlayerPrefs.GetInt("Daily Score") + score);
                return true;
            }
            else
            {
                bool isFirstClear = false;
                string scoreString = PlayerPrefs.GetString("Level Score");
                string hintString = PlayerPrefs.GetString("Level Hint");
                if (scoreString[targetLevel] == '0')
                {
                    PlayerPrefs.SetString("Level Score", Helper.ReplaceCharAt(scoreString, score.ToString(), targetLevel));
                    isFirstClear = true;
                }
                PlayerPrefs.SetString("Level Hint", Helper.ReplaceCharAt(hintString, "0", targetLevel));
                return isFirstClear;
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return false;
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
        else if (level >= questionList.Count)
        {
            return false;
        }
        return true;
    }
    public bool IsLevelLocked(int level)
    {
        if (!ProfileManager.Instance.IsAppAdFree() && level >= 40)
        {
            return true;
        }
        return false;
    }
    public int GetNumQuestionsWithDifficulty(QuestionSO.Difficulty difficulty)
    {
        return questionList.Count(question => question.difficulty == difficulty);
    }

    public bool IsDailySolved()
    {
        if (PlayerPrefs.GetString("Daily Date") == DateTime.Today.ToString("M/d/yyyy") && PlayerPrefs.GetInt("Daily Today Score") > 0)
        {
            return true;
        }
        return false;
    }

    public QuestionSO GetDailyQuestion()
    {
        if (dailyQuestionList.Count == 0)
        {
            return null;
        }
        if (PlayerPrefs.GetString("Daily Date") != DateTime.Today.ToString("M/d/yyyy"))
        {
            PlayerPrefs.SetString("Daily Date", DateTime.Today.ToString("M/d/yyyy"));
            PlayerPrefs.SetInt("Daily Hint", 0);
            PlayerPrefs.SetInt("Daily Today Score", 0);
        }
        int todayIndex = Helper.GetNthDayOfToday();
        return dailyQuestionList[todayIndex % dailyQuestionList.Count];
    }

    public void SetRankProfile()
    {
        PlayerPrefs.SetInt("Profile Set", 1);
    }

    public bool IsRankProfileSet()
    {
        return PlayerPrefs.GetInt("Profile Set") == 1;
    }

    public void SetPlayerProfile(string nickname, string country)
    {
        PlayerPrefs.SetString("nickname", nickname);
        PlayerPrefs.SetString("country", country);
    }

    public Dictionary<string, string> GetPlayerProfile()
    {
        return new Dictionary<string, string>() { { "nickname", PlayerPrefs.GetString("nickname") }, { "country", PlayerPrefs.GetString("country") } };
    }

    public int GetMaxScoreSumForDifficulty(QuestionSO.Difficulty difficulty)
    {
        return 3 * questionList.Count(question => question.difficulty == difficulty);
    }
}
