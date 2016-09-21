using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KerboKatz.UI
{
  public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
  {
    public GameObject prefab;

    [SerializeField]
    private string text;

    public string _text
    {
      get
      {
        return text;
      }
      set
      {
        if (text != value)
        {
          text = value;
          if (textComponent != null)
            textComponent.text = text;
          if (text.IsNullOrWhiteSpace())
            OnPointerExit(null);
          if (onElement && tooltip == null)
            OnPointerEnter(null);
        }
      }
    }

    private bool onElement = false;
    private Transform canvasTransform;
    private GameObject tooltip;
    private CanvasGroup tooltipGroup;
    public float fadeDuration = 1;
    private Coroutine fadeInCoroutine;
    private Text textComponent;

    private void GetCanvas()
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
      if (eventData != null)
        onElement = true;
      if (_text.IsNullOrWhiteSpace())
        return;
      if (canvasTransform == null)
        GetCanvas();
      tooltip = Instantiate(prefab);
      tooltip.transform.SetParent(canvasTransform, false);
      tooltipGroup = tooltip.GetComponent<CanvasGroup>();
      var controller = tooltip.GetComponent<TooltipController>();

      textComponent = controller.textComponent;
      textComponent.text = this._text;
      fadeInCoroutine = StartCoroutine(FadeUI.FadeTo(tooltipGroup, 1, fadeDuration));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
      if (eventData != null)
        onElement = false;
      if (tooltip != null)
      {
        if (fadeInCoroutine != null)
          StopCoroutine(fadeInCoroutine);
        var localTooltip = tooltip;
        StartCoroutine(FadeUI.FadeTo(tooltipGroup, 0, fadeDuration, () => { Destroy(localTooltip); }));
        tooltip = null;
      }
      //textComponent = null;
    }
  }
}