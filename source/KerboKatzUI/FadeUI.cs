using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace KerboKatz.UI
{
  public static class FadeUI
  {
    public static IEnumerator FadeTo(CanvasGroup fadeCanvasGroup, float alpha, float speed, Action callback = null)
    {
      if (alpha > 0 && Mathf.Approximately(fadeCanvasGroup.alpha, 0))
        fadeCanvasGroup.gameObject.SetActive(true);
      var startTime = Time.realtimeSinceStartup;
      while (!Mathf.Approximately(fadeCanvasGroup.alpha, alpha))
      {
        fadeCanvasGroup.alpha = Mathf.Lerp(fadeCanvasGroup.alpha, alpha, ((Time.realtimeSinceStartup - startTime) / speed));
        yield return null;
      }
      fadeCanvasGroup.alpha = alpha;
      if (Mathf.Approximately(fadeCanvasGroup.alpha, 0))
        fadeCanvasGroup.gameObject.SetActive(false);

      if (callback != null)
        callback();
    }

    public static IEnumerator FadeTo(Graphic image, float alpha, float speed, Action callback)
    {
      var startTime = Time.realtimeSinceStartup;
      while (!Mathf.Approximately(image.color.a, alpha))
      {
        var newAlpha = Mathf.Lerp(image.color.a, alpha, ((Time.realtimeSinceStartup - startTime) / speed)); ;
        SetImageAlpha(image, newAlpha);
        yield return null;
      }
      SetImageAlpha(image, alpha);
      if (callback != null)
        callback();
    }

    private static void SetImageAlpha(Graphic image, float newAlpha)
    {
      var color = image.color;
      color.a = newAlpha;
      image.color = color;
    }
  }
}