using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InspectorView : MonoBehaviour
{
    public event EventHandler AssembleBtnClicked;
    public event EventHandler UnAssembleBtnClicked;
    public event EventHandler CalculateBtnClicked;
   
	[SerializeField] private Button _assembleBtn, _unAssembleBtn, _calculateBtn;

    private void Awake()
    {
        
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
