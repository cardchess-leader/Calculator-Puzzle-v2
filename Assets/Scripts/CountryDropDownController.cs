using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Use the TextMeshPro namespace
using TMPro;

public class CountryDropDownController : MonoBehaviour
{
    // Reference to the TMP_Dropdown instead of the standard Dropdown
    public TMP_Dropdown dropdown;

    // Start is called before the first frame update
    void Start()
    {
        AddDropDownItems();
    }

    void AddDropDownItems()
    {
        foreach (string ctryCode in Helper.CtryCodeList)
        {
            // TMP_Dropdown.OptionData is used in the same way as Dropdown.OptionData
            TMP_Dropdown.OptionData newItem = new TMP_Dropdown.OptionData
            {
                text = Helper.MapCtryCodeToName(ctryCode), // Assuming you have a method to map country codes to names
                image = Resources.Load<Sprite>($"Flags/{ctryCode}") // Load the flag sprite from Resources/Flags folder
            };
            dropdown.options.Add(newItem);
        }
        // Optional: If you want to set a default value or refresh the dropdown after adding items
        dropdown.value = 0;
        dropdown.RefreshShownValue();
    }
}
