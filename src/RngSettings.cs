using System;
using System.Collections.Generic;

namespace AdjustedRNG;

[Serializable]
public class ContextValue
{
    public bool IsSingle = true;
    public float SingleValue = 0.5f;
    public float[] ArrayValue = {
        0.5f, 0.5f, 0.5f
    };
}

public class RngSettings
{
    public Dictionary<string, ContextValue> Contexts = new()
    {
        { "Test", new ContextValue()
        {
            IsSingle = true,
            SingleValue = 0.5f,
            ArrayValue = new float[] {
                0.5f, 0.5f, 0.5f
            }
        } }
    };
}