using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Kamgam.UIToolkitScrollViewPro;
public class RankController : MonoBehaviour
{
    VisualElement root;
    void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        InitializeHandler();
    }

    void InitializeHandler()
    {
        root.Q<Button>(className: "back-button").clicked += () =>
        {
            GameManager.instance.SwitchPage("Main");
        };
    }
}
