using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Helper
{
    public static string ReplaceCharAt(string str, string replaceChar, int index)
    {
        string res = "";
        for (int i = 0; i < str.Length; i++)
        {
            if (i == index)
            {
                res += replaceChar;
            }
            else
            {
                res += str[i];
            }
        }
        return res;
    }

    // public static void SetVisibility(VisualElement element, bool show)
    // {
    //     element.style.visibility = show ? Visibility.Visible : Visibility.Hidden;
    //     element.pickingMode = show ? PickingMode.Position : PickingMode.Ignore;
    // }

    public static int Modulo(int argument, int modulo)
    {
        return (argument % modulo + modulo) % modulo;
    }
}
