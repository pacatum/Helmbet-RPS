using System;
using System.Globalization;


public static class Utils {

    public static string GetFormatedDecimaNumber( string value ) {
        return Decimal.Parse( value, NumberStyles.Float ).ToString();
    }
}