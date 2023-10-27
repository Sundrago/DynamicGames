using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace MyUtility
{
    static class Converter {
        const string format = "yyyy/MM/dd HH:mm:ss";
        private static System.IFormatProvider provider;
        
        public static int StringToInt(string value) {
            int number;
            if (int.TryParse(value, out number))
                return number;
            else
                return 0;
        }

        public static string DateTimeToString(DateTime dateTime)
        {
            return dateTime.ToString(format);
        }

        public static DateTime StringToDateTime(string dateTimeString)
        {
            DateTime dateTime;
            bool success = DateTime.TryParseExact(dateTimeString, format, provider, DateTimeStyles.None, out dateTime);

            if (success) return dateTime;
            return DateTime.Now;
        }
    }
}