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
    private void InitStyle()
    {
      toolbarWindowStyle = new GUIStyle(HighLogic.Skin.window);
      toolbarWindowStyle.fixedWidth = 200;

      textStyle = new GUIStyle(HighLogic.Skin.label);
      textStyle.fixedWidth = 127;
      textStyle.margin.left = 10;

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
        if (GUILayout.Button(new GUIContent(currentMod.GameObject.displayName, currentMod.icon, currentMod.GameObject.tooltip), buttonStyle))
        {
          currentMod.onClick();
        }
      }
      GUILayout.EndVertical();
      Utilities.UI.updateTooltipAndDrag();
    }
  }
}