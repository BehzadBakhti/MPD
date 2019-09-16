using System;
using UnityEngine;

namespace RuntimeGizmos
{
	public struct IntersectPoints
	{
		public Vector3 First;
		public Vector3 Second;

		public IntersectPoints(Vector3 first, Vector3 second)
		{
			this.First = first;
			this.Second = second;
		}
	}
}