using System;
using System.Collections.Generic;
using UnityEngine;

public class SideBarView: MonoBehaviour
{
    public event Action<string> CreateIsPressed;
    public event Action AssembleBtnClicked;
    public event Action UnAssembleBtnClicked;
    public event Action CalculateBtnClicked;
    public event Action ApplyBtnClicked;
    public event Action CancelBtnClicked;

    private InspectorView _inspectorView;
    private CreatePanelView _createPanel;

    private void Awake()
    {
        _inspectorView = GetComponentInChildren<InspectorView>();
        _createPanel = GetComponentInChildren<CreatePanelView>();
    }

    public void Init(Dictionary<string, GameObject> toolsPrefabs)
    {
       

        _createPanel.Init(toolsPrefabs);
        _createPanel.CreateIsPressed += OnCreateIsPressed;

        _inspectorView.AssembleBtnClicked += _inspectorView_AssembleBtnClicked;
        _inspectorView.UnAssembleBtnClicked += _inspectorView_UnAssembleBtnClicked;
        _inspectorView.CalculateBtnClicked += _inspectorView_CalculateBtnClicked;
        _inspectorView.ApplyBtnClicked += OnApplyBtnClicked;
        _inspectorView.CancelBtnClicked += OnCancelBtnClicked;
    }

    private void _inspectorView_CalculateBtnClicked()
    {
        CalculateBtnClicked?.Invoke();
    }

    private void _inspectorView_UnAssembleBtnClicked()
    {
        UnAssembleBtnClicked?.Invoke();
    }

    private void _inspectorView_AssembleBtnClicked()
    {
        AssembleBtnClicked?.Invoke();
    }

    protected virtual void OnCreateIsPressed(string obj)
    {
        CreateIsPressed?.Invoke(obj);
    }

    public void InitPropertiesView(BaseTool tool)
    {
        _inspectorView.InitView(tool);
    }

    protected virtual void OnApplyBtnClicked()
    {
        ApplyBtnClicked?.Invoke();
    }

    protected virtual void OnCancelBtnClicked()
    {
        CancelBtnClicked?.Invoke();
    }
}


