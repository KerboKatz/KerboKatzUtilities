using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KerboKatz.UI
{
  public class DoubleClick : MonoBehaviour, IPointerClickHandler
  {
    public PointerEventData.InputButton doubleClickButton = PointerEventData.InputButton.Left;
    public float doubleClickTime = 0.2f;
    public Button.ButtonClickedEvent onDoubleClick = new Button.ButtonClickedEvent();
    private float lastClickTime = 0;

    public void OnPointerClick(PointerEventData eventData)
    {
      if (eventData.button == doubleClickButton)
      {
        if (Time.unscaledTime - lastClickTime < doubleClickTime)
        {
          onDoubleClick.Invoke();
        }
        else
        {
          lastClickTime = Time.unscaledTime;
        }
      }
    }
  }
}