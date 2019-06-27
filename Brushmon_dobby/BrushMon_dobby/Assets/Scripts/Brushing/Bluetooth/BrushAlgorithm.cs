using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


internal class BrushAlgorithm  {

    private static int nraw = 3;
    private static int nraws = 20;
    double[,] raws = new double[nraw,nraws];
    private static int nfeature = 3;
    private double[] feature = new double[nfeature];
    private  double classifier_acc = 0.5;
    private  int[] class_A = { 7, 8, 6, 9, 11, 16 };
    private  int[] class_B = { 5, 6, 9, 10, 13, 14 };
    private  int[] class_C = { 3, 4, 15 };
    private  int[] class_D = { 1, 2, 12 };
    private int[] regionScore_count = { -1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    private int[] regionScore = { -1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

    //상수들
    private  int START_AREA = 1, END_AREA = 16;
    private  int MATCH = 1, NOT_MATCH = 0;
    private  int BRUSHING_TIME = 5000; // AR 양치 시간

    //양치 종료 후 호출
    public int[] computeresult()
    {
        /**
         * 전체 양치 결과를 구하는 메소드
         * @return int[] 16개 영역의 최종 양치 결과
         */
        float brushingNumber = BRUSHING_TIME / 1000f * 6;
        int GOOD = 1, BAD = 0;
        int[] partScore = { -1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        for (int i = START_AREA; i <= END_AREA; i++)
        {
            regionScore[i] = (int)((regionScore_count[i]) / brushingNumber);
            if (regionScore[i] >= 0.4f)
            {
                partScore[i] = GOOD;
            }
            else
            {
                partScore[i] = BAD;
            }

            if (regionScore_count[i] == 0)
            {
                partScore[i] = BAD;
            }
        }
        return partScore;
    }

    int X = 0, Y = 1, Z = 2;
    double[] raw = new double[3];
    public int determineCurResult(byte[] sensorData, int curBrushArea)
    {
        short accx = (short)((((short)sensorData[1] & 0xff) << 8) | ((short)sensorData[2] & 0xff));    // 가속도 X Axis
        short accy = (short)((((short)sensorData[3] & 0xff) << 8) | ((short)sensorData[4] & 0xff));    // 가속도 Y Axis
        short accz = (short)((((short)sensorData[5] & 0xff) << 8) | ((short)sensorData[6] & 0xff));    // 가속도 Z Axis
        raw[X] = accy / 256f;
        raw[Y] = -accx / 256f;
        raw[Z] = accz / 256f;
        return determineCurResult(raw, curBrushArea);
    }

    //현재 화면이 ar로 양치하기 화면임 && bt로 데이터가 들어올 때 호출
    public int determineCurResult(double[] sensorData, int curBrushArea)
    {
        /**
         * 현재 양치 가이드를 잘 따라하고 있는지를 판별하는 메소드
         * @param double[] sensorData 센서 데이터 sensorData[0] : x축 / sensorData[1] : y축 / sensorData[2] : z축
         * @param int curBrushArea 현재 양치되고 있는 영역의 번호
         * @return int 1 good or bad
         */
        int curClass, curRes;
        curClass = determineCurClass(sensorData);
        curRes = matchRegion(curBrushArea, curClass);

        return curRes;
    }

    private int determineCurClass(double[] sensorData)
    {
        int determinedClass = 0, nsample = 10;
        //add raw
        for (int i = 0; i < nraw; i++)
        {
            for (int j = nraws - 1; j >= 0; j--)
            {
                if (j == 0)
                {
                    raws[i,j] = sensorData[i];
                }
                else
                {
                    raws[i,j] = raws[i,j - 1];
                }
            }
        }

        //extractFeatures
        for (int i = 0; i < 3; i++)
        {
            
            feature[i] = meanArray(copyOfRange((double[])GetRow(raws,i), 0, nsample));
        }

        //determineClass
        if (feature[0] > classifier_acc)
        {
            determinedClass = 1;
        }
        else if (feature[0] < -classifier_acc)
        {
            determinedClass = 2;
        }
        else if (feature[2] > classifier_acc)
        {
            determinedClass = 3;
        }
        else if (feature[2] < -classifier_acc)
        {
            determinedClass = 4;
        }

        return determinedClass;
    }

    private double meanArray(double[] array)
    {
        double sum = 0.0;
        double mean;
        for (int i = 0; i < array.Length; i++)
        {
            sum += array[i];
        }
        mean = sum / array.Length;

        return mean;
    }

    private int matchRegion(int curBrushArea, int curClass)
    {
        int curState;
        switch (curClass)
        {
            case 1:
                curState = isMember(curBrushArea, class_A);
                break;
            case 2:
                curState = isMember(curBrushArea, class_B);
                break;
            case 3:
                curState = isMember(curBrushArea, class_C);
                break;
            case 4:
                curState = isMember(curBrushArea, class_D);
                break;
            default:
                curState = NOT_MATCH;
                break;
        }

        if (curBrushArea <= END_AREA)
        {
            regionScore_count[curBrushArea] = regionScore_count[curBrushArea] + curState;
        }

        return curState;
    }

    private int isMember(int element, int[] array)
    {
        int tmp = NOT_MATCH;
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == element)
                tmp = MATCH;
        }
        return tmp;
    }

    double[] copyOfRange(double[] src, int start, int end)
    {
        int len = end - start;
        double[] dest = new double[len];
        // note i is always from 0
        for (int i = 0; i < len; i++)
        {
            dest[i] = src[start + i]; // so 0..n = 0+x..n+x
        }
        return dest;
    }

    public T[] GetRow<T>(T[,] matrix, int row)
    {
        var columns = matrix.GetLength(1);
        var array = new T[columns];
        for (int i = 0; i < columns; ++i)
            array[i] = matrix[row, i];
        return array;
    }

}
