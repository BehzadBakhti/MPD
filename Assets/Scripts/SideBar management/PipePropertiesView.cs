using System;
using UnityEngine;
using UnityEngine.UI;

public class PipePropertiesView : PropertiesView
{
    [SerializeField] private Text _toolName;
    [SerializeField] private InputField _od;
    [SerializeField] private InputField _id;
    [SerializeField] private InputField _length;
    [SerializeField] private InputField _roughness;
    private Pipe _pipe;


    public override void Init()
    {

    }

    public override void Show(BaseTool tool)
    {
        _pipe = tool as Pipe;
        _toolName.text = _pipe?.ToolName;
        _od.text = _pipe?.InnerDiameter.ToString();
    }

    public override void Apply()
    {
        _pipe.InnerDiameter = Convert.ToDouble(_id.text);
        _pipe.Length = Convert.ToDouble(_length.text);
        _pipe.OuterDiameter = Convert.ToDouble(_od.text);
        _pipe.Roughness = Convert.ToDouble(_roughness.text);

    }

    public override void Cancel()
    {

    }



}