using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleToolMovement : MonoBehaviour {
	private Camera _activeCam;
	private SceneMgr _thisSceneMgr;
	float _dist, _farPoint=30;
	public bool IsBeingCreated=false;
	Ray  _mousRay;
	Vector3 _offSet=Vector3.zero, _target=Vector3.zero;
	void Start () {
		_activeCam=GameObject.FindObjectOfType<Camera>();
		_thisSceneMgr=FindObjectOfType<SceneMgr>();	

	}
	
void OnMouseUp()
{
	IsBeingCreated=false;
	
}

void OnMouseDown(){
	_thisSceneMgr.SelectionHandler(gameObject);
}

public  void OnMouseDrag(){
		//if(!IsBeingCreated)
  //          return;
			
  //      _activeCam=GameObject.FindObjectOfType<Camera>();
  //      _mousRay= _activeCam.ScreenPointToRay(Input.mousePosition);
  //      RaycastHit[] mousLineObjects= Physics.RaycastAll(_mousRay);

  //      if(mousLineObjects.Length>1)
  //      {
  //          if(mousLineObjects[0].transform.name!=gameObject.name){
  //              _target= mousLineObjects[0].point;
  //          }else{
  //              _target= mousLineObjects[1].point;
  //          }
						
  //      }else if(mousLineObjects.Length>0){
  //          _target= mousLineObjects[0].point;
  //      }else{
  //          _target=_activeCam.transform.position+_activeCam.transform.forward*_farPoint;	
  //      }
		

  //      _dist=Vector3.Distance(_activeCam.transform.position, _target);
  //      if (_dist>_farPoint)
  //      {
  //          _dist=_farPoint;
  //      }

  //      transform.position=_activeCam.transform.position+_offSet+ _mousRay.direction*_dist;
  //      Vector3 pos = transform.position;
  //      pos.y =pos.y <= GetComponent<Collider>().bounds.extents.y?GetComponent<Collider>().bounds.extents.y:pos.y;
  //      //pos=	GetComponent<Renderer>().bounds.min;
  //      transform.position = pos;

		}

// End of file
}
