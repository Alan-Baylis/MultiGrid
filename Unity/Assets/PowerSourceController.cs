using UnityEngine;
using System.Collections;

public class PowerSourceController : MonoBehaviour
{

    public Color powerColor;

    void OnTriggerEnter(Collider other)
    {
        var corner = other.GetComponent<CornerController>();

        if (corner != null)
        {
            corner.SetMaxColor(powerColor);
            Debug.Log("Transferred Power to Corner");
        }
    }

    void Start()
    {

    }

    void Update()
    {
        ResetColor();
    }

    private Color _lastColor;
    private void ResetColor()
    {
        if (_lastColor != powerColor)
        {
            GetComponentInChildren<MeshRenderer>().material.color = powerColor;
            _lastColor = powerColor;
        }
    }
}
