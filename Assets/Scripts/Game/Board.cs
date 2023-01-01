using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
  public Tilemap Tilemap { get; private set; }
  public Tile tileUnknown, tileEmpty, tileMine, tileFlag, tileExploded, tile1, tile2, tile3, tile4, tile5, tile6, tile7, tile8;

  private void Awake()
  {
    Tilemap = GetComponent<Tilemap>();
  }

  public void Draw(Cell[,] cells)
  {
    int width = cells.GetLength(0);
    int height = cells.GetLength(1);

    for (int x = 0; x < width; x++)
    {
      for (int y = 0; y < height; y++)
      {
        Cell cell = cells[x, y];
        Vector3Int position = new Vector3Int(x, y, 0);
        Tilemap.SetTile(cell.position, GetTile(cell));
      }
    }
  }

  private Tile GetTile(Cell cell)
  {
    if (cell.isRevealed)
    {
      return GetRevealedTile(cell);
    }
    else if (cell.isFlagged)
    {
      return tileFlag;
    }
    else
    {
      return tileUnknown;
    }
  }

  private Tile GetRevealedTile(Cell cell)
  {
    switch (cell.type)
    {
      case Cell.CellType.Empty:
        return tileEmpty;
      case Cell.CellType.Mine:
        return cell.isExploded ? tileExploded : tileMine;
      case Cell.CellType.Number:
        return GetTileNumber(cell.number);
      default:
        return null;
    }
  }

  private Tile GetTileNumber(int number)
  {
    switch (number)
    {
      case 1:
        return tile1;
      case 2:
        return tile2;
      case 3:
        return tile3;
      case 4:
        return tile4;
      case 5:
        return tile5;
      case 6:
        return tile6;
      case 7:
        return tile7;
      case 8:
        return tile8;
      default:
        return null;
    }
  }
}
