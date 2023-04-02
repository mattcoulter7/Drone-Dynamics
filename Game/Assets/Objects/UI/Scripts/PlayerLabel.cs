using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Newtonsoft.Json.Bson;

public class PlayerLabel : MonoBehaviour
{
    public Transform objectToFollow;
    public Vector3 offset = new Vector3(0, 1, 0); // adjust offset as needed

    private Canvas canvas;
    private RectTransform rectTransform;
    private TMP_Text label;

    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
        label = GetComponent<TMP_Text>();
    }

    void LateUpdate()
    {
        rectTransform.position = objectToFollow.position + offset;
        rectTransform.LookAt(canvas.worldCamera.transform.position);
    }

    public void SetName(string name)
    {
        if (label != null)
            label.text = name;
    }
}
