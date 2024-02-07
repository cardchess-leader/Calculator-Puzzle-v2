using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Leaderboards;

public class ScoreRawEntry
{
    public string PlayerId { get; set; }
    public string Score { get; set; }
    public string Rank { get; set; }
    public string Metadata { get; set; }
}

public class ScoreEntry
{
    public string PlayerId { get; set; }
    public int Score { get; set; }
    public int Rank { get; set; }
    public Dictionary<string, string> Metadata { get; set; }
}

public class ScoreResponse
{
    public int limit;
    public int total;
    public List<ScoreRawEntry> results;
}

public class LeaderBoardController : MonoBehaviour
{
    public static LeaderBoardController instance;
    public bool initialized;
    public bool authenticated;
    void Awake()
    {
        instance = this;
        StartCoroutine(EnsureServicesInitialized());
    }

    IEnumerator EnsureServicesInitialized()
    {
        while (UnityServices.State != ServicesInitializationState.Initialized)
        {
            yield return new WaitForSeconds(1);
        }
        initialized = true;
        SetupEvents();
        Authenticate();
    }
    async void Authenticate()
    {
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        authenticated = true;
        MainController.instance.EnableRankBtn();
        await GetPlayerScore();
    }

    public async Task UpdateScore()
    {
        int totalScore = GameManager.instance.GetTotalScore();
        var playerEntry = await LeaderboardsService.Instance.AddPlayerScoreAsync(
            "Total_Score",
            totalScore,
            new AddPlayerScoreOptions
            {
                Metadata = GameManager.instance.GetPlayerProfile()
            }
        );
        // Debug.Log(JsonConvert.SerializeObject(playerEntry));
    }

    public async Task<List<ScoreEntry>> GetTop100Rank()
    {
        try
        {
            var scoresResponse = await LeaderboardsService.Instance.GetScoresAsync("Total_Score", new GetScoresOptions { Limit = 100, IncludeMetadata = true });
            Debug.Log(JsonConvert.SerializeObject(scoresResponse));
            var scoresResponseObject = JsonConvert.DeserializeObject<ScoreResponse>(JsonConvert.SerializeObject(scoresResponse));
            // Corrected to deserialize Metadata properly and added ToList()
            var scoreEntries = scoresResponseObject.results.Select(scoreRawEntry => new ScoreEntry
            {
                PlayerId = scoreRawEntry.PlayerId,
                Score = (int)float.Parse(scoreRawEntry.Score),
                Rank = (int)float.Parse(scoreRawEntry.Rank) + 1,
                Metadata = JsonConvert.DeserializeObject<Dictionary<string, string>>(scoreRawEntry.Metadata)
            }).ToList();

            return scoreEntries;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to retrieve leaderboard or authenticate: {e.Message}");
            return new List<ScoreEntry>(); // Correctly returns an empty list in case of an error
        }
    }

    public async Task<ScoreEntry> GetPlayerScore()
    {
        try
        {
            var scoreResponse = await LeaderboardsService.Instance.GetPlayerScoreAsync("Total_Score", new GetPlayerScoreOptions { IncludeMetadata = true });
            var scoreRawEntry = JsonConvert.DeserializeObject<ScoreRawEntry>(JsonConvert.SerializeObject(scoreResponse));
            return new ScoreEntry
            {
                PlayerId = scoreRawEntry.PlayerId,
                Score = (int)float.Parse(scoreRawEntry.Score),
                Rank = (int)float.Parse(scoreRawEntry.Rank) + 1,
                Metadata = JsonConvert.DeserializeObject<Dictionary<string, string>>(scoreRawEntry.Metadata)
            };
        }
        catch (System.Exception e)
        {
            return new ScoreEntry();
        }
    }

    void SetupEvents()
    {
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");
            Debug.Log($"Access Token: {AuthenticationService.Instance.AccessToken}");
        };

        AuthenticationService.Instance.SignInFailed += (err) =>
        {
            Debug.LogError(err);
        };

        AuthenticationService.Instance.SignedOut += () =>
        {
            Debug.Log("Player signed out.");
        };

        AuthenticationService.Instance.Expired += () =>
        {
            Debug.Log("Player session could not be refreshed and expired.");
        };
    }
}
