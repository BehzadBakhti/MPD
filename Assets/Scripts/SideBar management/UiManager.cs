using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public event Action<string> CreateIsPressed;
    public event EventHandler AssembleBtnClicked;
    public event EventHandler UnAssembleBtnClicked;
    public event EventHandler CalculateBtnClicked;

    private SideBarView _sideBarView;
    private TopBarView _topBarView;
    private LeftSideBarView _leftSideBar;

    private void Awake()
    {
        _sideBarView = GetComponentInChildren<SideBarView>();
        _topBarView = GetComponentInChildren<TopBarView>();
    }

    public void Init(Dictionary<string, GameObject> toolsPrefabs)
    {
        _sideBarView.Init(toolsPrefabs);
        _sideBarView.CreateIsPressed += OnCreateIsPressed;
        _sideBarView.AssembleBtnClicked += _sideBarView_AssembleBtnClicked;
        _sideBarView.UnAssembleBtnClicked += _sideBarView_UnAssembleBtnClicked;
        _sideBarView.CalculateBtnClicked += _sideBarView_CalculateBtnClicked;
    }

    private void _sideBarView_CalculateBtnClicked(object sender, EventArgs e)
    {
        CalculateBtnClicked?.Invoke(sender, e);
    }

    private void _sideBarView_UnAssembleBtnClicked(object sender, EventArgs e)
    {
        UnAssembleBtnClicked?.Invoke(this, EventArgs.Empty);
    }

    private void _sideBarView_AssembleBtnClicked(object sender, EventArgs e)
    {
        AssembleBtnClicked?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void OnCreateIsPressed(string obj)
    {
        CreateIsPressed?.Invoke(obj);
    }

    public void ShowProperties(BaseTool tool)
    {
        _sideBarView.InitPropertiesView(tool);
    }
}

public class LeftSideBarView :MonoBehaviour
{
    [SerializeField] private InputField _wellName, _mw, _viscosity;
    [SerializeField] private Button _applyBtn, _cancelBtn;

    private void Awake()
    {
        _applyBtn.onClick.AddListener(Apply);
        _cancelBtn.onClick.AddListener(Cancel);
    }

    public void Apply()
    {
        Fluids.Density =float.Parse(_mw.text);
    }

    public void Cancel()
    {

    }
}