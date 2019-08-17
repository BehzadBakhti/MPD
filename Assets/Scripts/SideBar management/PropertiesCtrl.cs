using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PropertiesCtrl : MonoBehaviour {

	// Use this for initialization
	private SceneMgr thisSceneMgr;
	public Button assembleBtn, dessembleBtn;

	void Start () {
		thisSceneMgr=FindObjectOfType<SceneMgr>();
	}
	
	// Update is called once per frame
	public void Assemble(){
		
		thisSceneMgr.selectedObjects[0].GetComponent<BaseTool>().Assemble();
	}

	public void Dissemble(){
		
	}
}
