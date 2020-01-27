using System;
using UnityEngine;

public class Force
{
    public static int sdfd;
    public Vector3 normalizedDirection;
    public float magnitude;
    public ForceMode2D mode;

    public Force()
    {
        normalizedDirection = new Vector3(0, 0, 0);
        magnitude = 0;
        mode = ForceMode2D.Force;
    }

    public Force (Vector3 direction, float magnitude, ForceMode2D mode)
    {
        this.normalizedDirection = direction.normalized;
        this.magnitude = magnitude;
        this.mode = mode;
    }
}
