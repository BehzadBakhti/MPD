using System;
using System.Collections.Generic;
using UnityEngine;

public class CreatePanelView :MonoBehaviour
{
    public event Action<string> CreateIsPressed;

    private List<ToolsBtnCtrl> _toolBtns;
    [SerializeField] private GameObject _createBtnPrefab;

    public void Init(List<GameObject> toolPrefabsList)
    {
        foreach (GameObject tool in toolPrefabsList)
        {
           // print(tool.name);
            ToolsBtnCtrl btnCtrl = Instantiate(_createBtnPrefab, this.transform).GetComponent<ToolsBtnCtrl>();
            btnCtrl.CreateIsPressed += OnCreateIsPressed;
            btnCtrl.Init(tool.gameObject.name);
        }
    }


    protected virtual void OnCreateIsPressed(string obj)
    {
        CreateIsPressed?.Invoke(obj);
    }
}