using System;
using UnityEngine;

public class SimpleConnection
{
    [Newtonsoft.Json.JsonIgnore]
    public SimpleConnectionPoint inPoint;
    [Newtonsoft.Json.JsonIgnore]
    public SimpleConnectionPoint outPoint;
    public ulong Id;

    /// <summary>
    /// Inicializador de la conexión
    /// </summary>
    /// <param name="inPoint">Nodo del que sale</param>
    /// <param name="outPoint">Nodo al que entra</param>
    /// <param name="OnClickRemoveConnection"></param>
    public SimpleConnection(ulong identifier, SimpleConnectionPoint inPoint, SimpleConnectionPoint outPoint)
    {
        this.inPoint = inPoint;
        this.outPoint = outPoint;
        Id = identifier;
    }

    public void UpdateCon(SimpleConnectionPoint In, SimpleConnectionPoint Out)
    {
        inPoint = In;
        outPoint = Out;
    }
}
