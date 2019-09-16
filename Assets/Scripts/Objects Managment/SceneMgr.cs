using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneMgr : MonoBehaviour {

	// Use this for initialization
	public List<GameObject> MpdAssembly;
	private List<GameObject> _selectedObjects;
	private InspectorView _inspectorView;
    private ToolsManager _toolsManager;
    private NodalNetwork _nodalNetwork;
    private UiManager _uiManager;
    private CameraManager _cameraManager;
    // initialization
    void Awake ()
    {
        _toolsManager = FindObjectOfType<ToolsManager>();
        _nodalNetwork = FindObjectOfType<NodalNetwork>();
        _uiManager = FindObjectOfType<UiManager>();
        _cameraManager = FindObjectOfType<CameraManager>();
        _uiManager.CreateIsPressed += _uiManager_CreateIsPressed;
        _uiManager.AssembleBtnClicked += _uiManager_AssembleBtnClicked;
        _uiManager.UnAssembleBtnClicked += _uiManager_UnAssembleBtnClicked;
		
		_toolsManager.OnToolConnected += _toolsManager_OnToolConnected;
        _toolsManager.ClickedOnObject += SelectionHandler;
        
	}

    private void _uiManager_UnAssembleBtnClicked(object sender, System.EventArgs e)
    {
    }

    private void _uiManager_AssembleBtnClicked(object sender, System.EventArgs e)
    {
        if (_selectedObjects.Count>1)
                return;
        
        _toolsManager.Assemble(_selectedObjects[0].GetComponent<BaseTool>());
    }

    private void Start()
    {
        Init();
        _selectedObjects= new List<GameObject>();
    }

    private void Init()
    {
       List<GameObject> prefabsList= _toolsManager.GetToolsPrefabList();
      // print(prefabsList.Count);
       _uiManager.Init(prefabsList);
    }

    // Events  
    private void _uiManager_CreateIsPressed(string tool)
    {
        // print(_toolsManager.name);
       GameObject obj= _toolsManager.CreateTool(tool, _cameraManager.ActiveCam);
       SelectionHandler(obj);
    }
    private void _toolsManager_OnToolConnected(Connection arg1, Connection arg2)
    {
        _nodalNetwork.CreateNode(arg1, arg2);
    }


    //Methods
    public void SelectionHandler(GameObject obj){
		BaseTool statusScript=obj.GetComponent<BaseTool>();
		if(Input.GetKey(KeyCode.LeftControl)||Input.GetKey(KeyCode.RightControl)){

			if(!statusScript.IsSelected){
				_selectedObjects.Add(obj);
			}else
			{
				_selectedObjects.Remove(obj);
			}
			statusScript.IsSelected=!statusScript.IsSelected;

			
		}else{
			foreach (GameObject item in _selectedObjects)
			{
				item.GetComponent<BaseTool>().IsSelected=false;		
			}
			_selectedObjects.Clear();
            _selectedObjects.Add(obj);
            statusScript.IsSelected=true;
		}


		//if (_selectedObjects.Count==1)
		//{
		//	_inspectorView.AssembleBtn.interactable=true;
		//	_inspectorView.DessembleBtn.interactable=true;
		//}else
		//{
		//	_inspectorView.AssembleBtn.interactable=false;
		//	_inspectorView.DessembleBtn.interactable=false;
			
		//}
	}



}
