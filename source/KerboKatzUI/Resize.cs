using UnityEngine;
using UnityEngine.EventSystems;

namespace KerboKatz.UI
{
  public class Resize : MonoBehaviour, IDragHandler
  {
    public RectTransform resizeObject;
    public bool resizeHeight = false;
    public bool resizeWidth = false;
    private Vector2 initialSize;

    private void OnEnable()
    {
      initialSize = new Vector2(resizeObject.rect.width, resizeObject.rect.height);
    }

    public void OnDrag(PointerEventData data)
    {
      var rect = resizeObject.rect;
      if (resizeWidth)
      {
        var width = rect.width;
        width += data.delta.x;
        width = Mathf.Max(width, initialSize.x);
        resizeObject.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
      }
      if (resizeHeight)
      {
        var height = rect.height;
        height -= data.delta.y;
        height = Mathf.Max(height, initialSize.y);
        resizeObject.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
      }
    }
  }
}