using UnityEngine;
using System.Collections.Generic;
using Com.LuisPedroFonseca.ProCamera2D;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{
    public int width = 10;
    public int height = 10;
    public GameObject cellPrefab;
    public Transform gridParent;
    public Button regenBtn;
    public Button findPathBtn;

    private Cell[,] grid;
    private Vector2Int startPos;
    private Vector2Int goalPos;

    public Color emptyColor = Color.white;
    public Color wallColor = Color.gray;
    public Color startColor = Color.green;
    public Color goalColor = Color.red;
    public Color visitedColor = Color.yellow;
    public Color pathColor = Color.cyan;
    
    private readonly Vector2Int[] directions = {
        Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
    };

    private void Start()
    {
        GenerateGrid();
        
        regenBtn.onClick.AddListener(GenerateGrid);
        findPathBtn.onClick.AddListener(FindPath);
    }

    private void DestroyGrid()
    {
        if (gridParent.childCount > 0)
        {
            for (int i = 0; i < gridParent.childCount; i++)
            {
                Destroy(gridParent.GetChild(i).gameObject);
            }
            ProCamera2D.Instance.CameraTargets.RemoveAt(1);
            ProCamera2D.Instance.CameraTargets.RemoveAt(0);
        }
    }

    private void GenerateGrid()
    {
        DestroyGrid();
        
        grid = new Cell[width, height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                GameObject go = Instantiate(cellPrefab, gridParent);
                go.transform.localPosition = new Vector3(x, y, 0);

                CellType type = (Random.value < 0.2f) ? CellType.Wall : CellType.Empty;

                grid[x, y] = new Cell(type, pos, go);
                SetColor(go, type);
            }
        }
        
        ProCamera2D.Instance.AddCameraTarget(grid[0, 0].GameObject.transform);
        ProCamera2D.Instance.AddCameraTarget(grid[width-1, height-1].GameObject.transform);
        
GEN_START_POS:        
        startPos = new Vector2Int(Random.Range(0, width), Random.Range(0, height));

        if (grid[startPos.x, startPos.y].Type == CellType.Wall)
        {
            goto GEN_START_POS;
        }
        
GEN_GOAL_POS:        
        goalPos = new Vector2Int(Random.Range(0, width), Random.Range(0, height));

        if (grid[goalPos.x, goalPos.y].Type == CellType.Wall || goalPos == startPos)
        {
            goto GEN_GOAL_POS;
        }

        grid[startPos.x, startPos.y].Type = CellType.Start;
        SetColor(grid[startPos.x, startPos.y].GameObject, CellType.Start);

        grid[goalPos.x, goalPos.y].Type = CellType.Goal;
        SetColor(grid[goalPos.x, goalPos.y].GameObject, CellType.Goal);
    }

    private void SetColor(GameObject obj, CellType type)
    {
        var sprite = obj.GetComponent<SpriteRenderer>();
        if (sprite == null) return;

        switch (type)
        {
            case CellType.Empty: 
                sprite.color = emptyColor; 
                break;
            
            case CellType.Wall: 
                sprite.color = wallColor; 
                break;
            
            case CellType.Start: 
                sprite.color = startColor; 
                break;
            
            case CellType.Goal: 
                sprite.color = goalColor; 
                break; 
            
            case CellType.Visited: 
                sprite.color = visitedColor; 
                break;
            
            case CellType.Path: 
                sprite.color = pathColor; 
                break;
        }
    }
    
    private void FindPath()
    {
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();

        queue.Enqueue(startPos);
        cameFrom[startPos] = startPos;

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            if (current == goalPos)
                break;

            foreach (Vector2Int dir in directions)
            {
                Vector2Int next = current + dir;
                if (IsInsideGrid(next) &&
                    grid[next.x, next.y].Type != CellType.Wall &&
                    !cameFrom.ContainsKey(next))
                {
                    cameFrom[next] = current;
                    queue.Enqueue(next);
                    
                    if (grid[next.x, next.y].Type != CellType.Goal)
                    {
                        grid[next.x, next.y].Type = CellType.Visited;
                        SetColor(grid[next.x, next.y].GameObject, CellType.Visited);
                    }
                }
            }
        }

        if (cameFrom.ContainsKey(goalPos))
        {
            Vector2Int current = goalPos;
            while (current != startPos)
            {
                if (grid[current.x, current.y].Type != CellType.Goal)
                {
                    grid[current.x, current.y].Type = CellType.Path;
                    SetColor(grid[current.x, current.y].GameObject, CellType.Path);
                }
                current = cameFrom[current];
            }
        }
    }

    private bool IsInsideGrid(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
    }
}

