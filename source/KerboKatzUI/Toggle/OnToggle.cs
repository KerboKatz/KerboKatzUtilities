using UnityEngine;
using UnityEngine.UI;

namespace KerboKatz.UI
{
  public class OnToggle : MonoBehaviour
  {
    public bool inverse = false;
    public Toggle.ToggleEvent onToggle;

    private Toggle toggle;

    private void Awake()
    {
      toggle = GetComponent<Toggle>();
      toggle.onValueChanged.AddListener(OnToggleChange);
      OnToggleChange(toggle.isOn);
    }

    private void OnToggleChange(bool arg0)
    {
      if (inverse)
        arg0 = !arg0;
      onToggle.Invoke(arg0);
    }
  }
}