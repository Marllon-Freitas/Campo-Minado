using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
  private Board board;
  private Cell[,] cells;
  private bool isGameOver;
  private bool isGameWon;
  public GameOverScreen gameOverScreen;

  public Text GameOverText;

  public int width = 16;
  public int height = 16;
  public int mines = 32;

  public void OnValidate()
  {
    mines = Mathf.Clamp(mines, 0, width * height);
  }

  private void Awake()
  {
    board = GetComponentInChildren<Board>();
  }

  private void Start()
  {
    NewGame();
  }

  private void NewGame()
  {
    cells = new Cell[width, height];
    isGameOver = false;
    isGameWon = false;
    GenerateCells();
    GenerateMines();
    GenerateNumbers();
    Camera.main.transform.position = new Vector3(width / 2, height / 2, -10);
    board.Draw(cells);
  }

  private void GenerateCells()
  {
    for (int x = 0; x < width; x++)
    {
      for (int y = 0; y < height; y++)
      {
        Cell cell = new Cell();
        cell.type = Cell.CellType.Empty;
        cell.position = new Vector3Int(x, y, 0);
        cells[x, y] = cell;
      }
    }
  }

  private void GenerateMines()
  {
    for (int i = 0; i < mines; i++)
    {
      int x = Random.Range(0, width);
      int y = Random.Range(0, height);
      while (cells[x, y].type == Cell.CellType.Mine)
      {
        x++;
        if (x >= width)
        {
          x = 0;
          y++;
          if (y >= height)
          {
            y = 0;
          }
        }
      }
      cells[x, y].type = Cell.CellType.Mine;
    }
  }

  private void GenerateNumbers()
  {
    for (int x = 0; x < width; x++)
    {
      for (int y = 0; y < height; y++)
      {
        Cell cell = cells[x, y];
        if (cell.type == Cell.CellType.Mine)
        {
          continue;
        }
        for (int i = -1; i <= 1; i++)
        {
          for (int j = -1; j <= 1; j++)
          {
            if (i == 0 && j == 0)
            {
              continue;
            }
            cell.number = CountMines(x, y);
            if (cell.number > 0)
            {
              cell.type = Cell.CellType.Number;
            }
            cells[x, y] = cell;
          }
        }
      }
    }
  }

  private int CountMines(int cellx, int cellY)
  {
    int count = 0;
    for (int i = -1; i <= 1; i++)
    {
      for (int j = -1; j <= 1; j++)
      {
        if (i == 0 && j == 0)
        {
          continue;
        }
        int x = cellx + i;
        int y = cellY + j;
        if (x < 0 || x >= width || y < 0 || y >= height)
        {
          continue;
        }
        if (cells[x, y].type == Cell.CellType.Mine)
        {
          count++;
        }
      }
    }
    return count;
  }

  private void Update()
  {
    if (Input.GetKeyDown(KeyCode.R))
    {
      NewGame();
      gameOverScreen.gameObject.SetActive(false);
    }
    if (!isGameOver)
    {
      if (Input.GetMouseButtonDown(0))
      {
        Vector3Int position = board.Tilemap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if (position.x < 0 || position.x >= width || position.y < 0 || position.y >= height)
        {
          return;
        }
        Reveal(position.x, position.y);
        board.Draw(cells);
      }
      else if (Input.GetMouseButtonDown(1))
      {
        FlagCell();
      }
    }
    else
    {
      HandleGameOver();
    }
  }

  private void FlagCell()
  {
    Vector3Int position = board.Tilemap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    if (position.x < 0 || position.x >= width || position.y < 0 || position.y >= height)
    {
      return;
    }
    Cell cell = cells[position.x, position.y];
    if (cell.isRevealed)
    {
      return;
    }
    cell.isFlagged = !cell.isFlagged;
    cells[position.x, position.y] = cell;
    board.Draw(cells);
  }

  private void Reveal(int x, int y)
  {
    Cell cell = cells[x, y];
    if (cell.isRevealed || cell.isFlagged)
    {
      return;
    }
    cell.isRevealed = true;
    cells[x, y] = cell;
    CheckWinCondition();
    if (cell.type == Cell.CellType.Mine)
    {
      cell.isExploded = true;
      cells[x, y] = cell;
      Explode(cell);
      return;
    }
    if (cell.type == Cell.CellType.Number)
    {
      return;
    }
    for (int i = -1; i <= 1; i++)
    {
      for (int j = -1; j <= 1; j++)
      {
        if (i == 0 && j == 0)
        {
          continue;
        }
        int x1 = x + i;
        int y1 = y + j;
        if (x1 < 0 || x1 >= width || y1 < 0 || y1 >= height)
        {
          continue;
        }
        Reveal(x1, y1);
      }
    }
  }

  private void CheckWinCondition()
  {
    for (int x = 0; x < width; x++)
    {
      for (int y = 0; y < height; y++)
      {
        Cell cell = cells[x, y];
        if (cell.type != Cell.CellType.Mine && !cell.isRevealed)
        {
          return;
        }
      }
    }
    isGameOver = true;
    isGameWon = true;
    for (int x = 0; x < width; x++)
    {
      for (int y = 0; y < height; y++)
      {
        Cell cell = cells[x, y];
        if (cell.type == Cell.CellType.Mine)
        {
          cell.isFlagged = true;
          cells[x, y] = cell;
        }
      }
    }
  }

  private void Explode(Cell cell)
  {
    isGameOver = true;
    cell.isRevealed = true;
    for (int x = 0; x < width; x++)
    {
      for (int y = 0; y < height; y++)
      {
        Cell cell1 = cells[x, y];
        if (cell1.type == Cell.CellType.Mine)
        {
          cell1.isRevealed = true;
          cells[x, y] = cell1;
        }
      }
    }
  }

  public void HandleGameOver()
  {
    if (isGameOver)
    {
      gameOverScreen.gameObject.SetActive(true);
      if (isGameWon)
      {
        gameOverScreen.ShowText("Você ganhou!");
      }
      else
      {
        gameOverScreen.ShowText("Você perdeu!");
      }
    }
    else
    {
      gameOverScreen.gameObject.SetActive(false);
    }
  }
}

