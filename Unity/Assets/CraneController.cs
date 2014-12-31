using UnityEngine;
using System.Collections.Generic;


public class CraneController : MonoBehaviour
{

    public float nextBlockDelay = 1f;
    public float craneHeight = 10;
    public float blockFallRate = 0.1f;
    public float forceModifierCraneToInput = 4.0f;
    public float forceModifierBlockToCrane = 4.0f;

    private GameObject _crane;
    private GameObject _craneGroundCube;
    private GameObject _craneLineCube;
    private GameObject _blocks;
    private GameObject _blockProto;
    private GridController _gridController;

    private List<BlockPosition> _blockList;
    private GameObject _attachedBlock;
    private float _attachmentLength;
    private float _timeToAttach = 0f;

    private Vector3 _craneResetPosition;

    void Start()
    {
        _crane = transform.FindChild("Crane").gameObject;
        _craneGroundCube = _crane.transform.FindChild("GroundCube").gameObject;
        _craneLineCube = _crane.transform.FindChild("LineCube").gameObject;
        _blocks = transform.FindChild("Blocks").gameObject;
        _blockProto = transform.FindChild("BlockProto").gameObject;
        SetGridController();

        _craneResetPosition = new Vector3(
            _crane.transform.localPosition.x,
            craneHeight,
            _crane.transform.localPosition.z);

        _crane.transform.position = _craneResetPosition;

        _timeToAttach = Time.time + 0.1f;
    }

    private void SetGridController()
    {
        if (_gridController == null && GridController.Instance != null)
        {
            _gridController = GridController.Instance;
            //_gridController.columnRowClickedCallback = ColumnRowClicked;
        }
    }



    void Update()
    {
        SetGridController();
        if (_gridController == null) { return; }

        // Create a list of needed blocks
        if (_blockList == null)
        {
            CreateBlockList();
        }

        // Attach block if ready
        if (IsReadyForNextBlock())
        {
            AttachNextBlock();
        }


        // Move crane to position
        if (_attachedBlock == null)
        {
            MoveCraneTo(_craneResetPosition);
        }
        else
        {
            if (Input.GetMouseButton(0))
            {
                var screenPos = Input.mousePosition;
                var worldPos = Camera.mainCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, Camera.mainCamera.transform.position.y));

                // If near to column, row, then use exact position
                var col = _gridController.lastColumn;
                var row = _gridController.lastRow;
                var colMidX = _gridController.lastColumnX + col * 0.5f;
                var rowMidZ = _gridController.lastRowZ + row * 0.5f;

                if (Mathf.Abs(colMidX - worldPos.x) < col
                    && Mathf.Abs(rowMidZ - worldPos.z) < row)
                {
                    worldPos = new Vector3(colMidX, 0, rowMidZ);
                }

                MoveCraneTo(worldPos + new Vector3(0, craneHeight, 0));
            }
        }

        // Move attached to crane
        if (_attachedBlock != null)
        {
            //Debug.Log("MoveCraneTo screenPos: " + screenPos + " worldPos:" + worldPos);

            // Move attached with crane
            MoveAttachedBlock();


            // Drop block
            if (Input.GetMouseButtonUp(0))
            {
                DropBlock();
            }

        }
    }

    private void DropBlock()
    {
        _attachedBlock.rigidbody.velocity = Vector3.zero;
        _attachedBlock.rigidbody.angularVelocity = Vector3.zero;

        _attachedBlock = null;
    }

    private void MoveAttachedBlock()
    {
        _attachmentLength += Time.deltaTime * blockFallRate;

        var target = (_crane.transform.position - new Vector3(0, _attachmentLength, 0));
        target += _attachedBlock.transform.localScale * -0.5f;

        var diff = target - _attachedBlock.transform.position;

        var force = diff * diff.magnitude;

        // TODO: Reduce force as length increases
        //_attachedBlock.rigidbody.AddForce(force * forceModifierBlockToCrane);

        _attachedBlock.transform.position = target;

        // Draw line cube
        _craneLineCube.transform.localScale = new Vector3(
            _attachedBlock.transform.localScale.x,
            _attachmentLength,
            _attachedBlock.transform.localScale.z
            );

        _craneLineCube.transform.position = _crane.transform.position + new Vector3(0, -0.5f * _attachmentLength, 0);

        // Draw ground cube
        _craneGroundCube.transform.localScale = new Vector3(
            _attachedBlock.transform.localScale.x,
            craneHeight,
            _attachedBlock.transform.localScale.z
            );

        _craneGroundCube.transform.position = _crane.transform.position + new Vector3(0, -0.5f * craneHeight, 0);
    }

    private void MoveCraneTo(Vector3 target)
    {
        Debug.Log("MoveCraneTo pos: " + target);

        var diff = target - _crane.transform.position;
        var force = diff * forceModifierCraneToInput;

        if (diff.sqrMagnitude < 1f)
        {
            force = diff * forceModifierCraneToInput * 2;
        }

        if (diff.sqrMagnitude < 0.5f)
        {
            force = diff * forceModifierCraneToInput * 3;
        }

        _crane.rigidbody.AddForce(force);

        //if (diff.sqrMagnitude < 0.5f)
        //{
        //    _crane.rigidbody.MovePosition(target);
        //}

        Debug.Log("MoveCraneTo to: " + target + " from:" + _crane.transform.position + " force:" + diff);
    }

    private void AttachNextBlock()
    {
        var nextBlock = _blockList[_blockList.Count - 1];
        _blockList.RemoveAt(_blockList.Count - 1);

        var col = nextBlock.column;
        var row = nextBlock.row;

        var block = Instantiate(_blockProto) as GameObject;
        //block.transform.localScale = new Vector3(1, 1, 1);
        //block.transform.localPosition = new Vector3(0, 0, 0);
        block.transform.parent = _blocks.transform;



        var bController = block.GetComponent<BlockController>();
        bController.width = col;
        bController.height = row;

        block.SetActive(true);


        block.transform.position = _crane.transform.position + new Vector3(0, -1, 0);
        _attachmentLength = 1;
        _attachedBlock = block;



    }

    private bool IsReadyForNextBlock()
    {
        if (_attachedBlock == null
            && (_crane.transform.position - _craneResetPosition).sqrMagnitude < 0.1f)
        {
            if (_timeToAttach <= 0)
            {
                _timeToAttach = Time.time + nextBlockDelay;
            }

            if (_timeToAttach < Time.time)
            {
                return true;
            }
        }
        else
        {
            _timeToAttach = -1;
        }

        return false;
    }

    private void CreateBlockList()
    {
        var minCol = _gridController.minColumnValue;
        var maxCol = _gridController.maxColumnValue;
        var minRow = _gridController.minRowValue;
        var maxRow = _gridController.maxRowValue;

        var blocks = new List<BlockPosition>();

        for (int iCol = minCol; iCol <= maxCol; iCol++)
        {
            for (int iRow = minRow; iRow <= maxRow; iRow++)
            {
                blocks.Add(new BlockPosition() { column = iCol, row = iRow });
            }
        }

        // Randomize order
        var rBlocks = new List<BlockPosition>();

        while (blocks.Count > 0)
        {
            var i = Random.Range(0, blocks.Count);

            var item = blocks[i];
            blocks.RemoveAt(i);
            rBlocks.Add(item);
        }

        _blockList = rBlocks;
    }
}

public struct BlockPosition
{
    public int column;
    public int row;
}
