using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodalNetwork : MonoBehaviour
{
   public List<Node> networkNodes;
   public List<Link> networkLinks;
  
   private int nodeCount=0 , linkCount=0;
   public List<string> equations;

   public static NodalNetwork instance;
    

    void Awake(){
        if(instance==null){
            instance=this;
            DontDestroyOnLoad(this);

        }else{
            Destroy(this);
        }
   }

    public void AddNode(Node n){
        nodeCount++;
        n.param="h"+(nodeCount).ToString();
        networkNodes.Add(n);
        foreach (Link lnk in n.links)
        {
            bool alreadyRegistered=false;
            foreach (Link item in networkLinks)
            {
              
                if(Object.ReferenceEquals(item, lnk)){
                    alreadyRegistered=true;
                    
                }
            }
          
            if(!alreadyRegistered){
                linkCount++;
                
                lnk.param="q"+(linkCount).ToString();
                networkLinks.Add(lnk);
            }
        }
    }
   public void GenerateTree(){}

   public void EqMatrix(){
        foreach (Node node in networkNodes)
        {
         equations.Add(node.GetEquation());
  
        }
        foreach(Link link in networkLinks){
            equations.Add(link.GetEquation());
        }
   }

   
}
