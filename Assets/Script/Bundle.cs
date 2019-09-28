using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bundle 
{
    public GameObject[] objects;
    public string[] names;

    public Bundle(GameObject[] objects,string[] names)
    {
        this.objects = objects;
        this.names = names;
    }
}
