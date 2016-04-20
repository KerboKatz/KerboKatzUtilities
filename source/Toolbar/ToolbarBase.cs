using KerboKatz.Assets;
using KerboKatz.Toolbar;
using KSP.UI.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace KerboKatz.Toolbar
{
  [KSPAddon(KSPAddon.Startup.Instantly, true)]
  public partial class ToolbarBase : KerboKatzBase<ToolbarSettings>
  {
    public static ToolbarBase instance;
    private ApplicationLauncherButton button;
    private ApplicationLauncher.AppScenes scences = ApplicationLauncher.AppScenes.NEVER;
    private Sprite icon = new Sprite();

    private HashSet<IToolbar> modules = new HashSet<IToolbar>();
    private List<IToolbar> visibleInThisScene = new List<IToolbar>();
    private bool isUpdateRequired;
    private Transform content;
    private Transform template;
    string toolbarUIName;
    public ToolbarBase()
    {
      modName = "KerboKatzToolbar";
      toolbarUIName = "KerboKatzToolbar";
      Log("Init done!");
    }
    public override void OnAwake()
    {
      instance = this;
      DontDestroyOnLoad(this);
      if (!ApplicationLauncher.Ready)
      {
        GameEvents.onGUIApplicationLauncherReady.Add(OnGuiAppLauncherReady);
      }
      else
      {
        OnGuiAppLauncherReady();
      }
      GameEvents.onGameSceneLoadRequested.Add(OnGameSceneLoadRequested);
      LoadSettings("", "ToolbarSettings");
      LoadUI(toolbarUIName);
    }

    private void OnGameSceneLoadRequested(GameScenes newScene)
    {
      Log("OnGameSceneLoadRequested");
      isUpdateRequired = true;
      LoadUI(toolbarUIName);
    }

    protected override void AfterDestroy()
    {
      GameEvents.onGameSceneLoadRequested.Remove(OnGameSceneLoadRequested);
      GameEvents.onGUIApplicationLauncherReady.Remove(OnGuiAppLauncherReady);
    }

    private void OnGuiAppLauncherReady()
    {
      if (button == null && ApplicationLauncher.Ready)
      {
        Texture2D texture = null;
        if (icon != null)
          texture = icon.texture;
        button = ApplicationLauncher.Instance.AddModApplication(
            OnToolbar, 	//RUIToggleButton.onTrue
            OnToolbar,	//RUIToggleButton.onFalse
            null, //RUIToggleButton.OnHover
            null, //RUIToggleButton.onHoverOut
            null, //RUIToggleButton.onEnable
            null, //RUIToggleButton.onDisable
            scences, //visibleInScenes
            texture//texture
        );
        button.onRightClick = OnToolbar;
        GameEvents.onGUIApplicationLauncherReady.Remove(OnGuiAppLauncherReady);
      }
    }

    #region UI
    protected override void OnUIElemntInit(UIData uiWindow)
    {
      var prefabWindow = uiWindow.gameObject.transform as RectTransform;
      content = prefabWindow.FindChild("Content");
      template = content.FindChild("Template");
      template.SetParent(prefabWindow);
      template.gameObject.SetActive(false);
    }
    #endregion

    private void OnToolbar()
    {
      if (visibleInThisScene.Count == 1)
      {
        visibleInThisScene[0].onClick();
      }
      else
      {
        var canvasGroup = GetUIData(toolbarUIName).canvasGroup;
        settings.showToolbar = !settings.showToolbar;
        if (settings.showToolbar)
        {
          FadeCanvasGroup(canvasGroup, 1, settings.uiFadeSpeed);
        }
        else
        {
          FadeCanvasGroup(canvasGroup, 0, settings.uiFadeSpeed);
        }
        settings.Save();
      }
    }
    public void Add(IToolbar data)
    {
      Log("ToolbarBase: Adding ", data.GetType());
      modules.Add(data);
      isUpdateRequired = true;
    }
    public void Remove(IToolbar data)
    {
      modules.Remove(data);
      isUpdateRequired = true;
    }
    private void Update()
    {
      var mainUIWindow = GetUIData(toolbarUIName);
      if (button == null || mainUIWindow == null || mainUIWindow.gameObject == null)
      {
        return;
      }
      if (isUpdateRequired)
      {
        UpdateVisibleList();
        if (visibleInThisScene.Count > 0)
        {
          if (!UpdateIcon(false))
          {
            SetIcon(AssetLoader.GetAsset<Sprite>("KerboKatzToolbar","Icons"));//Utilities.GetTexture("KerboKatzToolbar", "Textures"));
          }
          else
          {
            FadeCanvasGroup(mainUIWindow.canvasGroup, 0, settings.uiFadeSpeed);
            settings.showToolbar = false;
            //mainUIWindow.gameObject.SetActive(false);
            //mainUIWindow.canvasGroup.alpha = 0;
          }
          button.VisibleInScenes = ApplicationLauncher.AppScenes.ALWAYS;
        }
        else
        {
          button.VisibleInScenes = ApplicationLauncher.AppScenes.NEVER;
        }
        isUpdateRequired = false;
      }
    }
    public static bool UpdateIcon(bool updateAll = true)
    {
      if (instance.visibleInThisScene.Count == 1)
      {
        instance.SetIcon(instance.visibleInThisScene[0].icon);
        return true;
      }
      else if (updateAll)
      {
        instance.UpdateIcons();
      }
      return false;
    }

    private void UpdateIcons()
    {
      var i = 0;
      foreach (var currentMod in modules)
      {
        if (isVisible(currentMod))
        {
          var current = content.GetChild(i);
          var icon = current.FindChild("Image");
          icon.GetComponent<Image>().sprite = currentMod.icon;//Sprite.Create(currentMod.icon, new Rect(Vector2.zero, new Vector2(38, 38)), new Vector2(0.5f, 0.5f));
          i++;
        }
      }
    }

    private void SetIcon(Sprite sprite)
    {
      if (sprite == null)
        return;
      if (button == null)
        icon = sprite;
      else
        button.SetTexture(sprite.texture);
    }

    private void UpdateVisibleList()
    {
      visibleInThisScene.Clear();
      DeleteChildren(content);
      foreach (var currentMod in modules)
      {
        if (isVisible(currentMod))
        {
          var newToolbarOption = Instantiate(template.gameObject);
          newToolbarOption.SetActive(true);
          newToolbarOption.transform.SetParent(content, false);
          newToolbarOption.transform.FindChild("Text").GetComponent<Text>().text = currentMod.displayName;
          InitImage(newToolbarOption.transform, "Image", currentMod.icon);
          //newToolbarOption.transform.FindChild("Image").GetComponent<Image>().sprite = Sprite.Create(currentMod.icon, new Rect(Vector2.zero, new Vector2(38, 38)), new Vector2(0.5f, 0.5f));

          newToolbarOption.GetComponent<Button>().onClick.AddListener(currentMod.onClick);
          visibleInThisScene.Add(currentMod);
        }
      }
    }

    public bool isVisible(IToolbar currentMod)
    {
      if (currentMod.onClick != null && currentMod.activeScences.Contains(HighLogic.LoadedScene))
        return true;
      return false;
    }
  }
}