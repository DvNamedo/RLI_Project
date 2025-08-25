using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Matrix
{
    public List<List<float>> Sequence = new List<List<float>>();

    public Matrix(int rowLength, int columnLength)
    {
        List<float> col = new List<float>();

        for (int i = 0; i < columnLength; i++)
        {
            col.Add(0.0f);
        }

        for (int i = 0; i < rowLength; i++)
        {
            Sequence.Add(new List<float>(col)); //list 내부제네릭이 값형이라 가능한 문법
        }
    }

    public Matrix(int rowLength, int columnLength, float initNumber)
    {
        List<float> col = new List<float>();

        for (int i = 0; i < columnLength; i++)
        {
            col.Add(initNumber);
        }
        for (int i = 0; i < rowLength; i++)
        {
            Sequence.Add(new List<float>(col));
        }
    }

    public Matrix(int rowLength, int columnLength, List<float> initColumn)
    {
        for (int i = 0; i < rowLength; i++)
        {
            Sequence.Add(new List<float>(initColumn));
        }
    }

    public int[] Size()
    {
        return new int[2] { this.Sequence.Count, this.Sequence[0].Count };
    }

    // operator+ 오버로딩
    public static Matrix operator +(Matrix mat1, Matrix mat2)
    {
        if (mat1.Sequence.Count != mat2.Sequence.Count || mat1.Sequence[0].Count != mat2.Sequence[0].Count)
        {
            throw new ArgumentException("Matrix dimensions must match for addition.");
        }

        int rowLength = mat1.Sequence.Count;
        int columnLength = mat1.Sequence[0].Count;
        Matrix result = new Matrix(rowLength, columnLength);

        for (int i = 0; i < rowLength; i++)
        {
            for (int j = 0; j < columnLength; j++)
            {
                result.Sequence[i][j] = mat1.Sequence[i][j] + mat2.Sequence[i][j];
            }
        }

        return result;
    }

    public static Matrix operator -(Matrix mat1, Matrix mat2)
    {
        if (mat1.Sequence.Count != mat2.Sequence.Count || mat1.Sequence[0].Count != mat2.Sequence[0].Count)
        {

            throw new ArgumentException("Matrix dimensions must match for subtraction.");
        }

        int rowLength = mat1.Sequence.Count;
        int columnLength = mat1.Sequence[0].Count;
        Matrix result = new Matrix(rowLength, columnLength);

        for (int i = 0; i < rowLength; i++)
        {
            for (int j = 0; j < columnLength; j++)
            {
                result.Sequence[i][j] = mat1.Sequence[i][j] - mat2.Sequence[i][j];
            }
        }

        return result;
    }

    public static Matrix operator &(Matrix mat1, Matrix mat2)
    {
        if (mat1.Sequence.Count != mat2.Sequence.Count || mat1.Sequence[0].Count != mat2.Sequence[0].Count)
        {
            throw new ArgumentException("Matrix dimensions must match for subtraction.");
        }

        int rowLength = mat1.Sequence.Count;
        int columnLength = mat1.Sequence[0].Count;
        Matrix result = new Matrix(rowLength, columnLength);

        for (int i = 0; i < rowLength; i++)
        {
            for (int j = 0; j < columnLength; j++)
            {
                result.Sequence[i][j] = mat1.Sequence[i][j] * mat2.Sequence[i][j];
            }
        }

        return result;
    }

    public Matrix PowHadamard(int n)
    {
        if (n < 2)
            return this;

        Matrix sumMat = new Matrix(this.Size()[0], this.Size()[1], 1f);

        for (int i = 0; i < n; i++)
        {
            sumMat = sumMat & this;
        }

        return sumMat;
    }

    public Matrix ReadMatrix(int row_length, int column_length, int start_row, int start_column)
    {
        Matrix temp = new Matrix(row_length, column_length);
        if (Size()[0] < start_row + row_length || Size()[1] < start_column + column_length)
        {
            throw new ArgumentOutOfRangeException("Requested matrix region exceeds matrix bounds.");
        }

        for (int i = start_row; i < start_row + row_length; i++)
        {
            for (int j = start_column; j < start_column + column_length; j++)
            {
                temp.Sequence[i - start_row][j - start_column] = Sequence[i][j];
            }
        }

        return temp;
    }

    public Matrix WriteMatrix(Matrix args, int start_row, int start_column)
    {
        Matrix temp = this;

        if (Size()[0] < start_row + args.Size()[0] || Size()[1] < start_column + args.Size()[1])
        {
            throw new ArgumentOutOfRangeException("Requested matrix region exceeds matrix bounds.");
        }

        for (int i = start_row; i < start_row + args.Size()[0]; i++)
        {
            for (int j = start_column; j < start_column + args.Size()[1]; j++)
            {
                temp.Sequence[i][j] = args.Sequence[i - start_row][j - start_column];
            }
        }

        return temp;
    }

    public float Reduction2Scalar()
    {
        float sum = 0f;

        for (int i = 0; i < Size()[0]; i++)
        {
            for (int j = 0; j < Size()[1]; j++)
            {
                sum += Sequence[i][j];
            }
        }

        return sum;
    }

    public Matrix Pooling(int window_size)
    {
        if (window_size < 1)
            return this;

        int H = Size()[0];
        int W = Size()[1];
        Matrix result = new Matrix(H, W);

        for (int i = 0; i < Math.Ceiling((float)H / window_size); i++)
        {
            for (int j = 0; j < Math.Ceiling((float)W / window_size); j++)
            {
                int row_start = i * window_size;
                int col_start = j * window_size;
                int row_len = Math.Min(window_size, H - row_start);
                int col_len = Math.Min(window_size, W - col_start);

                Matrix patch = ReadMatrix(row_len, col_len, row_start, col_start);
                float avg = patch.Reduction2Scalar() / (row_len * col_len);
                Matrix filled = new Matrix(row_len, col_len, avg);
                result.WriteMatrix(filled, row_start, col_start);
            }
        }

        return result;
    }

    public static Matrix Dot2D(Matrix[] mat1, Matrix[] mat2)
    {
        if(mat1 == null || mat2 == null)
            throw new ArgumentNullException("mat1 or mat2 is null");

        if (mat1.Length != mat2.Length)
        {
            throw new ArgumentException("The number of mat1 and mat2 elements must match.");
        }

        int[] mat_eleSize = mat1[0].Size();

        if (!mat_eleSize.SequenceEqual(mat2[0].Size()))
        {
            throw new ArgumentException("Each mat1 and mat2 matrix must have the same shape.");
        }

        Matrix dotValue = new Matrix(mat_eleSize[0], mat_eleSize[1]);

        for (int i = 0; i < mat1.Length; i++)
        {
            dotValue += mat1[i] & mat2[i];
        }

        return dotValue;

    }

    public static Matrix[] VectorTranslationOn2DMatrix(Matrix[] pivot, Matrix[] vector)
    {
        if (pivot == null || vector == null)
            throw new ArgumentNullException("pivot or vector is null");

        if (pivot.Length != vector.Length)
        {
            throw new ArgumentException("The number of pivot and vector elements must match.");
        }

        int[] mat_eleSize = pivot[0].Size();

        if (!mat_eleSize.SequenceEqual(vector[0].Size()))
        {
            throw new ArgumentException("Each pivot and vector matrix must have the same shape.");
        }

        Matrix[] translatedVector = new Matrix[2];

        for (int i = 0; i < pivot.Length; i++)
        {
            // randomPoint - pivot
            translatedVector[i] = vector[i] - pivot[i];
        }

        return translatedVector;
    }

    public Matrix FunctionInMatrix(Func<float, float> func)
    {
        int row_length = Size()[0];
        int col_length = Size()[1];

        Matrix result = new(row_length, col_length);

        for (int i = 0; i < row_length; i++)
        {
            for (int j = 0; j < col_length; j++)
            {

                result.Sequence[i][j] = func(Sequence[i][j]);

            }
        }

        return result;

    }

    public string ToString(int formatting = 8)
    {
        string str = "";

        for (int i = 0; i < Size()[0]; i++)
        {
            for (int j = 0; j < Size()[1]; j++)
            {
                var val = Sequence[i][j];
                str += val < 0 ? "" : "+";
                str += Math.Round(val, formatting);
                str += " ";
            }

            if (i != Size()[0] - 1)
                str += "\n";
        }

        return base.ToString() + ":\n" + str;
    }

}
