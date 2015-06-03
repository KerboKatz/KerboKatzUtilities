using KerboKatz.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace KerboKatz
{
  [KSPAddon(KSPAddon.Startup.Instantly, true)]
  public partial class ModalWindow : KerboKatzBase
  {
    public static ModalWindow instance;
    private Dictionary<int, ModalWindowClass> ModalWindows = new Dictionary<int, ModalWindowClass>();
    private List<int> reservedWindowIDs = new List<int>();
    private List<int> toBeRemoved = new List<int>();
    private GUIStyle windowStyle;
    private GUIStyle buttonStyle;
    private GUIStyle labelStyle;
    public ModalWindow()
    {
      modName = "ModalWindow";
      requiresUtilities = new Version(1, 2, 0);
      DontDestroyOnLoad(this);
      instance = this;
      initStyle();
    }

    protected override void Started()
    {
      GameEvents.onGameSceneLoadRequested.Add(onSceneSwitch);
      setAppLauncherScenes(ApplicationLauncher.AppScenes.NEVER);
    }

    private void onSceneSwitch(GameScenes data)
    {
      foreach (var modal in ModalWindows.Values)
      {
        if (!modal.persistentOverScenes)
        {
          toBeRemoved.Add(modal.windowID);
        }
      }
    }

    private void initStyle()
    {
      windowStyle = new GUIStyle(HighLogic.Skin.window);
      windowStyle.fixedWidth = 250;
      buttonStyle = new GUIStyle(HighLogic.Skin.button);
      buttonStyle.fixedWidth = 110;
      labelStyle = new GUIStyle(HighLogic.Skin.label);
      labelStyle.fixedWidth = 240;
    }

    public void OnGUI()
    {
      foreach (var modal in ModalWindows.Values)
      {
        Utilities.UI.createWindow(modal.visible, modal.windowID, ref modal.rectangle, showModalWindow, modal.title, windowStyle);
      }
      if (Event.current.type == EventType.Layout && toBeRemoved.Count > 0)
      {
        foreach (var id in toBeRemoved)
        {
          ModalWindows.Remove(id);
        }
        toBeRemoved.Clear();
      }
    }

    private void showModalWindow(int id)
    {
      if (!ModalWindows.ContainsKey(id))
      {
        Utilities.debug(modName, "ModalWindows doesn't contain:" + id);
        return;
      }
      var modal = ModalWindows[id];
      GUILayout.BeginVertical();
      Utilities.UI.createLabel(modal.text, HighLogic.Skin.label);

      GUILayout.BeginHorizontal();
      if (modal.callbackConfirm != null)
      {
        if (Utilities.UI.createButton(modal.confirm, buttonStyle))
        {
          modal.callbackConfirm();
        }
      }
      if (modal.callbackAbort != null)
      {
        if (Utilities.UI.createButton(modal.abort, buttonStyle))
        {
          modal.callbackAbort();
        }
      }
      GUILayout.EndHorizontal();

      GUILayout.EndVertical();
    }

    public void add(ModalWindowClass modal)
    {
      ModalWindows.Add(modal.windowID, modal);
    }

    public void remove(ModalWindowClass modal)
    {
      if (modal == null)
        return;
      if (!modal.visible)
      {
        ModalWindows.Remove(modal.windowID);
        reservedWindowIDs.Add(modal.windowID);
        Utilities.UI.createWindow(false, modal.windowID, ref modal.rectangle, showModalWindow, modal.title, windowStyle);
      }
      else
      {
        modal.visible = false;
        toBeRemoved.Add(modal.windowID);
      }
    }

    public int getID()
    {
      if (reservedWindowIDs.Count == 0)
      {
        return Utilities.UI.getNewWindowID;
      }
      int freeID = 0;
      foreach (var id in reservedWindowIDs)
      {
        freeID = id;
      }
      reservedWindowIDs.Remove(freeID);
      return freeID;
    }
  }

  public class ModalWindowClass
  {
    public string title;
    public string text;
    public string confirm;
    public string abort;
    public bool visible = true;
    public bool persistentOverScenes;
    public Action callbackConfirm;
    public Action callbackAbort;
    public Rectangle rectangle = new Rectangle(Rectangle.updateType.Center);
    public int windowID;
    public ModalWindowClass(string title, string text, string confirm, string abort, Action callbackConfirm, Action callbackAbort, bool persistentOverScenes = false)
    {
      this.title = title;
      this.text = text;
      this.confirm = confirm;
      this.abort = abort;
      this.callbackConfirm = callbackConfirm;
      this.callbackAbort = callbackAbort;
      windowID = ModalWindow.instance.getID();
      this.persistentOverScenes = persistentOverScenes;
    }
  }
}