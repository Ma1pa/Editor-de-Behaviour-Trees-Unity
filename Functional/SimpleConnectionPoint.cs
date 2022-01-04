using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tipos de puntos de conexi�n de los nodos
/// </summary>
public enum ConnectionPointType
{
    In,
    Out
}

public class SimpleConnectionPoint
{
    public Rect rect;
    public ConnectionPointType type;
    [Newtonsoft.Json.JsonIgnore]
    public SimpleNode node;
    public List<SimpleConnection> conexiones;

    /// <summary>
    /// Inicializador del punto de conexi�n
    /// </summary>
    /// <param name="node">Nodo al que va unido</param>
    /// <param name="type">Tipo de uni�n</param>
    /// <param name="style">Estilo de la uni�n</param>
    /// <param name="OnClickConnectionPoint"></param>
    public SimpleConnectionPoint(SimpleNode node, ConnectionPointType type)
    {
        this.node = node;
        this.type = type;
        rect = new Rect(0, 0, 20f, 20f);
        conexiones = new List<SimpleConnection>();
    }

    public void updateNode(SimpleNode node)
    {
        this.node = node;            
    }
}
