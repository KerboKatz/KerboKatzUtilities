using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using KerboKatz.Extensions;

namespace KerboKatz
{
  public static partial class Utilities
  {
    private static Regex regexOnlyNumbers = new Regex(@"[^0-9]");
    private static Version utilitiesVersion;
    public static Version getUtilitiesVersion()
    {
      if (utilitiesVersion == null)
      {
        utilitiesVersion = new Version(FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion);
        debug("KerboKatzUtilities", "Version:" + utilitiesVersion);
      }
      return utilitiesVersion;
    }

    public static bool checkUtilitiesSupport(Version requiresUtilities, string modName)
    {
      if (getUtilitiesVersion().CompareTo(requiresUtilities) < 0)
      {
        debug(modName, "Requires KerboKatzUtilities version: " + requiresUtilities);
        return false;
      }
      return true;
    }

    public static bool canVesselBeControlled(Vessel vessel)
    {
      if (vessel == null)
        return false;
      if (vessel.isEVA)
        return true;
      var commandModules = vessel.FindPartModulesImplementing<ModuleCommand>();
      foreach (var commandModule in commandModules)
      {
        if (commandModule.part.protoModuleCrew.Count >= commandModule.minimumCrew)
          return true;
      }
      //vessel.HasControlSources()
      //var KerbalSeat = vessel.FindPartModulesImplementing<KerbalSeat>();
      if (vessel.FindPartModulesImplementing<KerbalSeat>().Count > 0)
      {
        if (vessel.GetCrewCount() > 0)
          return true;
      } 
      return false;
    }
    [Flags]
    public enum LogMode
    {
      Log = 1,
      Debug = 2,
      Warning = 4,
      Error = 8,
      Exception = 16,
    }
    public static List<string> debugList = new List<string>();
    public static void debug(string mod, LogMode mode, string message, params string[] strings)
    {
      if (strings.Length > 0)
        message = string.Format(message, strings);
      message = "[" + mod + "] " + message;
      if (mode.HasFlag(LogMode.Log))
      {
        UnityEngine.Debug.Log(message);
      }
      else if (mode.HasFlag(LogMode.Debug) && debugList.Contains(mod))
      {
        UnityEngine.Debug.Log(message);
      }
      else if (mode.HasFlag(LogMode.Warning))
      {
        UnityEngine.Debug.LogWarning(message);
      }
      else if (mode.HasFlag(LogMode.Error))
      {
        UnityEngine.Debug.LogError(message);
      }
      else if (mode.HasFlag(LogMode.Exception))
      {
        UnityEngine.Debug.LogException(new Exception(message));
      }
    }
    public static void debug(string mod, string message, params string[] strings)
    {
      if (strings.Length > 0)
        message = string.Format(message, strings);
      UnityEngine.Debug.Log("[" + mod + "] " + message);
    }

    public static string getEditorScene()
    {
      if (EditorLogic.fetch.ship.shipFacility == EditorFacility.SPH)
        return "SPH";
      else if (EditorLogic.fetch.ship.shipFacility == EditorFacility.VAB)
        return "VAB";
      return "null";
    }

    #region numbermethods
    public static float round(float number, float n = 2)
    {
      n = Mathf.Pow(10, n);
      return Mathf.Floor(number * n) / n;
    }

    public static double round(double number, double n = 2)
    {
      n = Math.Pow(10, n);
      return Math.Floor(number * n) / n;
    }

    public static double toDouble(string nString)
    {
      double n = 0;
      double.TryParse(nString, out n);
      return n;
    }

    public static float toFloat(string nString)
    {
      float n = 0;
      float.TryParse(nString, out n);
      return n;
    }

    public static float toFloat(double nDouble)
    {
      float n = 0;
      float.TryParse(nDouble.ToString(), out n);
      return n;
    }

    public static int toInt(string nString)
    {
      int n = 0;
      int.TryParse(nString, out n);
      return n;
    }

    #endregion numbermethods
    #region stringmethods
    public static string getOnlyNumbers(string str)
    {
      str = regexOnlyNumbers.Replace(str, "").TrimStart('0');
      if (string.IsNullOrEmpty(str))
      {
        TextEditor editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
        editor.selectPos = 1;
        editor.pos = 1;
        return "0";
      }
      return str;
    }

    #endregion stringmethods
    #region http://stackoverflow.com/a/444818
    public static bool Contains(this string source, string toCheck, StringComparison comp = StringComparison.OrdinalIgnoreCase)
    {
      return source.IndexOf(toCheck, comp) >= 0;
    }

    #endregion http://stackoverflow.com/a/444818
    #region http://stackoverflow.com/a/6535516
    public static bool IsNullOrWhiteSpace(this string value)
    {
      if (value == null) return true;
      return string.IsNullOrEmpty(value.Trim());
    }

    #endregion http://stackoverflow.com/a/6535516

    #region timemethods
    public static double getUnixTimestamp()
    {
      return (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local)).TotalSeconds;
    }

    public static DateTime convertUnixTimestampToDate(double unixTimestamp)
    {
      return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Local).AddSeconds(unixTimestamp).ToLocalTime();
    }

    #endregion timemethods
    public static Texture2D getTexture(string file, string path, bool asNormal = false)
    {
      return GameDatabase.Instance.GetTexture("KerboKatz/" + path + "/" + file, asNormal);
    }

    public static string[] reverseArray(string[] arrayToReverse)
    {
      Array.Reverse(arrayToReverse);
      return arrayToReverse;
    }

    //http://stackoverflow.com/a/972323
    public static IEnumerable<T> GetValues<T>()
    {
      return Enum.GetValues(typeof(T)).Cast<T>();
    }
  }
}