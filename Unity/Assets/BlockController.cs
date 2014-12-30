using UnityEngine;
using System.Collections;

public class BlockController : MonoBehaviour
{
    public int width;
    public int height;

    public Material[,] _materials = new Material[20, 20];

    private int _lastWidth;
    private int _lastHeight;

    private GameObject _cubes;

    void Start()
    {
        _cubes = transform.FindChild("Cubes").gameObject;
    }

    void Update()
    {

        // Update material to match width and height
        if (width != _lastWidth || height != _lastHeight)
        {
            var material = GetOrCreateMaterial(_cubes, _materials, width, height);
            _cubes.GetComponent<MeshRenderer>().material = material;

            _lastWidth = width;
            _lastHeight = height;
        }
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
