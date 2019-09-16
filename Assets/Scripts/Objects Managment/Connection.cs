using System;
using System.Collections.Generic;
using UnityEngine;


public class Connection : MonoBehaviour {

    // Use this for initialization
    public event Action<Connection, Connection> OnConnected;
	public bool IsConnected;
	public ConnectionType ConnectionType;
	public ConnectionSize ConnectionSize;
    [SerializeField]private Link _attachedLink;

    public Link AttachedLink
    {
        get => _attachedLink;
        set => _attachedLink = value;
    }

    public void Connect(Connection cn){
        
		cn.IsConnected=true;
		this.IsConnected=true;
		OnConnected?.Invoke(this,cn);
		// Node newNode= new Node();
		// newNode.AddLink(cn.AttachedLink);
		// newNode.AddLink(this.AttachedLink);
		
		// NodalNetwork.instance.CreateNode(newNode);
	}
	public void Disconnect(){
		
	}
 
 // END OF FILE

}
	
