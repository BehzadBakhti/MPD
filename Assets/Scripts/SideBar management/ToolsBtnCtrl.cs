using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolsBtnCtrl : MonoBehaviour {
	private SceneMgr thisSceneMgr;
	private Camera activeCam;

	public Image thumbNail;
	public GameObject toolModel;
	public GameObject createdObj;
	float dist, farPoint=30;
	Ray camRay, mousRay;
	RaycastHit hit;
	Vector3 target=Vector3.zero;
	void Start()
	{
	thisSceneMgr=FindObjectOfType<SceneMgr>();	
	}
	public void BeginDrag(){
		activeCam=GameObject.FindObjectOfType<Camera>(); //#################################################################
		camRay= new Ray(activeCam.transform.position, activeCam.transform.forward);
			if(Physics.Raycast(camRay, out hit))
			{
				target= hit.transform.position;
				//Debug.Log("Hit");
			}else{
				target=activeCam.transform.position+activeCam.transform.forward*farPoint;
			}

		dist=Vector3.Distance(activeCam.transform.position, target)/Mathf.Cos((activeCam.fieldOfView)*Mathf.PI/360);
		mousRay= activeCam.ScreenPointToRay(Input.mousePosition);
		Vector3 ObjectPos=activeCam.transform.position+ mousRay.direction*dist;
		createdObj=	Instantiate(toolModel,ObjectPos,Quaternion.identity);
		//createdObj.GetComponent<SingleToolStatus>().thisSceneMgr=thisSceneMgr;
		createdObj.GetComponent<SingleToolMovement>().isBeingCreated=true;
	}


	 public void Dragging(){
		createdObj.GetComponent<SingleToolMovement>().OnMouseDrag();
	}
	

	public void EndDrag(){
		thisSceneMgr.SelectionHandler(createdObj);

	}
}
