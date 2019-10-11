using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolsBtnCtrl : MonoBehaviour
{
    public event Action<string> CreateIsPressed; 
	
	private Camera _activeCam;
    [SerializeField] private Text _toolName;

	public GameObject ToolModel;
	private string _toolType;
    private float _dist, _farPoint=30;
    private Ray _camRay, _mouseRay;
    private RaycastHit _hit;

    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnCreateIsPressed);
        _toolName = GetComponentInChildren<Text>();


    }

    public void Init(string toolType )
    {
        _toolType = toolType;
        _toolName.text = toolType;

    }
	//public void BeginDrag(){
	//	_activeCam=GameObject.FindObjectOfType<Camera>(); //#################################################################
	//	_camRay= new Ray(_activeCam.transform.position, _activeCam.transform.forward);
	//		if(Physics.Raycast(_camRay, out _hit))
	//		{
	//			_target= _hit.transform.position;
	//			//Debug.Log("Hit");
	//		}else{
	//			_target=_activeCam.transform.position+_activeCam.transform.forward*_farPoint;
	//		}

	//	_dist=Vector3.Distance(_activeCam.transform.position, _target)/Mathf.Cos((_activeCam.fieldOfView)*Mathf.PI/360);
	//	_mouseRay= _activeCam.ScreenPointToRay(Input.mousePosition);
	//	Vector3 objectPos=_activeCam.transform.position+ _mouseRay.direction*_dist;
	//	_createdObj=	Instantiate(ToolModel,objectPos,Quaternion.identity);
	//	//createdObj.GetComponent<SingleToolStatus>().thisSceneMgr=thisSceneMgr;
	//	_createdObj.GetComponent<SingleToolMovement>().IsBeingCreated=true;
	//}


	// public void Dragging(){
	//	_createdObj.GetComponent<SingleToolMovement>().OnMouseDrag();
	//}
	

	//public void EndDrag(){
	//	_thisSceneMgr.SelectionHandler(_createdObj);

	//}

    protected virtual void OnCreateIsPressed()
    {
      //  print(ToolModel.GetComponent<BaseTool>().GetType().ToString());
        CreateIsPressed?.Invoke(_toolType);
    }
}
