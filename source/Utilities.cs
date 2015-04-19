using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

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
      List<ModuleCommand> commandModules = vessel.FindPartModulesImplementing<ModuleCommand>();
      foreach (ModuleCommand commandModule in commandModules)
      {
        if (commandModule.part.protoModuleCrew.Count >= commandModule.minimumCrew)
          return true;
      }
      return false;
    }

    public static void debug(string mod, string message, params string[] strings)//thanks Sephiroth018 for this part
    {
      //#if DEBUG
      if (strings.Length > 0)
        message = string.Format(message, strings);
      UnityEngine.Debug.Log("[" + mod + "] " + message);

      //#endif
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
    /**
     * taken and modified from https://github.com/CYBUTEK/Engineer/blob/master/Engineer/BuildEngineer.cs CheckEditorLock
     */
    public static void lockEditor(Rect window, string id)
    {
      Vector2 mousePos = Input.mousePosition;
      mousePos.y = Screen.height - mousePos.y;
      if (window.Contains(mousePos))
      {
        EditorLogic.fetch.Lock(true, true, true, id);
      }
      else
      {
        EditorLogic.fetch.Unlock(id);
      }
    }

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

  #region tuple
  /**
   * Tuple class added from http://stackoverflow.com/a/7121489 since it doesnt exist in .net 3.5
   */

  public class Tuple<T1>
  {
    public Tuple(T1 item1)
    {
      Item1 = item1;
    }

    public T1 Item1 { get; set; }
  }

  public class Tuple<T1, T2> : Tuple<T1>
  {
    public Tuple(T1 item1, T2 item2)
      : base(item1)
    {
      Item2 = item2;
    }

    public T2 Item2 { get; set; }
  }

  public class Tuple<T1, T2, T3> : Tuple<T1, T2>
  {
    public Tuple(T1 item1, T2 item2, T3 item3)
      : base(item1, item2)
    {
      Item3 = item3;
    }

    public T3 Item3 { get; set; }
  }

  public class Tuple<T1, T2, T3, T4> : Tuple<T1, T2, T3>
  {
    public Tuple(T1 item1, T2 item2, T3 item3, T4 item4)
      : base(item1, item2, item3)
    {
      Item4 = item4;
    }

    public T4 Item4 { get; set; }
  }

  public class Tuple<T1, T2, T3, T4, T5> : Tuple<T1, T2, T3, T4>
  {
    public Tuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5)
      : base(item1, item2, item3, item4)
    {
      Item5 = item5;
    }

    public T5 Item5 { get; set; }
  }

  public class Tuple<T1, T2, T3, T4, T5, T6> : Tuple<T1, T2, T3, T4, T5>
  {
    public Tuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6)
      : base(item1, item2, item3, item4, item5)
    {
      Item6 = item6;
    }

    public T6 Item6 { get; set; }
  }

  public class Tuple<T1, T2, T3, T4, T5, T6, T7> : Tuple<T1, T2, T3, T4, T5, T6>
  {
    public Tuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7)
      : base(item1, item2, item3, item4, item5, item6)
    {
      Item7 = item7;
    }

    public T7 Item7 { get; set; }
  }

  public class Tuple<T1, T2, T3, T4, T5, T6, T7, T8> : Tuple<T1, T2, T3, T4, T5, T6, T7>
  {
    public Tuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, T8 item8)
      : base(item1, item2, item3, item4, item5, item6, item7)
    {
      Item8 = item8;
    }

    public T8 Item8 { get; set; }
  }

  public class Tuple<T1, T2, T3, T4, T5, T6, T7, T8, T9> : Tuple<T1, T2, T3, T4, T5, T6, T7, T8>
  {
    public Tuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, T8 item8, T9 item9)
      : base(item1, item2, item3, item4, item5, item6, item7, item8)
    {
      Item9 = item9;
    }

    public T9 Item9 { get; set; }
  }

  public static class Tuple
  {
    public static Tuple<T1> Create<T1>(T1 item1)
    {
      return new Tuple<T1>(item1);
    }

    public static Tuple<T1, T2> Create<T1, T2>(T1 item1, T2 item2)
    {
      return new Tuple<T1, T2>(item1, item2);
    }

    public static Tuple<T1, T2, T3> Create<T1, T2, T3>(T1 item1, T2 item2, T3 item3)
    {
      return new Tuple<T1, T2, T3>(item1, item2, item3);
    }

    public static Tuple<T1, T2, T3, T4> Create<T1, T2, T3, T4>(T1 item1, T2 item2, T3 item3, T4 item4)
    {
      return new Tuple<T1, T2, T3, T4>(item1, item2, item3, item4);
    }

    public static Tuple<T1, T2, T3, T4, T5> Create<T1, T2, T3, T4, T5>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5)
    {
      return new Tuple<T1, T2, T3, T4, T5>(item1, item2, item3, item4, item5);
    }

    public static Tuple<T1, T2, T3, T4, T5, T6> Create<T1, T2, T3, T4, T5, T6>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6)
    {
      return new Tuple<T1, T2, T3, T4, T5, T6>(item1, item2, item3, item4, item5, item6);
    }

    public static Tuple<T1, T2, T3, T4, T5, T6, T7> Create<T1, T2, T3, T4, T5, T6, T7>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7)
    {
      return new Tuple<T1, T2, T3, T4, T5, T6, T7>(item1, item2, item3, item4, item5, item6, item7);
    }

    public static Tuple<T1, T2, T3, T4, T5, T6, T7, T8> Create<T1, T2, T3, T4, T5, T6, T7, T8>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, T8 item8)
    {
      return new Tuple<T1, T2, T3, T4, T5, T6, T7, T8>(item1, item2, item3, item4, item5, item6, item7, item8);
    }

    public static Tuple<T1, T2, T3, T4, T5, T6, T7, T8, T9> Create<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, T8 item8, T9 item9)
    {
      return new Tuple<T1, T2, T3, T4, T5, T6, T7, T8, T9>(item1, item2, item3, item4, item5, item6, item7, item8, item9);
    }
  }

  #endregion tuple
}