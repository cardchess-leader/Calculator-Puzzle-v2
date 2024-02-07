using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Hyperbyte;
using Kamgam.UIToolkitScrollViewPro;

public class ShopController : MonoBehaviour
{
    public static ShopController instance;
    public RectTransform btnPlaceHolder;
    int calcIndex = -1;
    float displacement;
    VisualElement root;
    void Awake()
    {
        instance = this;
    }
    void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        InitializeCalculators();
        InitializeHandler();
        InitUI();
        Helper.SetHapticToBtn(root);
    }
    void Update()
    {
        UpdateScreen();
    }
    void InitUI()
    {
        float aspectRatio = (float)Screen.height / (float)Screen.width;
        float iphoneAspectRatio = 2532 / 1170f;
        float minAspectRatio = 16 / 9f;
        displacement = Helper.BoundAndMapValue(iphoneAspectRatio, minAspectRatio, aspectRatio, 0, 70, 0, 70);
        if (aspectRatio < minAspectRatio)
        {
            root.Q("BarSpace").style.flexBasis = new StyleLength(Helper.BoundAndMapValue(minAspectRatio, 1.6f, aspectRatio, 100, 0, 0, 100));
            root.Q("BottomButtonGroup").transform.scale = new Vector2(0.7f, 0.7f);
            displacement = Helper.BoundAndMapValue(iphoneAspectRatio, 1.6f, aspectRatio, 0, 130, 0, 130);
        }
        root.Q("BottomButtonGroup").style.translate = new Translate(0, new Length(displacement, LengthUnit.Pixel));
    }
    void InitializeHandler()
    {
        root.Q<Button>(className: "back-button").clicked += () =>
        {
            GameManager.instance.SwitchPage("Main");
        };
        root.Q("BottomButtonGroup").RegisterCallback<ClickEvent>(clickEvent =>
        {
            CalculatorSO calc = GameManager.instance.calculatorList[calcIndex];
            if (calc.GetStatus() == CalculatorSO.Status.Locked) // prompt
            {
                PopupManager.instance.ShowPurchaseConfirmPopup(calc.title, calc.price);
            }
            else // equip the calculator
            {
                GameManager.instance.EquipCalculator(calcIndex);
                UpdateScreen(true);
            }
        });
    }
    public void AttemptPurchaseCalculator()
    {
        CalculatorSO calc = GameManager.instance.calculatorList[calcIndex];
        if (CurrencyManager.Instance.DeductGems(calc.price))
        {
            UIController.Instance.PlayDeductGemsAnimation(new Vector3(btnPlaceHolder.position.x, btnPlaceHolder.position.y - displacement, 0), 0.1F);
            PopupManager.instance.ShowPopup("CalcPurchseSuccessful");
            GameManager.instance.EquipCalculator(calcIndex);
            UpdateScreen(true);
        }
        else
        {
            PopupManager.instance.ShowPopup("CalcPurchseFailed");
        }
    }
    void InitializeCalculators()
    {
        foreach (CalculatorSO calc in GameManager.instance.calculatorList)
        {
            try
            {
                VisualElement element = new VisualElement();
                element.AddToClassList("calc-container");
                VisualElement calculator = Resources.Load<VisualTreeAsset>($"Calculator/{calc.title}/Calculator").CloneTree();
                element.Add(calculator);
                root.Q<ScrollViewPro>().Add(element);
            }
            catch (Exception e)
            {

            }
        }
    }

    void UpdateScreen(bool forceUpdate = false)
    {
        try
        {
            ScrollViewPro horizontalScrollView = root.Q<ScrollViewPro>();
            int index = Helper.Modulo((int)Mathf.Round(horizontalScrollView.horizontalScroller.value / horizontalScrollView.resolvedStyle.width), horizontalScrollView.contentContainer.childCount);
            if (!forceUpdate && index == calcIndex)
            {
                return;
            }
            calcIndex = index;
            CalculatorSO calc = GameManager.instance.calculatorList[index];
            root.Q<Label>("CalcName").text = $"{calc.title} + PRICE";
            root.Q<Label>("CalcPrice").text = $"= {calc.price}";
            Button lockedBtn = root.Q("BottomButtonGroup").Q<Button>("Locked"), unlockedBtn = root.Q("BottomButtonGroup").Q<Button>("Unlocked"), activeBtn = root.Q("BottomButtonGroup").Q<Button>("Active");
            lockedBtn.style.visibility = Visibility.Hidden;
            unlockedBtn.style.visibility = Visibility.Hidden;
            activeBtn.style.visibility = Visibility.Hidden;
            switch (calc.GetStatus())
            {
                case CalculatorSO.Status.Locked:
                    lockedBtn.style.visibility = Visibility.Visible;
                    break;
                case CalculatorSO.Status.Unlocked:
                    unlockedBtn.style.visibility = Visibility.Visible;
                    break;
                case CalculatorSO.Status.Active:
                    activeBtn.style.visibility = Visibility.Visible;
                    break;
            }
        }
        catch (Exception e)
        {
            Debug.Log("e is: " + e);
        }
    }
}
