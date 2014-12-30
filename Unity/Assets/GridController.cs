using UnityEngine;
using System.Collections;

public class GridController : MonoBehaviour
{

    public static GridController Instance;

    public int minColumnValue;
    public int maxColumnValue;
    public int minRowValue;
    public int maxRowValue;

    public System.Action<int, int> columnRowClickedCallback;
    public System.Action<int, int> columnRowChangedCallback;
    public int lastColumn;
    public int lastRow;
    public int lastColumnX;
    public int lastRowZ;

    private int _lastMinColumnValue;
    private int _lastMaxColumnValue;
    private int _lastMinRowValue;
    private int _lastMaxRowValue;

    private GameObject _border;
    private GameObject _rowColumnProto;
    private GameObject _rows;
    private GameObject _columns;

    private GameObject _powerSourceProto;
    private GameObject _powerSources;


    void Start()
    {
        if (Instance != null) { throw new UnityException("Cannot create multiple GridControllers"); }

        Instance = this;

        _border = transform.FindChild("Border").gameObject;
        _rowColumnProto = transform.FindChild("RowColumnProto").gameObject;
        _rows = transform.FindChild("Rows").gameObject;
        _columns = transform.FindChild("Columns").gameObject;
        _powerSourceProto = transform.FindChild("PowerSourceProto").gameObject;
        _powerSources = transform.FindChild("PowerSources").gameObject;
    }

    void Update()
    {
        UpdateGridSize();
        UpdateInput();
    }

    private void UpdateInput()
    {
        // Detect click or touch
        if (Input.touchCount > 0 || Input.anyKey)
        {
            var screenPos = Input.mousePosition;
            var worldPos = Camera.mainCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, Camera.mainCamera.transform.position.y));

            // Calculate input location in grid
            var x = worldPos.x;
            var z = worldPos.z;

            var total = 0;
            var col = minColumnValue;

            for (int v = minColumnValue; v < maxColumnValue; v++)
            {
                total += v;
                if (x < total) { break; }

                col++;
            }

            var row = minRowValue;
            total = 0;

            for (int v = minRowValue; v < maxRowValue; v++)
            {
                total += v;
                if (z < total) { break; }

                row++;
            }


            Debug.Log("screenPos: " + screenPos);
            Debug.Log("worldPos: " + worldPos);
            Debug.Log("col: " + col);
            Debug.Log("row: " + row);

            lastColumn = col;
            lastRow = row;
            lastColumnX = GetSize(minColumnValue, col - 1);
            lastRowZ = GetSize(minRowValue, row - 1);

            if (columnRowChangedCallback != null)
            {
                columnRowChangedCallback(col, row);
                Debug.Log("Called columnRowChangedCallback");
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (columnRowClickedCallback != null)
                {
                    columnRowClickedCallback(col, row);
                    Debug.Log("Called columnRowClickedCallback");
                }
            }
        }

    }

    private void UpdateGridSize()
    {
        var hasChanged =
            minColumnValue != _lastMinColumnValue
         || maxColumnValue != _lastMaxColumnValue
         || minRowValue != _lastMinRowValue
         || maxRowValue != _lastMaxRowValue;

        if (hasChanged)
        {
            var width = GetSize(minColumnValue, maxColumnValue);
            var height = GetSize(minRowValue, maxRowValue);

            _border.transform.localScale = new Vector3(width, 1, height);

            foreach (Transform c in _columns.transform)
            {
                Destroy(c);
            }

            foreach (Transform r in _rows.transform)
            {
                Destroy(r);
            }


            var powerSourcesToAddA = new System.Collections.Generic.List<Vector3>();
            var powerSourcesToAddB = new System.Collections.Generic.List<Vector3>();
            var powerSourcesToAddC = new System.Collections.Generic.List<Vector3>();
            var powerSourcesToAddD = new System.Collections.Generic.List<Vector3>();

            var x = 0;
            for (int i = minColumnValue; i <= maxColumnValue; i++)
            {
                var col = Instantiate(_rowColumnProto) as GameObject;
                col.transform.localScale = new Vector3(i, 1, height);
                col.transform.localPosition = new Vector3(x, 0, 0);
                col.transform.parent = _columns.transform;
                col.SetActive(true);

                //powerSourcesToAddA.Add(col.transform.localPosition + new Vector3(i * 0.5f, 0, 0));
                //powerSourcesToAddB.Add(col.transform.localPosition + new Vector3(i * 0.5f, 0, height));

                powerSourcesToAddA.Add(col.transform.localPosition + new Vector3(i, 0, 0));
                powerSourcesToAddB.Add(col.transform.localPosition + new Vector3(i, 0, height));

                x += i;
            }

            var y = 0;
            for (int j = minRowValue; j <= maxRowValue; j++)
            {
                var r = Instantiate(_rowColumnProto) as GameObject;
                r.transform.localScale = new Vector3(width, 1, j);
                r.transform.localPosition = new Vector3(0, 0, y);
                r.transform.parent = _rows.transform;
                r.SetActive(true);

                powerSourcesToAddC.Add(r.transform.localPosition + new Vector3(0, 0, j));
                powerSourcesToAddD.Add(r.transform.localPosition + new Vector3(width, 0, j));
                //powerSourcesToAddC.Add(r.transform.localPosition + new Vector3(0, 0, j * 0.5f));
                //powerSourcesToAddD.Add(r.transform.localPosition + new Vector3(width, 0, j * 0.5f));


                y += j;
            }

            powerSourcesToAddA.Reverse();
            powerSourcesToAddA.Add(new Vector3());
            powerSourcesToAddA.AddRange(powerSourcesToAddC);
            
            //powerSourcesToAddA.AddRange(powerSourcesToAddB);
            //powerSourcesToAddA.Add(new Vector3(width, 0, height));
            //powerSourcesToAddD.Reverse();
            //powerSourcesToAddA.AddRange(powerSourcesToAddD);

            var iPS = 0;
            foreach (var pSource in powerSourcesToAddA)
            {
                //if (iPS % 2 == 0)
                //{
                    CreatePowerSource(pSource, GetPowerSourceColor(iPS));
                //}

                iPS++;
            }

            powerSourcesToAddA.Clear();
            powerSourcesToAddB.Clear();
            powerSourcesToAddC.Clear();
            powerSourcesToAddD.Clear();
        }

        _lastMinColumnValue = minColumnValue;
        _lastMaxColumnValue = maxColumnValue;
        _lastMinRowValue = minRowValue;
        _lastMaxRowValue = maxRowValue;
    }

    private void CreatePowerSource(Vector3 vector3, Color color)
    {
        var g = Instantiate(_powerSourceProto) as GameObject;
        g.transform.localPosition = vector3;
        g.transform.parent = _powerSources.transform;
        g.SetActive(true);

        var pSource = g.GetComponent<PowerSourceController>();
        pSource.powerColor = color;
    }

    private Color GetPowerSourceColor(int val)
    {
        var type = val % 3;
        switch (type)
        {
            case 0:
                return new Color(1, 0, 0);
            case 1:
                return new Color(0, 1, 0);
            case 2:
            default:
                return new Color(0, 0, 1);
        }
    }

    private int GetSize(int min, int max)
    {
        var size = 0;

        for (int w = min; w <= max; w++)
        {
            size += w;
        }

        return size;
    }
}
