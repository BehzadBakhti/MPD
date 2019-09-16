using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class ToolFactory : MonoBehaviour
    {
        [SerializeField]private List<GameObject> _toolPrefabs;
        private Dictionary<string, GameObject> _tools;
        private float _dist, _farPoint = 30;

        private void Awake(){

            _tools=new Dictionary<string, GameObject>();
            
        }

        public void Init(List<GameObject> toolPrefabs)
        {
            _toolPrefabs = toolPrefabs;

            foreach (var prefab in _toolPrefabs)
            {
                _tools.Add(prefab.name, prefab);
            }
        }

        public  BaseTool CreateTool(string type, Camera activeCam)
        {
            Ray  camRay = new Ray(activeCam.transform.position, activeCam.transform.forward);
            Vector3 targetPos;
            
            if (Physics.Raycast(camRay, out RaycastHit _hit))
            {
                targetPos = _hit.transform.position;
                //Debug.Log("Hit");
            }
            else
            {
                targetPos = activeCam.transform.position + activeCam.transform.forward * _farPoint;
            }

            _dist = Vector3.Distance(activeCam.transform.position, targetPos);
            targetPos= activeCam.transform.position + camRay.direction * _dist* 0.7f;
        //  print(_tools.Count);
              GameObject obj=  Instantiate(_tools[type],targetPos, Quaternion.identity,this.transform);
              return obj.GetComponent<BaseTool>();
        }



    
}

public enum ToolType{
    Pipe,
    Valve,
    Pump,
    Chock,
    Rcd
}
