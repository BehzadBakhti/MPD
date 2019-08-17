using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Valve : BaseTool , IClosable
{
  [RangeAttribute(0,100)]
	public float openness;

  public Link valveBody;
  public Node[] valveHeads;
  
  public void Open(){
      if(this.openness<=95)
        this.openness+=5f;
  }


  public void Close(){
        if(this.openness>=5)
        this.openness+=5f;
  }

    public override string HeadLossEq(string param, double flowRate)
    {
        throw new System.NotImplementedException();
    }
}
