using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolsManager : MonoBehaviour
{
    public event Action<Connection, Connection> OnToolConnected;
    public event Action<BaseTool> ClickedOnObject;

    private List<Connection> _allConnections;
    private ToolFactory _factory;
    private Camera _activeCamera;
    public Dictionary<string, GameObject> MpdPrefabDictionary { get; private set; }
    public List<BaseTool> MpdToolsInAssembly { get; set; }

    private void Awake()
    {
        _factory=GetComponent<ToolFactory>();
        _allConnections= new List<Connection>();

    }

    private void Start()
    {
        MpdToolsInAssembly = new List< BaseTool>();
        MpdPrefabDictionary = new Dictionary<string, GameObject>();
        Init();

    }

    public void Init()
    {
        GetToolsPrefabList();
        foreach (var tool in GetComponentsInChildren<BaseTool>())
        {
            MpdToolsInAssembly.Add(tool);
        }
       
        _factory.Init(MpdPrefabDictionary);
    }

    private void GetAllConnections()
    {
        _allConnections.Clear();
        _allConnections.AddRange( FindObjectsOfType<Connection>());
        foreach (var conn in _allConnections) conn.OnConnected += Conn_OnConnected;
    }

  
    private void Conn_OnConnected(Connection conn1, Connection conn2)
    {
        OnToolConnected?.Invoke(conn1, conn2);
    }
    Transform _refTrnsfrm;

 

    public GameObject CreateTool(string type, Camera activeCamera){
       // print(type);
        if (CheckForExistance(type))
        {
            print("It is not possible to have more than 1 instance of " + type + " in your MPD Assembly");
            return null;
        }
        BaseTool obj= _factory.CreateTool(type, activeCamera);
        MpdToolsInAssembly.Add( obj);
        obj.ClickedOnObject += OnClickedOnObject;
        return obj.gameObject;
        //_mpdTools.Add(obj);

    }

   

    public void Assemble(BaseTool thisTool)
    {
        GameObject thisObject = thisTool.gameObject;
        GetAllConnections();
        Connection[] toolConnections=thisTool.GetConnections();
        Connection nearConnection=null,	desiredConnection=null;
       
        float dist=1000;	

        foreach (Connection myConn in toolConnections)
        {
            if (myConn.IsConnected) continue;

            foreach (Connection othersConn in _allConnections)
            {
                if (othersConn.IsConnected) continue;

                float currDist=Vector3.Distance(myConn.transform.position, othersConn.transform.position);
                if ((!(currDist < dist)) ||
                    (othersConn.GetComponentInParent<BaseTool>() == thisTool)) continue;
                
                nearConnection=othersConn;
                desiredConnection=myConn;
                dist=currDist;
            }
        }

        if (nearConnection != null)
        {

            _refTrnsfrm=nearConnection.transform;
               _refTrnsfrm.Rotate(180f,0f,0f);

            Vector3 localPos= desiredConnection.transform.localPosition;

            Quaternion newConnRot=_refTrnsfrm.rotation* Quaternion.Inverse(desiredConnection.transform.rotation);
            thisObject.transform.rotation *= newConnRot;
            thisObject.transform.position=_refTrnsfrm.position;
            thisObject.transform.Translate(-localPos);
        }

        desiredConnection.Connect(nearConnection);
    }

    public void UnAssemble(){

        }


    public void GetToolsPrefabList()
    {
        List<GameObject> prefabsList=new List<GameObject>();
        prefabsList.AddRange(Resources.LoadAll<GameObject>("Prefabs/MPDTools"));

        foreach (var prefab in prefabsList)
        {
            MpdPrefabDictionary.Add(prefab.name, prefab);
        }
       
    }


    private bool CheckForExistance(string type)
    {
        if (MpdPrefabDictionary[type].GetComponent<BaseTool>().IsUnique)
        {
            foreach (var item in MpdToolsInAssembly)
            {
                if (item.GetType().ToString() == type)
                {
                    return true;
                }    
            }
        }
        return false;
    }

    protected virtual void OnClickedOnObject(BaseTool obj)
    {
        ClickedOnObject?.Invoke(obj);
    }
}
