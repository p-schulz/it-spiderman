using UnityEngine;
using System.Collections;

public static class SuperMath {

    public static Vector3 ClampAngleOnPlane(Vector3 origin, Vector3 direction, float angle, Vector3 planeNormal)
    {
        float a = Vector3.Angle(origin, direction);

        if (a < angle)
            return direction;

        Vector3 r = Vector3.Cross(planeNormal, origin);

        float s = Vector3.Angle(r, direction);
        float rotationAngle = (s < 90 ? 1 : -1) * angle;
        Quaternion rotation = Quaternion.AngleAxis(rotationAngle, planeNormal);

        return rotation * origin;
    }

    /// <summary>
    /// Returns a value contained within a series of bounds approximating a curve
    /// </summary>
    /// <param name="bounds">Series of bounds, implicity enclosed by -Infinity and +Infinity</param>
    /// <param name="values">Series of values one length longer than the bounds, with each value belonging below each bound</param>
    /// <param name="t">Signifies where along the approximated curve the value should fall</param>
    public static float BoundedInterpolation(float[] bounds, float[] values, float t)
    {
        for (int i = 0; i < bounds.Length; i++)
        {
            if (t < bounds[i])
            {
                return values[i];
            }
        }

        return values[values.Length - 1];
    }

    /// <summary>
    /// Checks if point is above the plane
    /// </summary>
    public static bool PointAbovePlane(Vector3 planeNormal, Vector3 planePoint, Vector3 point) {
        Vector3 direction = point - planePoint;
        return Vector3.Angle(direction, planeNormal) < 90;
    }

    /// <summary>
    /// Checks if the duration since start time has elapsed
    /// </summary>
    public static bool Timer(float startTime, float duration)
    {
        return Time.time > startTime + duration;
    }

    /// <summary>
    /// Clamps any angle rotation min/-360 to max/360 degrees
    /// with continuing angle or without
    /// </summary>
    public static float ClampAngle(float angle, float min = -360F, float max = 360F, bool repeat = true)
    {
        if (angle < min)
            angle = (repeat) ? angle += min : angle = min;
        if (angle > max)
            angle = (repeat) ? angle -= max : angle = max;
        return angle;
    }

    public static float CalculateJumpSpeed(float jumpHeight, float gravity)
    {
        return Mathf.Sqrt(2 * jumpHeight * gravity);
    }
}
