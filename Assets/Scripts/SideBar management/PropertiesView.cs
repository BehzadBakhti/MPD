using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class PropertiesView:MonoBehaviour
{
    

    public abstract void Init();
    public abstract void Show(BaseTool tool);
    public abstract void Apply();
    public abstract void Cancel();

}

