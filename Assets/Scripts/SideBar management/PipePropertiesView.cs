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
        _od.text = _pipe?.OuterDiameter.ToString();
        _roughness.text = _pipe?.Roughness.ToString();
        _id.text = _pipe?.InnerDiameter.ToString();
        _length.text = _pipe?.Length.ToString();
 
    }

    public override void Apply()
    {
      //  print("Hello");
        _pipe.InnerDiameter = float.Parse(_id.text);
        _pipe.Length = float.Parse(_length.text);
        _pipe.OuterDiameter = float.Parse(_od.text);
        _pipe.Roughness = float.Parse(_roughness.text);

    }

    public override void Cancel()
    {
        print("bye");
    }



}