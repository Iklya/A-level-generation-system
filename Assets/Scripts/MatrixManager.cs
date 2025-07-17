using UnityEngine;

public class MatrixManager
{
    public int[,] Matrix;
    public int MatrixSize;
    public int CenterX;
    public int CenterY;

    // сделать тут типы комнат 

    public MatrixManager(int N)
    {
        MatrixSize = 3 * N;
        Matrix = new int[MatrixSize, MatrixSize];

        for (int x = 0; x < MatrixSize; x++)
            for (int y = 0; y < MatrixSize; y++)
                Matrix[x, y] = -1;

        CenterX = MatrixSize / 2;
        CenterY = MatrixSize / 2;

        Debug.Log($"ћатрица {MatrixSize}x{MatrixSize} с центром в [{CenterX}, {CenterY}]");
    }

    public void SetCell(int x, int y, int value)
    {
        Matrix[x, y] = value;
    }

    public int GetCell(int x, int y)
    {
        return Matrix[x, y];
    }
}
