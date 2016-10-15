using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KerboKatz.UI
{
  public class ResizeEvent : UnityEvent<ExpandingInputField, float> { }

  public class ExpandingInputField : UIBehaviour
  {
    public GameObject inputparent;
    public Vector2 paddingLeft;
    public Vector2 paddingRight;
    public InputField inputField;
    public LayoutElement layoutElement;
    public ResizeEvent onResize = new ResizeEvent();
    public ExpandingInputFieldSyncMaster syncWith;

    protected override void Awake()
    {
      if (layoutElement == null)
        layoutElement = inputparent.GetComponent<LayoutElement>();
      if (syncWith != null)
      {
        syncWith.Add(this);
      }
    }

    protected override void OnDestroy()
    {
      if (syncWith != null)
      {
        syncWith.Remove(this);
      }
    }

    protected override void Start()
    {
      inputField.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<string>(ResizeInput));
      ResizeInput(inputField.text);
    }

    private void ResizeInput(string text)
    {
      var fullText = inputField.text;

      Vector2 extents = inputField.textComponent.rectTransform.rect.size;
      var settings = inputField.textComponent.GetGenerationSettings(extents);
      settings.generateOutOfBounds = false;
      var width = new TextGenerator().GetPreferredWidth(fullText, settings) + paddingRight.x + paddingLeft.x;
      if (width > inputField.textComponent.rectTransform.rect.width - paddingRight.x)
      {
        if (syncWith != null)
          onResize.Invoke(this, width);
        else
          SetSizeTo(width);
      }
      else if (width < inputField.textComponent.rectTransform.rect.width + paddingRight.x)
      {
        if (syncWith != null)
          onResize.Invoke(this, width);
        else
          SetSizeTo(width);
      }
    }

    internal void SetSizeTo(float width)
    {
      layoutElement.preferredWidth = width;
      var pos = inputField.transform.localPosition;
      pos.x = paddingLeft.x;
      inputField.transform.localPosition = pos;
      (inputField.transform as RectTransform).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
    }
  }
}