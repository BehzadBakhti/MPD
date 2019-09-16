using System;
using System.Collections.Generic;
using UnityEngine;

public class SideBarView: MonoBehaviour
{
    public event Action<string> CreateIsPressed;
    public event EventHandler AssembleBtnClicked;
    public event EventHandler UnAssembleBtnClicked;

    private InspectorView _inspectorView;
    private CreatePanelView _createPanel;

    private void Awake()
    {
        _inspectorView = GetComponentInChildren<InspectorView>();
        _createPanel = GetComponentInChildren<CreatePanelView>();
    }

    public void Init(List<GameObject> toolsPrefabs)
    {
       // print(_createPanel.gameObject.name);

        _createPanel.Init(toolsPrefabs);
        _createPanel.CreateIsPressed += OnCreateIsPressed;

        _inspectorView.AssembleBtnClicked += _inspectorView_AssembleBtnClicked;
        _inspectorView.UnAssembleBtnClicked += _inspectorView_UnAssembleBtnClicked;
    }

    private void _inspectorView_UnAssembleBtnClicked(object sender, EventArgs e)
    {
        UnAssembleBtnClicked?.Invoke(this, EventArgs.Empty);
       
    }

    private void _inspectorView_AssembleBtnClicked(object sender, EventArgs e)
    {
        AssembleBtnClicked?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void OnCreateIsPressed(string obj)
    {
        CreateIsPressed?.Invoke(obj);
    }
}