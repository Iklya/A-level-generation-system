using System.IO;
using UnityEngine;

public class MatrixManager
{
    public int[,] Matrix;
    public int MatrixSize;

    public int CenterX;
    public int CenterY;

    public int CurrentX;
    public int CurrentY;

    public MatrixManager(int N)
    {
        MatrixSize = 2 * N;
        Matrix = new int[MatrixSize, MatrixSize];

        for (int x = 0; x < MatrixSize; x++)
            for (int y = 0; y < MatrixSize; y++)
                Matrix[x, y] = -1;

        CenterX = MatrixSize / 2;
        CenterY = MatrixSize / 2;

        CurrentX = CenterX;
        CurrentY = CenterY;

        Debug.Log($"Матрица {MatrixSize}x{MatrixSize} с центром в [{CenterX}, {CenterY}]");
        MatrixToFile();
    }

    public void SetCell(int x, int y, int value)
    {
        Matrix[x, y] = value;
        MatrixToFile();
    }

    public int GetCell(int x, int y)
    {
        return Matrix[x, y];
    }

    public void MatrixToFile()
    {
        string filePath = Path.Combine(Application.dataPath, "matrix2N.txt");
        using (TextWriter tw = new StreamWriter(filePath))
        {
            for (int y = MatrixSize - 1; y >= 0; y--)
            {
                for (int x = 0; x < MatrixSize; x++)
                {
                    tw.Write(Matrix[x, y].ToString().PadLeft(3) + " ");
                }
                tw.WriteLine();
            }
        }
    }

    public Vector2 MatrixToGame(int x, int y, Vector2 roomSize)
    {
        float gameX = (x - CenterX) * roomSize.x;
        float gameY = (y - CenterY) * roomSize.y;
        return new Vector2(gameX, gameY);
    }

    public void MoveToNextCell(int dir)
    {
        switch (dir)
        {
            case 0:
                CurrentX -= 1;
                break;
            case 1:
                CurrentX += 1;
                break;
            case 2:
                CurrentY += 1;
                break;
            case 3:
                CurrentY -= 1;
                break;
        }
    }

    public void Reset()
    {
        for (int x = 0; x < MatrixSize; x++)
            for (int y = 0; y < MatrixSize; y++)
                Matrix[x, y] = -1;

        CenterX = MatrixSize / 2;
        CenterY = MatrixSize / 2;

        CurrentX = CenterX;
        CurrentY = CenterY;
    }
}
