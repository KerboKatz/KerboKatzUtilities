using UnityEngine;
using UnityEngine.EventSystems;

namespace KerboKatz.UI
{
  public class DragPanel : MonoBehaviour, IDragHandler
  {
    private RectTransform panelRectTransform;
    private ClampToCanvas canvasClamper;

    private void Start()
    {
      panelRectTransform = transform as RectTransform;
      canvasClamper = GetComponent<ClampToCanvas>();
      if (canvasClamper == null)
      {
        canvasClamper = gameObject.AddComponent<ClampToCanvas>();
      }
    }

    public void OnDrag(PointerEventData data)
    {
      var rect = panelRectTransform;
      rect.position = rect.position + (Vector3)data.delta;
      canvasClamper.ClampToWindow();
    }
  }
}