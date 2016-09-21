using UnityEngine;
using UnityEngine.EventSystems;

namespace KerboKatz.UI
{
  public class Drag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
  {
    public GameObject DragObject;

    private GameObject privateDragObject;

    public void OnBeginDrag(PointerEventData eventData)
    {
      if (privateDragObject == null)
      {
        privateDragObject = Instantiate(DragObject);
        privateDragObject.transform.SetParent(GetComponentInParent<Canvas>().transform, false);
      }
      else
      {
        privateDragObject.SetActive(true);
      }
    }

    public void OnDrag(PointerEventData eventData)
    {
      var width = Screen.width / 2;
      var height = Screen.height / 2;
      privateDragObject.transform.localPosition = new Vector2(Input.mousePosition.x - width, Input.mousePosition.y - height);
      privateDragObject.transform.SetAsLastSibling();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
      foreach (var droped in eventData.hovered)
      {
        if (droped.gameObject == gameObject)
          continue;
        var dropedOn = droped.GetComponent<Drop>();
        if (dropedOn != null)
        {
          dropedOn.onObjectDroped.Invoke(transform);
          break;
        }
      }
      privateDragObject.SetActive(false);
    }

    private void OnDestroy()
    {
      if (privateDragObject != null)
        Destroy(privateDragObject);
    }
  }
}