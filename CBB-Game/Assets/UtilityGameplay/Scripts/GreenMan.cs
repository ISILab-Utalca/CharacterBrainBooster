using CBB.Api;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[UtilityAgent("Green man")]
public class GreenMan : ColorMan
{

    [UtilityAction("Run", "o:Position")]
    public void MoveFrom(Vector3 pos)
    {
        Direction = -(pos - transform.position).normalized;
        Moving = true;
    }

    [UtilityAction("Chase", "o:Position")]
    public void MoveTo(Vector3 pos)
    {
        Direction = (pos - transform.position).normalized;
        Moving = true;
    }
}
