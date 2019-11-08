using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    //public event Action ApplyBtnClicked;
    public event Action<string> CreateIsPressed;
    public event Action AssembleBtnClicked;
    public event Action UnAssembleBtnClicked;
    public event Action CalculateBtnClicked;
    public event Action ApplyBtnClicked;
    public event Action CancelBtnClicked;

    private SideBarView _sideBarView;
    private TopBarView _topBarView;
    private LeftSideBarView _leftSideBar;

    private void Awake()
    {
        _sideBarView = GetComponentInChildren<SideBarView>();
        _topBarView = GetComponentInChildren<TopBarView>();
        _leftSideBar = GetComponentInChildren<LeftSideBarView>();
    }

    public void Init(Dictionary<string, GameObject> toolsPrefabs, MpdData data)
    {
        _sideBarView.Init(toolsPrefabs);
        _leftSideBar.Init(data);
       // _leftSideBar.ApplyBtnClicked += () => ApplyBtnClicked?.Invoke();


        _sideBarView.CreateIsPressed += OnCreateIsPressed;
        _sideBarView.AssembleBtnClicked += _sideBarView_AssembleBtnClicked;
        _sideBarView.UnAssembleBtnClicked += _sideBarView_UnAssembleBtnClicked;
        _sideBarView.CalculateBtnClicked += _sideBarView_CalculateBtnClicked;
        _sideBarView.ApplyBtnClicked += OnApplyBtnClicked;
        _sideBarView.CancelBtnClicked += OnCancelBtnClicked;
    }

    private void _sideBarView_CalculateBtnClicked()
    {
        CalculateBtnClicked?.Invoke();
    }

    private void _sideBarView_UnAssembleBtnClicked()
    {
        UnAssembleBtnClicked?.Invoke();
    }

    private void _sideBarView_AssembleBtnClicked()
    {
        AssembleBtnClicked?.Invoke();
    }

    protected virtual void OnCreateIsPressed(string obj)
    {
        CreateIsPressed?.Invoke(obj);
    }

    public void ShowProperties(BaseTool tool)
    {
        _sideBarView.InitPropertiesView(tool);
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