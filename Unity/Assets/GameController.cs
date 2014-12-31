using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{

    private GameState _gameState;
    private GameState _lastGameState;

    private GridController _gridController;
    private CraneController _craneController;


    void Start()
    {
        _gameState = GameState.SetupLevel;
        SetupControllers();
    }

    private void SetupControllers()
    {
        if (_gridController == null && GridController.Instance != null)
        {
            _gridController = GridController.Instance;
        }

        if (_craneController == null && CraneController.Instance != null)
        {
            _craneController = CraneController.Instance;
        }
    }

    void Update()
    {
        var hasChanged = _lastGameState != _gameState;
        _lastGameState = _gameState;

        switch (_gameState)
        {
            case GameState.SetupLevel:
                UpdateSetupLevel(hasChanged);
                break;
            case GameState.PlayLevel:
                UpdatePlayLevel(hasChanged);
                break;
            case GameState.EndLevel:
                UpdateEndLevel(hasChanged);
                break;
            default:
                break;
        }
    }

    private void UpdateSetupLevel(bool hasChanged)
    {
        SetupControllers();
        if (_gridController == null || _craneController == null) { return; }


        if (hasChanged)
        {
            var min = 1;
            var max = 13;

            var rowCount = 4;
            var columnCount = 6;

            var maxRow = Random.Range(min + rowCount, max);
            var maxColumn = Random.Range(min + columnCount, max);

            var minRow = maxRow + 1 - rowCount;
            var minColumn = maxColumn + 1 - columnCount;

            // Reset grid
            _gridController.minRowValue = minRow;
            _gridController.maxRowValue = maxRow;
            _gridController.minColumnValue = minColumn;
            _gridController.maxColumnValue = maxColumn;

            // Reset crane
            _craneController.Reset();

            // Let everything else get setup first
            return;
        }
        else
        {
            // Position Camera
            PositionCamera();

            // Start game
            _gameState = GameState.PlayLevel;
        }
    }

    private void PositionCamera()
    {
        var gridBounds = _gridController.GetBounds();
        var craneBounds = _craneController.GetBounds();

        var minX = Mathf.Min(gridBounds.min.x, craneBounds.min.x);
        var maxX = Mathf.Max(gridBounds.max.x, craneBounds.max.x);
        var minZ = Mathf.Min(gridBounds.min.z, craneBounds.min.z);
        var maxZ = Mathf.Max(gridBounds.max.z, craneBounds.max.z);

        var maxHeight = Mathf.Max(gridBounds.max.y, craneBounds.max.y);

        var midX = maxX - minX;
        var midZ = maxZ - minZ;

        Camera.mainCamera.transform.position = new Vector3(midX, 15, midZ);

        Debug.Log("gridBounds: " + gridBounds);
        Debug.Log("craneBounds: " + craneBounds);
        Debug.Log("Moved Camera to: " + Camera.mainCamera.transform.position);

        // Zoom cam back until it can see everything
        var minWorldInScreen = Camera.mainCamera.ScreenToWorldPoint(new Vector3(minX, minZ, Camera.mainCamera.transform.position.y - maxHeight));
        var attempts = 0;

        while (minWorldInScreen.x > minX || minWorldInScreen.z > minZ)
        {
            if (attempts > 100) { break; }

            Camera.mainCamera.transform.position = new Vector3(midX, Camera.mainCamera.transform.position.y * 1.1f, midZ);
            minWorldInScreen = Camera.mainCamera.ScreenToWorldPoint(new Vector3(minX, minZ, Camera.mainCamera.transform.position.y - maxHeight));

            Debug.Log("Moved Camera to: " + Camera.mainCamera.transform.position);

            attempts++;
        }
    }

    private void UpdateEndLevel(bool hasChanged)
    {
        _gameState = GameState.SetupLevel;
    }


    private float? timeAtLaidAllBlocks = null;

    private void UpdatePlayLevel(bool hasChanged)
    {
        // Dectect game over
        if (_craneController.HasLaidAllBlocks())
        {
            if (!timeAtLaidAllBlocks.HasValue)
            {
                timeAtLaidAllBlocks = Time.time;
            }
        }
        else
        {
            timeAtLaidAllBlocks = null;
        }

        if (timeAtLaidAllBlocks.HasValue)
        {
            if (Time.time < timeAtLaidAllBlocks.Value + 5)
            {
                Debug.Log("Game Over");
                _gameState = GameState.EndLevel;
            }
        }


    }
}

public enum GameState
{
    //Title,
    SetupLevel,
    PlayLevel,
    EndLevel
}
