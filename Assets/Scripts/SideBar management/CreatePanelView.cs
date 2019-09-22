using System;
using System.Collections.Generic;
using UnityEngine;

public class CreatePanelView :MonoBehaviour
{
    public event Action<string> CreateIsPressed;

    private List<ToolsBtnCtrl> _toolBtns;
    [SerializeField] private GameObject _createBtnPrefab;

    public void Init(Dictionary<string, GameObject> toolPrefabsList)
    {
        foreach (var tool in toolPrefabsList)
        {
           // print(tool.name);
            ToolsBtnCtrl btnCtrl = Instantiate(_createBtnPrefab, this.transform).GetComponent<ToolsBtnCtrl>();
            btnCtrl.CreateIsPressed += OnCreateIsPressed;
            btnCtrl.Init(tool.Key);
        }
    }


    protected virtual void OnCreateIsPressed(string obj)
    {
        CreateIsPressed?.Invoke(obj);
    }
}