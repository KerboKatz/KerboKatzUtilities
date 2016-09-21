using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace KerboKatz.UI
{
  [KSPAddon(KSPAddon.Startup.EveryScene, false)]
  public class ButtonEventReplacer : MonoBehaviour
  {
    public static ButtonEventReplacer instance;
    private Dictionary<Button, HashSet<ButtonReplaceInfo>> buttonReplaceDic = new Dictionary<Button, HashSet<ButtonReplaceInfo>>();

    private void Awake()
    {
      instance = this;
    }

    private void Start()
    {
      foreach (var infosPair in buttonReplaceDic)
      {
        var removeExisting = false;
        foreach (var info in infosPair.Value)
        {//first we have to figure out if we want to remove the existing listeners
          if (info.removeExisting)
          {
            removeExisting = true;
            break;
          }
        }
        if (removeExisting)
        {
          infosPair.Key.onClick.RemoveAllListeners();
        }
        foreach (var info in infosPair.Value)
        {//now we can add our new listeners to the button
          infosPair.Key.onClick.AddListener(info.action);
        }
      }
    }

    //use this during awake only or else it won't do anything!
    public static void Add(Button button, UnityAction action, bool removeExisting)
    {
      instance.AddInternal(button, action, removeExisting);
    }

    private void AddInternal(Button button, UnityAction action, bool removeExisting)
    {
      HashSet<ButtonReplaceInfo> infos;
      if (!buttonReplaceDic.TryGetValue(button, out infos))
      {
        infos = new HashSet<ButtonReplaceInfo>();
        buttonReplaceDic.Add(button, infos);
      }
      var info = new ButtonReplaceInfo(action, removeExisting);
      infos.Add(info);
    }

    public class ButtonReplaceInfo : IEquatable<ButtonReplaceInfo>, IEqualityComparer<ButtonReplaceInfo>
    {
      public UnityAction action;
      public bool removeExisting;

      public ButtonReplaceInfo(UnityAction action, bool removeExisting)
      {
        this.action = action;
        this.removeExisting = removeExisting;
      }

      public bool Equals(ButtonReplaceInfo other)
      {
        return action == other.action;
      }

      public bool Equals(ButtonReplaceInfo x, ButtonReplaceInfo y)
      {
        return x.Equals(y);
      }

      public int GetHashCode(ButtonReplaceInfo obj)
      {
        return action.GetHashCode();
      }
    }
  }
}