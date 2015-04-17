using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KerboKatz
{
  public class KerboKatzBase : MonoBehaviour
  {
    protected string modName = "KerboKatzBase";
    protected settings currentSettings;
    protected ApplicationLauncherButton button;
    protected Version requiresUtilities;
    public virtual void Awake()
    {
      if (!Utilities.checkUtilitiesSupport(requiresUtilities, modName))
      {
        Destroy(this);
        return;
      }
      GameEvents.onGUIApplicationLauncherReady.Add(OnGuiAppLauncherReady);
    }
    public virtual void Start()
    {
      currentSettings = new settings();
      currentSettings.load(modName, "settings", modName);
    }


    public virtual void OnGuiAppLauncherReady()
    {
      button = ApplicationLauncher.Instance.AddModApplication(
          null, 	//RUIToggleButton.onTrue
          null,	//RUIToggleButton.onFalse
          null, //RUIToggleButton.OnHover
          null, //RUIToggleButton.onHoverOut
          null, //RUIToggleButton.onEnable
          null, //RUIToggleButton.onDisable
          ApplicationLauncher.AppScenes.NEVER, //visibleInScenes
          Utilities.getTexture("icon", "")//texture
      );
    }
    public virtual void OnDestroy()
    {
      if (currentSettings != null)
      {
        currentSettings.save();
      }
      GameEvents.onGUIApplicationLauncherReady.Remove(OnGuiAppLauncherReady);
      if (button != null)
      {
        ApplicationLauncher.Instance.RemoveModApplication(button);
      }
    }
  }
}
