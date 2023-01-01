using UnityEngine;

public struct Cell
{
  public enum CellType
  {
    Empty,
    Mine,
    Number
  }

  public CellType type;
  public Vector3Int position;
  public int number;
  public bool isRevealed;
  public bool isFlagged;
  public bool isExploded;
}
