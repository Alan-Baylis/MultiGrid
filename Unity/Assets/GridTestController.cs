using UnityEngine;
using System.Collections;

public class GridTestController : MonoBehaviour
{
    public float blockCreationHeight = 10;

    private GameObject _blocks;
    private GameObject _blockProto;
    private GridController _gridController;

    void Start()
    {
        _blocks = transform.FindChild("Blocks").gameObject;
        _blockProto = transform.FindChild("BlockProto").gameObject;
        SetGridController();
    }

    private void SetGridController()
    {
        if (_gridController == null && GridController.Instance != null)
        {
            _gridController = GridController.Instance;
            _gridController.columnRowClickedCallback = ColumnRowClicked;
        }
    }

    private int _clickedColumn;
    private int _clickedRow;

    private void ColumnRowClicked(int column, int row)
    {
        _clickedColumn = column;
        _clickedRow = row;
    }

    void Update()
    {
        SetGridController();

        if (_clickedColumn > 0 && _clickedRow > 0)
        {
            var block = Instantiate(_blockProto) as GameObject;
            block.transform.localScale = new Vector3(_clickedColumn, 1, _clickedRow);
            block.transform.localPosition = new Vector3(_gridController.lastColumnX, blockCreationHeight, _gridController.lastRowZ);
            block.transform.parent = _blocks.transform;

            var bController = block.GetComponent<BlockController>();
            bController.width = _clickedColumn;
            bController.height = _clickedRow;

            block.SetActive(true);
        }

        _clickedColumn = 0;
        _clickedRow = 0;
    }
}
