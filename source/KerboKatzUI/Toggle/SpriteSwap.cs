using UnityEngine;
using UnityEngine.UI;

namespace KerboKatz.UI
{
  public class SpriteSwap : MonoBehaviour
  {
    public Image targetImage;
    public Sprite isOnSprite;
    public Sprite isOffSprite;
    private Toggle toggle;

    private void Awake()
    {
      toggle = GetComponent<Toggle>();
      toggle.onValueChanged.AddListener(OnToggle);
      OnToggle(toggle.isOn);
    }

    private void OnToggle(bool isOn)
    {
      if (isOn)
      {
        targetImage.sprite = isOnSprite;
      }
      else
      {
        targetImage.sprite = isOffSprite;
      }
    }
  }
}