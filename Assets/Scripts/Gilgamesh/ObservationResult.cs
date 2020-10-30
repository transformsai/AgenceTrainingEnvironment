using UnityEngine;

public struct ObservationResult
{
    public int Dimensions;
    
    // ReSharper disable InconsistentNaming
    public float x;
    public float y;
    public float z;
    public float w;
    // ReSharper restore InconsistentNaming

    public static implicit operator ObservationResult(bool value) =>
        new ObservationResult()
        {
            Dimensions = 1,
            x = value ? 1f : 0f,
        };

    public static implicit operator ObservationResult(float value) =>
        new ObservationResult()
        {
            Dimensions = 1,
            x = value,
        };

    public static implicit operator ObservationResult(Vector2 value) =>
        new ObservationResult()
        {
            Dimensions = 2,
            x = value.x,
            y = value.y,
        };

    public static implicit operator ObservationResult(Vector3 value) =>
        new ObservationResult()
        {
            Dimensions = 3,
            x = value.x,
            y = value.y,
            z = value.z,
        };

    public static implicit operator ObservationResult(Vector4 value) =>
        new ObservationResult()
        {
            Dimensions = 4,
            x = value.x,
            y = value.y,
            z = value.z,
            w = value.w,
        };

    public static ObservationResult InverseLerp(ObservationResult a, ObservationResult b, ObservationResult value)
    {
        return new ObservationResult()
        {
            Dimensions = value.Dimensions,
            x = Mathf.InverseLerp(a.x, b.x, value.x),
            y = Mathf.InverseLerp(a.y, b.y, value.y),
            z = Mathf.InverseLerp(a.z, b.z, value.z),
            w = Mathf.InverseLerp(a.w, b.w, value.w),
        };
    }
}