using System;
using UnityEngine;

public class GestureDetector : MonoBehaviour
{
    public enum SwipeDirection
    {
        Up,
        Down,
        Right,
        Left
    }

    private bool swiping = false;
    private bool eventSent = false;
    private Vector2 lastPosition;
    public PlayerController PlayerController;

    void Update()
    {
        if (Input.touchCount == 0) return;

        if (Math.Abs(Input.GetTouch(0).deltaPosition.sqrMagnitude) > 0.2)
        {
            if (swiping == false)
            {
                swiping = true;
                lastPosition = Input.GetTouch(0).position;
            }
            else
            {
                if (eventSent) return;
                Vector2 direction = Input.GetTouch(0).position - lastPosition;
                if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                {
                    if (direction.x > 0)
                    {
                        PlayerController.YPosition += 1;
                    }
                    else
                    {
                        PlayerController.YPosition -= 1;
                    }
                }
                else
                {
                    if (direction.y > 0)
                    {
                        PlayerController.XPosition += 1;
                    }
                    else
                    {
                        PlayerController.XPosition -= 1;
                    }
                }

                eventSent = true;
            }
        }
        else
        {
            swiping = false;
            eventSent = false;
        }
    }
}