using System;
using UnityEngine;
using UnityEngine.UI;

public class ValvePropertiesView : PropertiesView
{
    [SerializeField] private Text _toolName;
    [SerializeField] private InputField _od;
    [SerializeField] private InputField _id;
    [SerializeField] private Toggle _isOpen;
    private Valve _valve;


    public override void Init()
    {

    }

    public override void Show(BaseTool tool)
    {
        _valve=tool as Valve;
        _toolName.text = _valve?.ToolName;
        _od.text = _valve?.InnerDiameter.ToString();
        if (_valve != null) _isOpen.isOn = _valve.IsOpen;
    }

    public override void Apply()
    {
        _valve.InnerDiameter = float.Parse(_id.text);
        _valve.IsOpen = _isOpen.isOn;
        
    }

    public override void Cancel()
    {

    }
    
}