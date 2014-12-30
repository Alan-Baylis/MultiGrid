using UnityEngine;
using System.Collections;

public class BlockController : MonoBehaviour
{
    public bool shouldShowBlockText;

    public int width;
    public int height;

    public Material[,] _materials = new Material[20, 20];

    private int _lastWidth;
    private int _lastHeight;

    private GameObject _cubes;
    private TextMesh _textMesh;

    public Color _powerColor = new Color(0, 0, 0);
    private Color _defaultColor;

    public void SetMaxColor(Color color)
    {
        var c = CornerController.GetMaxColor(_powerColor, color);
        if (c != _powerColor)
        {
            _cubes.GetComponent<MeshRenderer>().material.color = c;
        }
    }

    void Start()
    {
        _cubes = transform.FindChild("Cubes").gameObject;
        _textMesh = transform.FindChild("Text").GetComponent<TextMesh>();

        UpdateSize();
    }



    void Update()
    {

        UpdateSize();
    }

    private void UpdateSize()
    {
        // Update material to match width and height
        if (width != _lastWidth || height != _lastHeight)
        {
            var material = GetOrCreateMaterial(_cubes, _materials, width, height);
            _cubes.GetComponent<MeshRenderer>().material = material;

            transform.localScale = new Vector3(width, 1, height);

            _lastWidth = width;
            _lastHeight = height;

            // Set text
            var val = width * height;
            _textMesh.text = val + "";

            // Keep the text square
            var baseScale = 0.085f;
            var maxScale = Mathf.Max(width, height);
            _textMesh.transform.localScale = new Vector3(
                baseScale * height / maxScale,
                baseScale * width / maxScale,
                1);

            // Reduce the text size if needed
            if (val < 10)
            {
                _textMesh.characterSize = 1f;
            }
            else if (val < 100)
            {
                _textMesh.characterSize = 0.6f;
            }
            else
            {
                _textMesh.characterSize = 0.4f;
            }
        }

        _textMesh.gameObject.SetActive(shouldShowBlockText);
    }

    private static Material GetOrCreateMaterial(GameObject cubes, Material[,] materials, int width, int height)
    {
        // Set default material
        if (materials[0, 0] == null)
        {
            materials[0, 0] = cubes.GetComponent<MeshRenderer>().material;
        }

        var baseMaterial = materials[0, 0];

        if (width <= 0 || height <= 0) { return baseMaterial; }


        if (materials[width, height] == null)
        {
            var mat = new Material(baseMaterial);
            mat.mainTextureScale = new Vector2(width, height);

            materials[width, height] = mat;
        }

        return materials[width, height];
    }
}
