using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class CameraManager : MonoBehaviour {

	// Use this for initialization
	public Camera FreeCam, TopCam, LeftCam;
	public ToggleGroup ViewToggleGroup, ToolsToggleGroup;
	
    public Texture2D CursorPan, CursorZoom, CursorRotateCenter, CursorRotateAround, CursorSelect, CursorForbid;
	Texture2D _activeCursor;
    public CursorMode CursorMode = CursorMode.Auto;
    public Vector2 HotSpot = Vector2.zero;
	string _currentView;

    public Camera ActiveCam { get; private set; }

    private void Start()
    {
        ActiveCam = FreeCam;
    }
    // Update is called once per frame
	public void CameraChange() {
		//Toggle currentView;
		_currentView= ViewToggleGroup.ActiveToggles().FirstOrDefault()?.gameObject.name;
		switch (_currentView)
		{
			case "Free":
                ActiveCam = FreeCam;
				FreeCam.gameObject.SetActive(true);
				TopCam.gameObject.SetActive(false);
				LeftCam.gameObject.SetActive(false);

				break;
			case "Left":
                ActiveCam = LeftCam;
                FreeCam.gameObject.SetActive(false);
				TopCam.gameObject.SetActive(false);
				LeftCam.gameObject.SetActive(true);

				break;
			case "Top":
                ActiveCam = TopCam;
                FreeCam.gameObject.SetActive(false);
				TopCam.gameObject.SetActive(true);
				LeftCam.gameObject.SetActive(false);
				break;
		}
		
	}

	public void ToolChange() {
		//Toggle currentView;
		var activeTool= ToolsToggleGroup.ActiveToggles().FirstOrDefault()?.gameObject.name;
		switch (activeTool)
		{
			case "Select":
				SingleCameraCtrl.IsPanOn=false;
				SingleCameraCtrl.IsRotateOn=false;
				SingleCameraCtrl.IsZoomOn=false;
				SingleCameraCtrl.IsSelectOn=true;
				_activeCursor=null;

				break;
			case "Pan":
				SingleCameraCtrl.IsPanOn=true;
				SingleCameraCtrl.IsRotateOn=false;
				SingleCameraCtrl.IsZoomOn=false;
				SingleCameraCtrl.IsSelectOn=false;
				_activeCursor=CursorPan;

				break;
			case "Zoom":
				SingleCameraCtrl.IsPanOn=false;
				SingleCameraCtrl.IsRotateOn=false;
				SingleCameraCtrl.IsZoomOn=true;
				SingleCameraCtrl.IsSelectOn=false;
				_activeCursor= CursorZoom;
				break;

			case "Rotate":
				SingleCameraCtrl.IsPanOn=false;
				SingleCameraCtrl.IsRotateOn=true;
				SingleCameraCtrl.IsZoomOn=false;
				SingleCameraCtrl.IsSelectOn=false;
				_activeCursor = _currentView=="Free" ? CursorRotateAround : CursorForbid;
				
				break;
		}
        HotSpot=_activeCursor==null?Vector2.zero: new Vector2(_activeCursor.width*0.5f, _activeCursor.height*0.5F);
        Cursor.SetCursor(_activeCursor, HotSpot, CursorMode);

		
	}


	public void DefaultCurser(){
		Cursor.SetCursor(null, HotSpot, CursorMode);

	}

	public void CurrentCurser(){
		Cursor.SetCursor(_activeCursor, Vector2.zero, CursorMode);

	}

}
