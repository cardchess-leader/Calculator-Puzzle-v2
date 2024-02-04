using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestionSO", menuName = "Calculator-Puzzle/QuestionSO", order = 0)]
public class QuestionSO : ScriptableObject
{
    public enum Difficulty
    {
        Easy,
        Medium,
        Hard,
        Expert
    }
    public enum Symbol
    {
        Zero,
        ZeroZero,
        One,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Plus,
        Minus,
        Multiply,
        Divide,
        Root,
        Percent,
        Equal,
        // additionals hereafter //
        Log,
        Pow,
        Factorial,
        Inverse,
        Round
    }
    public Difficulty difficulty;
    public float initialNum;
    public float goalNum;
    public List<Symbol> answer;
    public List<Symbol> lockedSymbols;
    public List<Symbol> swapFromSymbols;
    public List<Symbol> swapToSymbols;
    public bool noLimit;
    public List<Symbol> hint1;
    public List<Symbol> hint2;
    public List<Symbol> hint3;
    public List<Symbol> hint4;
    public List<Symbol> hint5;
    public List<Symbol> hint6;
    public List<Symbol> hint7;
    public List<Symbol> hint8;
    public List<Symbol> hint9;
    public int GetMaxInputLimit()
    {
        return answer.Where(symbol => symbol != Symbol.Equal).ToList().Count;
    }
    public List<Symbol> GetHintAtLevel(int hintLevel) // hint level starts from 1
    {
        switch (hintLevel)
        {
            case 1: return (hint1?.Count ?? 0) > 0 ? hint1 : null;
            case 2: return (hint2?.Count ?? 0) > 0 ? hint2 : null;
            case 3: return (hint3?.Count ?? 0) > 0 ? hint3 : null;
            case 4: return (hint4?.Count ?? 0) > 0 ? hint4 : null;
            case 5: return (hint5?.Count ?? 0) > 0 ? hint5 : null;
            case 6: return (hint6?.Count ?? 0) > 0 ? hint6 : null;
            case 7: return (hint7?.Count ?? 0) > 0 ? hint7 : null;
            case 8: return (hint8?.Count ?? 0) > 0 ? hint8 : null;
            case 9: return (hint9?.Count ?? 0) > 0 ? hint9 : null;
        }
        return null;
    }
    public int GetMaxHintLevel()
    {
        for (int i = 1; i <= 9; i++)
        {
            if (GetHintAtLevel(i) == null)
            {
                return i - 1;
            }
        }
        return 9;
    }
}