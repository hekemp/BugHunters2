using UnityEngine;

public struct PlayerTransformData
{
    public Vector3 Position;
    public Quaternion Rotation;

    public Vector3 HeadPosition;
    public Vector3 LeftHandPosition;
    public Vector3 RightHandPosition;

    public Quaternion HeadRotation;
    public Quaternion LeftHandRotation;
    public Quaternion RightHandRotation;

    public PlayerTransformData(PlayerTransformData original) : this()
    {
        Position = original.Position;
        Rotation = original.Rotation;

        HeadPosition = original.HeadPosition;
        LeftHandPosition = original.LeftHandPosition;
        RightHandPosition = original.RightHandPosition;

        HeadRotation = original.HeadRotation;
        LeftHandRotation = original.LeftHandRotation;
        RightHandRotation = original.RightHandRotation;
    }
}