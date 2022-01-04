using System;
using System.Collections.Generic;
using UnityEngine;

public class SimpleNode
{
    public Rect rect;
    public string title;
    [Newtonsoft.Json.JsonIgnore]
    public bool isDragged;
    [Newtonsoft.Json.JsonIgnore]
    public bool isSelected;
    public SimpleConnectionPoint inPoint;
    public SimpleConnectionPoint outPoint;
    public SimpleNodeElement nodo;


    public SimpleNode( Vector2 position, float width, float height, TipoNodo tipo)
    {
        rect = new Rect(position.x, position.y, width, height);
        inPoint = new SimpleConnectionPoint(this, ConnectionPointType.In);
        outPoint = new SimpleConnectionPoint(this, ConnectionPointType.Out);
        nodo = new SimpleNodeElement(tipo, null, null);
    }

    public void updateConnectors(GUIStyle nodeStyle, GUIStyle selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle)
    {
        inPoint.updateNode(this);
        outPoint.updateNode(this);
    }
    public void updateConnectors()
    {
        inPoint.updateNode(this);
        outPoint.updateNode(this);
    }

    public void UpdateNode()
    {
        if (inPoint.conexiones.Count > 0)
        {
            List<SimpleNodeElement> nodos = new List<SimpleNodeElement>();
            foreach (SimpleConnection conexion in inPoint.conexiones)
            {
                //nodos.Add(conexion.inPoint.node.nodo);
                nodos.Add(conexion.outPoint.node.nodo);
            }
            nodo.padres = nodos;
            //nodo.padres = inPoint.conexiones[0].outPoint.node.nodo;
        }
        else
            nodo.padres = new List<SimpleNodeElement>();
        if (outPoint.conexiones.Count > 0)
        {
            List<SimpleNodeElement> nodos = new List<SimpleNodeElement>();
            foreach (SimpleConnection conexion in outPoint.conexiones)
            {
                //nodos.Add(conexion.outPoint.node.nodo);
                nodos.Add(conexion.inPoint.node.nodo);
            }
            nodo.hijos = nodos;
        }
        else
            nodo.hijos = new List<SimpleNodeElement>();
    }
}
