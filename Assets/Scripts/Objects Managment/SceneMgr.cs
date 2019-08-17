using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneMgr : MonoBehaviour {

	// Use this for initialization
	public List<GameObject> MPDAssembly;

	public List<GameObject> selectedObjects;
	
	private PropertiesCtrl thisPropertiesCtrl;

	void Start () {
		thisPropertiesCtrl=FindObjectOfType<PropertiesCtrl>();
		
	}

	public void SelectionHandler(GameObject obj){
		BaseTool statusScript=obj.GetComponent<BaseTool>();
		if(Input.GetKey(KeyCode.LeftControl)||Input.GetKey(KeyCode.RightControl)){

			if(!statusScript.isSelected){
				selectedObjects.Add(obj);
			}else
			{
				selectedObjects.Remove(obj);
			}
			statusScript.isSelected=!statusScript.isSelected;

			
		}else{
			foreach (GameObject item in selectedObjects)
			{
				item.GetComponent<BaseTool>().isSelected=false;		
			}
			selectedObjects.Clear();

			if(!statusScript.isSelected){
				selectedObjects.Add(obj);
			}

			statusScript.isSelected=true;
		}


		if (selectedObjects.Count==1)
		{
			thisPropertiesCtrl.assembleBtn.interactable=true;
			thisPropertiesCtrl.dessembleBtn.interactable=true;
		}else
		{
			thisPropertiesCtrl.assembleBtn.interactable=false;
			thisPropertiesCtrl.dessembleBtn.interactable=false;
			
		}
	}


	// public void AutoAssembleChange(){

	// 	autoAssemble=!autoAssemble;
	// }
	

}
