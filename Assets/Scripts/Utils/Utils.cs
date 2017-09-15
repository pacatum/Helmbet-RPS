using System;
using System.Globalization;


public static class Utils {

    public static string GetFormatedDecimaNumber( string value ) {
        return Decimal.Parse( value, NumberStyles.Float ).ToString();
    }

    public static string GetFormatedString( string text, int numberOfCut=10 ) {
        return text.Length > 10 ? text.Substring(0, numberOfCut) + "..." : text;
    }
}