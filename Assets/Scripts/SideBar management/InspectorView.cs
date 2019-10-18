﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InspectorView : MonoBehaviour
{
    public event EventHandler AssembleBtnClicked;
    public event EventHandler UnAssembleBtnClicked;
    public event EventHandler CalculateBtnClicked;
    private PropertiesView[] _propertiesViews;
    private PropertiesView _currentView;
   
	[SerializeField] private Button _assembleBtn, _unAssembleBtn, _calculateBtn;

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
            _propertiesViews[0].Show(tool);
            return;
        }

        for (int i = 0; i < _propertiesViews.Length; i++)
        {
            if (_propertiesViews[i].GetType()==tool.ViewType)
            {
                _currentView = _propertiesViews[i];
                _propertiesViews[i].Show(tool);
                continue;
            }
            else
            {
                _propertiesViews[i].gameObject.SetActive(false);
            }
        }
        
    }

    void OnEnable () {

        _assembleBtn.onClick.AddListener(OnAssembleBtnClicked);
        _unAssembleBtn.onClick.AddListener(OnUnAssembleBtnClicked);
        _calculateBtn.onClick.AddListener(OnCalculateBtnClicked);
    }

    private void OnCalculateBtnClicked()
    {
        CalculateBtnClicked?.Invoke(this, EventArgs.Empty);
    }

    void OnDisable()
    {
        _assembleBtn.onClick.RemoveListener(OnAssembleBtnClicked);
        _unAssembleBtn.onClick.RemoveListener(OnUnAssembleBtnClicked);
        _calculateBtn.onClick.RemoveAllListeners();
    }

    protected virtual void OnAssembleBtnClicked()
    {
        AssembleBtnClicked?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void OnUnAssembleBtnClicked()
    {
        UnAssembleBtnClicked?.Invoke(this, EventArgs.Empty);
    }


}
