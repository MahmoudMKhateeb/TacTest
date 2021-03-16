using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Common;

namespace TACHYON.Dates
{
  public static class DateHeleper
    {
        public static List<KeyValuePair> WeeksDayNamesList()
        {
            List<KeyValuePair> Weeks=new List<KeyValuePair>();
            for (int i = 0; i <= 6; i++)
            {
                Weeks.Add(new KeyValuePair { Value = i, Key = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.DayNames[i] });
            }
            return Weeks;
        }


        public static List<KeyValuePair> MonthNamesList()
        {
            List<KeyValuePair> Months = new List<KeyValuePair>();

            for (int i = 0; i <= 11; i++)
            {
                Months.Add(new KeyValuePair { Key = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.MonthNames[i], Value = i + 1 });
            }
            return Months;
        }
    }





}
