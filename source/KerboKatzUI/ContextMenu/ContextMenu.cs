using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace KerboKatz.UI
{
  public class ContextMenu : MonoBehaviour, IPointerClickHandler
  {
    public GameObject prefab;
    public PointerEventData.InputButton mouseButton = PointerEventData.InputButton.Right;
    public List<string> options = new List<string>();
    private Dictionary<string, ContextMenuOption> optionsList = new Dictionary<string, ContextMenuOption>();
    private GameObject template;
    private GameObject prefabCopy;
    private Canvas canvas;
    private bool showContextMenu;
    private bool isDirty;
    private readonly static Vector3 offScreen = new Vector3(100000, 100000);
    //private static GameObject coroutineGameObject = null;
    private void OnEnable()
    {
      CoroutineManager.instance.StartCoroutine(GetReady());
    }
    IEnumerator GetReady()
    {
      while (canvas == null)
      {
        canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
          yield return null;
      }
      if (prefabCopy == null)
      {
        prefabCopy = Instantiate(prefab);
        prefabCopy.transform.SetParent(canvas.transform, false);
      }
      if (template == null)
      {
        template = prefabCopy.transform.Find("Template").gameObject;
        template.SetActive(false);
        foreach (var option in options)
        {
          AddOption(option);
        }
      }
      prefabCopy.SetActive(showContextMenu);
    }

    public void Init()
    {//workaround some situations where we want it to be disabled but still need to have all the prefabs ready.
      OnEnable();
    }

    private void Start()
    {
      StartCoroutine(SkipFrame());
    }

    private IEnumerator SkipFrame()
    {
      prefabCopy.transform.localPosition = offScreen;
      yield return null;
      prefabCopy.SetActive(false);
    }

    public ContextMenuOption AddOption(string option)
    {
      ContextMenuOption value;
      if (optionsList.TryGetValue(option, out value))
      {
        return value;
      }
      var optionGameObj = Instantiate(template);
      optionGameObj.SetActive(true);
      var optionObj = optionGameObj.GetComponent<ContextMenuOption>();
      optionObj.displayName = option;
      optionGameObj.transform.SetParent(prefabCopy.transform, false);
      optionsList.Add(option, optionObj);
      return optionObj;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
      if (eventData.button == mouseButton)
      {
        showContextMenu = !showContextMenu;
        isDirty = true;
      }
    }

    private void LateUpdate()
    {
      if (isDirty)
      {
        UpdateVisibility();
      }
      else
      {
        for (var i = 0; i < 3; i++)
          if (Input.GetMouseButtonUp(i))
          {
            showContextMenu = false;
            isDirty = true;
            break;
          }
      }
    }

    private void UpdateVisibility()
    {
      prefabCopy.SetActive(showContextMenu);
      var rect = (prefabCopy.transform as RectTransform).rect;
      if (showContextMenu)
      {
        var vector2 = PositionRect(rect.width, rect.height, Input.mousePosition, Vector2.zero);
        prefabCopy.transform.localPosition = vector2;
        prefabCopy.transform.SetAsLastSibling();
      }
      if (rect.width != 0)
      {
        isDirty = false;
      }
    }

    internal Vector3 PositionRect(float width, float height, Vector2 position, Vector2 offset)
    {
      width = width / 2;
      height = height / 2;
      position.x = position.x + width;
      position.y = position.y - height;

      if ((position.x + width) > Screen.width)
      {
        position.x -= width * 2 - offset.x;
      }
      else
      {
        position.x += offset.x;
      }
      if ((position.y - height) < 0)
      {
        position.y += height * 2 - offset.y;
      }
      else
      {
        position.y += offset.y;
      }
      return new Vector2(position.x - Screen.width / 2, position.y - Screen.height / 2);
    }
  }
}