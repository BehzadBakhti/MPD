using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeftSideBarView :MonoBehaviour
{

    //public event Action ApplyBtnClicked;
    [SerializeField] private TMP_InputField _wellName, _mw, _viscosity, _flowRate;
    [SerializeField] private Button _applyBtn, _cancelBtn;
    private MpdData _data;

    private void Awake()
    {
        _applyBtn.onClick.AddListener(Apply);
        _cancelBtn.onClick.AddListener(Cancel);
    }

    public void Init(MpdData data)
    {
        _data = data;
        SetData();
    }

    public void Apply()
    {
       
        _data.WellName = _wellName.text;
        _data.FlowRate = float.Parse(_flowRate.text);
        _data.Viscosity= float.Parse(_viscosity.text);
        _data.MudWeight= float.Parse(_mw.text);

         //ApplyBtnClicked?.Invoke();

    }

    public void Cancel()
    {
        SetData();
    }

    private void SetData()
    {
        _mw.text = _data.MudWeight.ToString();
        _flowRate.text = _data.FlowRate.ToString();
        _viscosity.text = _data.Viscosity.ToString();
        _wellName.text = _data.WellName;

    }
}