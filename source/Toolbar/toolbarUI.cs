using KerboKatz.Extensions;
using UnityEngine;

namespace KerboKatz
{
  public partial class Toolbar : KerboKatzBase
  {
    private bool initStyle;
    private static int toolbarWindowID = Utilities.UI.getNewWindowID;
    private Rectangle settingsWindowRect = new Rectangle(Rectangle.updateType.Cursor);
    private GUIStyle toolbarWindowStyle;
    private GUIStyle textStyle;
    private GUIStyle buttonStyle;
    private GUIStyle iconStyle;
    private GUIStyle backgroundToolbarItem;
    private void InitStyle()
    {
      iconStyle = new GUIStyle();
      iconStyle.fixedHeight = 24;
      iconStyle.fixedWidth = 24;
      iconStyle.margin.top = 3;
      iconStyle.margin.left = 3;

      backgroundToolbarItem = new GUIStyle(HighLogic.Skin.button);
      backgroundToolbarItem.padding.setToZero();
      backgroundToolbarItem.margin.setToZero();
      backgroundToolbarItem.fixedHeight = 30;
      backgroundToolbarItem.fixedWidth = 190;
      backgroundToolbarItem.margin.left = 5;

      toolbarWindowStyle = new GUIStyle(HighLogic.Skin.window);
      toolbarWindowStyle.padding = toolbarWindowStyle.padding;
      toolbarWindowStyle.fixedWidth = 200;

      textStyle = new GUIStyle(HighLogic.Skin.label);
      textStyle.padding.setToZero();
      textStyle.margin.setToZero();
      textStyle.margin.left = 5;
      textStyle.fixedWidth = 150;
      //textStyle.margin.left = 10;
      textStyle.margin.top = 7;

      buttonStyle = new GUIStyle(HighLogic.Skin.button);
      buttonStyle.wordWrap = true;
      initStyle = true;
    }

    public void OnGUI()
    {
      if (!initStyle)
        InitStyle();
      Utilities.UI.createWindow((currentSettings.getBool("showToolbar") && isAnyActive() && visibleInThisScene.Count > 1), toolbarWindowID, ref settingsWindowRect, toolbarWindow, "KerboKatz Toolbar", toolbarWindowStyle);
      Utilities.UI.showTooltip();
    }

    private void toolbarWindow(int id)
    {

      GUILayout.BeginVertical();
      foreach (var currentMod in references.Values)
      {
        if (!isVisible(currentMod))
          continue;

        GUILayout.BeginHorizontal(backgroundToolbarItem);

        GUILayout.Label(new GUIContent(currentMod.icon), iconStyle);

        Utilities.UI.createLabel(currentMod.GameObject.displayName, textStyle, currentMod.GameObject.tooltip);
        GUILayout.EndHorizontal();

        if (Event.current.type == EventType.MouseUp)
        {
          if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
          {
            currentMod.onClick();
          }
        }
        GUILayout.Space(3);
      }
      //GUILayout.Space(7);
      GUILayout.EndVertical();
      Utilities.UI.updateTooltipAndDrag();
    }
  }
}