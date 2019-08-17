using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleCameraCtrl : MonoBehaviour {

	public float PanSpeed=20f;
	public float zoomSpeed=20f;
	public float rotateSpeed=20f;

	public static bool isSelectOn=true, isPanOn= false, isZoomOn=false, isRotateOn=false;
	public Vector3 defaultPosition;
	public Vector3 offsetPos;
		Vector3 lastPos;
	public Camera thisCamera;
	//public Vector3 defaultRotation;
	public Quaternion defaultRotation;

	
	void Start () {
		lastPos=Input.mousePosition;
		thisCamera= gameObject.GetComponent<Camera>();
		defaultPosition=transform.position;
		defaultRotation= transform.rotation;
	}
	
	void Update(){

		PanTheView();
		zoomTheView();
			if(gameObject.name=="Free Camera"){
				rotateTheView();
			}
		}

	private void PanTheView(){

		if((Input.GetMouseButtonDown(2) && !Input.GetKey("left alt")) || (isPanOn && Input.GetMouseButtonDown(0))){
					
				lastPos=Input.mousePosition;// transform.localPosition-thisCamera.ScreenToWorldPoint(Input.mousePosition);

		}else if((Input.GetMouseButton(2) && !Input.GetKey("left alt")) || (isPanOn && Input.GetMouseButton(0))){
				offsetPos= thisCamera.ScreenToViewportPoint(lastPos-Input.mousePosition);
				Vector3 move = new Vector3(offsetPos.x * PanSpeed,  offsetPos.y * PanSpeed, 0);
				transform.Translate(move, Space.Self);

				Vector3 pos = transform.position;
				pos.x = Mathf.Clamp(transform.position.x, -100, 100);
				pos.y = Mathf.Clamp(transform.position.y, 0.3f, 100);
				pos.z = Mathf.Clamp(transform.position.z, -100, 100);
				transform.position = pos;
				//transform.Translate()
				lastPos=Input.mousePosition;
	
		}
		
	}

private void zoomTheView(){

	
		if((Input.GetMouseButtonDown(1) && Input.GetKey("left alt") ) || (isZoomOn && Input.GetMouseButtonDown(0))){
					
				lastPos=Input.mousePosition;// transform.localPosition-thisCamera.ScreenToWorldPoint(Input.mousePosition);
				
		}else if((Input.GetMouseButton(1) && Input.GetKey("left alt") ) || (isZoomOn && Input.GetMouseButton(0)) ){
				offsetPos= thisCamera.ScreenToViewportPoint(lastPos-Input.mousePosition);
			
				float dY=offsetPos.y;
	
				transform.position-=transform.forward*dY*20;

				Vector3 pos = transform.position;
				pos.x = Mathf.Clamp(transform.position.x, -100, 100);
				pos.y = Mathf.Clamp(transform.position.y, 0.3f, 100);
				pos.z = Mathf.Clamp(transform.position.z, -100, 100);
				transform.position = pos;

				lastPos=Input.mousePosition;
			}

	}



	private void rotateTheView(){
		Vector3 mousPosWorld= thisCamera.ScreenToViewportPoint(lastPos);
		if((Input.GetMouseButtonDown(2) && Input.GetKey("left alt") ) || (isRotateOn && Input.GetMouseButtonDown(0))){

			
					
				lastPos=Input.mousePosition;// transform.localPosition-thisCamera.ScreenToWorldPoint(Input.mousePosition);
				mousPosWorld= thisCamera.ScreenToViewportPoint(lastPos);	
		}else if((Input.GetMouseButton(2) && Input.GetKey("left alt")  ) || (isRotateOn && Input.GetMouseButton(0))){
				offsetPos= thisCamera.ScreenToViewportPoint(lastPos-Input.mousePosition);
				float dX=offsetPos.x;
				float dY=offsetPos.y;
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
				transform.RotateAround(target, transform.right, rotateSpeed*dY);
				transform.RotateAround(target, transform.up, -rotateSpeed*dX);

				}else
				{

					Vector3 dragVect= new Vector3(dX,dY,0);
					Vector3 crossVect=Vector3.Cross(mosPosVect, dragVect);
					float angleSign= crossVect.z==0 ? 0 : crossVect.z/ Mathf.Abs(crossVect.z);
					float rotAngle= Vector3.Cross(mosPosVect, dragVect).magnitude*angleSign/mosPosVect.magnitude;
					//	Debug.Log(mosPosVect);
					transform.RotateAround(target, transform.forward, rotateSpeed*rotAngle);
				}
				lastPos=Input.mousePosition;

			}
		}


}
