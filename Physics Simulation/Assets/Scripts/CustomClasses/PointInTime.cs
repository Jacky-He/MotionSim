using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointInTime
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector2 velocity;
    public float angularVelocity;
    public Vector2 acceleration;

    public PointInTime (Vector3 _position, Quaternion _rotation, Vector2 _velocity, float _angularVelocity, Vector2 _acceleration)
    {
        this.position = _position;
        this.rotation = _rotation;
        this.velocity = _velocity;
        this.angularVelocity = _angularVelocity;
        this.acceleration = _acceleration;
    }

    public float getOption (GraphOptions option)
    {
        switch (option)
        {
            case GraphOptions.positionX:
                return this.position.x;
            case GraphOptions.positionY:
                return this.position.y;
            case GraphOptions.velocityX:
                return this.velocity.x;
            case GraphOptions.velocityY:
                return this.velocity.y;
            case GraphOptions.accelerationX:
                return this.acceleration.x;
            case GraphOptions.accelerationY:
                return this.acceleration.y;
            default:
                return 0f;
        }
    }

}
