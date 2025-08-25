using System;
using System.Collections.Generic;
using System.Linq;
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

    public Matrix GetPerlinMatrix()
    {
        return PerlinWeightMatrix(row_length, column_length, seed_1, seed_2).Pooling(batch_size);
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
                int[] randomVec2_Int = vec2rand.Rand();

                //for (int k = 0; k < 2; k++)
                //{

                //    randomVec2[k] = ((float)(randomVec2_Int[k] - 0x3FFF) / (float)0x3FFF)  * Mathf.Sqrt(0.5f);
                //}

                randomVec2[0] = (float)Math.Cos((randomVec2_Int[0] / (double)0x7FFF) * 2L * Math.PI);
                randomVec2[1] = (float)Math.Sin((randomVec2_Int[0] / (double)0x7FFF) * 2L * Math.PI);

                vector_x_mat.Sequence[i][j] = randomVec2[0];
                vector_y_mat.Sequence[i][j] = randomVec2[1];
            }
        }

        result[0] = vector_x_mat;
        result[1] = vector_y_mat;

        return result;

    }

    public Matrix[] GetRandomGridPointMatrix(int row_length, int column_length, uint seed)
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
                int[] randomVec2_Int = vec2rand.Rand();

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

    public Matrix PerlinWeightMatrix(int row_length, int column_length, uint seed1, uint seed2)
    {

        Matrix[] randomVectorMat = getRandomVectorMatrix(row_length, column_length, seed1);
        Matrix[] randomPointMat = GetRandomGridPointMatrix(row_length - 1, column_length - 1, seed2);

        //Matrix[] LUVectorMat = new Matrix[2],
        //    RUVectorMat = new Matrix[2],
        //    LDVectorMat = new Matrix[2],
        //    RDVectorMat = new Matrix[2];

        int dimension = 2;

        Matrix[,] VectorMats = new Matrix[dimension, (int)Corner.Len];

        int[] randomVectorMatSize = randomVectorMat[0].Size();



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
                VectorMats[j, i] = randomVectorMat[j].ReadMatrix(randomVectorMatSize[0] - 1, randomVectorMatSize[1] - 1, (i >> 1) & 1, i & 1);
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

            DotMats[i] = Matrix.Dot2D(pointMat, Matrix.VectorTranslationOn2DMatrix(pointMat, randomPointMat));
        }

        Func<Matrix, Matrix> fade = t => {
            return t.FunctionInMatrix((t) =>
            {
                float t2 = t * t;
                float t3 = t2 * t;

                return (6f * t2 - 15f * t + 10f) * t3; // (6t^2 - 15t + 10)*t^3
            });
        };

        lerpX1 = Lerp(DotMats[(int)Corner.RU], DotMats[(int)Corner.LU], randomPointMat[0], fade);
        lerpX2 = Lerp(DotMats[(int)Corner.RD], DotMats[(int)Corner.LD], randomPointMat[0], fade);
        
        lerpYnX = Lerp(lerpX1, lerpX2, randomPointMat[1], fade);


        return lerpYnX;
        // lerp + fade(rVx and rVy)SS

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
        Init_seed(seed1, seed2);
    }

    public IntVector2Random()
    {
        Init_seed((uint)((DateTime.Now.Millisecond >> 16) * 0x7FFF), (uint)((DateTime.Now.Millisecond * 1103515245 + 12345) >> 16) * 0x7FFF);
    }



    private const int RAND_MAX = 0x7FFF; // 일반적인 RAND_MAX 값
    private static uint next1 = 1;
    private static uint next2 = (uint)((next1 * 1103515245 + 12345) >> 16) & RAND_MAX;


    public int[] Rand()
    {
        int[] vector2 = new int[2];

        uint _next1 = next1;

        next1 = (uint)Math.Sqrt((next1 + next2) * next1) * 1103515245 + 12345;
        next2 = (uint)Math.Sqrt((_next1 + next2) * next2) * 1103515245 + 12345;

        

        vector2[0] = (int)((next1 >> 16) & RAND_MAX);
        vector2[1] = (int)((next2 >> 16) & RAND_MAX);


        return vector2;
    }

    public void Init_seed(uint seed1, uint seed2)
    {
        next1 = seed1;
        next2 = seed2;
    }
}