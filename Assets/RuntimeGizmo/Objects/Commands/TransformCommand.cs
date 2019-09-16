using System;
using CommandUndoRedo;
using UnityEngine;

namespace RuntimeGizmos
{
	public class TransformCommand : ICommand
	{
		TransformValues _newValues;
		TransformValues _oldValues;

		Transform _transform;
		TransformGizmo _transformGizmo;

		public TransformCommand(TransformGizmo transformGizmo, Transform transform)
		{
			this._transformGizmo = transformGizmo;
			this._transform = transform;

			_oldValues = new TransformValues() {Position=transform.position, Rotation=transform.rotation, Scale=transform.localScale};
		}

		public void StoreNewTransformValues()
		{
			_newValues = new TransformValues() {Position=_transform.position, Rotation=_transform.rotation, Scale=_transform.localScale};
		}
		
		public void Execute()
		{
			_transform.position = _newValues.Position;
			_transform.rotation = _newValues.Rotation;
			_transform.localScale = _newValues.Scale;

			_transformGizmo.SetPivotPoint();
		}

		public void UnExecute()
		{
			_transform.position = _oldValues.Position;
			_transform.rotation = _oldValues.Rotation;
			_transform.localScale = _oldValues.Scale;

			_transformGizmo.SetPivotPoint();
		}

		struct TransformValues
		{
			public Vector3 Position;
			public Quaternion Rotation;
			public Vector3 Scale;
		}
	}
}
