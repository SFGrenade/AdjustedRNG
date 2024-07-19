using System;
using System.Collections.Generic;
using UnityEngine;
using Logger = Modding.Logger;

namespace AdjustedRNG;

internal class CountedContextValue
{
    public int Index = 0;
    public ContextValue Value;
}

internal class ContextDependendRandom
{
    private static Dictionary<string, CountedContextValue> contextMap = new ();

    public static void AddContext(string context, ContextValue value)
    {
        contextMap.Add(context, new CountedContextValue()
        {
            Index = 0,
            Value = value
        });
    }

    private static float getValueForContext(string context)
    {
        if (!contextMap.ContainsKey(context))
        {
            Logger.LogWarn($"[AdjustedRNG][ContextDependendRandom] - Context '{context}' not present in settings!");
            AddContext(context, new ContextValue()
            {
                IsSingle = true,
                SingleValue = 0.5f,
                ArrayValue = Array.Empty<float>()
            });
            return 0.5f;
        }
        var entry = contextMap[context];
        if (entry.Value.IsSingle)
        {
            return entry.Value.SingleValue;
        }
        else
        {
            float ret = entry.Value.ArrayValue[contextMap[context].Index];
            entry.Index = (entry.Index + 1) % entry.Value.ArrayValue.Length;
            return ret;
        }
    }

    public static float Range(float min, float max, string context)
    {
        if (max < min)
        {
            (min, max) = (max, min);
        }
        float ret = min + ((max - min) * getValueForContext(context));
        Logger.LogDebug($"[AdjustedRNG][ContextDependendRandom][Range]`float` ({min}, {max}, '{context}') => {ret}");
        return ret;
    }
    public static int Range(int min, int max, string context)
    {
        int ret = RandomRangeInt(min, max, context);
        Logger.LogDebug($"[AdjustedRNG][ContextDependendRandom][Range]`int` ({min}, {max}, '{context}') => {ret}");
        return ret;
    }
    public static int RandomRangeInt(int min, int max, string context)
    {
        if (max == min)
        {
            getValueForContext(context);
            return min;
        }
        if (max < min)
        {
            (min, max) = (max, min);
        }
        int ret = (int) (min + ((max - min) * getValueForContext(context)));
        if (ret == max)
        {
            ret = max - 1;
        }
        Logger.LogDebug($"[AdjustedRNG][ContextDependendRandom][RandomRangeInt] ({min}, {max}, '{context}') => {ret}");
        return ret;
    }
    public static float get_value(string context)
    {
        float ret = getValueForContext(context);
        Logger.LogDebug($"[AdjustedRNG][ContextDependendRandom][get_value] ('{context}') => {ret}");
        return ret;
    }
    public static Vector3 get_insideUnitSphere(string context)
    {
        Vector3 ret = get_onUnitSphere(context);
        float radius = Mathf.Pow(getValueForContext(context), 1 / 3);
        float x = ret.x * radius;
        float y = ret.y * radius;
        float z = ret.z * radius;
        ret.Set(x, y, z);
        Logger.LogDebug($"[AdjustedRNG][ContextDependendRandom][get_insideUnitSphere] ('{context}') => {ret}");
        return ret;
    }
    public static Vector2 get_insideUnitCircle(string context)
    {
        Vector2 ret = new Vector2();
        float x = getValueForContext(context);
        float y = getValueForContext(context);
        float radius = Mathf.Pow(getValueForContext(context), 1 / 2);
        ret.Set(x, y);
        float mag = ret.magnitude;
        ret.Set((x / mag) * radius, (y / mag) * radius);
        Logger.LogDebug($"[AdjustedRNG][ContextDependendRandom][get_insideUnitCircle] ('{context}') => {ret}");
        return ret;
    }
    public static Vector3 get_onUnitSphere(string context)
    {
        Vector3 ret = new Vector3();
        float x = getValueForContext(context);
        float y = getValueForContext(context);
        float z = getValueForContext(context);
        ret.Set(x, y, z);
        float mag = ret.magnitude;
        ret.Set(x / mag, y / mag, z / mag);
        Logger.LogDebug($"[AdjustedRNG][ContextDependendRandom][get_onUnitSphere] ('{context}') => {ret}");
        return ret;
    }
    public static Quaternion get_rotation(string context)
    {
        Quaternion ret = new Quaternion();
        ret.x = Range(-1.0f, 1.0f, context);
        ret.y = Range(-1.0f, 1.0f, context);
        ret.z = Range(-1.0f, 1.0f, context);
        ret.w = Range(-1.0f, 1.0f, context);
        Logger.LogDebug($"[AdjustedRNG][ContextDependendRandom][get_rotation] ('{context}') => {ret}");
        return ret;
    }
    public static Quaternion get_rotationUniform(string context)
    {
        Quaternion ret = get_rotation(context);
        Logger.LogDebug($"[AdjustedRNG][ContextDependendRandom][get_rotationUniform] ('{context}') => {ret}");
        return ret;
    }
    public static int RandomRange(int min, int max, string context)
    {
        int ret = Range(min, max, context);
        Logger.LogDebug($"[AdjustedRNG][ContextDependendRandom][RandomRange]`int` ({min}, {max}, '{context}') => {ret}");
        return ret;
    }
    public static float RandomRange(float min, float max, string context)
    {
        float ret = Range(min, max, context);
        Logger.LogDebug($"[AdjustedRNG][ContextDependendRandom][RandomRange]`float` ({min}, {max}, '{context}') => {ret}");
        return ret;
    }

}