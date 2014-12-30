using UnityEngine;
using System.Collections;

public class CornerController : MonoBehaviour
{
    public Color _powerColor = new Color(0, 0, 0);
    public bool isVertical = false;
    public bool isEdge = true;

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
        var newColor = GetMaxColor(color, _powerColor);

        if (newColor != _powerColor)
        {
            _powerColor = newColor;

            ResetColor();
            Debug.Log("SetMaxColorInner _powerColor:" + _powerColor);

            TransferPower();
        }
    }

    private void TransferPower()
    {
        var ratio = 0.8f;

        var color = _powerColor;

        // Reduce the power
        color = new Color(color.r * ratio, color.g * ratio, color.b * ratio);
        Debug.Log("TransferPower _powerColor:" + _powerColor + " -> color:" + color);


        // Transfer power to entire block
        //collider.attachedRigidbody.GetComponent<BlockController>().SetMaxColor(color);

        //var corners = collider.attachedRigidbody.GetComponentsInChildren<CornerController>();

        //foreach (var corner in corners)
        //{
        //    if (corner != this)
        //    {
        //        corner.SetMaxColor(color);
        //    }
        //}
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

        if (otherCorner != null && canTransfer(otherCorner))
        {
            var maxPower = GetMaxColor(_powerColor, otherCorner._powerColor);

            SetMaxColor(maxPower);
            otherCorner.SetMaxColor(maxPower);
        }

    }

    private bool canTransfer(CornerController otherCorner)
    {
        return this.isVertical == otherCorner.isVertical;
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
