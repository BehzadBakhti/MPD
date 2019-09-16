using System;
using CommandUndoRedo;
using UnityEngine;
using System.Collections.Generic;

namespace RuntimeGizmos
{
	public abstract class SelectCommand : ICommand
	{
		protected Transform Target;
		protected TransformGizmo TransformGizmo;

		public SelectCommand(TransformGizmo transformGizmo, Transform target)
		{
			this.TransformGizmo = transformGizmo;
			this.Target = target;
		}

		public abstract void Execute();
		public abstract void UnExecute();
	}

	public class AddTargetCommand : SelectCommand
	{
		List<Transform> _targetRoots = new List<Transform>();

		public AddTargetCommand(TransformGizmo transformGizmo, Transform target, List<Transform> targetRoots) : base(transformGizmo, target)
		{
			//Since we might have had a child selected and then selected the parent, the child would have been removed from the selected,
			//so we store all the targetRoots before we add so that if we undo we can properly have the children selected again.
			this._targetRoots.AddRange(targetRoots);
		}

		public override void Execute()
		{
			TransformGizmo.AddTarget(Target, false);
		}

		public override void UnExecute()
		{
			TransformGizmo.RemoveTarget(Target, false);

			for(int i = 0; i < _targetRoots.Count; i++)
			{
				TransformGizmo.AddTarget(_targetRoots[i], false);
			}
		}
	}

	public class RemoveTargetCommand : SelectCommand
	{
		public RemoveTargetCommand(TransformGizmo transformGizmo, Transform target) : base(transformGizmo, target) {}

		public override void Execute()
		{
			TransformGizmo.RemoveTarget(Target, false);
		}

		public override void UnExecute()
		{
			TransformGizmo.AddTarget(Target, false);
		}
	}

	public class ClearTargetsCommand : SelectCommand
	{
		List<Transform> _targetRoots = new List<Transform>();

		public ClearTargetsCommand(TransformGizmo transformGizmo, List<Transform> targetRoots) : base(transformGizmo, null)
		{
			this._targetRoots.AddRange(targetRoots);
		}

		public override void Execute()
		{
			TransformGizmo.ClearTargets(false);
		}

		public override void UnExecute()
		{
			for(int i = 0; i < _targetRoots.Count; i++)
			{
				TransformGizmo.AddTarget(_targetRoots[i], false);
			}
		}
	}

	public class ClearAndAddTargetCommand : SelectCommand
	{
		List<Transform> _targetRoots = new List<Transform>();

		public ClearAndAddTargetCommand(TransformGizmo transformGizmo, Transform target, List<Transform> targetRoots) : base(transformGizmo, target)
		{
			this._targetRoots.AddRange(targetRoots);
		}

		public override void Execute()
		{
			TransformGizmo.ClearTargets(false);
			TransformGizmo.AddTarget(Target, false);
		}

		public override void UnExecute()
		{
			TransformGizmo.RemoveTarget(Target, false);

			for(int i = 0; i < _targetRoots.Count; i++)
			{
				TransformGizmo.AddTarget(_targetRoots[i], false);
			}
		}
	}
}
