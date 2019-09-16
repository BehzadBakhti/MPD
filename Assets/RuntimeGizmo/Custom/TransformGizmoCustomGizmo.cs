using System;
using UnityEngine;

namespace RuntimeGizmos
{
//Currently doesnt really handle TransformType.All
	public class TransformGizmoCustomGizmo : MonoBehaviour
	{
		public bool AutoFindTransformGizmo = true;
		public TransformGizmo TransformGizmo;

		public CustomTransformGizmos CustomTranslationGizmos = new CustomTransformGizmos();
		public CustomTransformGizmos CustomRotationGizmos = new CustomTransformGizmos();
		public CustomTransformGizmos CustomScaleGizmos = new CustomTransformGizmos();

		public bool ScaleBasedOnDistance = true;
		public float ScaleMultiplier = .4f;

		public int GizmoLayer = 2; //2 is the ignoreRaycast layer. Set to whatever you want.

		LayerMask _mask;

		void Awake()
		{
			if(TransformGizmo == null && AutoFindTransformGizmo)
			{
				TransformGizmo = GameObject.FindObjectOfType<TransformGizmo>();
			}

			TransformGizmo.ManuallyHandleGizmo = true;

			//Since we are using a mesh, rotating can get weird due to how the rotation method works,
			//so we use a different rotation method that will let us rotate by acting like our custom rotation gizmo is a wheel.
			//Can still give weird results depending on camera angle, but I think its more understanding for the user as to why its messing up.
			TransformGizmo.CircularRotationMethod = true;

			_mask = LayerMask.GetMask(LayerMask.LayerToName(GizmoLayer));

			CustomTranslationGizmos.Init(GizmoLayer);
			CustomRotationGizmos.Init(GizmoLayer);
			CustomScaleGizmos.Init(GizmoLayer);
		}

		void OnEnable()
		{
			TransformGizmo.OnCheckForSelectedAxis += CheckForSelectedAxis;
			TransformGizmo.OnDrawCustomGizmo += OnDrawCustomGizmos;
		}
		void OnDisable()
		{
			TransformGizmo.OnCheckForSelectedAxis -= CheckForSelectedAxis;
			TransformGizmo.OnDrawCustomGizmo -= OnDrawCustomGizmos;
		}

		void CheckForSelectedAxis()
		{
			ShowProperGizmoType();

			if(Input.GetMouseButtonDown(0))
			{
				RaycastHit hitInfo;
				if(Physics.Raycast(TransformGizmo.MyCamera.ScreenPointToRay(Input.mousePosition), out hitInfo, Mathf.Infinity, _mask))
				{
					Axis selectedAxis = Axis.None;
					TransformType type = TransformGizmo.TransformType;

					if(selectedAxis == Axis.None && TransformGizmo.TransformTypeContains(TransformType.Move))
					{
						selectedAxis = CustomTranslationGizmos.GetSelectedAxis(hitInfo.collider);
						type = TransformType.Move;
					}
					if(selectedAxis == Axis.None && TransformGizmo.TransformTypeContains(TransformType.Rotate))
					{
						selectedAxis = CustomRotationGizmos.GetSelectedAxis(hitInfo.collider);
						type = TransformType.Rotate;
					}
					if(selectedAxis == Axis.None && TransformGizmo.TransformTypeContains(TransformType.Scale))
					{
						selectedAxis = CustomScaleGizmos.GetSelectedAxis(hitInfo.collider);
						type = TransformType.Scale;
					}

					TransformGizmo.SetTranslatingAxis(type, selectedAxis);
				}
			}
		}

		void OnDrawCustomGizmos()
		{
			if(TransformGizmo.TranslatingTypeContains(TransformType.Move)) DrawCustomGizmo(CustomTranslationGizmos);
			if(TransformGizmo.TranslatingTypeContains(TransformType.Rotate)) DrawCustomGizmo(CustomRotationGizmos);
			if(TransformGizmo.TranslatingTypeContains(TransformType.Scale)) DrawCustomGizmo(CustomScaleGizmos);
		}

		void DrawCustomGizmo(CustomTransformGizmos customGizmo)
		{
			AxisInfo axisInfo = TransformGizmo.GetAxisInfo();
			customGizmo.SetAxis(axisInfo);
			customGizmo.SetPosition(TransformGizmo.PivotPoint);

			Vector4 totalScaleMultiplier = Vector4.one;
			if(ScaleBasedOnDistance)
			{
				totalScaleMultiplier.w *= (ScaleMultiplier * TransformGizmo.GetDistanceMultiplier());
			}

			if(TransformGizmo.TransformingType == TransformType.Scale)
			{
				float totalScaleAmount = 1f + TransformGizmo.TotalScaleAmount;
				if(TransformGizmo.TranslatingAxis == Axis.Any) totalScaleMultiplier += (Vector4.one * totalScaleAmount);
				else if(TransformGizmo.TranslatingAxis == Axis.X) totalScaleMultiplier.x *= totalScaleAmount;
				else if(TransformGizmo.TranslatingAxis == Axis.Y) totalScaleMultiplier.y *= totalScaleAmount;
				else if(TransformGizmo.TranslatingAxis == Axis.Z) totalScaleMultiplier.z *= totalScaleAmount;
			}

			customGizmo.ScaleMultiply(totalScaleMultiplier);
		}

		void ShowProperGizmoType()
		{
			bool hasSelection = TransformGizmo.MainTargetRoot != null;
			CustomTranslationGizmos.SetEnable(hasSelection && TransformGizmo.TranslatingTypeContains(TransformType.Move));
			CustomRotationGizmos.SetEnable(hasSelection && TransformGizmo.TranslatingTypeContains(TransformType.Rotate));
			CustomScaleGizmos.SetEnable(hasSelection && TransformGizmo.TranslatingTypeContains(TransformType.Scale));
		}
	}

	[Serializable]
	public class CustomTransformGizmos
	{
		public Transform XAxisGizmo;
		public Transform YAxisGizmo;
		public Transform ZAxisGizmo;
		public Transform AnyAxisGizmo;

		Collider _xAxisGizmoCollider;
		Collider _yAxisGizmoCollider;
		Collider _zAxisGizmoCollider;
		Collider _anyAxisGizmoCollider;

		Vector3 _originalXAxisScale;
		Vector3 _originalYAxisScale;
		Vector3 _originalZAxisScale;
		Vector3 _originalAnyAxisScale;

		public void Init(int layer)
		{
			if(XAxisGizmo != null)
			{
				SetLayerRecursively(XAxisGizmo.gameObject, layer);
				_xAxisGizmoCollider = XAxisGizmo.GetComponentInChildren<Collider>();
				_originalXAxisScale = XAxisGizmo.localScale;
			}
			if(YAxisGizmo != null)
			{
				SetLayerRecursively(YAxisGizmo.gameObject, layer);
				_yAxisGizmoCollider = YAxisGizmo.GetComponentInChildren<Collider>();
				_originalYAxisScale = YAxisGizmo.localScale;
			}
			if(ZAxisGizmo != null)
			{
				SetLayerRecursively(ZAxisGizmo.gameObject, layer);
				_zAxisGizmoCollider = ZAxisGizmo.GetComponentInChildren<Collider>();
				_originalZAxisScale = ZAxisGizmo.localScale;
			}
			if(AnyAxisGizmo != null)
			{
				SetLayerRecursively(AnyAxisGizmo.gameObject, layer);
				_anyAxisGizmoCollider = AnyAxisGizmo.GetComponentInChildren<Collider>();
				_originalAnyAxisScale = AnyAxisGizmo.localScale;
			}
		}

		public void SetEnable(bool enable)
		{
			if(XAxisGizmo != null && XAxisGizmo.gameObject.activeSelf != enable) XAxisGizmo.gameObject.SetActive(enable);
			if(YAxisGizmo != null && YAxisGizmo.gameObject.activeSelf != enable) YAxisGizmo.gameObject.SetActive(enable);
			if(ZAxisGizmo != null && ZAxisGizmo.gameObject.activeSelf != enable) ZAxisGizmo.gameObject.SetActive(enable);
			if(AnyAxisGizmo != null && AnyAxisGizmo.gameObject.activeSelf != enable) AnyAxisGizmo.gameObject.SetActive(enable);
		}

		public void SetAxis(AxisInfo axisInfo)
		{
			Quaternion lookRotation = Quaternion.LookRotation(axisInfo.ZDirection, axisInfo.YDirection);

			if(XAxisGizmo != null) XAxisGizmo.rotation = lookRotation;
			if(YAxisGizmo != null) YAxisGizmo.rotation = lookRotation;
			if(ZAxisGizmo != null) ZAxisGizmo.rotation = lookRotation;
			if(AnyAxisGizmo != null) AnyAxisGizmo.rotation = lookRotation;
		}

		public void SetPosition(Vector3 position)
		{
			if(XAxisGizmo != null) XAxisGizmo.position = position;
			if(YAxisGizmo != null) YAxisGizmo.position = position;
			if(ZAxisGizmo != null) ZAxisGizmo.position = position;
			if(AnyAxisGizmo != null) AnyAxisGizmo.position = position;
		}

		public void ScaleMultiply(Vector4 scaleMultiplier)
		{
			if(XAxisGizmo != null) XAxisGizmo.localScale = Vector3.Scale(_originalXAxisScale, new Vector3(scaleMultiplier.w + scaleMultiplier.x, scaleMultiplier.w, scaleMultiplier.w));
			if(YAxisGizmo != null) YAxisGizmo.localScale = Vector3.Scale(_originalYAxisScale, new Vector3(scaleMultiplier.w, scaleMultiplier.w + scaleMultiplier.y, scaleMultiplier.w));
			if(ZAxisGizmo != null) ZAxisGizmo.localScale = Vector3.Scale(_originalZAxisScale, new Vector3(scaleMultiplier.w, scaleMultiplier.w, scaleMultiplier.w + scaleMultiplier.z));
			if(AnyAxisGizmo != null) AnyAxisGizmo.localScale = _originalAnyAxisScale * scaleMultiplier.w;
		}

		public Axis GetSelectedAxis(Collider selectedCollider)
		{
			if(_xAxisGizmoCollider != null && _xAxisGizmoCollider == selectedCollider) return Axis.X;
			if(_yAxisGizmoCollider != null && _yAxisGizmoCollider == selectedCollider) return Axis.Y;
			if(_zAxisGizmoCollider != null && _zAxisGizmoCollider == selectedCollider) return Axis.Z;
			if(_anyAxisGizmoCollider != null && _anyAxisGizmoCollider == selectedCollider) return Axis.Any;

			return Axis.None;
		}

		void SetLayerRecursively(GameObject gameObject, int layer)
		{
			Transform[] selfAndChildren = gameObject.GetComponentsInChildren<Transform>(true);

			for(int i = 0; i < selfAndChildren.Length; i++)
			{
				selfAndChildren[i].gameObject.layer = layer;
			}
		}
	}
}
