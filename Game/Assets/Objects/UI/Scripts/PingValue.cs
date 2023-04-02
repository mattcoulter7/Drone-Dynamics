using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using TMPro;

public class PingValue : MonoBehaviour
{
    [SerializeField] private TMP_Text text;

    private void Update()
    {
        Connection conn = FindObjectOfType<Connection>();
        if (!conn) return;

        text.text = $"{conn.udpPing}ms";
    }
}
