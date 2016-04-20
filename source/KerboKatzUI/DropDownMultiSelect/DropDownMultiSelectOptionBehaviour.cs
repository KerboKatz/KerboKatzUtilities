using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace KerboKatz.UI
{
  [Serializable]
  public class DropDownMultiSelectOptionBehaviour : MonoBehaviour
  {
    public DropDownMultiSelectOption data;
    public InputField field;
    public Toggle toggle;
    public Button renameButton;
    public Button deleteButton;

    public void UpdateWithData()
    {
      field.text = data.name;
      toggle.isOn = data.selected;

      toggle.onValueChanged.AddListener(OnToggle);
      deleteButton.onClick.AddListener(OnDelete);
      renameButton.onClick.AddListener(OnStartRename);
      field.onEndEdit.AddListener(OnEndRename);
      field.targetGraphic.enabled = false;

      if (data.parent.gameObject.activeInHierarchy)
        StartCoroutine(DisableAtEndOfFrame());
    }

    private void OnEnable()
    {
      StartCoroutine(DisableAtEndOfFrame());
    }

    private IEnumerator DisableAtEndOfFrame()
    {
      yield return new WaitForEndOfFrame();
      field.enabled = false;
    }

    private void OnEndRename(string arg0)
    {
      field.enabled = false;
      field.targetGraphic.enabled = false;
      data.parent.RenameOption(data, arg0);
    }

    public void OnToggle(bool arg0)
    {
      if (data.selected != arg0)
      {
        data.selected = arg0;
        data.parent.OnOptionToggled.Invoke(data.name, arg0);
      }
    }

    internal void OnDelete()
    {
      data.parent.DeleteOption(data);
    }

    internal void OnStartRename()
    {
      field.enabled = true;
      field.targetGraphic.enabled = true;
      field.Select();
    }
  }
}