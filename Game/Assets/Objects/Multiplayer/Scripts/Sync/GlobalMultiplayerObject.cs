using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalMultiplayerObject : MonoBehaviour
{
    public string playerName = "";

    public GameObject playerLabelPrefab;
    private RectTransform labelsParent;
    private GameObject playerLabelInstance;
    private PlayerLabel playerLabelComponentInstance;

    public Vector3 targetPosition = new Vector3();
    public Vector3 targetEulerAngles = new Vector3();

    public float interpolation = 0.2f;

    private void Start()
    {
        labelsParent = GameObject.FindGameObjectWithTag("PlayerLabels").transform as RectTransform;
        if (labelsParent != null && playerLabelPrefab != null)
        {
            playerLabelInstance = Instantiate(playerLabelPrefab, labelsParent);

            playerLabelComponentInstance = playerLabelInstance.GetComponent<PlayerLabel>();

            playerLabelComponentInstance.objectToFollow = transform;
            playerLabelComponentInstance.SetName(playerName);
        }
    }

    private void OnDestroy()
    {
        if (playerLabelInstance != null)
        {
            Destroy(playerLabelInstance);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerLabelComponentInstance != null)
            playerLabelComponentInstance.SetName(playerName);

        transform.position = Vector3.Lerp(transform.position, targetPosition, interpolation);
        transform.eulerAngles = targetEulerAngles;
    }
}
