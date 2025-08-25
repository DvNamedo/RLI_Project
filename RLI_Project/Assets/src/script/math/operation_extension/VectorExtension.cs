using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class VectorExtension
{
    public static Vector2 RotateAround(this Vector2 v, Vector2 u, float angle)
    {
        var (cos, sin) = (Mathf.Cos(angle), Mathf.Sin(angle));
        var (dx, dy) = (u.x - v.x, u.y - v.y);
        var (rx, ry) = (dx * cos - dy * sin, dx * sin + dy * cos);
        return new Vector2(rx + v.x, ry + v.y);
    }

    public static Vector2 DirectionTo(this Vector2 from, Vector2 to)
        => (to - from).normalized;

    public static Vector2Int DirectionTo(this Vector2Int from, Vector2Int to)
    {
        return Vector2Int.RoundToInt(from.DirectionTo(to));
    }

    public static Vector2Int RotateAround(this Vector2Int v, Vector2Int u, float angle)
    {
        Vector2 temp = v.RotateAround(u, angle);
        return Vector2Int.RoundToInt(temp);
    }

    // --- Vector3: radians, default Z-axis (2D와 동일한 평면 회전) ---
    public static Vector3 RotateAround(this Vector3 v, Vector3 u, float angleRad)
    {
        return RotateAround(v, u, angleRad, Vector3.forward);
    }

    // --- Vector3: radians + arbitrary axis ---
    public static Vector3 RotateAround(this Vector3 v, Vector3 u, float angleRad, Vector3 axis)
    {
        if (axis.sqrMagnitude < 1e-12f) return u; // 축이 0이면 변화 없음
        var q = Quaternion.AngleAxis(angleRad * Mathf.Rad2Deg, axis.normalized);
        return v + q * (u - v);
    }

    // --- Vector3Int: radians, default Z-axis ---
    public static Vector3Int RotateAround(this Vector3Int v, Vector3Int u, float angleRad)
    {
        Vector3 temp = ((Vector3)v).RotateAround((Vector3)u, angleRad);
        return Vector3Int.RoundToInt(temp);
    }

    // --- Vector3Int: radians + arbitrary axis ---
    public static Vector3Int RotateAround(this Vector3Int v, Vector3Int u, float angleRad, Vector3 axis)
    {
        Vector3 temp = ((Vector3)v).RotateAround((Vector3)u, angleRad, axis);
        return Vector3Int.RoundToInt(temp);
    }

}
