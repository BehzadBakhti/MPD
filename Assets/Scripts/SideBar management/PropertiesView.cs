using System;

using UnityEngine;
using UnityEngine.UI;

public abstract class PropertiesView
{
    public abstract void Init();
    public abstract void Show();
    public abstract void Apply();

    public abstract void Cancel();

}

public class PipePropertiesView : PropertiesView
{
    [SerializeField] private Text _toolName;
    [SerializeField] private InputField _od;
    [SerializeField] private InputField _length;


    public override void Init()
    {

    }
    public override void Apply()
    {

    }

    public override void Cancel()
    {

    }



    public override void Show()
    {

    }
}