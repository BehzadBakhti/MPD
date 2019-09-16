using System;
using UnityEngine;

namespace RuntimeGizmos
{
	public struct Square
	{
		public Vector3 BottomLeft;
		public Vector3 BottomRight;
		public Vector3 TopLeft;
		public Vector3 TopRight;

		public Vector3 this[int index]
		{
			get
			{
				switch (index)
				{
					case 0:
						return this.BottomLeft;
					case 1:
						return this.TopLeft;
					case 2:
						return this.TopRight;
					case 3:
						return this.BottomRight;
					case 4:
						return this.BottomLeft; //so we wrap around back to start
					default:
						return Vector3.zero;
				}
			}
		}
	}
}
