using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointInTime
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 velocity;
    public float angularVelocity;

    public PointInTime (Vector3 _position, Quaternion _rotation, Vector2 _velocity, float _angularVelocity)
    {
        this.position = _position;
        this.rotation = _rotation;
        this.velocity = _velocity;
        this.angularVelocity = _angularVelocity;
    }

    public float getOption (GraphOptions option)
    {
        switch (option)
        {
            case GraphOptions.positionX:
                return this.position.x;
            case GraphOptions.positionY:
                return this.position.y;
            default:
                return 0f;
        }
    }

}
