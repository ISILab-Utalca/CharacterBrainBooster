using System;
using System.Collections.Generic;
using UnityEngine;

[Obsolete("This class is not being used.")]
public class MinionsObserver : MonoBehaviour
{
    public static MinionsObserver Instance;
    public List<ColorMan> AllMinions = new List<ColorMan>();

    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddMinion(ColorMan newMinion)
    {
        AllMinions.Add(newMinion);
    }

    public void RemoveMinion(ColorMan oldMinion)
    {
        AllMinions.Remove(oldMinion);
    }



    // Update is called once per frame
    void Update()
    {

    }
}
