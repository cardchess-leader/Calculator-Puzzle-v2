using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RankProfileController : MonoBehaviour
{
    public TMP_InputField nicknameField;
    public TMP_Dropdown countryDropDown;

    public async void OnRankProfileUpdate()
    {
        string trimmedText = nicknameField.text.Trim();
        string nickname = trimmedText.Length > 15 ? trimmedText.Substring(0, 15) : trimmedText;
        string country = Helper.CtryCodeList[countryDropDown.value];
        // await LeaderBoardController.instance.UpdateProfile(nickname, country);
        GameManager.instance.SetPlayerProfile(nickname, country);
        GameManager.instance.SetRankProfile();
        if (RankController.instance != null && RankController.instance.gameObject.activeSelf)
        {
            RankController.instance.ShowRanks();
        }
    }
}
