using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolsManager : MonoBehaviour
{
    public event Action<Connection, Connection> OnToolConnected;
    public event Action<GameObject> ClickedOnObject;
    private List<BaseTool> _mpdTools;
    private List<Connection> _allConnections;
    private ToolFactory _factory;
    private Camera _activeCamera;

    private void Awake()
    {
        _factory=GetComponent<ToolFactory>();
        _allConnections= new List<Connection>();

    }

    private void Start()
    {
        Init();
    }

    public void Init()
    {
       
        _factory.Init(GetToolsPrefabList());
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
        
       BaseTool obj= _factory.CreateTool(type, activeCamera);
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
            //Vector3 pos= new Vector3();
            //Quaternion rot= new Quaternion();
            //   pos= nearConnection.transform.position;
            //   rot = nearConnection.transform.rotation;
            //   _refTrnsfrm.position = pos;
            //   _refTrnsfrm.rotation = rot;
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


    public List<GameObject> GetToolsPrefabList()
    {
        List<GameObject> prefabsList=new List<GameObject>();
        prefabsList.AddRange(Resources.LoadAll<GameObject>("Prefabs/MPDTools"));
       

        return prefabsList;
    }

    protected virtual void OnClickedOnObject(GameObject obj)
    {
        ClickedOnObject?.Invoke(obj);
    }
}
