using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KerboKatz.Classes;

namespace KerboKatz
{
  public static partial class Utilities
  {
    public static partial class UI
    {
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
      private static float spaceSize;
      public static void updateTooltipAndDrag(bool drag = true)
      {
        updateTooltipAndDrag(null, 200, drag);
      }
      public static void updateTooltipAndDrag()
      {
        updateTooltipAndDrag(true);
      }
      public static void updateTooltipAndDrag(GUIStyle style = null,float tooltipWidth = 200, bool drag = true)
      {
        if (style == null)
        {
          style = getTooltipStyle();
        }
        if (drag)
          GUI.DragWindow();
        if (CurrentTooltip != GUI.tooltip)
        {
          CurrentTooltip = GUI.tooltip;
          var guiContent = new GUIContent(CurrentTooltip);
          var tooltipSize = style.CalcSize(guiContent);
          tooltipRect.x = Input.mousePosition.x + 10;
          tooltipRect.y = Screen.height - Input.mousePosition.y + 10;
          if (tooltipSize.x < tooltipWidth)
            tooltipRect.width = tooltipSize.x;
          else
            tooltipRect.width = tooltipWidth;
          tooltipRect.height = style.CalcHeight(guiContent, tooltipRect.width);
          clampToScreen(ref tooltipRect);
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
        //return tooltipStyle;
      }

      public static void showTooltip(GUIStyle tooltipStyle = null)
      {
        if (!String.IsNullOrEmpty(CurrentTooltip))
        {
          if (tooltipStyle == null)
            tooltipStyle = getTooltipStyle();
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

      public static  string createAlignedOutString(List<alignedTooltip> alignedTooltipList, float maxSize)
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
    }

    #region deprecated
    [Obsolete("Use Utilities.UI instead")]
    public static void createWindow(bool showWindow, int windowID, ref Rect windowRect, GUI.WindowFunction windowFunction, string windowName, GUIStyle windowStyle, bool lockEditorUI = false)
    {
      UI.createWindow(showWindow, windowID, ref windowRect, windowFunction, windowName, windowStyle, lockEditorUI);
    }

    [Obsolete("Use Utilities.UI instead")]
    public static void clampToScreen(ref Rect rect)
    {
      UI.clampToScreen(ref rect);
    }

    [Obsolete("Use Utilities.UI instead")]
    public static void createLabel(string label, GUIStyle style, string tooltip = "")
    {
      UI.createLabel(label, style, tooltip);
    }

    [Obsolete("Use Utilities.UI instead")]
    public static bool createButton(string label, GUIStyle style)
    {
      return UI.createButton(label,style);
    }

    [Obsolete("Use Utilities.UI instead")]
    public static bool createButton(string label, GUIStyle style, string tooltip = "")
    {
      return UI.createButton( label,  style,  tooltip );
    }

    [Obsolete("Use Utilities.UI instead")]
    public static bool createButton(string label, GUIStyle style, string tooltip = "", bool disable = false)
    {
      return UI.createButton(label, style, tooltip, disable);
    }

    [Obsolete("Use Utilities.UI instead")]
    public static bool createButton(string label, GUIStyle style, bool disable = false, string tooltip = "")
    {
      return UI.createButton(label, style, tooltip, disable);
    }

    [Obsolete("Use Utilities.UI instead")]
    public static bool createToggle(string label, bool toggle, GUIStyle style, string tooltip = "", bool disable = false)
    {
      return UI.createToggle( label,  toggle,  style,  tooltip ,  disable);
    }

    [Obsolete("Use Utilities.UI instead")]
    public static void createOptionSwitcher(string optionName, List<string> options, ref int optionSelected, GUIStyle labelStyle, GUIStyle optionStyle, GUIStyle prevButtonStyle, GUIStyle nextButtonStyle, string tooltip = "")
    {
      UI.createOptionSwitcher(optionName, options, ref  optionSelected, labelStyle, optionStyle, prevButtonStyle, nextButtonStyle, tooltip);
    }
    [Obsolete("Use Utilities.UI instead")]
    public static void updateTooltipAndDrag(GUIStyle style = null)
    {
      UI.updateTooltipAndDrag(style);
    }

    [Obsolete("Use Utilities.UI instead")]
    public static GUIStyle getTooltipStyle(GUIStyle style = null)
    {
      return UI.getTooltipStyle();
    }

    [Obsolete("Use Utilities.UI instead")]
    public static void showTooltip()
    {
      UI.showTooltip();
    }

    [Obsolete("Use Utilities.UI instead")]
    public static Vector2 beginScrollView(Vector2 scrollPosition, float width, float height, bool alwaysShowHorizontal = false, bool alwaysShowVertical = false, GUIStyle styleHorizontal = null, GUIStyle styleVertical = null, GUIStyle scrollView = null)
    {
      return UI.beginScrollView(scrollPosition, width, height, alwaysShowHorizontal, alwaysShowVertical, styleHorizontal, styleVertical, scrollView);
    }
    #endregion
  }
}