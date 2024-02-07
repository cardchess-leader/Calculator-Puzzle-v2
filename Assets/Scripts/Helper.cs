using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Hyperbyte;

public class Helper
{
    public static List<string> CtryCodeList = new List<string> { "UN", "AF", "AL", "DZ", "AD", "AO", "AG", "AR", "AM", "AU", "AT", "AZ", "BS", "BH", "BD", "BB", "BY", "BE", "BZ", "BJ", "BT", "BO", "BA", "BW", "BR", "BN", "BG", "BF", "BI", "KH", "CM", "CA", "CF", "TD", "CL", "CN", "CO", "KM", "CG", "CR", "HR", "CU", "CY", "CZ", "CD", "DK", "DJ", "DM", "DO", "EC", "EG", "SV", "GQ", "ER", "EE", "SZ", "ET", "FJ", "FI", "FR", "GA", "GM", "GE", "DE", "GH", "GR", "GD", "GT", "GN", "GW", "GY", "HT", "HN", "HK", "HU", "IS", "IN", "ID", "IR", "IQ", "IE", "IL", "IT", "CI", "JM", "JP", "JO", "KZ", "KE", "KI", "KW", "KG", "LA", "LV", "LB", "LS", "LR", "LY", "LI", "LT", "LU", "MC", "MG", "MW", "MY", "MV", "ML", "MT", "MH", "MR", "MU", "MX", "FM", "MD", "MN", "ME", "MA", "MZ", "MM", "NA", "NR", "NP", "NL", "NZ", "NI", "NE", "NG", "KP", "MK", "NO", "OM", "PK", "PW", "PA", "PG", "PY", "PE", "PH", "PL", "PT", "QA", "RO", "RU", "RW", "KN", "LC", "VC", "WS", "SM", "ST", "SA", "SN", "RS", "SC", "SL", "SG", "SK", "SI", "SB", "SO", "ZA", "KR", "SS", "ES", "LK", "SD", "SR", "SE", "CH", "SY", "TW", "TJ", "TZ", "TH", "TL", "TG", "TO", "TT", "TN", "TR", "TM", "TV", "UG", "UA", "AE", "GB", "US", "UY", "UZ", "VU", "VA", "VE", "VN", "YE", "ZM", "ZW" };

    public static void SetHapticToBtn(VisualElement root, string className, bool isPointerDown, AudioClip audioClip = null)
    {
        if (isPointerDown)
        {
            root.RegisterCallback<PointerDownEvent>(evt =>
            {
                var targetElement = evt.target as VisualElement;
                if (targetElement != null && targetElement.ClassListContains(className))
                {
                    if (audioClip != null) // Correct null check
                    {
                        AudioController.Instance.PlayClip(audioClip);
                    }
                    UIFeedback.Instance.PlayHapticLight();
                }
            }, TrickleDown.TrickleDown);
        }
        else
        {
            root.RegisterCallback<ClickEvent>(evt =>
            {
                Debug.Log("12345");
                var targetElement = evt.target as VisualElement;
                if (targetElement != null && targetElement.ClassListContains(className))
                {
                    Debug.Log("6789");
                    if (audioClip != null) // Correct null check
                    {
                        AudioController.Instance.PlayClip(audioClip);
                    }
                    UIFeedback.Instance.PlayHapticLight();
                }
            }, TrickleDown.TrickleDown);
        }
    }

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
    public static int Modulo(int argument, int modulo)
    {
        return (argument % modulo + modulo) % modulo;
    }

    public static float BoundAndMapValue(float value1, float value2, float valueSubject, float mapValue1, float mapValue2, float min, float max)
    {
        float x = (valueSubject - value1) / (value2 - value1);
        float mapValue = mapValue1 * (1 - x) + mapValue2 * (x);
        return Mathf.Min(Mathf.Max(min, mapValue), max);
    }

    public static int GetNthDayOfToday()
    {
        DateTime today = DateTime.Today;
        DateTime firstDayOfYear = new DateTime(today.Year, 1, 1);
        return (today - firstDayOfYear).Days;
    }

    public static string MapCtryCodeToName(string ctryCode)
    {
        switch (ctryCode)
        {
            case "UN": return "Global";
            case "AF": return "Afghanistan";
            case "AL": return "Albania";
            case "DZ": return "Algeria";
            case "AD": return "Andorra";
            case "AO": return "Angola";
            case "AG": return "Antigua and Barbuda";
            case "AR": return "Argentina";
            case "AM": return "Armenia";
            case "AU": return "Australia";
            case "AT": return "Austria";
            case "AZ": return "Azerbaijan";
            case "BS": return "Bahamas";
            case "BH": return "Bahrain";
            case "BD": return "Bangladesh";
            case "BB": return "Barbados";
            case "BY": return "Belarus";
            case "BE": return "Belgium";
            case "BZ": return "Belize";
            case "BJ": return "Benin";
            case "BT": return "Bhutan";
            case "BO": return "Bolivia";
            case "BA": return "Bosnia and Herzegovina";
            case "BW": return "Botswana";
            case "BR": return "Brazil";
            case "BN": return "Brunei";
            case "BG": return "Bulgaria";
            case "BF": return "Burkina Faso";
            case "BI": return "Burundi";
            case "KH": return "Cambodia";
            case "CM": return "Cameroon";
            case "CA": return "Canada";
            case "CF": return "Central African Republic";
            case "TD": return "Chad";
            case "CL": return "Chile";
            case "CN": return "China";
            case "CO": return "Colombia";
            case "KM": return "Comoros";
            case "CG": return "Congo";
            case "CR": return "Costa Rica";
            case "HR": return "Croatia";
            case "CU": return "Cuba";
            case "CY": return "Cyprus";
            case "CZ": return "Czech Republic";
            case "CD": return "Democratic Republic of the Congo";
            case "DK": return "Denmark";
            case "DJ": return "Djibouti";
            case "DM": return "Dominica";
            case "DO": return "Dominican Republic";
            case "EC": return "Ecuador";
            case "EG": return "Egypt";
            case "SV": return "El Salvador";
            case "GQ": return "Equatorial Guinea";
            case "ER": return "Eritrea";
            case "EE": return "Estonia";
            case "SZ": return "Eswatini";
            case "ET": return "Ethiopia";
            case "FJ": return "Fiji";
            case "FI": return "Finland";
            case "FR": return "France";
            case "GA": return "Gabon";
            case "GM": return "Gambia";
            case "GE": return "Georgia";
            case "DE": return "Germany";
            case "GH": return "Ghana";
            case "GR": return "Greece";
            case "GD": return "Grenada";
            case "GT": return "Guatemala";
            case "GN": return "Guinea";
            case "GW": return "Guinea-Bissau";
            case "GY": return "Guyana";
            case "HT": return "Haiti";
            case "HN": return "Honduras";
            case "HK": return "Hong Kong";
            case "HU": return "Hungary";
            case "IS": return "Iceland";
            case "IN": return "India";
            case "ID": return "Indonesia";
            case "IR": return "Iran";
            case "IQ": return "Iraq";
            case "IE": return "Ireland";
            case "IL": return "Israel";
            case "IT": return "Italy";
            case "CI": return "Ivory Coast";
            case "JM": return "Jamaica";
            case "JP": return "Japan";
            case "JO": return "Jordan";
            case "KZ": return "Kazakhstan";
            case "KE": return "Kenya";
            case "KI": return "Kiribati";
            case "KW": return "Kuwait";
            case "KG": return "Kyrgyzstan";
            case "LA": return "Laos";
            case "LV": return "Latvia";
            case "LB": return "Lebanon";
            case "LS": return "Lesotho";
            case "LR": return "Liberia";
            case "LY": return "Libya";
            case "LI": return "Liechtenstein";
            case "LT": return "Lithuania";
            case "LU": return "Luxembourg";
            case "MC": return "Macau";
            case "MG": return "Madagascar";
            case "MW": return "Malawi";
            case "MY": return "Malaysia";
            case "MV": return "Maldives";
            case "ML": return "Mali";
            case "MT": return "Malta";
            case "MH": return "Marshall Islands";
            case "MR": return "Mauritania";
            case "MU": return "Mauritius";
            case "MX": return "Mexico";
            case "FM": return "Micronesia";
            case "MD": return "Moldova";
            case "MN": return "Mongolia";
            case "ME": return "Montenegro";
            case "MA": return "Morocco";
            case "MZ": return "Mozambique";
            case "MM": return "Myanmar";
            case "NA": return "Namibia";
            case "NR": return "Nauru";
            case "NP": return "Nepal";
            case "NL": return "Netherlands";
            case "NZ": return "New Zealand";
            case "NI": return "Nicaragua";
            case "NE": return "Niger";
            case "NG": return "Nigeria";
            case "KP": return "North Korea";
            case "MK": return "North Macedonia";
            case "NO": return "Norway";
            case "OM": return "Oman";
            case "PK": return "Pakistan";
            case "PW": return "Palau";
            case "PA": return "Panama";
            case "PG": return "Papua New Guinea";
            case "PY": return "Paraguay";
            case "PE": return "Peru";
            case "PH": return "Philippines";
            case "PL": return "Poland";
            case "PT": return "Portugal";
            case "QA": return "Qatar";
            case "RO": return "Romania";
            case "RU": return "Russia";
            case "RW": return "Rwanda";
            case "KN": return "Saint Kitts and Nevis";
            case "LC": return "Saint Lucia";
            case "VC": return "Saint Vincent and the Grenadines";
            case "WS": return "Samoa";
            case "SM": return "San Marino";
            case "ST": return "Sao Tome and Principe";
            case "SA": return "Saudi Arabia";
            case "SN": return "Senegal";
            case "RS": return "Serbia";
            case "SC": return "Seychelles";
            case "SL": return "Sierra Leone";
            case "SG": return "Singapore";
            case "SK": return "Slovakia";
            case "SI": return "Slovenia";
            case "SB": return "Solomon Islands";
            case "SO": return "Somalia";
            case "ZA": return "South Africa";
            case "KR": return "South Korea";
            case "SS": return "South Sudan";
            case "ES": return "Spain";
            case "LK": return "Sri Lanka";
            case "SD": return "Sudan";
            case "SR": return "Suriname";
            case "SE": return "Sweden";
            case "CH": return "Switzerland";
            case "SY": return "Syria";
            case "TW": return "Taiwan";
            case "TJ": return "Tajikistan";
            case "TZ": return "Tanzania";
            case "TH": return "Thailand";
            case "TL": return "Timor-Leste";
            case "TG": return "Togo";
            case "TO": return "Tonga";
            case "TT": return "Trinidad and Tobago";
            case "TN": return "Tunisia";
            case "TR": return "Turkey";
            case "TM": return "Turkmenistan";
            case "TV": return "Tuvalu";
            case "UG": return "Uganda";
            case "UA": return "Ukraine";
            case "AE": return "United Arab Emirates";
            case "GB": return "United Kingdom";
            case "US": return "United States of America";
            case "UY": return "Uruguay";
            case "UZ": return "Uzbekistan";
            case "VU": return "Vanuatu";
            case "VA": return "Vatican City";
            case "VE": return "Venezuela";
            case "VN": return "Vietnam";
            case "YE": return "Yemen";
            case "ZM": return "Zambia";
            case "ZW": return "Zimbabwe";
            default: return "Not Found";
        }
    }
}
