using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace KerboKatz.UI
{
  public class DropDownMultiSelect : MonoBehaviour
  {
    public class DropDownMulitSelectOptionEvent : UnityEvent<DropDownMultiSelectOption> { }

    public class RenameEvent : UnityEvent<string, string> { }

    public class StringEvent : UnityEvent<string> { }

    public class ToggleEvent : UnityEvent<string, bool> { }

    public Dictionary<string, DropDownMultiSelectOption> options = new Dictionary<string, DropDownMultiSelectOption>();
    private HashSet<DropDownMultiSelect> syncList = new HashSet<DropDownMultiSelect>();
    public InputField inputComponent;
    public GameObject scrollViewContent;
    public GameObject template;
    public float fadeSpeed;
    public CanvasGroup canvasGroup;

    public StringEvent OnOptionRemoved = new StringEvent();
    public RenameEvent OnOptionRenamed = new RenameEvent();
    public StringEvent OnOptionAdd = new StringEvent();
    public ToggleEvent OnOptionToggled = new ToggleEvent();
    private Transform parent;
    private Vector3 localPosition;
    private Coroutine lastCoroutine;
    public bool deactivateOnStart = true;

    private void Awake()
    {
      parent = transform.parent;
      localPosition = transform.localPosition;
      inputComponent.onEndEdit.AddListener(AddNewOption);

      OnOptionRemoved.AddListener(OnRemove);
      OnOptionRenamed.AddListener(OnRename);
      OnOptionAdd.AddListener(OnAdd);
      canvasGroup.alpha = 0;
    }

    private void Start()
    {
      if (deactivateOnStart)
      {
        gameObject.SetActive(false);
      }
      template.SetActive(false);
    }

    #region sync

    public void BeginSync(DropDownMultiSelect syncWith)
    {
      foreach (var option in syncWith.options)
      {
        if (!options.ContainsKey(option.Key))
          OnAdd(option.Key);
      }
      if (syncList.Add(syncWith))
      {
        syncWith.OnOptionRemoved.AddListener(OnRemove);
        syncWith.OnOptionRenamed.AddListener(OnRename);
        syncWith.OnOptionAdd.AddListener(OnAdd);
        syncWith.BeginSync(this);
      }
    }

    private void OnAdd(string name)
    {
      var option = new DropDownMultiSelectOption();
      option.name = name;
      options.Add(option.name, option);

      AddOptionToDropDown(option);
    }

    private void OnRename(string oldName, string newName)
    {
      var data = options[oldName];
      options.Remove(oldName);
      options.Add(newName, data);
      data.prevName = oldName;
      data.name = newName;
    }

    private void OnRemove(string name)
    {
      var data = options[name];
      options.Remove(name);
      Destroy(data.gameObject);
    }

    #endregion sync

    public void SetToggle(string name, bool status)
    {
      var option = options[name];
      option.behaviour.toggle.isOn = status;
      option.selected = status;
    }

    internal void RenameOption(DropDownMultiSelectOption data, string arg0)
    {
      if (data.name != arg0)
      {
        OnOptionRenamed.Invoke(data.name, arg0);
      }
    }

    private void AddOptionToDropDown(DropDownMultiSelectOption option)
    {
      option.parent = this;
      option.gameObject = Instantiate(template);
      option.gameObject.SetActive(true);
      option.gameObject.transform.SetParent(scrollViewContent.transform, false);

      option.behaviour = option.gameObject.GetComponent<DropDownMultiSelectOptionBehaviour>();
      option.behaviour.data = option;

      option.behaviour.UpdateWithData();
    }

    public void DeleteOption(DropDownMultiSelectOption option)
    {
      OnOptionRemoved.Invoke(option.name);
    }

    public void AddNewOption(string arg0)
    {
      if (arg0.IsNullOrWhiteSpace())
        return;
      if (options.ContainsKey(arg0))
        return;
      OnOptionAdd.Invoke(arg0);
    }

    public void ToggleDropDown(bool isOn)
    {
      if (lastCoroutine != null)
        StopCoroutine(lastCoroutine);
      //if (gameObject.activeSelf)
      if (!isOn)
      {
        //gameObject.SetActive(false);
        //reset the old parent and position so it always appears in the same location
        lastCoroutine = StartCoroutine(FadeUI.FadeTo(canvasGroup, 0, fadeSpeed/*, () =>
        {
          transform.SetParent(parent);
          transform.localPosition = localPosition;
        }*/));
      }
      else
      {
        UpdateNames();
        gameObject.SetActive(true);
        /*transform.SetParent(activeParent);*/
        lastCoroutine = StartCoroutine(FadeUI.FadeTo(canvasGroup, 1, fadeSpeed));
      }
    }

    private void UpdateNames()
    {
      foreach (var option in options)
      {
        option.Value.behaviour.field.text = option.Value.name;
        option.Value.behaviour.field.textComponent.text = option.Value.name;
      }
    }
  }
}