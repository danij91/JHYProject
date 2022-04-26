using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public static class Extensions
{
    public static bool IsNullOrEmpty(this string inValue)
    {
        return string.IsNullOrEmpty(inValue);
    }

    public static int[] SplitInt(this string inValue, params char[] inSeperators)
    {
        if (inValue.IsNullOrEmpty())
        {
            return null;
        }

        var splitDatas = inValue.Split(inSeperators);
        var length = splitDatas.Length;
        if (length == 0)
        {
            return null;
        }

        var retDatas = new int[length];
        for (int i = 0; i < length; ++i)
        {
            if (int.TryParse(splitDatas[i], out var parsedData))
            {
                retDatas[i] = parsedData;
            }
            else
            {
                retDatas[i] = 0;
            }
        }

        return retDatas;
    }

    public static float[] SplitFloat(this string inValue, params char[] inSeperators)
    {
        if (inValue.IsNullOrEmpty())
        {
            return null;
        }

        var splitDatas = inValue.Split(inSeperators);
        var length = splitDatas.Length;
        if (length == 0)
        {
            return null;
        }

        var retDatas = new float[length];
        for (int i = 0; i < length; ++i)
        {
            if (float.TryParse(splitDatas[i], out var parsedData))
            {
                retDatas[i] = parsedData;
            }
            else
            {
                retDatas[i] = 0f;
            }
        }

        return retDatas;
    }

    public static double[] SplitDouble(this string inValue, params char[] inSeperators)
    {
        if (inValue.IsNullOrEmpty())
        {
            return null;
        }

        var splitDatas = inValue.Split(inSeperators);
        var length = splitDatas.Length;
        if (length == 0)
        {
            return null;
        }

        var retDatas = new double[length];
        for (int i = 0; i < length; ++i)
        {
            if (double.TryParse(splitDatas[i], out var parsedData))
            {
                retDatas[i] = parsedData;
            }
            else
            {
                retDatas[i] = 0;
            }
        }

        return retDatas;
    }

    public static byte[] SplitByte(this string inValue, params char[] inSeperators)
    {
        var splitDatas = inValue.Split(inSeperators);
        var length = splitDatas.Length;
        if (length == 0)
        {
            return null;
        }

        var retDatas = new byte[length];
        for (int i = 0; i < length; ++i)
        {
            if (byte.TryParse(splitDatas[i], out var parsedData))
            {
                retDatas[i] = parsedData;
            }
            else
            {
                retDatas[i] = 0;
            }
        }

        return retDatas;
    }
}
