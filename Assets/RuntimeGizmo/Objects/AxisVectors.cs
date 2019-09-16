using System;
using System.Collections.Generic;
using UnityEngine;

namespace RuntimeGizmos
{
	public class AxisVectors
	{
		public List<Vector3> X = new List<Vector3>();
		public List<Vector3> Y = new List<Vector3>();
		public List<Vector3> Z = new List<Vector3>();
		public List<Vector3> All = new List<Vector3>();

		public void Add(AxisVectors axisVectors)
		{
			X.AddRange(axisVectors.X);
			Y.AddRange(axisVectors.Y);
			Z.AddRange(axisVectors.Z);
			All.AddRange(axisVectors.All);
		}

		public void Clear()
		{
			X.Clear();
			Y.Clear();
			Z.Clear();
			All.Clear();
		}
	}
}