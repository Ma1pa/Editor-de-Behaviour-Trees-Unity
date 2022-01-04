using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tipos de puntos de conexión de los nodos
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
    /// Inicializador del punto de conexión
    /// </summary>
    /// <param name="node">Nodo al que va unido</param>
    /// <param name="type">Tipo de unión</param>
    /// <param name="style">Estilo de la unión</param>
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
