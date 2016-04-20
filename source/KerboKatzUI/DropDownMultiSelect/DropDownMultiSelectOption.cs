using System;
using System.Collections.Generic;
using UnityEngine;

namespace KerboKatz.UI
{
  [Serializable]
  public class DropDownMultiSelectOption : IEqualityComparer<DropDownMultiSelectOption>
  {
    public bool selected = false;
    public string name = "";
    public GameObject gameObject;
    public DropDownMultiSelect parent;
    public DropDownMultiSelectOptionBehaviour behaviour;
    public string prevName;

    public bool Equals(DropDownMultiSelectOption x, DropDownMultiSelectOption y)
    {
      return (x.name == y.name);
    }

    public int GetHashCode(DropDownMultiSelectOption obj)
    {
      return obj.name.GetHashCode();
    }
  }
}