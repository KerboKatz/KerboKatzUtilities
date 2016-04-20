using UnityEngine;

namespace KerboKatz.UI
{
  public class ClampToCanvas : MonoBehaviour
  {
    private RectTransform parentRectTransform;
    private RectTransform panelRectTransform;

    private void Start()
    {
      parentRectTransform = transform.parent as RectTransform;
      panelRectTransform = transform as RectTransform;
      ClampToWindow();
    }

    public void ClampToWindow()
    {
      if (parentRectTransform == null)
      {
        //workaround for bug that causes the parent to be null even though its not
        //this is probably caused by OnEnable being called before the actual setup of the ui is done
        parentRectTransform = transform.parent as RectTransform;
        if (parentRectTransform == null)
          return;
      }
      Vector3 pos = panelRectTransform.localPosition;

      Vector3 minPosition = parentRectTransform.rect.min - panelRectTransform.rect.min;
      Vector3 maxPosition = parentRectTransform.rect.max - panelRectTransform.rect.max;

      pos.x = Mathf.Clamp(panelRectTransform.localPosition.x, minPosition.x, maxPosition.x);
      pos.y = Mathf.Clamp(panelRectTransform.localPosition.y, minPosition.y, maxPosition.y);

      panelRectTransform.localPosition = pos;
    }
  }
}