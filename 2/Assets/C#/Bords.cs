using UnityEngine;

public class Board : MonoBehaviour
{
    public static int width = 10;
    public static int height = 20;

    public static Transform[,] grid = new Transform[width, height];

    public static void AddToGrid(Transform block)
    {
        Vector2 pos = Round(block.position);
        grid[(int)pos.x, (int)pos.y] = block;
    }

    public static bool IsFullLine(int y)
    {
        for (int x = 0; x < width; x++)
            if (grid[x, y] == null)
                return false;
        return true;
    }

    public static void DeleteLine(int y)
    {
        for (int x = 0; x < width; x++)
        {
            Destroy(grid[x, y].gameObject);
            grid[x, y] = null;
        }
    }

    public static void MoveDown(int y)
    {
        for (int x = 0; x < width; x++)
        {
            if (grid[x, y] != null)
            {
                grid[x, y - 1] = grid[x, y];
                grid[x, y] = null;
                grid[x, y - 1].position += Vector3.down;
            }
        }
    }

    public static void CheckLines()
    {
        for (int y = 0; y < height; y++)
        {
            if (IsFullLine(y))
            {
                DeleteLine(y);
                for (int i = y + 1; i < height; i++)
                    MoveDown(i);
                y--;
            }
        }
    }

    private static Vector2 Round(Vector2 pos)
    {
        return new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y));
    }
}
