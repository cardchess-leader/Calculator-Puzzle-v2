using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "CalculatorSO", menuName = "Calculator-Puzzle/CalculatorSO", order = 1)]
public class CalculatorSO : ScriptableObject
{
    public enum Status
    {
        Locked,
        Unlocked,
        Active
    }
    public string title;
    public int price;
    public Status GetStatus()
    {
        return (Status)PlayerPrefs.GetInt($"Calc {title}");
    }
}