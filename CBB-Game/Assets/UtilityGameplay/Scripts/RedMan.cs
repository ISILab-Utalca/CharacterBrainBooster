using CBB.Api;
using UnityEngine;

[UtilityAgent("Red man")]
public class RedMan : ColorMan
{

    [UtilityAction("MoveRandom")]
    public void MoveRandom()
    {
        Direction = -(new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f))).normalized;
        Moving = true;
    }

    [UtilityAction("Grow")]
    public void Grow()
    {
        Size *= 1.2f;
    }
}
