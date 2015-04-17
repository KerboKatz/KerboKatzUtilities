using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

    #region uimethods
    public static void createWindow(bool showWindow, int windowID, ref Rect windowRect, GUI.WindowFunction windowFunction, string windowName, GUIStyle windowStyle, bool lockEditorUI = false)
    {
      if (showWindow)
      {
        windowRect = GUILayout.Window(windowID, windowRect, windowFunction, windowName, windowStyle);
        clampToScreen(ref windowRect);
        if (lockEditorUI)
          lockEditor(windowRect, windowID.ToString());
      }
      else
      {
        if (lockEditorUI)
          EditorLogic.fetch.Unlock(windowID.ToString());
      }
    }

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

    public static bool createToggle(string label, bool toggle, GUIStyle style, string tooltip = "", bool disable = false)
    {
      bool disableR = GUI.enabled;
      if (disable)
      {
        GUI.enabled = false;
      }
      var rToggle = GUILayout.Toggle(toggle, new GUIContent(label, tooltip), style);
      GUI.enabled = disableR;
      return rToggle;
    }

    public static void createOptionSwitcher(string optionName, List<string> options, ref int optionSelected, GUIStyle labelStyle, GUIStyle optionStyle, GUIStyle prevButtonStyle, GUIStyle nextButtonStyle, string tooltip = "")
    {
      string prev, next;
      if (prevButtonStyle == nextButtonStyle)
      {
        prev = "◀";
        next = "▶";
      }
      else
      {
        prev = "";
        next = "";
      }
      GUILayout.BeginHorizontal();
      createLabel(optionName, labelStyle, tooltip);
      if (GUILayout.Button(prev, prevButtonStyle))
      {
        optionSelected--;
        if (optionSelected < 0)
        {
          optionSelected = options.Count - 1;
        }
      }
      createLabel(options[optionSelected], optionStyle);
      if (GUILayout.Button(next, nextButtonStyle))
      {
        optionSelected++;
        if (optionSelected >= options.Count)
        {
          optionSelected = 0;
        }
      }
      GUILayout.EndHorizontal();
    }

    private static GUIStyle tooltipStyle;
    private static Rect tooltipRect = new Rect();
    private static string CurrentTooltip;
    public static void updateTooltipAndDrag(GUIStyle style = null)
    {
      if (style == null)
      {
        style = getTooltipStyle();
      }
      GUI.DragWindow();
      if (CurrentTooltip != GUI.tooltip)
      {
        CurrentTooltip = GUI.tooltip;
        var guiContent = new GUIContent(CurrentTooltip);
        var tooltipSize = style.CalcSize(guiContent);
        tooltipRect.x = Input.mousePosition.x + 10;
        tooltipRect.y = Screen.height - Input.mousePosition.y + 10;
        if (tooltipSize.x < 200)
          tooltipRect.width = tooltipSize.x;
        else
          tooltipRect.width = 200;
        tooltipRect.height = style.CalcHeight(guiContent, 200);
        Utilities.clampToScreen(ref tooltipRect);
      }
    }

    public static GUIStyle getTooltipStyle(GUIStyle style = null)
    {
      if (tooltipStyle == null)
        initStyle();
      return tooltipStyle;
    }

    private static void initStyle()
    {
      tooltipStyle                   = new GUIStyle(HighLogic.Skin.label);
      tooltipStyle.fixedWidth        = 0;
      tooltipStyle.padding.top       = 5;
      tooltipStyle.padding.left      = 5;
      tooltipStyle.padding.right     = 5;
      tooltipStyle.padding.bottom    = 5;
      tooltipStyle.fontSize          = 10;
      tooltipStyle.normal.background = getTexture("tooltipBG", "Textures");
      tooltipStyle.normal.textColor  = Color.white;
      tooltipStyle.border.top        = 1;
      tooltipStyle.border.bottom     = 1;
      tooltipStyle.border.left       = 8;
      tooltipStyle.border.right      = 8;
      tooltipStyle.stretchHeight     = true;
      //return tooltipStyle;
    }

    public static void showTooltip()
    {
      if (!String.IsNullOrEmpty(CurrentTooltip))
      {
        GUI.Label(tooltipRect, CurrentTooltip, tooltipStyle);
        GUI.depth = 0;
      }
    }

    private static GUISkin guiSkin;
    public static Vector2 beginScrollView(Vector2 scrollPosition, float width, float height, bool alwaysShowHorizontal = false, bool alwaysShowVertical = false, GUIStyle styleHorizontal = null, GUIStyle styleVertical = null, GUIStyle scrollView = null)
    {
      if (guiSkin == null)
      {
        guiSkin = GUI.skin;
      }
      if (scrollView == null)
      {
        scrollView = GUI.skin.scrollView;
      }
      if (styleHorizontal == null)
      {
        styleHorizontal = GUI.skin.horizontalScrollbar;
      }
      if (styleVertical == null)
      {
        styleVertical = GUI.skin.verticalScrollbar;
      }
      GUI.skin = HighLogic.Skin;
      scrollPosition = GUILayout.BeginScrollView(scrollPosition, alwaysShowHorizontal, alwaysShowVertical, styleHorizontal, styleVertical, scrollView, GUILayout.Width(width), GUILayout.Height(height));//420
      GUI.skin = guiSkin;
      return scrollPosition;
    }

    #endregion uimethods
    #region timemethods
    public static double getUnixTimestamp()
    {
      return (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local)).TotalSeconds;
    }

    public static DateTime convertUnixTimestampToDate(double unixTimestamp)
    {
      return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Local).AddSeconds(unixTimestamp).ToLocalTime();
      ;
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

    public static string[] getCraftCategories(string file)
    {
      return ConfigNode.Load(file).GetValues("category");
    }

    public static string getPartAndStageString(int count, string p, bool zeroNumber = true)
    {
      if (count > 1)
      {
        return count + " " + p + "s";
      }
      else if (count == 1)
      {
        return "1 " + p;
      }
      else if (count == 0 && zeroNumber)
      {
        return "0 " + p + "s";
      }
      else
      {
        return "No " + p + "s";
      }
    }

    public static void getCraftCostAndStages(ConfigNode nodes, ConfigNode[] partList, out int craftStages, out float craftCost, out bool completeCraft, out string[] craftCategories)
    {
      craftCost = 0;
      craftStages = 0;
      completeCraft = true;
      craftCategories = nodes.GetValues("category");
      foreach (ConfigNode part in partList)
      {
        if (part.HasValue("istg"))
        {
          var partStage = toInt(part.GetValue("istg"));
          if (partStage > craftStages)
            craftStages = partStage;
        }
        float partCost = 0;
        if (getPartCost(part, out partCost))
        {
          craftCost += partCost;
        }
        else
        {
          completeCraft = false;
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
      if (!includeFuel)
        total = dryCost;
      return true;
    }

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

    public static float getScienceValue(Dictionary<string, int> experimentCount, ScienceExperiment experiment, ScienceSubject currentScienceSubject)
    {
      float currentScienceValue;
      if (experimentCount.ContainsKey(currentScienceSubject.id))
      {
        currentScienceValue = ResearchAndDevelopment.GetNextScienceValue(experiment.baseValue * experiment.dataScale, currentScienceSubject) * HighLogic.CurrentGame.Parameters.Career.ScienceGainMultiplier;
        if (experimentCount[currentScienceSubject.id] >= 2)//taken from scienceAlert to get somewhat accurate science values after the second experiment
          currentScienceValue = currentScienceValue / Mathf.Pow(4f, experimentCount[currentScienceSubject.id] - 2);
      }
      else
      {
        currentScienceValue = ResearchAndDevelopment.GetScienceValue(experiment.baseValue * experiment.dataScale, currentScienceSubject) * HighLogic.CurrentGame.Parameters.Career.ScienceGainMultiplier;
      }
      return currentScienceValue;
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