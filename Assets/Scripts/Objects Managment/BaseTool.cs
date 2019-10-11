using System.Collections;
using System;
using System.Collections.Generic;
using TouchScript.Gestures;
using UnityEngine;

public abstract class BaseTool : MonoBehaviour
{

    public event Action<BaseTool> ClickedOnObject;
    public Type ViewType ;
    [SerializeField] protected Connection[] Connections;
    public string NominalSize;
    public bool IsSelected, IsAssembeled, IsUnique;
    public Node CenterNode { get; set; } = null;


    private PressGesture _press;

    protected virtual void Awake()
    {
        Connections = GetComponentsInChildren<Connection>();
        _press = GetComponent<PressGesture>();
    }

    public Connection[] GetConnections()
    {
        return Connections;
    }
    public abstract string HeadLossEq(string param, double flowRate);


    private void OnEnable()
    {
        _press.Pressed += _press_Pressed;
    }

    private void OnDisable()
    {
        _press.Pressed -= _press_Pressed;
    }
    private void _press_Pressed(object sender, EventArgs e)
    {
        ClickedOnObject?.Invoke(this);
    }
}
