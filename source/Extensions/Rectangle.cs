using UnityEngine;

namespace KerboKatz.Extensions
{
  public class Rectangle
  {
    public enum updateType
    {
      none,
      Cursor,
      Center
    };

    public static Vector2 defaultVector = new Vector2(1, 1);
    public Rect rect;
    public updateType UpdatePosition;
    public bool isLocking = false;

    public Rectangle()
    {
      init();
    }

    public Rectangle(Rect rect = new Rect(), updateType needsUpdate = updateType.none)
    {
      init(rect, needsUpdate);
    }

    public Rectangle(updateType needsUpdate = updateType.none)
    {
      init(rect, needsUpdate);
    }

    private void init(Rect rect = new Rect(), updateType needsUpdate = updateType.none)
    {
      this.rect = rect;
      this.UpdatePosition = needsUpdate;
    }

    public void performUpdate()
    {
      if (Event.current.type == EventType.Repaint)
      {
        switch (UpdatePosition)
        {
          case updateType.Cursor:
            moveToCursor();
            break;

          case updateType.Center:
            moveToCenter();
            break;
        }
        UpdatePosition = updateType.none;
      }
    }

    public void moveToCursor()
    {
      moveToCursor(defaultVector, defaultVector);
    }

    public void moveToCursor(Vector2 offsetRight, Vector2 offsetLeft, bool ignoreSet = false)
    {
      if (x != 0 && y != 0 && !ignoreSet)
        return;
      x = Input.mousePosition.x;
      y = Screen.height - Input.mousePosition.y;
      if ((x + width) > Screen.width)
      {
        x -= width + offsetLeft.x;
      }
      else
      {
        x += offsetRight.x;
      }
      if ((y + height) > Screen.height)
      {
        y -= height + offsetLeft.y;
      }
      else
      {
        y += offsetRight.y;
      }
    }

    public void moveToCenter()
    {
      if (x != 0 && y != 0)
        return;
      x = Screen.width / 2 - width / 2;
      y = Screen.height / 2 - height / 2;
    }

    public void clampToScreen()
    {
      x = Mathf.Clamp(x, 0, Screen.width - width);
      y = Mathf.Clamp(y, 0, Screen.height - height);
    }

    public float x
    {
      get
      {
        return rect.x;
      }
      set
      {
        rect.x = value;
      }
    }

    public float y
    {
      get
      {
        return rect.y;
      }
      set
      {
        rect.y = value;
      }
    }

    public float height
    {
      get
      {
        return rect.height;
      }
      set
      {
        rect.height = value;
      }
    }

    public float width
    {
      get
      {
        return rect.width;
      }
      set
      {
        rect.width = value;
      }
    }
  }
}