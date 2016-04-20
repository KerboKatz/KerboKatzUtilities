using KerboKatz.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace KerboKatz
{

  [Flags]
  public enum LogMode
  {
    Log = 1,
    Warning = 2,
    Error = 4,
    Exception = 8,
  }
  public static partial class Utilities
  {
    private static Version utilitiesVersion;
    public static Version GetUtilitiesVersion()
    {
      if (utilitiesVersion == null)
      {
        utilitiesVersion = new Version(FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion);
        Debug("KerboKatzUtilities", "Version:" + utilitiesVersion);
      }
      return utilitiesVersion;
    }

    public static bool CheckUtilitiesSupport(Version requiresUtilities, string modName)
    {
      if (GetUtilitiesVersion().CompareTo(requiresUtilities) < 0)
      {
        Debug(modName, "Requires KerboKatzUtilities version: " + requiresUtilities);
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
    
    public static void Debug(string modName, LogMode mode, params object[] debugStrings)
    {
      if (debugStrings.Length == 0)
        return;
      var debugStringBuilder = new StringBuilder();
      debugStringBuilder.Append("[");
      debugStringBuilder.Append(modName);
      debugStringBuilder.Append("] ");
      foreach (var debugString in debugStrings)
      {
        debugStringBuilder.Append(debugString.ToString());
      }
      if (mode == LogMode.Log)
      {
        UnityEngine.Debug.Log(debugStringBuilder);
      }
      else if (mode == LogMode.Warning)
      {
        UnityEngine.Debug.LogWarning(debugStringBuilder);
      }
      else if (mode == LogMode.Error)
      {
        UnityEngine.Debug.LogError(debugStringBuilder);
      }
      else if (mode == LogMode.Exception)
      {
        UnityEngine.Debug.LogException(new Exception(debugStringBuilder.ToString()));
      }
    }

    public static void Debug(string mod, params object[] strings)
    {
      Debug(mod, LogMode.Log, strings);
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
    /*public static Texture2D GetTexture(string file, string path, bool asNormal = false)
    {
      return GameDatabase.Instance.GetTexture("KerboKatz/" + path + "/" + file, asNormal);
    }*/

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