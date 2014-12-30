using UnityEngine;
using System.Collections;

public class GridController : MonoBehaviour
{

    public int minColumnValue;
    public int maxColumnValue;
    public int minRowValue;
    public int maxRowValue;

    private int _lastMinColumnValue;
    private int _lastMaxColumnValue;
    private int _lastMinRowValue;
    private int _lastMaxRowValue;

    private GameObject _border;
    private GameObject _rowColumnProto;
    private GameObject _rows;
    private GameObject _columns;

    void Start()
    {
        _border = transform.FindChild("Border").gameObject;
        _rowColumnProto = transform.FindChild("RowColumnProto").gameObject;
        _rows = transform.FindChild("Rows").gameObject;
        _columns = transform.FindChild("Columns").gameObject;
    }

    void Update()
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

            var x = 0;
            for (int i = minColumnValue; i <= maxColumnValue; i++)
            {
                var col = Instantiate(_rowColumnProto) as GameObject;
                col.transform.localScale = new Vector3(i, 1, height);
                col.transform.localPosition = new Vector3(x, 0, 0);
                col.transform.parent = _columns.transform;
                col.SetActive(true);

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

                y += j;
            }
        }


        _lastMinColumnValue = minColumnValue;
        _lastMaxColumnValue = maxColumnValue;
        _lastMinRowValue = minRowValue;
        _lastMaxRowValue = maxRowValue;
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
