using UnityEngine;
using UnityEngine.UI;

namespace KerboKatz.UI
{
  public class ContextMenuOption : MonoBehaviour
  {
    public Button button;
    public Text textObj;
    private string _displayName;

    public string displayName
    {
      get
      {
        return _displayName;
      }
      set
      {
        if (_displayName != value)
        {
          _displayName = value;
          textObj.text = displayName;
          name = displayName;
        }
      }
    }
  }
}