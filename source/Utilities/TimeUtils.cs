using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KerboKatz
{
  public class TimeUtils
  {
    public const int SecondsInMinute = 60;
    public const int SecondsInHour = SecondsInMinute * 60;
    public static int SecondsInDay
    {
      get
      {
        return SecondsInHour * HoursInDay;
      }
    }
    public static int SecondsInYear
    {
      get
      {
        return SecondsInDay * DaysInYear;
      }
    }
    public static int HoursInDay
    {
      get
      {
        return (GameSettings.KERBIN_TIME ? 6 : 24);
      }
    }
    public static int DaysInYear
    {
      get
      {
        return (GameSettings.KERBIN_TIME ? 426 : 365);
      }
    }

    public static int GetYears(double timeStamp)
    {
      return (int)Math.Floor(timeStamp / SecondsInYear);
    }
    public static int GetYears()
    {
      return GetYears(Planetarium.GetUniversalTime()) + 1;
    }

    public static int GetDays(double timeStamp)
    {
      return (int)Math.Floor(timeStamp % SecondsInYear / SecondsInDay);
    }
    public static int GetDays()
    {
      return GetDays(Planetarium.GetUniversalTime()) + 1;
    }

    public static int GetHours(double timeStamp)
    {
      return (int)Math.Floor(timeStamp % SecondsInDay / SecondsInHour);
    }
    public static int GetHours()
    {
      return GetHours(Planetarium.GetUniversalTime());
    }

    public static int GetMinutes(double timeStamp)
    {
      return (int)Math.Floor(timeStamp % SecondsInHour / SecondsInMinute);
    }
    public static int GetMinutes()
    {
      return GetMinutes(Planetarium.GetUniversalTime());
    }

    public static int GetSeconds(double timeStamp)
    {
      return (int)Math.Floor(timeStamp % SecondsInMinute);
    }
    public static int GetSeconds()
    {
      return GetSeconds(Planetarium.GetUniversalTime());
    }
  }
}
