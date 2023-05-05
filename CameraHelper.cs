using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraHelper
{
    public Bounds visible;


    public CameraHelper()
    {
        FindScreenLimits();
    }


    // Find the World coordinate space values for the
    // edge of the viewable screen.
    // worldMin is the bottom, left corner coordinate.
    // worldMax is the top, right corner coordinate.
    // Assumes we are in a 2d scene.
    private void FindScreenLimits()
    {
        Camera.main.orthographic = true;
        Vector2 worldMin = Camera.main.ScreenToWorldPoint(Vector2.zero);
        Vector2 worldMax = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));

        Vector3 size = new Vector3(worldMax.x - worldMin.x, worldMax.y - worldMin.y, 0f);
        Vector3 center = new Vector3(worldMin.x + size.x / 2f, worldMin.y + size.y / 2f, 0f);
        visible = new Bounds(center, size);
    }
}

