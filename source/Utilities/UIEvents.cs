using UnityEngine;

namespace KerboKatz
{
  [KSPAddon(KSPAddon.Startup.Instantly, true)]
  public partial class UIEvents : MonoBehaviour
  {
    public static UIEvents _instance;
    public bool hiddenUI;

    public static UIEvents instance
    {
      get
      {
        return _instance;
      }
    }

    private void Start()
    {
      DontDestroyOnLoad(this);
      _instance = this;
      _instance.enabled = false;
      GameEvents.onHideUI.Add(hideUI);
      GameEvents.onShowUI.Add(showUI);
    }

    private void showUI()
    {
      hiddenUI = false;
    }

    private void hideUI()
    {
      hiddenUI = true;
    }
  }
}