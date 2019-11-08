using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneMgr : MonoBehaviour {

	// Use this for initialization
	public List<GameObject> MpdAssembly;
	private List<GameObject> _selectedObjects;
    private ToolsManager _toolsManager;
    private NodalNetwork _nodalNetwork;
    private UiManager _uiManager;
    private CameraManager _cameraManager;

    private MpdData _data;
    // initialization
    private void Awake ()
    {
        _data= new MpdData();
        _toolsManager = FindObjectOfType<ToolsManager>();
        _nodalNetwork = FindObjectOfType<NodalNetwork>();
        _uiManager = FindObjectOfType<UiManager>();
        _cameraManager = FindObjectOfType<CameraManager>();
        _uiManager.CreateIsPressed += _uiManager_CreateIsPressed;
        _uiManager.AssembleBtnClicked += _uiManager_AssembleBtnClicked;
        _uiManager.UnAssembleBtnClicked += _uiManager_UnAssembleBtnClicked;
        _uiManager.CalculateBtnClicked += _uiManager_CalculateBtnClicked;
       // _uiManager.ApplyBtnClicked += () => print(_data.FlowRate);
		
		_toolsManager.OnToolConnected += _toolsManager_OnToolConnected;
        _toolsManager.ClickedOnObject += SelectionHandler;
        
	}

    private void _uiManager_CalculateBtnClicked()
    {
        _nodalNetwork.CalculateHeads();
    }

    private void _uiManager_UnAssembleBtnClicked()
    {
    }

    private void _uiManager_AssembleBtnClicked()
    {
        if (_selectedObjects.Count>1)
                return;
        _toolsManager.Assemble(_selectedObjects[0].GetComponent<BaseTool>());
    }

    private void Start()
    {
        _selectedObjects= new List<GameObject>();
        Init();
    }

    private void Init()
    {
        Dictionary<string, GameObject> prefabsList = _toolsManager.MpdPrefabDictionary;

        _uiManager.Init(prefabsList, _data);
        _nodalNetwork.Init(_toolsManager.MpdToolsInAssembly, _data);
    }

    // Events  
    private void _uiManager_CreateIsPressed(string tool)
    {
        // print(_toolsManager.name);
       var obj= _toolsManager.CreateTool(tool, _cameraManager.ActiveCam);
        if(obj)
           SelectionHandler(obj.GetComponent<BaseTool>());
    }
    private void _toolsManager_OnToolConnected(Connection arg1, Connection arg2)
    {
        _nodalNetwork.CreateNode(arg1, arg2);
    }


    //Methods
    public void SelectionHandler(BaseTool tool){
		
		if(Input.GetKey(KeyCode.LeftControl)||Input.GetKey(KeyCode.RightControl)){

			if(!tool.IsSelected){
				_selectedObjects.Add(tool.gameObject);
                _uiManager.ShowProperties(tool);
            }else
			{
				_selectedObjects.Remove(tool.gameObject);
			}
			tool.IsSelected=!tool.IsSelected;
        }else{
			foreach (GameObject item in _selectedObjects)
			{
				item.GetComponent<BaseTool>().IsSelected=false;		
			}
			_selectedObjects.Clear();
            _selectedObjects.Add(tool.gameObject);
            tool.IsSelected=true;
            _uiManager.ShowProperties(tool);
        }

	}
}

public class MpdData
{
    public string WellName;
    public float MudWeight;
    public float FlowRate;
    public float Viscosity;

    public MpdData()
    {
        WellName = "Default";
        MudWeight = 0;
        FlowRate = 0;
        Viscosity = 0;
    }

}
