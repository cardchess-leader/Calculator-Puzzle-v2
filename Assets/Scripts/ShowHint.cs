using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hyperbyte;
using Hyperbyte.Ads;

public class ShowHint : MonoBehaviour
{
    public void ShowHintWithoutAd()
    {
        if (InputManager.Instance.canInput())
        {
            UIFeedback.Instance.PlayButtonPressEffect();
            InGameController.instance.RevealMoreHint();
            gameObject.Deactivate();
        }
    }

    public void ShowHintWithAd()
    {
        if (InputManager.Instance.canInput())
        {
            UIFeedback.Instance.PlayButtonPressEffect();
            AdManager.Instance.ShowRewardedWithTag(InGameController.hintTag);
            gameObject.Deactivate();
        }
    }

    public void OnCloseBtnPressed()
    {
        if (InputManager.Instance.canInput())
        {
            UIFeedback.Instance.PlayButtonPressEffect();
            gameObject.Deactivate();
        }
    }
}
