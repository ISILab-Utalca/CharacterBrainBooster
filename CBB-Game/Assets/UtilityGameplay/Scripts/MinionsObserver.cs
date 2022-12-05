using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Obsolete("This class is not being used.")]
public class MinionsObserver : MonoBehaviour
{
    public static MinionsObserver Instance;
    public List<GreenMan> AllMinions = new List<GreenMan>();

    // Start is called before the first frame update
    void Start()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddMinion(GreenMan newMinion)
    {
        AllMinions.Add(newMinion);
    }

    public void RemoveMinion(GreenMan oldMinion)
    {
        AllMinions.Remove(oldMinion);
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
