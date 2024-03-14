using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hyperbyte;
using Hyperbyte.Ads;
public class PopupManager : MonoBehaviour
{
    public static PopupManager instance;
    public GameObject ShowHintWithAd;
    public GameObject ShowHintWithoutAd;
    public GameObject CalcPurchaseConfirm;
    public GameObject RankProfileSetup;
    // public GameObject NoAdsPurchasePopup;
    public GameObject PurchaseInfo;
    public Text CalcPurchaseConfirmContentText;
    void Awake()
    {
        instance = this;
    }

    public void ShowPopup(string tag)
    {
        switch (tag)
        {
            case "Hint":
                if (ProfileManager.Instance.IsAppAdFree())
                {
                    ShowHintWithoutAd.Activate();
                }
                else
                {
                    ShowHintWithAd.Activate();
                }
                break;
            case "CalcPurchseFailed":
                UIController.Instance.ShowMessage("Purchase Failed", "Not enough Gems!");
                break;
            case "CalcPurchseSuccessful":
                UIController.Instance.ShowMessage("Success", "Purchase Successful!");
                break;
            case "RankProfileSetup":
                RankProfileSetup.Activate();
                break;
            case "PurchaseInfo":
                PurchaseInfo.Activate();
                break;
                // case "NoAdsPurchasePopup":
                //     NoAdsPurchasePopup.Activate();
                //     break;
        }
    }

    public void ShowPurchaseConfirmPopup(string calcTitle, int price)
    {
        CalcPurchaseConfirm.Activate();
        CalcPurchaseConfirmContentText.text = $"Buy the {calcTitle} Calculator for {price} Gems?";
    }

    public void HidePopup(GameObject popup)
    {
        UIFeedback.Instance.PlayButtonPressEffect();
        popup.Deactivate();
    }

    public void OnCalcPurchaseConfirm()
    {
        UIFeedback.Instance.PlayButtonPressEffect();
        CalcPurchaseConfirm.Deactivate();
        ShopController.instance?.AttemptPurchaseCalculator();
    }

    public void OnShowHintWithAd()
    {
        if (AdManager.Instance.IsRewardedAvailable())
        {
            AdManager.Instance.ShowRewardedWithTag(InGameController.hintTag);
            UIFeedback.Instance.PlayButtonPressEffect();
            ShowHintWithAd.Deactivate();
        }
        else
        {
            UIFeedback.Instance.PlayButtonPressEffect();
            ShowHintWithAd.Deactivate();
        }
    }

    public void OnShowHintWithoutAd()
    {
        InGameController.instance?.RevealMoreHint();
        UIFeedback.Instance.PlayButtonPressEffect();
        ShowHintWithoutAd.Deactivate();
    }
}
