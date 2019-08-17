using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleToolMovement : MonoBehaviour {
	private Camera activeCam;
	private SceneMgr thisSceneMgr;
	float dist, farPoint=30;
	public bool isBeingCreated=false;
	Ray  mousRay;
	Vector3 offSet=Vector3.zero, target=Vector3.zero;
	void Start () {
		activeCam=GameObject.FindObjectOfType<Camera>();
		thisSceneMgr=FindObjectOfType<SceneMgr>();	

	}
	
void OnMouseUp()
{
	isBeingCreated=false;
	
}

void OnMouseDown(){
	thisSceneMgr.SelectionHandler(gameObject);
}

public  void OnMouseDrag(){
		if(!isBeingCreated)
		return;
			
			activeCam=GameObject.FindObjectOfType<Camera>();
			mousRay= activeCam.ScreenPointToRay(Input.mousePosition);
			RaycastHit[] mousLineObjects= Physics.RaycastAll(mousRay);

				if(mousLineObjects.Length>1)
				{
						if(mousLineObjects[0].transform.name!=gameObject.name){
							target= mousLineObjects[0].point;
						}else{
							target= mousLineObjects[1].point;
						}
						
				}else if(mousLineObjects.Length>0){
					target= mousLineObjects[0].point;
				}else{
					target=activeCam.transform.position+activeCam.transform.forward*farPoint;	
				}
		

			dist=Vector3.Distance(activeCam.transform.position, target);
				if (dist>farPoint)
				{
					dist=farPoint;
				}

				transform.position=activeCam.transform.position+offSet+ mousRay.direction*dist;
				Vector3 pos = transform.position;
				pos.y =pos.y <= GetComponent<Collider>().bounds.extents.y?GetComponent<Collider>().bounds.extents.y:pos.y;
			//pos=	GetComponent<Renderer>().bounds.min;
				transform.position = pos;

		}

// End of file
}
