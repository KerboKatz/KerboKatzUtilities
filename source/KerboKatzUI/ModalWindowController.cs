using UnityEngine;
using UnityEngine.UI;

namespace KerboKatz.UI
{
  public class ModalWindowController : MonoBehaviour
  {
    public Button confirm;
    public Button deny;
    public Button close;
    public Text title;
    public Text mainText;
    public CanvasGroup canvasGroup;
    public float fadeSpeed = 0.5f;

    private void Awake()
    {
      canvasGroup.alpha = 0;
      if (confirm != null)
        confirm.onClick.AddListener(StartCloseAnimation);
      if (deny != null)
        deny.onClick.AddListener(StartCloseAnimation);
      if (close != null)
        close.onClick.AddListener(StartCloseAnimation);
    }

    private void OnEnable()
    {
      StartCoroutine(FadeUI.FadeTo(canvasGroup, 1, fadeSpeed));
    }

    private void StartCloseAnimation()
    {
      StartCoroutine(FadeUI.FadeTo(canvasGroup, 0, fadeSpeed, DestroyGameObject));
    }

    private void DestroyGameObject()
    {
      Destroy(gameObject);
    }
  }
}