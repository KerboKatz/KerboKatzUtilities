using KerboKatz.Extensions;
using KSP.UI;
using KSP.UI.Screens;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using KerboKatz.Assets;
using System.Text;
using KerboKatz.UI;

namespace KerboKatz
{
  public class KerboKatzBase<T> : MonoBehaviour where T : SettingsBase<T>, new()
  {
    public T settings;
    public string modName { get; set; }
    public string displayName { get; set; }
    public string tooltip { get; set; }
    protected Version requiresUtilities;
    Dictionary<UnityEngine.Object, Coroutine> fadeCoroutines = new Dictionary<UnityEngine.Object, Coroutine>();
    //private List<UIData> uiElements = new List<UIData>();

    protected void LoadSettings(string path, string file)
    {
      if (settings != null)
        DestroyUIElements();
      settings = SettingsBase<T>.LoadSettings(path, file);
      //debug = settings.debug;
    }
    protected virtual void Awake()
    {
      if (!Utilities.CheckUtilitiesSupport(requiresUtilities, modName))
      {
        Destroy(this);
        return;
      }
      OnAwake();
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
    }

    public void Log(params object[] debugStrings)
    {
        Log(LogMode.Log, debugStrings);
    }
    public void Log(LogMode mode, params object[] debugStrings)
    {
      if (settings == null || settings.debug)
      {
        Utilities.Debug(modName, mode, debugStrings);
      }
    }

    public virtual void OnAwake()
    {
    }

    private void sceneLoad(GameScenes data)
    {
      if (this != null)
      {
        this.enabled = true;
      }
      GameEvents.onGameSceneLoadRequested.Remove(sceneLoad);
    }
    protected virtual void OnDestroy()
    {
      BeforeSaveOnDestroy();

      SaveUIPosition();
      DestroyUIElements();

      AfterDestroy();
      //RemoveUiListeners();
    }

    private void DestroyUIElements()
    {
      foreach (var uiWindow in settings.uiElements)
      {
        if (uiWindow.gameObject != null)
        {
          Destroy(uiWindow.gameObject);
        }
      }
    }

    protected virtual void BeforeSaveOnDestroy()
    {
    }

    protected virtual void AfterDestroy()
    {
    }
    #region UI
    protected void LoadUI(string objectName, string bundle = "kerbokatz")
    {
      AssetLoader.Add(bundle, objectName, AssetLoaded);
    }

    private void AssetLoaded(GameObject requestedAsset)
    {
      if (requestedAsset == null || this == null || gameObject == null)
      {
        return;
      }
      UIData uiWindow = GetUIData(requestedAsset.name, true);
      if (uiWindow != null)
      {
        if (uiWindow.gameObject != null)
        {
          DestroyOldUIWindow(uiWindow);
        }
      }
      else
      {
        uiWindow = new UIData();
        uiWindow.name = requestedAsset.name;
        settings.uiElements.Add(uiWindow);
      }
      uiWindow.prefab = requestedAsset;
      InstantiateUIWindow(uiWindow);
      Log("uiWindow", uiWindow.name);
      OnUIElemntInit(uiWindow);

      Log("UI initialized");
    }

    public static void DestroyOldUIWindow(UIData uiWindow)
    {
      UpdateUIData(uiWindow);
      Destroy(uiWindow.gameObject);
    }

    public static void InstantiateUIWindow(UIData uiWindow)
    {
      uiWindow.gameObject = Instantiate(uiWindow.prefab);
      var panelDrager = uiWindow.gameObject.GetComponent<ClampToCanvas>();
      if (panelDrager != null)
        panelDrager.ClampToWindow();

      uiWindow.gameObject.transform.SetParent(UIMasterController.Instance.appCanvas.transform, false);
      uiWindow.canvasGroup = uiWindow.gameObject.GetComponent<CanvasGroup>();

      if (uiWindow.position != Vector3.zero)
      {
        uiWindow.gameObject.transform.localPosition = uiWindow.position;
      }

      uiWindow.gameObject.SetActive(uiWindow.active);
      if (uiWindow.canvasGroup != null)
      {
        if (uiWindow.active)
          uiWindow.canvasGroup.alpha = 1;
        else
          uiWindow.canvasGroup.alpha = 0;
      }
    }

    protected virtual void OnUIElemntInit(UIData uiWindow)
    {
    }

    protected virtual UIData GetUIData(string name, bool showDebug = false)
    {
      foreach (var value in settings.uiElements)
      {
        if (showDebug)
          Debug.Log("Looking for: " + name + ". Found:" + value.name);
        if (value.name == name)
          return value;
      }
      if (showDebug)
        Debug.Log("Looking for: " + name + ". Found: Nothing");
      return null;
    }

    protected void SaveUIPosition()
    {
      if (settings.uiElements != null)
      {
        foreach (var uiData in settings.uiElements)
        {
          UpdateUIData(uiData);
        }
      }
      //settings.uiElements = settings.uiElements;
      settings.Save();
    }

    private static void UpdateUIData(UIData uiData)
    {
      if (uiData.gameObject == null)
        return;
      uiData.position = uiData.gameObject.transform.localPosition;
      uiData.active = uiData.gameObject.activeSelf;
    }
    protected void DeleteChildren(Transform parent)
    {
      for (int i = parent.childCount - 1; i >= 0; i--)
      {
        Destroy(parent.GetChild(i).gameObject);
      }
    }
    protected C GetComponentInChild<C>(Transform content, string childName) where C : Component
    {
      if (content == null)
      {
        Log("Transform is null!");
        return null;
      }
      var child = content.FindChild(childName);
      if (child == null)
      {
        Log(content.name, ": child(", childName, ") is null!");
        return null;
      }
      var component = child.GetComponent<C>();
      if (component == null)
      {
        Log("Couldn't find Component(", typeof(C), ") on child GameObject. Checking its children!");
        component = child.GetComponentInChildren<C>();
        if (component != null)
        {
          Log("Found Component in it's children.");
          return component;
        }
        var comps = child.GetComponentsInChildren<C>(true);
        if (comps.Length > 0)
        {
          Log("This shouldn't have happend. I found no ", typeof(C), " with GetComponentInChildren but found ", comps.Length, " with GetComponentsInChildren!");
          return comps[0];
        }
        Log(child.name, ": Component(", typeof(C), ") is null");
        return null;
      }
      return component;
      //content.FindChild(childName).GetComponentInChildren<Text>();
    }
    protected Text InitTextField(Transform content, string childName, string initStatus)
    {
      var uiObject = GetComponentInChild<Text>(content, childName);
      if (uiObject == null)
      {
        return null;
      }
      uiObject.text = initStatus;
      return uiObject;
    }
    protected Toggle InitToggle(Transform content, string childName, bool initStatus, UnityAction<bool> eventListener = null)
    {
      var uiObject = GetComponentInChild<Toggle>(content, childName);
      if (uiObject == null)
      {
        return null;
      }
      uiObject.isOn = initStatus;
      if (eventListener != null)
        uiObject.onValueChanged.AddListener(eventListener);
      return uiObject;
    }
    protected InputField InitInputField(Transform content, string childName, string initStatus, UnityAction<string> eventListener = null)
    {
      var uiObject = GetComponentInChild<InputField>(content, childName);
      if (uiObject == null)
      {
        return null;
      }
      if (eventListener != null)
        uiObject.onEndEdit.AddListener(eventListener);
      //uiObject.onEndEdit
      uiObject.text = initStatus;
      return uiObject;
    }
    protected Dropdown InitDropdown(Transform content, string childName, UnityAction<int> eventListener = null, List<string> initOptions = null)
    {
      var uiObject = GetComponentInChild<Dropdown>(content, childName);
      if (uiObject == null)
      {
        return null;
      }
      if (eventListener != null)
        uiObject.onValueChanged.AddListener(eventListener);
      uiObject.value = 0;
      if (initOptions != null)
      {
        AddOptionsToDropdown(initOptions, uiObject);
      }
      var stockCanvas = UIMasterController.Instance.appCanvas;
      //fix dropdown sorting layer
      GetComponentInChild<Canvas>(uiObject.transform, "Template").sortingLayerID = UIMasterController.Instance.appCanvas.sortingLayerID;
      //uiObject.transform.FindChild("Template").GetComponent<Canvas>().sortingLayerID = stockCanvas.sortingLayerID;

      return uiObject;
    }
    protected Slider InitSlider(Transform content, string childName, float initStatus, UnityAction<float> eventListener = null)
    {
      var uiObject = GetComponentInChild<Slider>(content, childName);
      if (uiObject == null)
      {
        return null;
      }
      if (eventListener != null)
        uiObject.onValueChanged.AddListener(eventListener);
      uiObject.value = initStatus;
      return uiObject;
    }
    protected Button InitButton(Transform content, string childName, UnityAction eventListener = null)
    {
      var uiObject = GetComponentInChild<Button>(content, childName);
      if (uiObject == null)
      {
        return null;
      }
      if (eventListener != null)
        uiObject.onClick.AddListener(eventListener);
      return uiObject;
    }
    protected Image InitImage(Transform content, string childName, Texture2D spriteImage)
    {
      if (spriteImage == null)
      {
        Log("Texture2D is null!");
        return null;
      }
      return InitImage(content, childName, Sprite.Create(spriteImage, new Rect(Vector2.zero, new Vector2(spriteImage.width, spriteImage.height)), new Vector2(0.5f, 0.5f)));
    }
    protected Image InitImage(Transform content, string childName, Sprite spriteImage)
    {
      if (spriteImage == null)
      {
        Log("SpriteImage is null!");
        return null;
      }
      var uiObject = GetComponentInChild<Image>(content, childName);
      if (uiObject == null)
      {
        return null;
      }
      uiObject.sprite = spriteImage;
      return uiObject;
    }

    public static void AddOptionsToDropdown(List<string> initOptions, Dropdown uiObject)
    {
      foreach (var option in initOptions)
      {
        AddOptionToDropdown(uiObject, option);
      }
    }

    public static void AddOptionToDropdown(Dropdown uiObject, string option)
    {
      uiObject.options.Add(new Dropdown.OptionData(option));
    }
    public void FadeCanvasGroup(CanvasGroup fadeCanvasGroup, float alpha, float speed, Action callback = null)
    {
      var newRoutine = StartCoroutine(FadeUI.FadeTo(fadeCanvasGroup, alpha, speed, callback));
      Coroutine coroutine;
      if (fadeCoroutines.TryGetValue(fadeCanvasGroup, out coroutine))
      {
        if (coroutine != null)
          StopCoroutine(coroutine);
        fadeCoroutines[fadeCanvasGroup] = newRoutine;
      }
      else
      {
        fadeCoroutines.Add(fadeCanvasGroup, newRoutine);
      }
    }
    public void FadeGraphic(Graphic uiObject, bool status)
    {
      Coroutine coroutine, newCoroutine;
      if (status)
      {
        uiObject.enabled = true;
        newCoroutine = StartCoroutine(FadeUI.FadeTo(uiObject, 1, settings.uiFadeSpeed, null));
      }
      else
      {
        newCoroutine = StartCoroutine(FadeUI.FadeTo(uiObject, 0, settings.uiFadeSpeed, () =>
        {
          uiObject.enabled = false;
        }));
      };
      if (fadeCoroutines.TryGetValue(uiObject, out coroutine))
      {
        if (coroutine != null)
          StopCoroutine(coroutine);
        fadeCoroutines[uiObject] = newCoroutine;
      }
      else
      {
        fadeCoroutines.Add(uiObject, coroutine);
      }
    }
    #endregion
  }
}