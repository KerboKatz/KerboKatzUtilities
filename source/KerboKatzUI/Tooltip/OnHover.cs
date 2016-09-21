using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace KerboKatz.UI
{
  public class OnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
  {
    public UnityEvent onHover = new UnityEvent();
    public UnityEvent onExit = new UnityEvent();

    public void OnPointerEnter(PointerEventData eventData)
    {
      onHover.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
      onExit.Invoke();
    }
  }
}