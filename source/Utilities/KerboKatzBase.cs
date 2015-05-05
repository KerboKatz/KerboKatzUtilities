using KerboKatz.Extensions;
using System;
using UnityEngine;

namespace KerboKatz
{
  public class KerboKatzBase : MonoBehaviour
  {
    public string modName = "KerboKatzBase";
    public string displayName = null;
    protected settings currentSettings;
    protected ApplicationLauncherButton button;
    protected Version requiresUtilities;
    protected ToolbarClass thisButton;
    private Texture2D icon = new Texture2D(0, 0);
    private ApplicationLauncher.AppScenes scences;
    private bool _useToolbar = true;
    protected int toolbarSelected;
    public string tooltip;

    protected bool useToolbar
    {
      get
      {
        return _useToolbar;
      }
      set
      {
        if (_useToolbar != value)
        {
          _useToolbar = value;
          updateToolbarStatus();
        }
      }
    }

    protected virtual void Awake()
    {
      if (!Utilities.checkUtilitiesSupport(requiresUtilities, modName))
      {
        Destroy(this);
        return;
      }
      updateToolbarStatus();
    }

    protected void updateToolbarStatus()
    {
      if (displayName == null)
        displayName = modName;
      if (thisButton == null)
        thisButton = new ToolbarClass(this, onToolbar, Utilities.getTexture("icon", modName + "/Textures"), GameScenes.MAINMENU);
      removeFromApplicationLauncher();
      removeFromToolbar();
      if (useToolbar)
      {
        Toolbar.add(thisButton);
        if (currentSettings != null)
          currentSettings.set("useToolbar", true);
      }
      else
      {
        if (!ApplicationLauncher.Ready)
        {
          GameEvents.onGUIApplicationLauncherReady.Add(OnGuiAppLauncherReady);
        }
        else
        {
          OnGuiAppLauncherReady();
        }
        if (currentSettings != null)
          currentSettings.set("useToolbar", false);
      }
    }

    protected virtual void onToolbar()
    {
    }

    private void Start()
    {
      if (HighLogic.LoadedScene == GameScenes.LOADING)
      {
        //no need to load up everything during startup
        //it messes with caching textures
        //Destroy(this);
        this.enabled = false;
        //incase the class is starting up once we dont want to kill it
        //or it wont start up again so we add it to the scene change event
        GameEvents.onGameSceneLoadRequested.Add(sceneLoad);
      }
      currentSettings = new settings();
      currentSettings.load(modName, "settings", modName);
      Started();
      if (useToolbar == true)
        loadToolbarSettings();
    }

    private void sceneLoad(GameScenes data)
    {
      if (this != null)
      {
        this.enabled = true;
      }
      GameEvents.onGameSceneLoadRequested.Remove(sceneLoad);
    }

    protected virtual void Started()
    {
    }

    public void setAppLauncherScenes(ApplicationLauncher.AppScenes scences)
    {
      thisButton.activeScences.Clear();
      if (scences.HasFlag(ApplicationLauncher.AppScenes.ALWAYS))
      {
        thisButton.activeScences.AddUnique(GameScenes.SPACECENTER);
        thisButton.activeScences.AddUnique(GameScenes.FLIGHT);
        thisButton.activeScences.AddUnique(GameScenes.TRACKSTATION);
        thisButton.activeScences.AddUnique(GameScenes.EDITOR);
        thisButton.activeScences.AddUnique(GameScenes.TRACKSTATION);
      }
      else
      {
        if (scences.HasFlag(ApplicationLauncher.AppScenes.SPH) || scences.HasFlag(ApplicationLauncher.AppScenes.VAB))
        {
          thisButton.activeScences.AddUnique(GameScenes.EDITOR);
        }
        if (scences.HasFlag(ApplicationLauncher.AppScenes.SPACECENTER))
        {
          thisButton.activeScences.AddUnique(GameScenes.SPACECENTER);
        }
        if (scences.HasFlag(ApplicationLauncher.AppScenes.TRACKSTATION))
        {
          thisButton.activeScences.AddUnique(GameScenes.TRACKSTATION);
        }
        if (scences.HasFlag(ApplicationLauncher.AppScenes.FLIGHT))
        {
          thisButton.activeScences.AddUnique(GameScenes.FLIGHT);
        }
      }
      if (button != null)
      {
        button.VisibleInScenes = scences;
      }
      else
      {
        this.scences = scences;
      }
    }

    public void setIcon(Texture2D icon)
    {
      if (this.icon == icon)
        return;
      if (button != null)
      {
        button.SetTexture(icon);
      }
      if (thisButton != null)
      {
        thisButton.icon = icon;
      }
      this.icon = icon;
    }

    protected virtual void OnGuiAppLauncherReady()
    {
      button = ApplicationLauncher.Instance.AddModApplication(
          onToolbar, 	//RUIToggleButton.onTrue
          onToolbar,	//RUIToggleButton.onFalse
          null, //RUIToggleButton.OnHover
          null, //RUIToggleButton.onHoverOut
          null, //RUIToggleButton.onEnable
          null, //RUIToggleButton.onDisable
          scences, //visibleInScenes
          icon//texture
      );
    }

    protected virtual void OnDestroy()
    {
      beforeSaveOnDestroy();
      if (currentSettings != null)
      {
        currentSettings.save();
      }
      removeFromToolbar();
      removeFromApplicationLauncher();
      afterDestroy();
    }

    protected virtual void beforeSaveOnDestroy()
    {
    }

    protected virtual void afterDestroy()
    {
    }

    protected void loadToolbarSettings()
    {
      currentSettings.setDefault("useToolbar", "true");
      if (currentSettings.isSet("useToolbar"))
      {
        useToolbar = currentSettings.getBool("useToolbar");
        if (useToolbar)
        {
          toolbarSelected = 0;
        }
        else
        {
          toolbarSelected = 1;
        }
      }
    }

    protected void updateToolbarBool()
    {
      if (toolbarSelected == 0 && useToolbar == false)
      {
        useToolbar = true;
      }
      else if (toolbarSelected == 1 && useToolbar == true)
      {
        useToolbar = false;
      }
    }

    protected void removeFromToolbar()
    {
      if (thisButton != null)
      {
        Toolbar.remove(modName);
      }
    }

    protected void removeFromApplicationLauncher()
    {
      if (button != null)
      {
        ApplicationLauncher.Instance.RemoveModApplication(button);
      }
      GameEvents.onGUIApplicationLauncherReady.Remove(OnGuiAppLauncherReady);
    }
  }
}