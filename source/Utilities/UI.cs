using KerboKatz.Classes;
using KerboKatz.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace KerboKatz
{
  public static partial class Utilities
  {
    public static partial class UI
    {
      public static void createWindow(bool showWindow, int windowID, ref Rectangle windowRect, GUI.WindowFunction windowFunction, string windowName, GUIStyle windowStyle, bool lockEditorUI = false)
      {
        bool containing = false;
        if (showWindow)
        {
          windowRect.rect = GUILayout.Window(windowID, windowRect.rect, windowFunction, windowName, windowStyle);
          windowRect.performUpdate();
          windowRect.clampToScreen();
          Vector2 mousePos = Input.mousePosition;
          mousePos.y = Screen.height - mousePos.y;
          containing = windowRect.rect.Contains(mousePos);
        }
        if (containing)
        {
          if (!windowRect.isLocking)
          {
            InputLockManager.SetControlLock(ControlTypes.All, windowName + windowID);
            windowRect.isLocking = true;
          }
        }
        else if (windowRect.isLocking)
        {
          InputLockManager.RemoveControlLock(windowName + windowID);
          windowRect.isLocking = false;
        }
      }
      /*
      private static void clampToScreen(ref Rect rect)
      {
        rect.x = Mathf.Clamp(rect.x, 0, Screen.width - rect.width);
        rect.y = Mathf.Clamp(rect.y, 0, Screen.height - rect.height);
      }*/

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

      public static void createOptionSwitcher(string optionName, List<string> options, ref int optionSelected, GUIStyle labelStyle = null, GUIStyle optionStyle = null, GUIStyle prevButtonStyle = null, GUIStyle nextButtonStyle = null, string tooltip = "")
      {
        if (labelStyle == null)
        {
          labelStyle = sortTextStyle;
        }
        if (optionStyle == null)
        {
          optionStyle = sortOptionTextStyle;
        }
        if (prevButtonStyle == null)
        {
          prevButtonStyle = sortButtonStyle;
        }
        if (nextButtonStyle == null)
        {
          nextButtonStyle = sortButtonStyle;
        }
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
      private static Rectangle tooltipRect = new Rectangle();
      private static string CurrentTooltip;
      private static float spaceSize;
      public static GUIStyle sortButtonStyle;
      public static GUIStyle sortTextStyle;
      public static GUIStyle sortOptionTextStyle;
      public static void updateTooltipAndDrag(bool drag = true)
      {
        updateTooltipAndDrag(null, 200, drag);
      }

      public static void updateTooltipAndDrag()
      {
        updateTooltipAndDrag(null, 200, true);
      }

      public static void updateTooltipAndDrag(GUIStyle style = null, float tooltipWidth = 200, bool drag = true)
      {
        if (drag)
          GUI.DragWindow();
        if (CurrentTooltip != GUI.tooltip)
        {
          if (style == null)
          {
            style = getTooltipStyle();
          }
          CurrentTooltip = GUI.tooltip;
          var guiContent = new GUIContent(CurrentTooltip);
          var tooltipSize = style.CalcSize(guiContent);
          if (tooltipSize.x < tooltipWidth)
            tooltipRect.width = tooltipSize.x;
          else
            tooltipRect.width = tooltipWidth;
          tooltipRect.height = style.CalcHeight(guiContent, tooltipRect.width);
          tooltipRect.moveToCursor(new Vector2(10, 10), Rectangle.defaultVector, true);
          tooltipRect.clampToScreen();
        }
      }

      public static GUIStyle getTooltipStyle()
      {
        if (tooltipStyle == null)
          initStyle();
        return tooltipStyle;
      }

      private static void initStyle()
      {
        sortButtonStyle = new GUIStyle(GUI.skin.button);
        sortButtonStyle.fixedWidth = 20;
        sortButtonStyle.fixedHeight = 20;
        sortButtonStyle.margin.top = 0;
        sortButtonStyle.active = sortButtonStyle.hover;

        sortTextStyle = new GUIStyle(HighLogic.Skin.label);
        sortTextStyle.margin.top = 2;
        sortTextStyle.padding.setToZero();
        sortTextStyle.fixedWidth = 50;

        sortOptionTextStyle = new GUIStyle(sortTextStyle);
        sortOptionTextStyle.margin.left = 0;
        sortOptionTextStyle.padding.left = 0;
        sortOptionTextStyle.fixedWidth = 130;
        sortOptionTextStyle.alignment = TextAnchor.MiddleCenter;

        tooltipStyle = new GUIStyle(HighLogic.Skin.label);
        tooltipStyle.fixedWidth = 0;
        tooltipStyle.padding.top = 5;
        tooltipStyle.padding.left = 5;
        tooltipStyle.padding.right = 5;
        tooltipStyle.padding.bottom = 5;
        tooltipStyle.fontSize = 10;
        tooltipStyle.normal.background = getTexture("tooltipBG", "Textures");
        tooltipStyle.normal.textColor = Color.white;
        tooltipStyle.border.top = 1;
        tooltipStyle.border.bottom = 1;
        tooltipStyle.border.left = 8;
        tooltipStyle.border.right = 8;
        tooltipStyle.stretchHeight = true;

        tooltipStyle.padding.left = 0;
        tooltipStyle.padding.right = 0;
        spaceSize = tooltipStyle.CalcSize(new GUIContent("_ _")).x - (tooltipStyle.CalcSize(new GUIContent("_")).x * 2);
        if (float.IsInfinity(spaceSize) || spaceSize <= 0)
        {
          spaceSize = 1;
        }
        tooltipStyle.padding.left = 5;
        tooltipStyle.padding.right = 5;
      }

      public static void showTooltip(GUIStyle tooltipStyle = null)
      {
        if (!String.IsNullOrEmpty(CurrentTooltip))
        {
          if (tooltipStyle == null)
            tooltipStyle = getTooltipStyle();

          GUI.Window(1701999999, tooltipRect.rect, showTooltip, "", GUIStyle.none);
        }
      }

      private static void showTooltip(int id)
      {
        GUI.Label(new Rect(0, 0, tooltipRect.width, tooltipRect.height), CurrentTooltip, tooltipStyle);
        GUI.depth = 0;
      }

      public static Vector2 beginScrollView(Vector2 scrollPosition, float width, float height, bool alwaysShowHorizontal = false, bool alwaysShowVertical = false, GUIStyle styleHorizontal = null, GUIStyle styleVertical = null, GUIStyle scrollView = null)
      {
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
        GUI.skin = null;
        return scrollPosition;
      }

      public static float addToTooltipList(ref List<alignedTooltip> alignedTooltipList, float maxSize, string name, float value)
      {
        getTooltipStyle();
        var valueString = value.ToString("N0");
        var nameSize = tooltipStyle.CalcSize(new GUIContent(name)).x;
        var valueSize = tooltipStyle.CalcSize(new GUIContent(valueString)).x;
        var thisSize = nameSize + valueSize;
        alignedTooltipList.Add(new alignedTooltip(name, valueString, nameSize, valueSize, thisSize));
        if (thisSize > maxSize)
        {
          maxSize = thisSize;
        }
        return maxSize;
      }

      public static string createAlignedOutString(List<alignedTooltip> alignedTooltipList, float maxSize)
      {
        string outString = "";
        foreach (var current in alignedTooltipList)
        {
          string spaces = "  ";
          if (current.totalSize < maxSize)
          {
            var toBeAdded = (maxSize - current.totalSize) / spaceSize;
            for (int i = 0; i < toBeAdded; i++)
            {
              spaces += " ";
            }
          }
          if (outString != "")
            outString += "\n";
          outString += current.name + ":" + spaces + current.value;
        }
        return outString;
      }

      public static float createSlider(string label, float current, float minValue, float maxValue, GUIStyle textStyle = null, GUIStyle numberFieldStyle = null, GUIStyle horizontalSlider = null, GUIStyle horizontalSliderThumb = null, string tooltip = "", float limitValue = 0)
      {
        if (textStyle == null)
          textStyle = HighLogic.Skin.label;
        if (horizontalSlider == null)
          horizontalSlider = HighLogic.Skin.horizontalSlider;
        if (horizontalSliderThumb == null)
          horizontalSliderThumb = HighLogic.Skin.horizontalSliderThumb;
        if (numberFieldStyle == null)
          numberFieldStyle = HighLogic.Skin.textField;

        Utilities.UI.createLabel(label, textStyle, tooltip);
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();
        current = Utilities.round(GUILayout.HorizontalSlider(current, minValue, maxValue, horizontalSlider, horizontalSliderThumb), 0);
        GUILayout.EndVertical();
        current = Utilities.round(Utilities.toFloat(Utilities.getOnlyNumbers(GUILayout.TextField(current.ToString(), numberFieldStyle))), 0);
        if (limitValue == 0)
        {
          if (current > maxValue)
          {
            current = maxValue;
          }
        }
        else
        {
          if (current > limitValue)
          {
            current = limitValue;
          }
        }
        if (current < minValue)
          current = minValue;
        GUILayout.EndHorizontal();
        return current;
      }
    }
  }
}