using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UIElements;

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

    public int[] size()
    {
        return new int[2] { this.Sequence.Count, this.Sequence[0].Count };
    }

    // operator+ 오버로딩
    public static Matrix operator +(Matrix mat1, Matrix mat2)
    {
        if (mat1.Sequence.Count != mat2.Sequence.Count || mat1.Sequence[0].Count != mat2.Sequence[0].Count)
        {
            Debug.LogError("Matrix dimensions must match for addition.");
            return null;
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
            Debug.LogError("Matrix dimensions must match for subtraction.");
            return null;
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
            Debug.LogError("Matrix dimensions must match for subtraction.");
            return null;
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

    public Matrix powHadamard(int n)
    {
        if (n < 2)
            return this;

        Matrix sumMat = new Matrix(this.size()[0], this.size()[1], 1f);

        for(int i = 0;i < n; i++)
        {
            sumMat = sumMat & this;
        }

        return sumMat;
    }

    public Matrix readMatrix(int row_length, int column_length, int start_row, int start_column)
    {
        Matrix temp = new Matrix(row_length, column_length);
        if (size()[0] < start_row + row_length || size()[1] < start_column + column_length)
        {
            Debug.LogError($"행렬 범위를 초과했습니다. \n len: ({size()[0]}, {size()[1]}) , copylen: ({row_length},{column_length}) , origin: ({start_row}, {start_column})");
            return null;
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

    public Matrix writeMatrix(Matrix args, int start_row, int start_column)
    {
        Matrix temp = this;

        if (size()[0] < start_row + args.size()[0] || size()[1] < start_column + args.size()[1])
        {
            Debug.LogError("행렬 범위를 초과했습니다.");
            return null;
        }

        for (int i = start_row; i < start_row + args.size()[0]; i++)
        {
            for (int j = start_column; j < start_column + args.size()[1]; j++)
            {
                temp.Sequence[i][j] = args.Sequence[i - start_row][j - start_column];
            }
        }

        return temp;
    }

    public float reduction2Scalar()
    {
        float sum = 0f;

        for (int i = 0; i < size()[0]; i++)
        {
            for (int j = 0; j < size()[1]; j++)
            {
                sum += Sequence[i][j];
            }
        }

        return sum;
    }

    public Matrix DownScaling(int window_size)
    {
        // 횟수를 내림해서 포문돌려서 그 안에서 평균값으로 통일시키기
        // 평균값 구할 땐 Reduction2Scalar 사용해서 합산
        // 남는 부분이 있으면 남는 부분끼리 평균내서 수정하기

        Matrix scaledMat = new Matrix(size()[0], size()[1]);

        Matrix kernal;

        for (int i = 0; i < (int)Mathf.Ceil((float)size()[0] / (float)window_size); i++)
        {
            if (i == (int)Mathf.Ceil((float)size()[1] / (float)window_size) - 1)
            {
                // 열의 맨 끝쪽
                for (int j = 0; j < (int)Mathf.Ceil((float)size()[1] / (float)window_size); j++)
                {
                    // 행의 맨 끝쪽
                    if (j == (int)Mathf.Ceil((float)size()[1] / (float)window_size) - 1)
                    {
                        kernal = new Matrix(size()[0] - (i * window_size), size()[1] - (j * window_size),
                            readMatrix(size()[0] - (i * window_size), size()[1] - (j * window_size), i * window_size, j * window_size)
                            .reduction2Scalar() / ((size()[0] - (i * window_size)) * (size()[1] - (j * window_size))));

                        scaledMat.writeMatrix(kernal, size()[0] - (i * window_size), size()[1] - (j * window_size));
                    }
                    else
                    {
                        kernal = new Matrix(size()[0] - (i * window_size), window_size,
                            readMatrix(size()[0] - (i * window_size), window_size, i * window_size, j * window_size)
                            .reduction2Scalar() / (window_size * (size()[0] - (i * window_size))));

                        scaledMat.writeMatrix(kernal, size()[0] - (i * window_size), j * window_size);
                    }
                }
            }
            else
            {
                for (int j = 0; j < (int)Mathf.Ceil((float)size()[1] / (float)window_size); j++)
                {
                    //행의 맨 끝쪽
                    if (j == (int)Mathf.Ceil((float)size()[1] / (float)window_size) - 1)
                    {
                        kernal = new Matrix(window_size, size()[1] - (j * window_size),
                            readMatrix(window_size, size()[1] - (j * window_size), i * window_size, j * window_size)
                            .reduction2Scalar() / (window_size * (size()[1] - (j * window_size))));

                        scaledMat.writeMatrix(kernal, i * window_size, size()[1] - (j * window_size));
                    }
                    else
                    {
                        kernal = new Matrix(window_size, window_size,
                            readMatrix(window_size, window_size, i * window_size, j * window_size)
                            .reduction2Scalar() / (window_size * window_size));

                        scaledMat.writeMatrix(kernal, i * window_size, j * window_size);
                    }
                }
            }

        }

        return scaledMat;
    }

    public static Matrix dot2D(Matrix[] mat1, Matrix[] mat2)
    {
        if(mat1.Length != mat2.Length)
        {
            Debug.LogError("벡터의 차원 수가 서로 불일치 합니다.");
            return null;
        }

        int[] mat_eleSize = mat1[0].size();

        if (!mat_eleSize.SequenceEqual(mat2[0].size()))
        {
            Debug.LogError("벡터의 차원 수는 서로 일치하지만, 요소가 되는 행렬의 크기가 서로 불일치합니다.");
            return null;
        }

        Matrix dotValue = new Matrix(mat_eleSize[0], mat_eleSize[1]);

        for (int i = 0; i < mat1.Length; i++)
        {
            dotValue += mat1[i] & mat2[i];
        }

        return dotValue;

    }

    public static Matrix[] vectorTranslationOn2DMatrix(Matrix[] pivot, Matrix[] vector)
    {
        if (pivot.Length != vector.Length)
        {
            Debug.LogError("벡터의 차원 수가 서로 불일치 합니다.");
            return null;
        }

        int[] mat_eleSize = pivot[0].size();

        if (!mat_eleSize.SequenceEqual(vector[0].size()))
        {
            Debug.LogError("벡터의 차원 수는 서로 일치하지만, 요소가 되는 행렬의 크기가 서로 불일치합니다.");
            return null;
        }

        Matrix[] translatedVector = new Matrix[2];

        for (int i = 0; i < pivot.Length; i++)
        {
            // randomPoint - pivot
            translatedVector[i] = vector[i] - pivot[i];
        }

        return translatedVector;
    }

    public override string ToString()
    {
        string str = "";

        for (int i = 0; i < size()[0]; i++)
        {
            for (int j = 0; j < size()[1]; j++)
            {
                str += Sequence[i][j];
                str += " ";
            }
            
            if (i != size()[0] - 1) 
                str += "\n";
        }

        return base.ToString() + ":\n" + str;
    }

}
public class PerlinNoise
{
    private int row_length { get; set; }
    private int column_length { get; set; }
    private uint seed_1 { get; set; }
    private uint seed_2 { get; set; }
    private int batch_size { get; set; }

    public PerlinNoise(int row_length, int column_length, uint seed_1, uint seed_2, int batch_size = 1)
    {
        this.row_length = row_length;
        this.column_length = column_length;
        this.seed_1 = seed_1;
        this.seed_2 = seed_2;
        this.batch_size = batch_size;
    }

    public Matrix getPerlinMatrix()
    {
        return perlinWeightMatrix(row_length, column_length, seed_1, seed_2).DownScaling(batch_size);
    }

    public Matrix[] getRandomVectorMatrix(int row_length, int column_length, uint seed)
    {
        Matrix[] result = new Matrix[2];
        IntVector2Random vec2rand = new IntVector2Random(seed >> 16, seed & 0xFFFF);
        Matrix vector_x_mat = new Matrix(row_length + 1, column_length + 1);
        Matrix vector_y_mat = new Matrix(row_length + 1, column_length + 1);

        for (int i = 0; i < row_length + 1; i++)
        {

            for (int j = 0; j < column_length + 1; j++)
            {

                float[] randomVec2 = new float[2];
                int[] randomVec2_Int = vec2rand.rand();

                for (int k = 0; k < 2; k++)
                {

                    randomVec2[k] = ((float)(randomVec2_Int[k] - 0x3FFF) / (float)0x3FFF)  * Mathf.Sqrt(0.5f);
                }

                vector_x_mat.Sequence[i][j] = randomVec2[0];
                vector_y_mat.Sequence[i][j] = randomVec2[1];
            }
        }

        result[0] = vector_x_mat;
        result[1] = vector_y_mat;

        return result;

    }

    public Matrix[] getRandomGridPointMatrix(int row_length, int column_length, uint seed)
    {
        Matrix[] result = new Matrix[2];
        IntVector2Random vec2rand = new IntVector2Random(seed >> 16, seed & 0xFFFF);
        Matrix vector_x_mat = new Matrix(row_length + 1, column_length + 1);
        Matrix vector_y_mat = new Matrix(row_length + 1, column_length + 1);

        for (int i = 0; i < row_length + 1; i++)
        {

            for (int j = 0; j < column_length + 1; j++)
            {

                float[] randomVec2 = new float[2];
                int[] randomVec2_Int = vec2rand.rand();

                for (int k = 0; k < 2; k++)
                {

                    randomVec2[k] = ((float)randomVec2_Int[k] / (float)0x7FFF);
                }

                vector_x_mat.Sequence[i][j] = randomVec2[0];
                vector_y_mat.Sequence[i][j] = randomVec2[1];
            }
        }

        result[0] = vector_x_mat;
        result[1] = vector_y_mat;

        return result;

    }

    private enum Corner{
        LU = 0b00,
        LD = 0b01,
        RU = 0b10,
        RD = 0b11,
        Len
    }

    public Matrix perlinWeightMatrix(int row_length, int column_length, uint seed1, uint seed2)
    {
        Matrix weightSumMat = new Matrix(row_length, column_length);
        Matrix[] randomVectorMat = getRandomVectorMatrix(row_length, column_length, seed1);
        Matrix[] randomPointMat = getRandomGridPointMatrix(row_length - 1, column_length - 1, seed2);

        //Matrix[] LUVectorMat = new Matrix[2],
        //    RUVectorMat = new Matrix[2],
        //    LDVectorMat = new Matrix[2],
        //    RDVectorMat = new Matrix[2];

        int dimension = 2;

        Matrix[,] VectorMats = new Matrix[dimension, (int)Corner.Len];

        int[] randomVectorMatSize = randomVectorMat[0].size();



        //for (int i = 0; i < 2; i++)
        //{
        //    LUVectorMat[i] = randomVectorMat[i].readMatrix(randomVectorMatSize[0] - 1, randomVectorMatSize[1] - 1, 0, 0);
        //    RUVectorMat[i] = randomVectorMat[i].readMatrix(randomVectorMatSize[0] - 1, randomVectorMatSize[1] - 1, 1, 0);
        //    LDVectorMat[i] = randomVectorMat[i].readMatrix(randomVectorMatSize[0] - 1, randomVectorMatSize[1] - 1, 0, 1);
        //    RDVectorMat[i] = randomVectorMat[i].readMatrix(randomVectorMatSize[0] - 1, randomVectorMatSize[1] - 1, 1, 1);
        //}

        for (int i = 0; i < VectorMats.GetLength(1); i++) // 모서리 수
        {
            for (int j = 0; j < VectorMats.GetLength(0); j++) // 차원 수
            {
                VectorMats[j, i] = randomVectorMat[j].readMatrix(randomVectorMatSize[0] - 1, randomVectorMatSize[1] - 1, (i >> 1) & 1, i & 1);
            }
        }

        Matrix[] DotMats = new Matrix[(int)Corner.Len];

        Matrix lerpX1, lerpX2, lerpYnX;

        // dot = gradiant (dot) distance(|rV - grad|)
        //LUDotMat = Matrix.dot2D(LUVectorMat, Matrix.vectorTranslationOn2DMatrix(LUVectorMat, randomPointMat));
        // etc...

        for (int i = 0; i < DotMats.Length; i++)
        {
            Matrix[] pointMat = new Matrix[dimension];
            
            for (int j = 0;j < dimension; j++)
            {
                pointMat[j] = VectorMats[j, i];
            }

            DotMats[i] = Matrix.dot2D(pointMat, Matrix.vectorTranslationOn2DMatrix(pointMat, randomPointMat));
        }

        Func<Matrix, Matrix> fade = t => {
            int x = t.size()[0];
            int y = t.size()[1];
            return t.powHadamard(3) & ((t & (t & (new Matrix(x, y, 6f)) - (new Matrix(x, y, 15f))) + (new Matrix(x, y, 10f))));
        };

        lerpX1 = Lerp(DotMats[(int)Corner.RU], DotMats[(int)Corner.RD], randomPointMat[0], fade);
        lerpX2 = Lerp(DotMats[(int)Corner.LU], DotMats[(int)Corner.LD], randomPointMat[0], fade);

        lerpYnX = Lerp(lerpX1, lerpX2, randomPointMat[1], fade);

        return lerpYnX;

        // lerp + fade(rVx and rVy)

        //weightSumMat =
        //    ((LUVectorMat[0] & randomPointMat[0] + LUVectorMat[1] & randomPointMat[1])
        //    + (RUVectorMat[0] & randomPointMat[0] + RUVectorMat[1] & randomPointMat[1])
        //    + (LDVectorMat[0] & randomPointMat[0] + LDVectorMat[1] &  randomPointMat[1]) 
        //    + (RDVectorMat[0] & randomPointMat[0] + RDVectorMat[1] & randomPointMat[1])) & new Matrix(row_length, column_length, 0.25f);

        //return weightSumMat;
    }

    Matrix Lerp(Matrix m1, Matrix m2, Matrix weight, Func<Matrix,Matrix> interpolation)
    {
        return m1 + (interpolation(weight) & (m2 - m1));
    }

    
}



public class IntVector2Random
{
    public IntVector2Random(uint seed1, uint seed2)
    {
        init_seed(seed1, seed2);
    }

    public IntVector2Random()
    {
        init_seed((uint)((DateTime.Now.Millisecond >> 16) * 0x7FFF), (uint)((DateTime.Now.Millisecond * 1103515245 + 12345) >> 16) * 0x7FFF);
    }



    private const int RAND_MAX = 0x7FFF; // 일반적인 RAND_MAX 값
    private static uint next1 = 1;
    private static uint next2 = (uint)((next1 * 1103515245 + 12345) >> 16) & RAND_MAX;


    public int[] rand()
    {
        int[] vector2 = new int[2];

        uint _next1 = next1;

        next1 = (uint)Mathf.Sqrt((next1 + next2) * next1) * 1103515245 + 12345;
        next2 = (uint)Mathf.Sqrt((_next1 + next2) * next2) * 1103515245 + 12345;

        vector2[0] = (int)((next1 >> 16) & RAND_MAX);
        vector2[1] = (int)((next2 >> 16) & RAND_MAX);


        return vector2;
    }

    public void init_seed(uint seed1, uint seed2)
    {
        next1 = seed1;
        next2 = seed2;
    }
}