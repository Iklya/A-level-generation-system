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
        MatrixSize = 3 * N;
        Matrix = new int[MatrixSize, MatrixSize];

        for (int x = 0; x < MatrixSize; x++)
            for (int y = 0; y < MatrixSize; y++)
                Matrix[x, y] = -1;

        CenterX = MatrixSize / 2;
        CenterY = MatrixSize / 2;

        CurrentX = CenterX;
        CurrentY = CenterY;

        Debug.Log($"Матрица {MatrixSize}x{MatrixSize} с центром в [{CenterX}, {CenterY}]");
        MatrixInFile();
    }

    public void SetCell(int x, int y, int value)
    {
        Matrix[x, y] = value;
        MatrixInFile();
    }

    public int GetCell(int x, int y)
    {
        return Matrix[x, y];
    }

    public void MatrixInFile()
    {
        string filePath = Path.Combine(Application.dataPath, "matrix3N.txt");
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
}
