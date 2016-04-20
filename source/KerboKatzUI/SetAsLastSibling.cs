using UnityEngine;
using UnityEngine.EventSystems;

namespace KerboKatz.UI
{
  public class SetAsLastSibling : MonoBehaviour, IPointerDownHandler
  {
    public void OnPointerDown(PointerEventData eventData)
    {
      transform.SetAsLastSibling();
    }
  }
}