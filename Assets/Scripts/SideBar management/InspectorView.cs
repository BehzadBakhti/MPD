using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InspectorView : MonoBehaviour
{
    public event Action AssembleBtnClicked;
    public event Action UnAssembleBtnClicked;
    public event Action CalculateBtnClicked;
    public event Action ApplyBtnClicked;
    public event Action CancelBtnClicked;

    private PropertiesView[] _propertiesViews;
    private PropertiesView _currentView;
   
	[SerializeField] private Button _assembleBtn;
    [SerializeField] private Button _unAssembleBtn;
    [SerializeField] private Button _calculateBtn;
    [SerializeField] private Button _applyBtn;
    [SerializeField] private Button _cancelBtn;

    private void Awake()
    {
        _propertiesViews = GetComponentsInChildren<PropertiesView>();
        InitView(null);
    }

    public void InitView(BaseTool tool)
    {
        if (tool == null)
        {
            _currentView = _propertiesViews[0];
            _propertiesViews[0].Show(null);
            return;
        }

        foreach (var view in _propertiesViews)
        {
            if (view.GetType()==tool.ViewType)
            {
                _currentView = view;
                view.Show(tool);
                continue;
            }
            else
            {
                view.gameObject.SetActive(false);
            }
        }
    }

    void OnEnable () {

        _assembleBtn.onClick.AddListener(OnAssembleBtnClicked);
        _unAssembleBtn.onClick.AddListener(OnUnAssembleBtnClicked);
        _calculateBtn.onClick.AddListener(OnCalculateBtnClicked);
        _applyBtn.onClick.AddListener(OnApplyBtnClicked);
        _cancelBtn.onClick.AddListener(OnCancelBtnClicked);
    }

    private void OnCalculateBtnClicked()
    {
        CalculateBtnClicked?.Invoke();
    }

    void OnDisable()
    {
        _assembleBtn.onClick.RemoveListener(OnAssembleBtnClicked);
        _unAssembleBtn.onClick.RemoveListener(OnUnAssembleBtnClicked);
        _calculateBtn.onClick.RemoveAllListeners();
        _applyBtn.onClick.RemoveAllListeners();
        _cancelBtn.onClick.RemoveAllListeners();
    }

    protected virtual void OnAssembleBtnClicked()
    {
        AssembleBtnClicked?.Invoke();
    }

    protected virtual void OnUnAssembleBtnClicked()
    {
        UnAssembleBtnClicked?.Invoke();
    }


    protected virtual void OnApplyBtnClicked()
    {
        _currentView.Apply();
        ApplyBtnClicked?.Invoke();
    }

    protected virtual void OnCancelBtnClicked()
    {
        _currentView.Cancel();
        CancelBtnClicked?.Invoke();
    }
}
