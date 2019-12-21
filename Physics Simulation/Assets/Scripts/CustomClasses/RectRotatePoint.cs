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

    public Vector3 getWorldPoint ()
    {
        Vector3 res = new Vector3();
        Transform transform = gameObject.GetComponent<Transform>();
        Vector3 position = transform.position;
        float angle = transform.rotation.z;
        res.z = 0;
        res.x = position.x + Mathf.Cos(angle * Mathf.Deg2Rad) * translate.x;
        res.y = position.y + Mathf.Sin(angle * Mathf.Deg2Rad) * translate.x;
        res.x = position.x + Mathf.Sin(angle * Mathf.Deg2Rad) * translate.y;
        res.y = position.y - Mathf.Cos(angle * Mathf.Deg2Rad) * translate.y;
        return res;
    }

    public static void setConfig(Transform transform, RectRotatePoint p1, RectRotatePoint p2)
    {
        float heightMultiplier = 1f;
        Vector3 position1 = p1.getWorldPoint();
        Vector3 position2 = p2.getWorldPoint();
        Vector3 displacement = position2 - position1;
        float angle = 0f;
        Vector3 scale = new Vector3();
        if (!(Mathf.Abs (displacement.x) < EPSILON))
        {
            angle = Util.GetAngleFromVectorFloat(new Vector2(displacement.x, displacement.y));
        }
        scale.z = 0f;
        scale.x = transform.localScale.x;
        float distance = Vector3.Distance(position2, position1);
        scale.y = distance / heightMultiplier;
        transform.localScale = scale;
        transform.position = (position1 + position2)/2f;
        transform.localEulerAngles = new Vector3 (0f, 0f, angle-90f);
    }

    public static void setConfig(Transform transform, RectRotatePoint p1)
    {
        transform.position = p1.getWorldPoint();
        transform.rotation = p1.gameObject.GetComponent<Transform>().rotation;
    }
}
