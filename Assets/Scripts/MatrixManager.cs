using System.IO;
using UnityEngine;

public class MatrixManager
{
    public int[,] Matrix;
    public int MatrixSize;
    public int CenterX;
    public int CenterY;

    public MatrixManager(int N)
    {
        MatrixSize = 3 * N;
        Matrix = new int[MatrixSize, MatrixSize];

        for (int x = 0; x < MatrixSize; x++)
            for (int y = 0; y < MatrixSize; y++)
                Matrix[x, y] = -1;

        CenterX = MatrixSize / 2;
        CenterY = MatrixSize / 2;

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
            for (int j = 0; j < MatrixSize; j++)
            {
                for (int i = 0; i < MatrixSize; i++)
                {
                    tw.Write(Matrix[i, j].ToString().PadLeft(3) + " ");
                }
                tw.WriteLine();
            }
        }
    }
}
