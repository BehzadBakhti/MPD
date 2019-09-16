using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InspectorView : MonoBehaviour
{
    public event EventHandler AssembleBtnClicked;
    public event EventHandler UnAssembleBtnClicked;
   
	[SerializeField] private Button _assembleBtn, _unAssembleBtn;

    private void Awake()
    {
        
    }
	void OnEnable () {

        _assembleBtn.onClick.AddListener(OnAssembleBtnClicked);
        _unAssembleBtn.onClick.AddListener(OnUnAssembleBtnClicked);
    }
    void OnDisable()
    {

        _assembleBtn.onClick.RemoveListener(OnAssembleBtnClicked);
        _unAssembleBtn.onClick.RemoveListener(OnUnAssembleBtnClicked);
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
