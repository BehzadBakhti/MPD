using System;
using UnityEngine;

namespace RuntimeGizmos
{
	public struct AxisInfo
	{
		public Vector3 Pivot;
		public Vector3 XDirection;
		public Vector3 YDirection;
		public Vector3 ZDirection;

		public void Set(Transform target, Vector3 pivot, TransformSpace space)
		{
			if(space == TransformSpace.Global)
			{
				XDirection = Vector3.right;
				YDirection = Vector3.up;
				ZDirection = Vector3.forward;
			}
			else if(space == TransformSpace.Local)
			{
				XDirection = target.right;
				YDirection = target.up;
				ZDirection = target.forward;
			}

			this.Pivot = pivot;
		}

		public Vector3 GetXAxisEnd(float size)
		{
			return Pivot + (XDirection * size);
		}
		public Vector3 GetYAxisEnd(float size)
		{
			return Pivot + (YDirection * size);
		}
		public Vector3 GetZAxisEnd(float size)
		{
			return Pivot + (ZDirection * size);
		}
		public Vector3 GetAxisEnd(Vector3 direction, float size)
		{
			return Pivot + (direction * size);
		}
	}
}
