using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

namespace KerboKatz
{
  public static class Utilities
  {
    private static Regex regexOnlyNumbers = new Regex(@"[^0-9]");
    private static Version utilitiesVersion;
    public static Version getUtilitiesVersion()
    {
      if (utilitiesVersion == null)
        utilitiesVersion = Assembly.GetExecutingAssembly().GetName().Version;
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
      Debug.Log(string.Format("[" + mod + "] " + message, strings));
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
    #region uimethods
    public static void clampToScreen(ref Rect rect)
    {
      rect.x = Mathf.Clamp(rect.x, 0, Screen.width - rect.width);
      rect.y = Mathf.Clamp(rect.y, 0, Screen.height - rect.height);
    }

    public static void createLabel(string label, GUIStyle style, string tooltip = "")
    {
      if (!String.IsNullOrEmpty(tooltip))
        GUILayout.Label(new GUIContent(label, tooltip), style);
      else
        GUILayout.Label(label, style);
    }

    public static bool createButton(string label, GUIStyle style)
    {
      return GUILayout.Button(label, style);
    }

    public static bool createButton(string label, GUIStyle style, string tooltip = "")
    {
      if (!String.IsNullOrEmpty(tooltip))
        return GUILayout.Button(new GUIContent(label, tooltip), style);
      else
        return GUILayout.Button(label, style);
    }

    public static bool createButton(string label, GUIStyle style, string tooltip = "", bool disable = false)
    {
      bool disableR = GUI.enabled;
      if (disable)
      {
        GUI.enabled = false;
      }
      bool button = createButton(label, style, tooltip);
      GUI.enabled = disableR;
      return button;
    }

    public static bool createButton(string label, GUIStyle style, bool disable = false, string tooltip = "")
    {
      return createButton(label, style, tooltip, disable);
    }

    #endregion uimethods
    #region timemethods
    public static double getUnixTimestamp()
    {
      return (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
    }

    public static DateTime convertUnixTimestampToDate(double unixTimestamp)
    {
      return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(unixTimestamp).ToLocalTime();
      ;
    }

    #endregion timemethods
    #region lockEditor https://github.com/CYBUTEK/Engineer/blob/master/Engineer/BuildEngineer.cs CheckEditorLock
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
      else //if (!window.Contains(mousePos))
      {
        EditorLogic.fetch.Unlock(id);
      }
    }

    #endregion lockEditor https://github.com/CYBUTEK/Engineer/blob/master/Engineer/BuildEngineer.cs CheckEditorLock
    public static void getVesselCostAndStages(ConfigNode[] partList, out int vesselStages, out float vesselCost, out bool completeVessel)
    {
      vesselCost = 0;
      vesselStages = 0;
      completeVessel = true;
      foreach (ConfigNode part in partList)
      {
        if (part.HasValue("istg"))
        {
          var partStage = toInt(part.GetValue("istg"));
          if (partStage > vesselStages)
            vesselStages = partStage;
        }
        float partCost = 0;
        if (getPartCost(part, out partCost))
        {
          vesselCost += partCost;
        }
        else
        {
          completeVessel = false;
        }
      }
    }

    private static bool getPartCost(ConfigNode part, out float total, bool includeFuel = true)
    {
      string name = getPartName(part);
      float dryCost, fuelCost;
      total = 0;
      var aP = getAvailablePart(name);
      if (aP == null)
      {
        return false;
      }
      total = ShipConstruction.GetPartCosts(part, getAvailablePart(name), out dryCost, out fuelCost);
      if (includeFuel)
        return true;
      else
        total = dryCost;
      return true;
    }
      /*string name = getPartName(part);
      float dryCost, fuelCost;
      var aP = getAvailablePart(name);
      if (aP == null)
      {
        return;
      }
      float total = ShipConstruction.GetPartCosts(part, getAvailablePart(name), out dryCost, out fuelCost);
      if (includeFuel)
        return total;
      else
        return dryCost;*/

    private static string getPartName(ConfigNode part)
    {
      string name = part.GetValue("part");
      if (name != null)
        return name.Split('_')[0];
      else
        return part.GetValue("name");
    }

    private static Dictionary<string, AvailablePart> availablePartList;
    private static AvailablePart getAvailablePart(string partName)
    {
      if (availablePartList == null)
      {
        availablePartList = new Dictionary<string, AvailablePart>();
        foreach (AvailablePart part in PartLoader.LoadedPartsList)
        {
          if (!availablePartList.ContainsKey(part.name))
            availablePartList.Add(part.name, part);
        }
      }
      if (availablePartList.ContainsKey(partName))
      {
        return availablePartList[partName];
      }
      return null;
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
  }

  #endregion tuple
}