using UnityEngine;
using UnityEngine.EventSystems;

namespace KerboKatz.UI
{
  public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
  {
    public GameObject prefab;
    public string text;
    private Transform canvasTransform;
    private GameObject tooltip;
    private CanvasGroup tooltipGroup;
    public float fadeDuration = 1;
    private Coroutine fadeInCoroutine;

    private void Awake()
    {
      var canvas = transform.root.GetComponent<Canvas>();
      if (canvas == null)
      {
        canvas = transform.GetComponentInParent<Canvas>();
        if (canvas == null)
        {
          Debug.LogError("Tooltip couldn't find canvas!");
          return;
        }
      }
      canvasTransform = canvas.transform;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
      tooltip = Instantiate(prefab);
      tooltip.transform.SetParent(canvasTransform, false);
      tooltipGroup = tooltip.GetComponent<CanvasGroup>();
      var controller = tooltip.GetComponent<TooltipController>();

      var text = controller.textComponent;
      text.text = this.text;
      fadeInCoroutine = StartCoroutine(FadeUI.FadeTo(tooltipGroup, 1, fadeDuration));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
      if (tooltip != null)
      {
        if (fadeInCoroutine != null)
          StopCoroutine(fadeInCoroutine);
        var localTooltip = tooltip;
        StartCoroutine(FadeUI.FadeTo(tooltipGroup, 0, fadeDuration, () => { Destroy(localTooltip); }));
      }
    }
  }
}