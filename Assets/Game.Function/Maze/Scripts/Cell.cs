using UnityEngine;

public class Cell
{
    public CellType Type;
    public Vector2Int Position;
    public GameObject GameObject;

    public Cell(CellType type, Vector2Int position, GameObject obj)
    {
        Type = type;
        Position = position;
        GameObject = obj;
    }
}

public enum CellType
{
    Empty,
    Wall,
    Start,
    Goal,
    Visited,
    Path
}