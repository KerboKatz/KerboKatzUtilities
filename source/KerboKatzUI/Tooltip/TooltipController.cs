using UnityEngine;
using UnityEngine.UI;

namespace KerboKatz.UI
{
  public class TooltipController : MonoBehaviour
  {
    public Text textComponent;
    public Vector3 offset = new Vector3(5, 5);
    private RectTransform rect;
    private RectTransform child;
    private bool isEditor;

    private void Awake()
    {
      isEditor = Application.isEditor;
      rect = transform as RectTransform;
      child = (rect.GetChild(0) as RectTransform);
    }

    internal void LateUpdate()
    {
      Position();
    }

    private void Position()
    {
      var width = child.rect.width / 2;
      var height = child.rect.height / 2;
      var x = Input.mousePosition.x + width;
      var y = Input.mousePosition.y - height;

      if ((x + width) > Screen.width)
      {
        x -= width * 2 - offset.x;
      }
      else
      {
        x += offset.x;
      }
      if ((y - height) < 0)
      {
        y += height * 2 - offset.y;
      }
      else
      {
        y += offset.y;
      }

      if (isEditor)
        rect.position = new Vector3(x, y, rect.position.z);
      else
        rect.position = new Vector3(x - Screen.width / 2, y - Screen.height / 2, rect.position.z);
      rect.SetAsLastSibling();
    }
  }
}