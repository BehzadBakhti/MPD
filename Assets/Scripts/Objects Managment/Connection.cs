using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connection : MonoBehaviour {

	// Use this for initialization
	public bool isConnected;
	public string connectionName;
	public Connection connectedTo;
	public ConnectionType connectionType;
	public ConnectionSize connectionSize;


	//public Node attachedNode;

	//[HideInInspector]
	public Link attachedLink;



	public void Connect(Connection cn){
		cn.isConnected=true;
		this.isConnected=true;

		Node newNode= new Node();
		newNode.AddLink(cn.attachedLink);
		newNode.AddLink(this.attachedLink);
		
		NodalNetwork.instance.AddNode(newNode);
	}
	public void Disconnect(){
		
	}
 
 // END OF FILE
}
	
