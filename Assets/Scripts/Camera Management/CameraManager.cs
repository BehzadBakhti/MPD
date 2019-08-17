using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class CameraManager : MonoBehaviour {

	// Use this for initialization
	public Camera freeCam, topCam, leftCam;
	public ToggleGroup viewTogglegroup, toolsTogglegroup;
	
    public Texture2D cursorPan, cursorZoom, cursorRotateCenter, cursorRotateAround, cursorSelect, cursorForbid;
	Texture2D activeCurser;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;
	string currentView;
	Camera activeCam;
	
	
	// Update is called once per frame
	public void CameraChange() {
		//Toggle currentView;
		currentView= viewTogglegroup.ActiveToggles().FirstOrDefault().gameObject.name;
		switch (currentView)
		{
			case "Free":
				freeCam.gameObject.SetActive(true);
				topCam.gameObject.SetActive(false);
				leftCam.gameObject.SetActive(false);

				break;
			case "Left":
				freeCam.gameObject.SetActive(false);
				topCam.gameObject.SetActive(false);
				leftCam.gameObject.SetActive(true);

				break;
			case "Top":
				freeCam.gameObject.SetActive(false);
				topCam.gameObject.SetActive(true);
				leftCam.gameObject.SetActive(false);
				break;
		}
		
	}

	public void ToolChange() {
		//Toggle currentView;
		var activeTool= toolsTogglegroup.ActiveToggles().FirstOrDefault().gameObject.name;
		switch (activeTool)
		{
			case "Select":
				SingleCameraCtrl.isPanOn=false;
				SingleCameraCtrl.isRotateOn=false;
				SingleCameraCtrl.isZoomOn=false;
				SingleCameraCtrl.isSelectOn=true;
				activeCurser=null;

				break;
			case "Pan":
				SingleCameraCtrl.isPanOn=true;
				SingleCameraCtrl.isRotateOn=false;
				SingleCameraCtrl.isZoomOn=false;
				SingleCameraCtrl.isSelectOn=false;
				activeCurser=cursorPan;

				break;
			case "Zoom":
				SingleCameraCtrl.isPanOn=false;
				SingleCameraCtrl.isRotateOn=false;
				SingleCameraCtrl.isZoomOn=true;
				SingleCameraCtrl.isSelectOn=false;
				activeCurser= cursorZoom;
				break;

			case "Rotate":
				SingleCameraCtrl.isPanOn=false;
				SingleCameraCtrl.isRotateOn=true;
				SingleCameraCtrl.isZoomOn=false;
				SingleCameraCtrl.isSelectOn=false;
				if(currentView=="Free"){
					activeCurser=cursorRotateAround;	
				}else{
					activeCurser=cursorForbid;
				}
				
				break;
		}
hotSpot=activeCurser==null?Vector2.zero: new Vector2(activeCurser.width*0.5f, activeCurser.height*0.5F);
Cursor.SetCursor(activeCurser, hotSpot, cursorMode);

		
	}


	public void DefaultCurser(){
		Cursor.SetCursor(null, hotSpot, cursorMode);

	}

	public void CurrentCurser(){
		Cursor.SetCursor(activeCurser, Vector2.zero, cursorMode);

	}

}
