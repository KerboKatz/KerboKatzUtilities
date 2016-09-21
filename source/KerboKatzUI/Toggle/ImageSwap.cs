using UnityEngine;
using UnityEngine.UI;

namespace KerboKatz.UI
{
  public class ImageSwap : MonoBehaviour
  {
    public GameObject isOnObject;
    public GameObject isOffObject;
    private Toggle toggle;

    private void Awake()
    {
      toggle = GetComponent<Toggle>();
      toggle.onValueChanged.AddListener(OnToggle);
      OnToggle(toggle.isOn);
    }

    private void OnToggle(bool isOn)
    {
      isOnObject.SetActive(isOn);
      isOffObject.SetActive(!isOn);
    }
  }
}