using UnityEngine;
using UnityEditor;

public static class PhysicsTools
{
    public static Vector3 CalcBallisticVelocityVector(Vector3 source, Vector3 target, float angle)
    {
        Vector3 direction = target - source;
        float h = direction.y;
        direction.y = 0;
        float distance = direction.magnitude;
        float a = angle * Mathf.Deg2Rad;
        direction.y = distance * Mathf.Tan(a);
        distance += h / Mathf.Tan(a);

        // calculate velocity
        float velocity = Mathf.Sqrt(distance * Physics.gravity.magnitude / Mathf.Sin(2 * a));
        return velocity * direction.normalized;
    }

    public static Vector3 GetCurrentDistance(Vector3 startPoint ,Vector3 targetPoint, float angle)
    {
        // adjacent height
        float height = Vector3.Distance(startPoint, targetPoint);
        // convert angle into radians
        var radians = angle * (Mathf.PI / 180);
        var hypoteuseLenght = Mathf.Atan(radians) * height;
        return new Vector3(startPoint.x, startPoint.y, startPoint.z + hypoteuseLenght);
    }
}