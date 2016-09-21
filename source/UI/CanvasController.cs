using KSP.UI;
using System.Collections;
using UnityEngine;

namespace KerboKatz.UI
{
  [KSPAddon(KSPAddon.Startup.Instantly, true)]
  public class CanvasController : MonoBehaviour
  {
    public static CanvasController instance;
    public GameObject canvasObject;
    public Canvas canvas;

    //UIMasterController.Instance.appCanvas
    private void Awake()
    {
      instance = this;
      StartCoroutine(WaitForCanvas());
    }

    private IEnumerator WaitForCanvas()
    {
      while (UIMasterController.Instance == null || UIMasterController.Instance.appCanvas == null)
      {
        yield return null;
      }
      canvasObject = Instantiate(UIMasterController.Instance.appCanvas.gameObject);
      canvasObject.transform.SetParent(UIMasterController.Instance.appCanvas.gameObject.transform.parent);
      canvas = canvasObject.GetComponent<Canvas>();
      canvas.pixelPerfect = false;
    }
  }
}