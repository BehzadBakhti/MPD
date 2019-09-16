using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleCameraCtrl : MonoBehaviour {

	public float PanSpeed=20f;
	public float ZoomSpeed=20f;
	public float RotateSpeed=20f;

	public static bool IsSelectOn=true, IsPanOn= false, IsZoomOn=false, IsRotateOn=false;
	public Vector3 DefaultPosition;
	public Vector3 OffsetPos;
		Vector3 _lastPos;
	public Camera ThisCamera;
	//public Vector3 defaultRotation;
	public Quaternion DefaultRotation;

	
	void Start () {
		_lastPos=Input.mousePosition;
		ThisCamera= gameObject.GetComponent<Camera>();
		DefaultPosition=transform.position;
		DefaultRotation= transform.rotation;
	}
	
	void Update(){

		PanTheView();
		ZoomTheView();
			if(gameObject.name=="Free Camera"){
				RotateTheView();
			}
		}

	private void PanTheView(){

		if((Input.GetMouseButtonDown(2) && !Input.GetKey("left alt")) || (IsPanOn && Input.GetMouseButtonDown(0))){
					
				_lastPos=Input.mousePosition;// transform.localPosition-thisCamera.ScreenToWorldPoint(Input.mousePosition);

		}else if((Input.GetMouseButton(2) && !Input.GetKey("left alt")) || (IsPanOn && Input.GetMouseButton(0))){
				OffsetPos= ThisCamera.ScreenToViewportPoint(_lastPos-Input.mousePosition);
				Vector3 move = new Vector3(OffsetPos.x * PanSpeed,  OffsetPos.y * PanSpeed, 0);
				transform.Translate(move, Space.Self);

				Vector3 pos = transform.position;
				pos.x = Mathf.Clamp(transform.position.x, -100, 100);
				pos.y = Mathf.Clamp(transform.position.y, 0.3f, 100);
				pos.z = Mathf.Clamp(transform.position.z, -100, 100);
				transform.position = pos;
				//transform.Translate()
				_lastPos=Input.mousePosition;
	
		}
		
	}

private void ZoomTheView(){

	
		if((Input.GetMouseButtonDown(1) && Input.GetKey("left alt") ) || (IsZoomOn && Input.GetMouseButtonDown(0))){
					
				_lastPos=Input.mousePosition;// transform.localPosition-thisCamera.ScreenToWorldPoint(Input.mousePosition);
				
		}else if((Input.GetMouseButton(1) && Input.GetKey("left alt") ) || (IsZoomOn && Input.GetMouseButton(0)) ){
				OffsetPos= ThisCamera.ScreenToViewportPoint(_lastPos-Input.mousePosition);
			
				float dY=OffsetPos.y;
	
				transform.position-=transform.forward*dY*20;

				Vector3 pos = transform.position;
				pos.x = Mathf.Clamp(transform.position.x, -100, 100);
				pos.y = Mathf.Clamp(transform.position.y, 0.3f, 100);
				pos.z = Mathf.Clamp(transform.position.z, -100, 100);
				transform.position = pos;

				_lastPos=Input.mousePosition;
			}

	}



	private void RotateTheView(){
		Vector3 mousPosWorld= ThisCamera.ScreenToViewportPoint(_lastPos);
		if((Input.GetMouseButtonDown(2) && Input.GetKey("left alt") ) || (IsRotateOn && Input.GetMouseButtonDown(0))){

			
					
				_lastPos=Input.mousePosition;// transform.localPosition-thisCamera.ScreenToWorldPoint(Input.mousePosition);
				mousPosWorld= ThisCamera.ScreenToViewportPoint(_lastPos);	
		}else if((Input.GetMouseButton(2) && Input.GetKey("left alt")  ) || (IsRotateOn && Input.GetMouseButton(0))){
				OffsetPos= ThisCamera.ScreenToViewportPoint(_lastPos-Input.mousePosition);
				float dX=OffsetPos.x;
				float dY=OffsetPos.y;
				Vector3 target;
		Ray camRay= new Ray(transform.position, transform.forward);
		RaycastHit hit;
			if(Physics.Raycast(camRay, out hit))
			{
				target= hit.transform.position;
			}else
			{
				target=transform.position+transform.forward*20;
			}
				Vector3 mosPosVect= new Vector3(mousPosWorld.x-0.5f, mousPosWorld.y-0.5f,0);
			
				if(mosPosVect.magnitude<0.25){
				transform.RotateAround(target, transform.right, RotateSpeed*dY);
				transform.RotateAround(target, transform.up, -RotateSpeed*dX);

				}else
				{

					Vector3 dragVect= new Vector3(dX,dY,0);
					Vector3 crossVect=Vector3.Cross(mosPosVect, dragVect);
					float angleSign= crossVect.z==0 ? 0 : crossVect.z/ Mathf.Abs(crossVect.z);
					float rotAngle= Vector3.Cross(mosPosVect, dragVect).magnitude*angleSign/mosPosVect.magnitude;
					//	Debug.Log(mosPosVect);
					transform.RotateAround(target, transform.forward, RotateSpeed*rotAngle);
				}
				_lastPos=Input.mousePosition;

			}
		}


}
