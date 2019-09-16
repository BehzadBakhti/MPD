﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    public event Action<string> CreateIsPressed;
    public event EventHandler AssembleBtnClicked;
    public event EventHandler UnAssembleBtnClicked;

    private SideBarView _sideBarView;
    private TopBarView _topBarView;

    private void Awake()
    {
        _sideBarView = GetComponentInChildren<SideBarView>();
        _topBarView = GetComponentInChildren<TopBarView>();
    }

    public void Init(List<GameObject> toolsPrefabs)
    {
        _sideBarView.Init(toolsPrefabs);
        _sideBarView.CreateIsPressed += OnCreateIsPressed;
        _sideBarView.AssembleBtnClicked += _sideBarView_AssembleBtnClicked;
        _sideBarView.UnAssembleBtnClicked += _sideBarView_UnAssembleBtnClicked;
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
}