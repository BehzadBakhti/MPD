using System;
using UnityEngine;

public abstract class PropertiesView:MonoBehaviour
{
    public abstract void Init();
    public abstract void Show(BaseTool tool);
    public abstract void Apply();
    public abstract void Cancel();

}