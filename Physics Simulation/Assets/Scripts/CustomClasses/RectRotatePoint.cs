using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectRotatePoint
{
    public GameObject gameObject;
    public Vector2 translate;

    private const float EPSILON = 0.00001f;

    public RectRotatePoint (GameObject gameObject, Vector2 translate)
    {
        this.gameObject = gameObject;
        this.translate = translate;
    }

    public Vector3 getWorldPoint()
    {
        Transform transform = gameObject.GetComponent<Transform>();
        Vector3 position = transform.position;
        position.z = 0;
        return position + Util.RotateAroundOrigin(translate, transform.eulerAngles.z);
    }
}
