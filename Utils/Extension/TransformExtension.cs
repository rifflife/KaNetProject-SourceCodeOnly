using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MouseExtension
{
	public static Vector2 GetMouseDirectionBy(Transform transform)
	{
		var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Vector2 direction = mouseWorldPos - transform.position.GetOnlyXY();
		direction.Normalize();
		return direction;
	}

	public static Vector2 GetWorldMousePosition()
	{
		return Camera.main.ScreenToWorldPoint(Input.mousePosition);
	}
}
