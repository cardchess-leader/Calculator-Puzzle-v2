using System.Collections;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Leaderboards;
using Newtonsoft.Json;

public class LeaderBoardController : MonoBehaviour
{
    public static LeaderBoardController instance;
    // Flag to prevent multiple initializations.
    // private bool isInitializing = false;
    public bool initialized;
    void Awake()
    {
        instance = this;
        StartCoroutine(EnsureServicesInitialized());
    }

    IEnumerator EnsureServicesInitialized()
    {
        // Prevent entering this coroutine multiple times.
        // if (isInitializing) yield break;
        // isInitializing = true;

        // Wait until UnityServices are initialized.
        while (UnityServices.State != ServicesInitializationState.Initialized)
        {
            yield return new WaitForSeconds(1);
            // Optionally, attempt to initialize UnityServices here if not already done elsewhere.
        }
        initialized = true;
        MainController.instance.EnableRankBtn();
        SetupEvents();
        // isInitializing = false;
        // Proceed with leaderboard retrieval.
        GetLeaderBoard();
    }

    async void GetLeaderBoard()
    {
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            await LeaderboardsService.Instance.AddPlayerScoreAsync("Total_Score", 1);
            var scoresResponse = await LeaderboardsService.Instance.GetScoresAsync("Total_Score");
            Debug.Log(JsonConvert.SerializeObject(scoresResponse));
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to retrieve leaderboard or authenticate: {e.Message}");
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
