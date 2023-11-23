using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorExtension
{
    public static Vector3Int Int(this Vector3 vector) => new Vector3Int((int)vector.x, (int)vector.y, (int)vector.z);
    public static Vector3 Float(this Vector3Int vector) => new Vector3(vector.x, vector.y, vector.z);
    public static Vector3Int Floor(this Vector3 vector) => new Vector3Int(Mathf.FloorToInt(vector.x), Mathf.FloorToInt(vector.y), Mathf.FloorToInt(vector.z));
    public static Vector3 GetOnlyXY(this Vector3 vector) => new Vector3(vector.x, vector.y, 0);
	public static Vector2Int Int(this Vector2 vector) => new Vector2Int((int)vector.x, (int)vector.y);
    public static Vector2 ToVector2(this Vector3 vector) => new Vector2(vector.x, vector.y);
	public static Vector3 ToVector3(this Vector2 vector) => new Vector3(vector.x, vector.y, 0);
	public static Vector2 GetOnlyX(this Vector2 vector) => new Vector2(vector.x, 0);
	public static Vector2 GetOnlyY(this Vector2 vector) => new Vector2(0, vector.y);
    public static Vector2 SetX(this in Vector2 vector, float x) => new Vector2(x, vector.y);
	public static Vector2 SetY(this in Vector2 vector, float y) => new Vector2(vector.x, y);

	public static bool IsVectorInRect(this Vector2 vector, Rect rect)
    {
        bool value = vector.x > rect.x && vector.y > rect.y && vector.x < rect.xMax && vector.y < rect.yMax;
        return value;
	}
    public static Vector2 ClampInRect(this Vector2 vector, Rect rect)
    {
		vector.x = Mathf.Clamp(vector.x, rect.x, rect.xMax);
		vector.y = Mathf.Clamp(vector.y, rect.y, rect.yMax);
        return vector;
	}

    public static Quaternion ToQuaternion(this Vector3 vector)
    {
        Quaternion q = Quaternion.Euler(vector);
        return q;
    }

    /// <summary>벡터의 x를 min으로 y를 max로 하는 범위에서 렌덤한 값을 반환합니다.</summary>
    /// <returns>렌덤한 값</returns>
    public static float GetRandomFromMinMax(this Vector2 vector)
    {
        return Random.Range(vector.x, vector.y);
    }
}
