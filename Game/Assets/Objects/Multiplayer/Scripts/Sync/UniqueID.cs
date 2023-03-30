using System;
using UnityEngine;

public class UniqueID : MonoBehaviour
{
    public string _id { get; private set; }
    private void Start()
    {
        _id = Guid.NewGuid().ToString();
    }
}