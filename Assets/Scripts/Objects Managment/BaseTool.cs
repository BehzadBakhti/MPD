using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseTool : MonoBehaviour {

	
	public Connection[] connections;
	public string nominalSize;
	public bool isSelected, isAssembeled;
	private	Connection nearConnection=null, desiredConnection=null;
	private	Connection[] allConnections;


	public void Assemble()
	{

		nearConnection=null;
		desiredConnection=null;
		allConnections= FindObjectsOfType<Connection>();
		foreach (Connection item in allConnections)
		{
			//if(!item.isConnected)
		//	Debug.Log(item.transform.root.gameObject.GetInstanceID());
		}
		//return;
		float dist=1000;	
	
		for (int j = 0; j < connections.Length; j++)
		{
			for (int i = 0; i < allConnections.Length; i++)
			{
				float currDist=Vector3.Distance(connections[j].transform.position, allConnections[i].transform.position);
				if((currDist<dist) && (allConnections[i].transform.root.gameObject.GetInstanceID()!=gameObject.GetInstanceID()))
				{
					if (!allConnections[i].isConnected)
					{
						nearConnection=allConnections[i];
						desiredConnection=connections[j];
						dist=currDist;
					}
				}	
			}
		}

		Transform refTrnsfrm=nearConnection.transform;
		Vector3 localPos= desiredConnection.transform.localPosition;
		Quaternion localRot= desiredConnection.transform.localRotation;
		Quaternion newConnRot= Quaternion.FromToRotation(desiredConnection.transform.up, refTrnsfrm.up);

		transform.rotation=newConnRot*Quaternion.Inverse(localRot);
		transform.position=refTrnsfrm.position;
		transform.Translate(-localPos);
		
		desiredConnection.Connect(nearConnection);
	}

	public void UnAssembel(){

	}

	public abstract string HeadLossEq(string param, double flowRate);


// End of file
}
