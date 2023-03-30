using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniqueID : MonoBehaviour
{
    public string _id { get; private set; }
    // Start is called before the first frame update
    void Start()
    {
        _id = System.Guid.NewGuid().ToString();
    }
}
