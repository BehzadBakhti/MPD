﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class NodalNetwork : MonoBehaviour
{
   public HashSet<Node> NetworkNodes;
   public HashSet<Link> NetworkLinks;

  
   [SerializeField] private int _nodeCount=0 , _linkCount=0;
   public HashSet<string> Equations;


   private void Awake(){
        NetworkNodes= new HashSet<Node>();
        NetworkLinks = new HashSet<Link>();
        Equations= new HashSet<string>();
   }

    public void CreateNode(Connection conn1, Connection conn2){
        Node nd = new Node {Param = "h" + _nodeCount};
        _nodeCount++;
         nd.AddLink(conn1.AttachedLink);
         nd.AddLink(conn2.AttachedLink);
        AddNode(nd);

    }


    private void AddNode(Node nd)
    {
       
       bool isNewNode= NetworkNodes.Add(nd);
       if (!isNewNode) return;
       _nodeCount++;

        foreach (Link lnk in nd.Links)
        {
            bool isNewLink = NetworkLinks.Add(lnk);
            if (!isNewLink) continue;

            _linkCount++;
            lnk.Param = "q" + _linkCount;
            foreach (var lnkNode in lnk.Nodes)
            {
                AddNode(lnkNode);
            }
        }
    }
    
    public void RemoveLink(Link lnk){
           foreach (var node in lnk.Nodes)
           {
               node.RemoveLink(lnk);
           }

           lnk.Nodes[0] = null;
           lnk.Nodes[1] = null;
           NetworkLinks.Remove(lnk);
    }

    public void GetEquationTree(Node rootNode)
    {
        Equations.Add(rootNode.GetEquation());
            foreach (var lnk in rootNode.Links)
            {
                Equations.Add(lnk.GetEquation());
                foreach (var node in lnk.Nodes)
                {
                    if(node==rootNode) continue;
                        GetEquationTree(node);

                }
            }
    }
  

    public void EqMatrix(){
       ValidateLinks();
        foreach(Node node in NetworkNodes){
            string eq=node.GetEquation();
            if(node.Param=="h0"){
                eq="q_in-"+eq;
            }
            Equations.Add(eq);

        }
        foreach(Link link in NetworkLinks){
            if(link.IsOpen)
                Equations.Add(link.GetEquation());
        }
   }


    public void ValidateLinks(){
       foreach (Link lnk in NetworkLinks){
           if(!lnk.Validate()) {
               CompleteNetwork(lnk);
           }
       }
   }

    void CompleteNetwork(Link lnk){
        Node lastNode = new Node {Param = "h_out", Head = Fluids.BackPressure};
        lnk.AddNode(lastNode);
       }

   
}
