using UnityEngine;
using System.Collections;

public class CornerController : MonoBehaviour
{
    private Color _powerColor = new Color(0, 0, 0);

    public void SetMaxColor(Color color)
    {
        var waitTime = 0.25f;
        StartCoroutine(WaitAndSetColor(waitTime, color));
    }

    IEnumerator WaitAndSetColor(float waitTime, Color color)
    {
        yield return new WaitForSeconds(waitTime);
        SetMaxColorInner(color);
    }

    private void SetMaxColorInner(Color color)
    {
        if (color != _powerColor)
        {
            _powerColor = GetMaxColor(color, _powerColor);
            ResetColor();
            Debug.Log("_powerColor:" + _powerColor);

            // Transfer power to entire block
            collider.attachedRigidbody.GetComponent<BlockController>().SetMaxColor(_powerColor);

            var corners = collider.attachedRigidbody.GetComponentsInChildren<CornerController>();

            foreach (var corner in corners)
            {
                if (corner != this)
                {
                    corner.SetMaxColor(_powerColor);
                }
            }
        }
    }

    private void ResetColor()
    {
        GetComponent<MeshRenderer>().material.color = _powerColor;
    }

    void Start()
    {
        ResetColor();
    }

    void Update()
    {

    }

    void OnTriggerStay(Collider other)
    {
        // When a corner hits another corner
        // Transfer power

        var otherCorner = other.GetComponent<CornerController>();

        if (otherCorner != null)
        {
            var maxPower = GetMaxColor(_powerColor, otherCorner._powerColor);

            SetMaxColor(maxPower);
            otherCorner.SetMaxColor(maxPower);
        }

    }

    public static Color GetMaxColor(Color a, Color b)
    {
        return new Color(
             Mathf.Max(a.r, b.r),
             Mathf.Max(a.g, b.g),
             Mathf.Max(a.b, b.b),
             Mathf.Max(a.a, b.a));
    }


}
