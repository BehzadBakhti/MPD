using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using CommandUndoRedo;

namespace RuntimeGizmos
{
	//To be safe, if you are changing any transforms hierarchy, such as parenting an object to something,
	//you should call ClearTargets before doing so just to be sure nothing unexpected happens... as well as call UndoRedoManager.Clear()
	//For example, if you select an object that has children, move the children elsewhere, deselect the original object, then try to add those old children to the selection, I think it wont work.

	[RequireComponent(typeof(Camera))]
	public class TransformGizmo : MonoBehaviour
	{
		public TransformSpace Space = TransformSpace.Global;
		public TransformType TransformType = TransformType.Move;
		public TransformPivot Pivot = TransformPivot.Pivot;
		public CenterType CenterType = CenterType.All;
		public ScaleType ScaleType = ScaleType.FromPoint;

		//These are the same as the unity editor hotkeys
		public KeyCode SetMoveType = KeyCode.W;
		public KeyCode SetRotateType = KeyCode.E;
		public KeyCode SetScaleType = KeyCode.R;
		//public KeyCode SetRectToolType = KeyCode.T;
		public KeyCode SetAllTransformType = KeyCode.Y;
		public KeyCode SetSpaceToggle = KeyCode.X;
		public KeyCode SetPivotModeToggle = KeyCode.Z;
		public KeyCode SetCenterTypeToggle = KeyCode.C;
		public KeyCode SetScaleTypeToggle = KeyCode.S;
		public KeyCode AddSelection = KeyCode.LeftShift;
		public KeyCode RemoveSelection = KeyCode.LeftControl;
		public KeyCode ActionKey = KeyCode.LeftShift; //Its set to shift instead of control so that while in the editor we dont accidentally undo editor changes =/
		public KeyCode UndoAction = KeyCode.Z;
		public KeyCode RedoAction = KeyCode.Y;

		public Color XColor = new Color(1, 0, 0, 0.8f);
		public Color YColor = new Color(0, 1, 0, 0.8f);
		public Color ZColor = new Color(0, 0, 1, 0.8f);
		public Color AllColor = new Color(.7f, .7f, .7f, 0.8f);
		public Color SelectedColor = new Color(1, 1, 0, 0.8f);
		public Color HoverColor = new Color(1, .75f, 0, 0.8f);
		public float PlanesOpacity = .5f;
		//public Color rectPivotColor = new Color(0, 0, 1, 0.8f);
		//public Color rectCornerColor = new Color(0, 0, 1, 0.8f);
		//public Color rectAnchorColor = new Color(.7f, .7f, .7f, 0.8f);
		//public Color rectLineColor = new Color(.7f, .7f, .7f, 0.8f);

		public float HandleLength = .25f;
		public float HandleWidth = .003f;
		public float PlaneSize = .035f;
		public float TriangleSize = .03f;
		public float BoxSize = .03f;
		public int CircleDetail = 40;
		public float AllMoveHandleLengthMultiplier = 1f;
		public float AllRotateHandleLengthMultiplier = 1.4f;
		public float AllScaleHandleLengthMultiplier = 1.6f;
		public float MinSelectedDistanceCheck = .01f;
		public float MoveSpeedMultiplier = 1f;
		public float ScaleSpeedMultiplier = 1f;
		public float RotateSpeedMultiplier = 1f;
		public float AllRotateSpeedMultiplier = 20f;

		public bool UseFirstSelectedAsMain = true;

		//If circularRotationMethod is true, when rotating you will need to move your mouse around the object as if turning a wheel.
		//If circularRotationMethod is false, when rotating you can just click and drag in a line to rotate.
		public bool CircularRotationMethod;

		//Mainly for if you want the pivot point to update correctly if selected objects are moving outside the transformgizmo.
		//Might be poor on performance if lots of objects are selected...
		public bool forceUpdatePivotPointOnChange = true;

		public int MaxUndoStored = 100;

		public bool ManuallyHandleGizmo;

		public LayerMask SelectionMask = Physics.DefaultRaycastLayers;

		public Action OnCheckForSelectedAxis;
		public Action OnDrawCustomGizmo;

		public Camera MyCamera {get; private set;}

		public bool IsTransforming {get; private set;}
		public float TotalScaleAmount {get; private set;}
		public Quaternion TotalRotationAmount {get; private set;}
		public Axis TranslatingAxis {get {return _nearAxis;}}
		public Axis TranslatingAxisPlane {get {return _planeAxis;}}
		public bool HasTranslatingAxisPlane {get {return TranslatingAxisPlane != Axis.None && TranslatingAxisPlane != Axis.Any;}}
		public TransformType TransformingType {get {return _translatingType;}}

		public Vector3 PivotPoint {get; private set;}
		Vector3 _totalCenterPivotPoint;

		public Transform MainTargetRoot {get {return (_targetRootsOrdered.Count > 0) ? (UseFirstSelectedAsMain) ? _targetRootsOrdered[0] : _targetRootsOrdered[_targetRootsOrdered.Count - 1] : null;}}

		AxisInfo _axisInfo;
		Axis _nearAxis = Axis.None;
		Axis _planeAxis = Axis.None;
		TransformType _translatingType;

		AxisVectors _handleLines = new AxisVectors();
		AxisVectors _handlePlanes = new AxisVectors();
		AxisVectors _handleTriangles = new AxisVectors();
		AxisVectors _handleSquares = new AxisVectors();
		AxisVectors _circlesLines = new AxisVectors();

		//We use a HashSet and a List for targetRoots so that we get fast lookup with the hashset while also keeping track of the order with the list.
		List<Transform> _targetRootsOrdered = new List<Transform>();
		Dictionary<Transform, TargetInfo> _targetRoots = new Dictionary<Transform, TargetInfo>();
		HashSet<Renderer> _highlightedRenderers = new HashSet<Renderer>();
		HashSet<Transform> _children = new HashSet<Transform>();

		List<Transform> _childrenBuffer = new List<Transform>();
		List<Renderer> _renderersBuffer = new List<Renderer>();
		List<Material> _materialsBuffer = new List<Material>();

		WaitForEndOfFrame _waitForEndOfFrame = new WaitForEndOfFrame();
		Coroutine _forceUpdatePivotCoroutine;

		static Material _lineMaterial;
		static Material _outlineMaterial;

		void Awake()
		{
			MyCamera = GetComponent<Camera>();
			SetMaterial();
		}

		void OnEnable()
		{
			_forceUpdatePivotCoroutine = StartCoroutine(ForceUpdatePivotPointAtEndOfFrame());
		}

		void OnDisable()
		{
			ClearTargets(); //Just so things gets cleaned up, such as removing any materials we placed on objects.

			StopCoroutine(_forceUpdatePivotCoroutine);
		}

		void OnDestroy()
		{
			ClearAllHighlightedRenderers();
		}

		void Update()
		{
			HandleUndoRedo();

			SetSpaceAndType();

			if(ManuallyHandleGizmo)
			{
				if(OnCheckForSelectedAxis != null) OnCheckForSelectedAxis();
			}else{
				SetNearAxis();
			}
			
			GetTarget();

			if(MainTargetRoot == null) return;
			
			TransformSelected();
		}

		void LateUpdate()
		{
			if(MainTargetRoot == null) return;

			//We run this in lateupdate since coroutines run after update and we want our gizmos to have the updated target transform position after TransformSelected()
			SetAxisInfo();
			
			if(ManuallyHandleGizmo)
			{
				if(OnDrawCustomGizmo != null) OnDrawCustomGizmo();
			}else{
				SetLines();
			}
		}

		void OnPostRender()
		{
			if(MainTargetRoot == null || ManuallyHandleGizmo) return;

			_lineMaterial.SetPass(0);

			Color xColor = (_nearAxis == Axis.X) ? (IsTransforming) ? SelectedColor : HoverColor : this.XColor;
			Color yColor = (_nearAxis == Axis.Y) ? (IsTransforming) ? SelectedColor : HoverColor : this.YColor;
			Color zColor = (_nearAxis == Axis.Z) ? (IsTransforming) ? SelectedColor : HoverColor : this.ZColor;
			Color allColor = (_nearAxis == Axis.Any) ? (IsTransforming) ? SelectedColor : HoverColor : this.AllColor;

			//Note: The order of drawing the axis decides what gets drawn over what.

			TransformType moveOrScaleType = (TransformType == TransformType.Scale || (IsTransforming && _translatingType == TransformType.Scale)) ? TransformType.Scale : TransformType.Move;
			DrawQuads(_handleLines.Z, GetColor(moveOrScaleType, this.ZColor, zColor, HasTranslatingAxisPlane));
			DrawQuads(_handleLines.X, GetColor(moveOrScaleType, this.XColor, xColor, HasTranslatingAxisPlane));
			DrawQuads(_handleLines.Y, GetColor(moveOrScaleType, this.YColor, yColor, HasTranslatingAxisPlane));

			DrawTriangles(_handleTriangles.X, GetColor(TransformType.Move, this.XColor, xColor, HasTranslatingAxisPlane));
			DrawTriangles(_handleTriangles.Y, GetColor(TransformType.Move, this.YColor, yColor, HasTranslatingAxisPlane));
			DrawTriangles(_handleTriangles.Z, GetColor(TransformType.Move, this.ZColor, zColor, HasTranslatingAxisPlane));

			DrawQuads(_handlePlanes.Z, GetColor(TransformType.Move, this.ZColor, zColor, PlanesOpacity, !HasTranslatingAxisPlane));
			DrawQuads(_handlePlanes.X, GetColor(TransformType.Move, this.XColor, xColor, PlanesOpacity, !HasTranslatingAxisPlane));
			DrawQuads(_handlePlanes.Y, GetColor(TransformType.Move, this.YColor, yColor, PlanesOpacity, !HasTranslatingAxisPlane));

			DrawQuads(_handleSquares.X, GetColor(TransformType.Scale, this.XColor, xColor));
			DrawQuads(_handleSquares.Y, GetColor(TransformType.Scale, this.YColor, yColor));
			DrawQuads(_handleSquares.Z, GetColor(TransformType.Scale, this.ZColor, zColor));
			DrawQuads(_handleSquares.All, GetColor(TransformType.Scale, this.AllColor, allColor));

			DrawQuads(_circlesLines.All, GetColor(TransformType.Rotate, this.AllColor, allColor));
			DrawQuads(_circlesLines.X, GetColor(TransformType.Rotate, this.XColor, xColor));
			DrawQuads(_circlesLines.Y, GetColor(TransformType.Rotate, this.YColor, yColor));
			DrawQuads(_circlesLines.Z, GetColor(TransformType.Rotate, this.ZColor, zColor));
		}

		Color GetColor(TransformType type, Color normalColor, Color nearColor, bool forceUseNormal = false)
		{
			return GetColor(type, normalColor, nearColor, false, 1, forceUseNormal);
		}
		Color GetColor(TransformType type, Color normalColor, Color nearColor, float alpha, bool forceUseNormal = false)
		{
			return GetColor(type, normalColor, nearColor, true, alpha, forceUseNormal);
		}
		Color GetColor(TransformType type, Color normalColor, Color nearColor, bool setAlpha, float alpha, bool forceUseNormal = false)
		{
			Color color;
			if(!forceUseNormal && TranslatingTypeContains(type, false))
			{
				color = nearColor;
			}else{
				color = normalColor;
			}

			if(setAlpha)
			{
				color.a = alpha;
			}

			return color;
		}

		void HandleUndoRedo()
		{
			if(MaxUndoStored != UndoRedoManager.MaxUndoStored) { UndoRedoManager.MaxUndoStored = MaxUndoStored; }

			if(Input.GetKey(ActionKey))
			{
				if(Input.GetKeyDown(UndoAction))
				{
					UndoRedoManager.Undo();
				}
				else if(Input.GetKeyDown(RedoAction))
				{
					UndoRedoManager.Redo();
				}
			}
		}

		//We only support scaling in local space.
		public TransformSpace GetProperTransformSpace()
		{
			return TransformType == TransformType.Scale ? TransformSpace.Local : Space;
		}

		public bool TransformTypeContains(TransformType type)
		{
			return TransformTypeContains(TransformType, type);
		}
		public bool TranslatingTypeContains(TransformType type, bool checkIsTransforming = true)
		{
			TransformType transType = !checkIsTransforming || IsTransforming ? _translatingType : TransformType;
			return TransformTypeContains(transType, type);
		}
		public bool TransformTypeContains(TransformType mainType, TransformType type)
		{
			return ExtTransformType.TransformTypeContains(mainType, type, GetProperTransformSpace());
		}
		
		public float GetHandleLength(TransformType type, Axis axis = Axis.None, bool multiplyDistanceMultiplier = true)
		{
			float length = HandleLength;
			if(TransformType == TransformType.All)
			{
				if(type == TransformType.Move) length *= AllMoveHandleLengthMultiplier;
				if(type == TransformType.Rotate) length *= AllRotateHandleLengthMultiplier;
				if(type == TransformType.Scale) length *= AllScaleHandleLengthMultiplier;
			}

			if(multiplyDistanceMultiplier) length *= GetDistanceMultiplier();

			if(type == TransformType.Scale && IsTransforming && (TranslatingAxis == axis || TranslatingAxis == Axis.Any)) length += TotalScaleAmount;

			return length;
		}

		void SetSpaceAndType()
		{
			if(Input.GetKey(ActionKey)) return;

			if(Input.GetKeyDown(SetMoveType)) TransformType = TransformType.Move;
			else if(Input.GetKeyDown(SetRotateType)) TransformType = TransformType.Rotate;
			else if(Input.GetKeyDown(SetScaleType)) TransformType = TransformType.Scale;
			//else if(Input.GetKeyDown(SetRectToolType)) type = TransformType.RectTool;
			else if(Input.GetKeyDown(SetAllTransformType)) TransformType = TransformType.All;

			if(!IsTransforming) _translatingType = TransformType;

			if(Input.GetKeyDown(SetPivotModeToggle))
			{
				if(Pivot == TransformPivot.Pivot) Pivot = TransformPivot.Center;
				else if(Pivot == TransformPivot.Center) Pivot = TransformPivot.Pivot;

				SetPivotPoint();
			}

			if(Input.GetKeyDown(SetCenterTypeToggle))
			{
				if(CenterType == CenterType.All) CenterType = CenterType.Solo;
				else if(CenterType == CenterType.Solo) CenterType = CenterType.All;

				SetPivotPoint();
			}

			if(Input.GetKeyDown(SetSpaceToggle))
			{
				if(Space == TransformSpace.Global) Space = TransformSpace.Local;
				else if(Space == TransformSpace.Local) Space = TransformSpace.Global;
			}

			if(Input.GetKeyDown(SetScaleTypeToggle))
			{
				if(ScaleType == ScaleType.FromPoint) ScaleType = ScaleType.FromPointOffset;
				else if(ScaleType == ScaleType.FromPointOffset) ScaleType = ScaleType.FromPoint;
			}

			if(TransformType == TransformType.Scale)
			{
				if(Pivot == TransformPivot.Pivot) ScaleType = ScaleType.FromPoint; //FromPointOffset can be inaccurate and should only really be used in Center mode if desired.
			}
		}

		void TransformSelected()
		{
			if(MainTargetRoot != null)
			{
				if(_nearAxis != Axis.None && Input.GetMouseButtonDown(0))
				{
					StartCoroutine(TransformSelected(_translatingType));
				}
			}
		}
		
		IEnumerator TransformSelected(TransformType transType)
		{
			IsTransforming = true;
			TotalScaleAmount = 0;
			TotalRotationAmount = Quaternion.identity;

			Vector3 originalPivot = PivotPoint;

			Vector3 axis = GetNearAxisDirection();
			Vector3 planeNormal = HasTranslatingAxisPlane ? axis : (transform.position - originalPivot).normalized;
			Vector3 projectedAxis = Vector3.ProjectOnPlane(axis, planeNormal).normalized;
			Vector3 previousMousePosition = Vector3.zero;

			List<ICommand> transformCommands = new List<ICommand>();
			for(int i = 0; i < _targetRootsOrdered.Count; i++)
			{
				transformCommands.Add(new TransformCommand(this, _targetRootsOrdered[i]));
			}

			while(!Input.GetMouseButtonUp(0))
			{
				Ray mouseRay = MyCamera.ScreenPointToRay(Input.mousePosition);
				Vector3 mousePosition = Geometry.LinePlaneIntersect(mouseRay.origin, mouseRay.direction, originalPivot, planeNormal);

				if(previousMousePosition != Vector3.zero && mousePosition != Vector3.zero)
				{
					if(transType == TransformType.Move)
					{
						Vector3 movement = Vector3.zero;

						if(HasTranslatingAxisPlane)
						{
							movement = mousePosition - previousMousePosition;
						}else{
							float moveAmount = ExtVector3.MagnitudeInDirection(mousePosition - previousMousePosition, projectedAxis) * MoveSpeedMultiplier;
							movement = axis * moveAmount;
						}

						for(int i = 0; i < _targetRootsOrdered.Count; i++)
						{
							Transform target = _targetRootsOrdered[i];

							target.Translate(movement, UnityEngine.Space.World);
						}

						SetPivotPointOffset(movement);
					}
					else if(transType == TransformType.Scale)
					{
						Vector3 projected = (_nearAxis == Axis.Any) ? transform.right : projectedAxis;
						float scaleAmount = ExtVector3.MagnitudeInDirection(mousePosition - previousMousePosition, projected) * ScaleSpeedMultiplier;
						
						//WARNING - There is a bug in unity 5.4 and 5.5 that causes InverseTransformDirection to be affected by scale which will break negative scaling. Not tested, but updating to 5.4.2 should fix it - https://issuetracker.unity3d.com/issues/transformdirection-and-inversetransformdirection-operations-are-affected-by-scale
						Vector3 localAxis = (GetProperTransformSpace() == TransformSpace.Local && _nearAxis != Axis.Any) ? MainTargetRoot.InverseTransformDirection(axis) : axis;
						
						Vector3 targetScaleAmount = Vector3.one;
						if(_nearAxis == Axis.Any) targetScaleAmount = (ExtVector3.Abs(MainTargetRoot.localScale.normalized) * scaleAmount);
						else targetScaleAmount = localAxis * scaleAmount;

						for(int i = 0; i < _targetRootsOrdered.Count; i++)
						{
							Transform target = _targetRootsOrdered[i];

							Vector3 targetScale = target.localScale + targetScaleAmount;

							if(Pivot == TransformPivot.Pivot)
							{
								target.localScale = targetScale;
							}
							else if(Pivot == TransformPivot.Center)
							{
								if(ScaleType == ScaleType.FromPoint)
								{
									target.SetScaleFrom(originalPivot, targetScale);
								}
								else if(ScaleType == ScaleType.FromPointOffset)
								{
									target.SetScaleFromOffset(originalPivot, targetScale);
								}
							}
						}

						TotalScaleAmount += scaleAmount;
					}
					else if(transType == TransformType.Rotate)
					{
						float rotateAmount = 0;
						Vector3 rotationAxis = axis;

						if(_nearAxis == Axis.Any)
						{
							Vector3 rotation = transform.TransformDirection(new Vector3(Input.GetAxis("Mouse Y"), -Input.GetAxis("Mouse X"), 0));
							Quaternion.Euler(rotation).ToAngleAxis(out rotateAmount, out rotationAxis);
							rotateAmount *= AllRotateSpeedMultiplier;
						}else{
							if(CircularRotationMethod)
							{
								float angle = Vector3.SignedAngle(previousMousePosition - originalPivot, mousePosition - originalPivot, axis);
								rotateAmount = angle * RotateSpeedMultiplier;
							}else{
								Vector3 projected = (_nearAxis == Axis.Any || ExtVector3.IsParallel(axis, planeNormal)) ? planeNormal : Vector3.Cross(axis, planeNormal);
								rotateAmount = (ExtVector3.MagnitudeInDirection(mousePosition - previousMousePosition, projected) * (RotateSpeedMultiplier * 100f)) / GetDistanceMultiplier();
							}
						}

						for(int i = 0; i < _targetRootsOrdered.Count; i++)
						{
							Transform target = _targetRootsOrdered[i];

							if(Pivot == TransformPivot.Pivot)
							{
								target.Rotate(rotationAxis, rotateAmount, UnityEngine.Space.World);
							}
							else if(Pivot == TransformPivot.Center)
							{
								target.RotateAround(originalPivot, rotationAxis, rotateAmount);
							}
						}

						TotalRotationAmount *= Quaternion.Euler(rotationAxis * rotateAmount);
					}
				}

				previousMousePosition = mousePosition;

				yield return null;
			}

			for(int i = 0; i < transformCommands.Count; i++)
			{
				((TransformCommand)transformCommands[i]).StoreNewTransformValues();
			}
			CommandGroup commandGroup = new CommandGroup();
			commandGroup.Set(transformCommands);
			UndoRedoManager.Insert(commandGroup);

			TotalRotationAmount = Quaternion.identity;
			TotalScaleAmount = 0;
			IsTransforming = false;
			SetTranslatingAxis(TransformType, Axis.None);

			SetPivotPoint();
		}

		Vector3 GetNearAxisDirection()
		{
			if(_nearAxis != Axis.None)
			{
				if(_nearAxis == Axis.X) return _axisInfo.XDirection;
				if(_nearAxis == Axis.Y) return _axisInfo.YDirection;
				if(_nearAxis == Axis.Z) return _axisInfo.ZDirection;
				if(_nearAxis == Axis.Any) return Vector3.one;
			}
			return Vector3.zero;
		}
	
		void GetTarget()
		{
			if(_nearAxis == Axis.None && Input.GetMouseButtonDown(0))
			{
				bool isAdding = Input.GetKey(AddSelection);
				bool isRemoving = Input.GetKey(RemoveSelection);

				RaycastHit hitInfo; 
				if(Physics.Raycast(MyCamera.ScreenPointToRay(Input.mousePosition), out hitInfo, Mathf.Infinity, SelectionMask))
				{
					Transform target = hitInfo.transform;

					if(isAdding)
					{
						AddTarget(target);
					}
					else if(isRemoving)
					{
						RemoveTarget(target);
					}
					else if(!isAdding && !isRemoving)
					{
						ClearAndAddTarget(target);
					}
				}else{
					if(!isAdding && !isRemoving)
					{
						ClearTargets();
					}
				}
			}
		}

		public void AddTarget(Transform target, bool addCommand = true)
		{
			if(target != null)
			{
				if(_targetRoots.ContainsKey(target)) return;
				if(_children.Contains(target)) return;

				if(addCommand) UndoRedoManager.Insert(new AddTargetCommand(this, target, _targetRootsOrdered));

				AddTargetRoot(target);
				AddTargetHighlightedRenderers(target);

				SetPivotPoint();
			}
		}

		public void RemoveTarget(Transform target, bool addCommand = true)
		{
			if(target != null)
			{
				if(!_targetRoots.ContainsKey(target)) return;

				if(addCommand) UndoRedoManager.Insert(new RemoveTargetCommand(this, target));

				RemoveTargetHighlightedRenderers(target);
				RemoveTargetRoot(target);

				SetPivotPoint();
			}
		}

		public void ClearTargets(bool addCommand = true)
		{
			if(addCommand) UndoRedoManager.Insert(new ClearTargetsCommand(this, _targetRootsOrdered));

			ClearAllHighlightedRenderers();
			_targetRoots.Clear();
			_targetRootsOrdered.Clear();
			_children.Clear();
		}

		void ClearAndAddTarget(Transform target)
		{
			UndoRedoManager.Insert(new ClearAndAddTargetCommand(this, target, _targetRootsOrdered));

			ClearTargets(false);
			AddTarget(target, false);
		}

		void AddTargetHighlightedRenderers(Transform target)
		{
			if(target != null)
			{
				GetTargetRenderers(target, _renderersBuffer);

				for(int i = 0; i < _renderersBuffer.Count; i++)
				{
					Renderer render = _renderersBuffer[i];

					if(!_highlightedRenderers.Contains(render))
					{
						_materialsBuffer.Clear();
						_materialsBuffer.AddRange(render.sharedMaterials);

						if(!_materialsBuffer.Contains(_outlineMaterial))
						{
							_materialsBuffer.Add(_outlineMaterial);
							render.materials = _materialsBuffer.ToArray();
						}

						_highlightedRenderers.Add(render);
					}
				}

				_materialsBuffer.Clear();
			}
		}

		void GetTargetRenderers(Transform target, List<Renderer> renderers)
		{
			renderers.Clear();
			if(target != null)
			{
				target.GetComponentsInChildren<Renderer>(true, renderers);
			}
		}

		void ClearAllHighlightedRenderers()
		{
			foreach(var target in _targetRoots)
			{
				RemoveTargetHighlightedRenderers(target.Key);
			}

			//In case any are still left, such as if they changed parents or what not when they were highlighted.
			_renderersBuffer.Clear();
			_renderersBuffer.AddRange(_highlightedRenderers);
			RemoveHighlightedRenderers(_renderersBuffer);
		}

		void RemoveTargetHighlightedRenderers(Transform target)
		{
			GetTargetRenderers(target, _renderersBuffer);

			RemoveHighlightedRenderers(_renderersBuffer);
		}

		void RemoveHighlightedRenderers(List<Renderer> renderers)
		{
			for(int i = 0; i < _renderersBuffer.Count; i++)
			{
				Renderer render = _renderersBuffer[i];
				if(render != null)
				{
					_materialsBuffer.Clear();
					_materialsBuffer.AddRange(render.sharedMaterials);

					if(_materialsBuffer.Contains(_outlineMaterial))
					{
						_materialsBuffer.Remove(_outlineMaterial);
						render.materials = _materialsBuffer.ToArray();
					}
				}

				_highlightedRenderers.Remove(render);
			}

			_renderersBuffer.Clear();
		}

		void AddTargetRoot(Transform targetRoot)
		{
			_targetRoots.Add(targetRoot, new TargetInfo());
			_targetRootsOrdered.Add(targetRoot);

			AddAllChildren(targetRoot);
		}
		void RemoveTargetRoot(Transform targetRoot)
		{
			if(_targetRoots.Remove(targetRoot))
			{
				_targetRootsOrdered.Remove(targetRoot);

				RemoveAllChildren(targetRoot);
			}
		}

		void AddAllChildren(Transform target)
		{
			_childrenBuffer.Clear();
			target.GetComponentsInChildren<Transform>(true, _childrenBuffer);
			_childrenBuffer.Remove(target);

			for(int i = 0; i < _childrenBuffer.Count; i++)
			{
				Transform child = _childrenBuffer[i];
				_children.Add(child);
				RemoveTargetRoot(child); //We do this in case we selected child first and then the parent.
			}

			_childrenBuffer.Clear();
		}
		void RemoveAllChildren(Transform target)
		{
			_childrenBuffer.Clear();
			target.GetComponentsInChildren<Transform>(true, _childrenBuffer);
			_childrenBuffer.Remove(target);

			for(int i = 0; i < _childrenBuffer.Count; i++)
			{
				_children.Remove(_childrenBuffer[i]);
			}

			_childrenBuffer.Clear();
		}

		public void SetPivotPoint()
		{
			if(MainTargetRoot != null)
			{
				if(Pivot == TransformPivot.Pivot)
				{
					PivotPoint = MainTargetRoot.position;
				}
				else if(Pivot == TransformPivot.Center)
				{
					_totalCenterPivotPoint = Vector3.zero;

					Dictionary<Transform, TargetInfo>.Enumerator targetsEnumerator = _targetRoots.GetEnumerator(); //We avoid foreach to avoid garbage.
					while(targetsEnumerator.MoveNext())
					{
						Transform target = targetsEnumerator.Current.Key;
						TargetInfo info = targetsEnumerator.Current.Value;
						info.CenterPivotPoint = target.GetCenter(CenterType);

						_totalCenterPivotPoint += info.CenterPivotPoint;
					}

					_totalCenterPivotPoint /= _targetRoots.Count;

					if(CenterType == CenterType.Solo)
					{
						PivotPoint = _targetRoots[MainTargetRoot].CenterPivotPoint;
					}
					else if(CenterType == CenterType.All)
					{
						PivotPoint = _totalCenterPivotPoint;
					}
				}
			}
		}
		void SetPivotPointOffset(Vector3 offset)
		{
			PivotPoint += offset;
			_totalCenterPivotPoint += offset;
		}


		IEnumerator ForceUpdatePivotPointAtEndOfFrame()
		{
			while(this.enabled)
			{
				ForceUpdatePivotPointOnChange();
				yield return _waitForEndOfFrame;
			}
		}

		void ForceUpdatePivotPointOnChange()
		{
			if(forceUpdatePivotPointOnChange)
			{
				if(MainTargetRoot != null && !IsTransforming)
				{
					bool hasSet = false;
					Dictionary<Transform, TargetInfo>.Enumerator targets = _targetRoots.GetEnumerator();
					while(targets.MoveNext())
					{
						if(!hasSet)
						{
							if(targets.Current.Value.PreviousPosition != Vector3.zero && targets.Current.Key.position != targets.Current.Value.PreviousPosition)
							{
								SetPivotPoint();
								hasSet = true;
							}
						}

						targets.Current.Value.PreviousPosition = targets.Current.Key.position;
					}
				}
			}
		}

		public void SetTranslatingAxis(TransformType type, Axis axis, Axis planeAxis = Axis.None)
		{
			this._translatingType = type;
			this._nearAxis = axis;
			this._planeAxis = planeAxis;
		}

		public AxisInfo GetAxisInfo()
		{
			AxisInfo currentAxisInfo = _axisInfo;

			if(IsTransforming && GetProperTransformSpace() == TransformSpace.Global && _translatingType == TransformType.Rotate)
			{
				currentAxisInfo.XDirection = TotalRotationAmount * Vector3.right;
				currentAxisInfo.YDirection = TotalRotationAmount * Vector3.up;
				currentAxisInfo.ZDirection = TotalRotationAmount * Vector3.forward;
			}

			return currentAxisInfo;
		}

		void SetNearAxis()
		{
			if(IsTransforming) return;

			SetTranslatingAxis(TransformType, Axis.None);

			if(MainTargetRoot == null) return;

			float distanceMultiplier = GetDistanceMultiplier();
			float handleMinSelectedDistanceCheck = (this.MinSelectedDistanceCheck + HandleWidth) * distanceMultiplier;

			if(_nearAxis == Axis.None && (TransformTypeContains(TransformType.Move) || TransformTypeContains(TransformType.Scale)))
			{
				//Important to check scale lines before move lines since in TransformType.All the move planes would block the scales center scale all gizmo.
				if(_nearAxis == Axis.None && TransformTypeContains(TransformType.Scale))
				{
					float tipMinSelectedDistanceCheck = (this.MinSelectedDistanceCheck + BoxSize) * distanceMultiplier;
					HandleNearestPlanes(TransformType.Scale, _handleSquares, tipMinSelectedDistanceCheck);
				}

				if(_nearAxis == Axis.None && TransformTypeContains(TransformType.Move))
				{
					//Important to check the planes first before the handle tip since it makes selecting the planes easier.
					float planeMinSelectedDistanceCheck = (this.MinSelectedDistanceCheck + PlaneSize) * distanceMultiplier;
					HandleNearestPlanes(TransformType.Move, _handlePlanes, planeMinSelectedDistanceCheck);
						
					if(_nearAxis != Axis.None)
					{
						_planeAxis = _nearAxis;
					}
					else
					{
						float tipMinSelectedDistanceCheck = (this.MinSelectedDistanceCheck + TriangleSize) * distanceMultiplier;
						HandleNearestLines(TransformType.Move, _handleTriangles, tipMinSelectedDistanceCheck);
					}
				}

				if(_nearAxis == Axis.None)
				{
					//Since Move and Scale share the same handle line, we give Move the priority.
					TransformType transType = TransformType == TransformType.All ? TransformType.Move : TransformType;
					HandleNearestLines(transType, _handleLines, handleMinSelectedDistanceCheck);
				}
			}
			
			if(_nearAxis == Axis.None && TransformTypeContains(TransformType.Rotate))
			{
				HandleNearestLines(TransformType.Rotate, _circlesLines, handleMinSelectedDistanceCheck);
			}
		}

		void HandleNearestLines(TransformType type, AxisVectors axisVectors, float minSelectedDistanceCheck)
		{
			float xClosestDistance = ClosestDistanceFromMouseToLines(axisVectors.X);
			float yClosestDistance = ClosestDistanceFromMouseToLines(axisVectors.Y);
			float zClosestDistance = ClosestDistanceFromMouseToLines(axisVectors.Z);
			float allClosestDistance = ClosestDistanceFromMouseToLines(axisVectors.All);

			HandleNearest(type, xClosestDistance, yClosestDistance, zClosestDistance, allClosestDistance, minSelectedDistanceCheck);
		}

		void HandleNearestPlanes(TransformType type, AxisVectors axisVectors, float minSelectedDistanceCheck)
		{
			float xClosestDistance = ClosestDistanceFromMouseToPlanes(axisVectors.X);
			float yClosestDistance = ClosestDistanceFromMouseToPlanes(axisVectors.Y);
			float zClosestDistance = ClosestDistanceFromMouseToPlanes(axisVectors.Z);
			float allClosestDistance = ClosestDistanceFromMouseToPlanes(axisVectors.All);

			HandleNearest(type, xClosestDistance, yClosestDistance, zClosestDistance, allClosestDistance, minSelectedDistanceCheck);
		}

		void HandleNearest(TransformType type, float xClosestDistance, float yClosestDistance, float zClosestDistance, float allClosestDistance, float minSelectedDistanceCheck)
		{
			if(type == TransformType.Scale && allClosestDistance <= minSelectedDistanceCheck) SetTranslatingAxis(type, Axis.Any);
			else if(xClosestDistance <= minSelectedDistanceCheck && xClosestDistance <= yClosestDistance && xClosestDistance <= zClosestDistance) SetTranslatingAxis(type, Axis.X);
			else if(yClosestDistance <= minSelectedDistanceCheck && yClosestDistance <= xClosestDistance && yClosestDistance <= zClosestDistance) SetTranslatingAxis(type, Axis.Y);
			else if(zClosestDistance <= minSelectedDistanceCheck && zClosestDistance <= xClosestDistance && zClosestDistance <= yClosestDistance) SetTranslatingAxis(type, Axis.Z);
			else if(type == TransformType.Rotate && MainTargetRoot != null)
			{
				Ray mouseRay = MyCamera.ScreenPointToRay(Input.mousePosition);
				Vector3 mousePlaneHit = Geometry.LinePlaneIntersect(mouseRay.origin, mouseRay.direction, PivotPoint, (transform.position - PivotPoint).normalized);
				if((PivotPoint - mousePlaneHit).sqrMagnitude <= (GetHandleLength(TransformType.Rotate)).Squared()) SetTranslatingAxis(type, Axis.Any);
			}
		}

		float ClosestDistanceFromMouseToLines(List<Vector3> lines)
		{
			Ray mouseRay = MyCamera.ScreenPointToRay(Input.mousePosition);

			float closestDistance = float.MaxValue;
			for(int i = 0; i + 1 < lines.Count; i++)
			{
				IntersectPoints points = Geometry.ClosestPointsOnSegmentToLine(lines[i], lines[i + 1], mouseRay.origin, mouseRay.direction);
				float distance = Vector3.Distance(points.First, points.Second);
				if(distance < closestDistance)
				{
					closestDistance = distance;
				}
			}
			return closestDistance;
		}

		float ClosestDistanceFromMouseToPlanes(List<Vector3> planePoints)
		{
			float closestDistance = float.MaxValue;

			if(planePoints.Count >= 4)
			{
				Ray mouseRay = MyCamera.ScreenPointToRay(Input.mousePosition);

				for(int i = 0; i < planePoints.Count; i += 4)
				{
					Plane plane = new Plane(planePoints[i], planePoints[i + 1], planePoints[i + 2]);

					float distanceToPlane;
					if(plane.Raycast(mouseRay, out distanceToPlane))
					{
						Vector3 pointOnPlane = mouseRay.origin + (mouseRay.direction * distanceToPlane);
						Vector3 planeCenter = (planePoints[0] + planePoints[1] + planePoints[2] + planePoints[3]) / 4f;

						float distance = Vector3.Distance(planeCenter, pointOnPlane);
						if(distance < closestDistance)
						{
							closestDistance = distance;
						}
					}
				}
			}

			return closestDistance;
		}

		//float DistanceFromMouseToPlane(List<Vector3> planeLines)
		//{
		//	if(planeLines.Count >= 4)
		//	{
		//		Ray mouseRay = myCamera.ScreenPointToRay(Input.mousePosition);
		//		Plane plane = new Plane(planeLines[0], planeLines[1], planeLines[2]);

		//		float distanceToPlane;
		//		if(plane.Raycast(mouseRay, out distanceToPlane))
		//		{
		//			Vector3 pointOnPlane = mouseRay.origin + (mouseRay.direction * distanceToPlane);
		//			Vector3 planeCenter = (planeLines[0] + planeLines[1] + planeLines[2] + planeLines[3]) / 4f;

		//			return Vector3.Distance(planeCenter, pointOnPlane);
		//		}
		//	}

		//	return float.MaxValue;
		//}

		void SetAxisInfo()
		{
			if(MainTargetRoot != null)
			{
				_axisInfo.Set(MainTargetRoot, PivotPoint, GetProperTransformSpace());
			}
		}

		//This helps keep the size consistent no matter how far we are from it.
		public float GetDistanceMultiplier()
		{
			if(MainTargetRoot == null) return 0f;

			if(MyCamera.orthographic) return Mathf.Max(.01f, MyCamera.orthographicSize * 2f);
			return Mathf.Max(.01f, Mathf.Abs(ExtVector3.MagnitudeInDirection(PivotPoint - transform.position, MyCamera.transform.forward)));
		}

		void SetLines()
		{
			SetHandleLines();
			SetHandlePlanes();
			SetHandleTriangles();
			SetHandleSquares();
			SetCircles(GetAxisInfo(), _circlesLines);
		}

		void SetHandleLines()
		{
			_handleLines.Clear();

			if(TranslatingTypeContains(TransformType.Move) || TranslatingTypeContains(TransformType.Scale))
			{
				float lineWidth = HandleWidth * GetDistanceMultiplier();

				float xLineLength = 0;
				float yLineLength = 0;
				float zLineLength = 0;
				if(TranslatingTypeContains(TransformType.Move))
				{
					xLineLength = yLineLength = zLineLength = GetHandleLength(TransformType.Move);
				}
				else if(TranslatingTypeContains(TransformType.Scale))
				{
					xLineLength = GetHandleLength(TransformType.Scale, Axis.X);
					yLineLength = GetHandleLength(TransformType.Scale, Axis.Y);
					zLineLength = GetHandleLength(TransformType.Scale, Axis.Z);
				}

				AddQuads(PivotPoint, _axisInfo.XDirection, _axisInfo.YDirection, _axisInfo.ZDirection, xLineLength, lineWidth, _handleLines.X);
				AddQuads(PivotPoint, _axisInfo.YDirection, _axisInfo.XDirection, _axisInfo.ZDirection, yLineLength, lineWidth, _handleLines.Y);
				AddQuads(PivotPoint, _axisInfo.ZDirection, _axisInfo.XDirection, _axisInfo.YDirection, zLineLength, lineWidth, _handleLines.Z);
			}
		}
		int AxisDirectionMultiplier(Vector3 direction, Vector3 otherDirection)
		{
			return ExtVector3.IsInDirection(direction, otherDirection) ? 1 : -1;
		}

		void SetHandlePlanes()
		{
			_handlePlanes.Clear();

			if(TranslatingTypeContains(TransformType.Move))
			{
				Vector3 pivotToCamera = MyCamera.transform.position - PivotPoint;
				float cameraXSign = Mathf.Sign(Vector3.Dot(_axisInfo.XDirection, pivotToCamera));
				float cameraYSign = Mathf.Sign(Vector3.Dot(_axisInfo.YDirection, pivotToCamera));
				float cameraZSign = Mathf.Sign(Vector3.Dot(_axisInfo.ZDirection, pivotToCamera));

				float planeSize = this.PlaneSize;
				if(TransformType == TransformType.All) { planeSize *= AllMoveHandleLengthMultiplier; }
				planeSize *= GetDistanceMultiplier();

				Vector3 xDirection = (_axisInfo.XDirection * planeSize) * cameraXSign;
				Vector3 yDirection = (_axisInfo.YDirection * planeSize) * cameraYSign;
				Vector3 zDirection = (_axisInfo.ZDirection * planeSize) * cameraZSign;

				Vector3 xPlaneCenter = PivotPoint + (yDirection + zDirection);
				Vector3 yPlaneCenter = PivotPoint + (xDirection + zDirection);
				Vector3 zPlaneCenter = PivotPoint + (xDirection + yDirection);

				AddQuad(xPlaneCenter, _axisInfo.YDirection, _axisInfo.ZDirection, planeSize, _handlePlanes.X);
				AddQuad(yPlaneCenter, _axisInfo.XDirection, _axisInfo.ZDirection, planeSize, _handlePlanes.Y);
				AddQuad(zPlaneCenter, _axisInfo.XDirection, _axisInfo.YDirection, planeSize, _handlePlanes.Z);
			}
		}

		void SetHandleTriangles()
		{
			_handleTriangles.Clear();

			if(TranslatingTypeContains(TransformType.Move))
			{
				float triangleLength = TriangleSize * GetDistanceMultiplier();
				AddTriangles(_axisInfo.GetXAxisEnd(GetHandleLength(TransformType.Move)), _axisInfo.XDirection, _axisInfo.YDirection, _axisInfo.ZDirection, triangleLength, _handleTriangles.X);
				AddTriangles(_axisInfo.GetYAxisEnd(GetHandleLength(TransformType.Move)), _axisInfo.YDirection, _axisInfo.XDirection, _axisInfo.ZDirection, triangleLength, _handleTriangles.Y);
				AddTriangles(_axisInfo.GetZAxisEnd(GetHandleLength(TransformType.Move)), _axisInfo.ZDirection, _axisInfo.YDirection, _axisInfo.XDirection, triangleLength, _handleTriangles.Z);
			}
		}

		void AddTriangles(Vector3 axisEnd, Vector3 axisDirection, Vector3 axisOtherDirection1, Vector3 axisOtherDirection2, float size, List<Vector3> resultsBuffer)
		{
			Vector3 endPoint = axisEnd + (axisDirection * (size * 2f));
			Square baseSquare = GetBaseSquare(axisEnd, axisOtherDirection1, axisOtherDirection2, size / 2f);

			resultsBuffer.Add(baseSquare.BottomLeft);
			resultsBuffer.Add(baseSquare.TopLeft);
			resultsBuffer.Add(baseSquare.TopRight);
			resultsBuffer.Add(baseSquare.TopLeft);
			resultsBuffer.Add(baseSquare.BottomRight);
			resultsBuffer.Add(baseSquare.TopRight);

			for(int i = 0; i < 4; i++)
			{
				resultsBuffer.Add(baseSquare[i]);
				resultsBuffer.Add(baseSquare[i + 1]);
				resultsBuffer.Add(endPoint);
			}
		}

		void SetHandleSquares()
		{
			_handleSquares.Clear();

			if(TranslatingTypeContains(TransformType.Scale))
			{
				float boxSize = this.BoxSize * GetDistanceMultiplier();
				AddSquares(_axisInfo.GetXAxisEnd(GetHandleLength(TransformType.Scale, Axis.X)), _axisInfo.XDirection, _axisInfo.YDirection, _axisInfo.ZDirection, boxSize, _handleSquares.X);
				AddSquares(_axisInfo.GetYAxisEnd(GetHandleLength(TransformType.Scale, Axis.Y)), _axisInfo.YDirection, _axisInfo.XDirection, _axisInfo.ZDirection, boxSize, _handleSquares.Y);
				AddSquares(_axisInfo.GetZAxisEnd(GetHandleLength(TransformType.Scale, Axis.Z)), _axisInfo.ZDirection, _axisInfo.XDirection, _axisInfo.YDirection, boxSize, _handleSquares.Z);
				AddSquares(PivotPoint - (_axisInfo.XDirection * (boxSize * .5f)), _axisInfo.XDirection, _axisInfo.YDirection, _axisInfo.ZDirection, boxSize, _handleSquares.All);
			}
		}

		void AddSquares(Vector3 axisStart, Vector3 axisDirection, Vector3 axisOtherDirection1, Vector3 axisOtherDirection2, float size, List<Vector3> resultsBuffer)
		{
			AddQuads(axisStart, axisDirection, axisOtherDirection1, axisOtherDirection2, size, size * .5f, resultsBuffer);
		}
		void AddQuads(Vector3 axisStart, Vector3 axisDirection, Vector3 axisOtherDirection1, Vector3 axisOtherDirection2, float length, float width, List<Vector3> resultsBuffer)
		{
			Vector3 axisEnd = axisStart + (axisDirection * length);
			AddQuads(axisStart, axisEnd, axisOtherDirection1, axisOtherDirection2, width, resultsBuffer);
		}
		void AddQuads(Vector3 axisStart, Vector3 axisEnd, Vector3 axisOtherDirection1, Vector3 axisOtherDirection2, float width, List<Vector3> resultsBuffer)
		{
			Square baseRectangle = GetBaseSquare(axisStart, axisOtherDirection1, axisOtherDirection2, width);
			Square baseRectangleEnd = GetBaseSquare(axisEnd, axisOtherDirection1, axisOtherDirection2, width);

			resultsBuffer.Add(baseRectangle.BottomLeft);
			resultsBuffer.Add(baseRectangle.TopLeft);
			resultsBuffer.Add(baseRectangle.TopRight);
			resultsBuffer.Add(baseRectangle.BottomRight);

			resultsBuffer.Add(baseRectangleEnd.BottomLeft);
			resultsBuffer.Add(baseRectangleEnd.TopLeft);
			resultsBuffer.Add(baseRectangleEnd.TopRight);
			resultsBuffer.Add(baseRectangleEnd.BottomRight);

			for(int i = 0; i < 4; i++)
			{
				resultsBuffer.Add(baseRectangle[i]);
				resultsBuffer.Add(baseRectangleEnd[i]);
				resultsBuffer.Add(baseRectangleEnd[i + 1]);
				resultsBuffer.Add(baseRectangle[i + 1]);
			}
		}

		void AddQuad(Vector3 axisStart, Vector3 axisOtherDirection1, Vector3 axisOtherDirection2, float width, List<Vector3> resultsBuffer)
		{
			Square baseRectangle = GetBaseSquare(axisStart, axisOtherDirection1, axisOtherDirection2, width);

			resultsBuffer.Add(baseRectangle.BottomLeft);
			resultsBuffer.Add(baseRectangle.TopLeft);
			resultsBuffer.Add(baseRectangle.TopRight);
			resultsBuffer.Add(baseRectangle.BottomRight);
		}

		Square GetBaseSquare(Vector3 axisEnd, Vector3 axisOtherDirection1, Vector3 axisOtherDirection2, float size)
		{
			Square square;
			Vector3 offsetUp = ((axisOtherDirection1 * size) + (axisOtherDirection2 * size));
			Vector3 offsetDown = ((axisOtherDirection1 * size) - (axisOtherDirection2 * size));
			//These might not really be the proper directions, as in the bottomLeft might not really be at the bottom left...
			square.BottomLeft = axisEnd + offsetDown;
			square.TopLeft = axisEnd + offsetUp;
			square.BottomRight = axisEnd - offsetUp;
			square.TopRight = axisEnd - offsetDown;
			return square;
		}

		void SetCircles(AxisInfo axisInfo, AxisVectors axisVectors)
		{
			axisVectors.Clear();

			if(TranslatingTypeContains(TransformType.Rotate))
			{
				float circleLength = GetHandleLength(TransformType.Rotate);
				AddCircle(PivotPoint, axisInfo.XDirection, circleLength, axisVectors.X);
				AddCircle(PivotPoint, axisInfo.YDirection, circleLength, axisVectors.Y);
				AddCircle(PivotPoint, axisInfo.ZDirection, circleLength, axisVectors.Z);
				AddCircle(PivotPoint, (PivotPoint - transform.position).normalized, circleLength, axisVectors.All, false);
			}
		}

		void AddCircle(Vector3 origin, Vector3 axisDirection, float size, List<Vector3> resultsBuffer, bool depthTest = true)
		{
			Vector3 up = axisDirection.normalized * size;
			Vector3 forward = Vector3.Slerp(up, -up, .5f);
			Vector3 right = Vector3.Cross(up, forward).normalized * size;
		
			Matrix4x4 matrix = new Matrix4x4();
		
			matrix[0] = right.x;
			matrix[1] = right.y;
			matrix[2] = right.z;
		
			matrix[4] = up.x;
			matrix[5] = up.y;
			matrix[6] = up.z;
		
			matrix[8] = forward.x;
			matrix[9] = forward.y;
			matrix[10] = forward.z;
		
			Vector3 lastPoint = origin + matrix.MultiplyPoint3x4(new Vector3(Mathf.Cos(0), 0, Mathf.Sin(0)));
			Vector3 nextPoint = Vector3.zero;
			float multiplier = 360f / CircleDetail;

			Plane plane = new Plane((transform.position - PivotPoint).normalized, PivotPoint);

			float circleHandleWidth = HandleWidth * GetDistanceMultiplier();

			for(int i = 0; i < CircleDetail + 1; i++)
			{
				nextPoint.x = Mathf.Cos((i * multiplier) * Mathf.Deg2Rad);
				nextPoint.z = Mathf.Sin((i * multiplier) * Mathf.Deg2Rad);
				nextPoint.y = 0;
			
				nextPoint = origin + matrix.MultiplyPoint3x4(nextPoint);
			
				if(!depthTest || plane.GetSide(lastPoint))
				{
					Vector3 centerPoint = (lastPoint + nextPoint) * .5f;
					Vector3 upDirection = (centerPoint - origin).normalized;
					AddQuads(lastPoint, nextPoint, upDirection, axisDirection, circleHandleWidth, resultsBuffer);
				}

				lastPoint = nextPoint;
			}
		}

		void DrawLines(List<Vector3> lines, Color color)
		{
			GL.Begin(GL.LINES);
			GL.Color(color);

			for(int i = 0; i < lines.Count; i += 2)
			{
				GL.Vertex(lines[i]);
				GL.Vertex(lines[i + 1]);
			}

			GL.End();
		}

		void DrawTriangles(List<Vector3> lines, Color color)
		{
			GL.Begin(GL.TRIANGLES);
			GL.Color(color);

			for(int i = 0; i < lines.Count; i += 3)
			{
				GL.Vertex(lines[i]);
				GL.Vertex(lines[i + 1]);
				GL.Vertex(lines[i + 2]);
			}

			GL.End();
		}

		void DrawQuads(List<Vector3> lines, Color color)
		{
			GL.Begin(GL.QUADS);
			GL.Color(color);

			for(int i = 0; i < lines.Count; i += 4)
			{
				GL.Vertex(lines[i]);
				GL.Vertex(lines[i + 1]);
				GL.Vertex(lines[i + 2]);
				GL.Vertex(lines[i + 3]);
			}

			GL.End();
		}

		void DrawFilledCircle(List<Vector3> lines, Color color)
		{
			Vector3 center = Vector3.zero;
			for(int i = 0; i < lines.Count; i++)
			{
				center += lines[i];
			}
			center /= lines.Count;

			GL.Begin(GL.TRIANGLES);
			GL.Color(color);

			for(int i = 0; i + 1 < lines.Count; i++)
			{
				GL.Vertex(lines[i]);
				GL.Vertex(lines[i + 1]);
				GL.Vertex(center);
			}

			GL.End();
		}

		void SetMaterial()
		{
			if(_lineMaterial == null)
			{
				_lineMaterial = new Material(Shader.Find("Custom/Lines"));
				_outlineMaterial = new Material(Shader.Find("Custom/Outline"));
			}
		}
	}
}
