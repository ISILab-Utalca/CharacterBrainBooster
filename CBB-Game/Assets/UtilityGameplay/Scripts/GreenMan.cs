using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CBB.Api;

[UtilityAgentAttribute("Green man")]
public class GreenMan : MonoBehaviour
{
    [UtilityInput("Size")]
    public float Size = 1;
    bool isObserved = false;
    public Rigidbody2D rg2D;
    public Vector2 Direction;
    public float SpeedModifier=10;
    public float SizeLossModifier = 0.1f;
    public bool Moving = false;

    [UtilityInput("Position")]
    public Vector3 Position => transform.position;

    // Start is called before the first frame update
    void Start()
    {
        if (isObserved)
        {
            return;
        }
        if (MinionsObserver.Instance == null)
        {
            return;
        }
        MinionsObserver.Instance.AddMinion(this);
        isObserved = true;
        rg2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isObserved)
        {
            Start();
            return;
        }
        if (Moving)
        {
            rg2D.AddForce(Direction * (1 / Size) * SpeedModifier * Time.deltaTime);
            float NewSize = Size - Time.deltaTime * SizeLossModifier;
            transform.localScale *= (NewSize) / Size;
            Size = NewSize;
        }

    }

    [UtilityAction("Run", "Position")]
    public void MoveFrom(Vector3 pos)
    {
        Direction = -(pos - transform.position).normalized;
        Moving = true;
    }

    [UtilityAction("Chase", "Position")]
    public void MoveTo(Vector3 pos)
    {
        Direction = (pos - transform.position).normalized;
        Moving = true;
    }

    public void MoveTo(GreenMan objective, bool escape = false)
    {
        Direction = (objective.transform.position - transform.position).normalized;
        if (escape)
        {
            Direction *= -1;
        }
        Moving = true;
    }

    [UtilityAction("Stop")]
    public void Stop()
    {
        Moving = false;
    }

    private void OnDestroy()
    {
        MinionsObserver.Instance?.RemoveMinion(this);
    }

    public void Eat(GreenMan other)
    {
        transform.localScale *= (Size + other.Size) / Size;
        Size += other.Size;        
        Destroy(other.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GreenMan otherMan = collision.gameObject.GetComponent<GreenMan>();
        if (otherMan != null)
        {
            if (otherMan.Size > Size)
            {
                otherMan.Eat(this);                
            }
        }
    }
}
