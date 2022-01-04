using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Tipos de puntos de conexi�n de los nodos
/// </summary>
//public enum ConnectionPointType
//{
//    In,
//    Out
//}

public class ConnectionPoint
{
    public Rect rect;
    public ConnectionPointType type;
    [Newtonsoft.Json.JsonIgnore]
    public BehaviourNode node;
    private GUIStyle style;
    private Action<ConnectionPoint> OnClickConnectionPoint;
    public List<Connection> conexiones;
    private BehaviourEditor editor;

    /// <summary>
    /// Inicializador del punto de conexi�n
    /// </summary>
    /// <param name="node">Nodo al que va unido</param>
    /// <param name="type">Tipo de uni�n</param>
    /// <param name="style">Estilo de la uni�n</param>
    /// <param name="OnClickConnectionPoint"></param>
    public ConnectionPoint(BehaviourNode node, ConnectionPointType type, GUIStyle style)
    {
        this.node = node;
        this.type = type;
        this.style = style;
        if(type == ConnectionPointType.In)
            this.OnClickConnectionPoint = OnClickInPoint;
        else
            this.OnClickConnectionPoint = OnClickOutPoint;
        rect = new Rect(0, 0, 20f, 20f);
        editor = EditorWindow.GetWindow<BehaviourEditor>();
        conexiones = new List<Connection>();
    }

    public void updateNode(BehaviourNode node, GUIStyle style)
    {
        this.node = node;
        this.style = style;
        if (type == ConnectionPointType.In)
            this.OnClickConnectionPoint = OnClickInPoint;
        else
            this.OnClickConnectionPoint = OnClickOutPoint;
        rect = new Rect(0, 0, 10f, 20f);
        editor = EditorWindow.GetWindow<BehaviourEditor>();
            
    }

    public void updateNode(BehaviourNode node)
    {
        this.node = node;
        if (type == ConnectionPointType.In)
            this.OnClickConnectionPoint = OnClickInPoint;
        else
            this.OnClickConnectionPoint = OnClickOutPoint;
        rect = new Rect(0, 0, 10f, 20f);
    }

    /// <summary>
    /// Funci�n para dibujar el punto de conexi�n
    /// </summary>
    public void Draw()
    {
        rect.x = node.rect.x + (node.rect.width * 0.5f) - rect.width * 0.5f;

        switch (type)
        {
            case ConnectionPointType.In:
                rect.y = node.rect.y - rect.height + 10f;
                break;

            case ConnectionPointType.Out:
                rect.y = node.rect.y + node.rect.height - 10f;
                break;
        }

        if (GUI.Button(rect, "", style))
        {
            if (OnClickConnectionPoint != null)
            {
                OnClickConnectionPoint(this);
            }
        }
    }

    /// <summary>
    /// Funci�n que se llama en una conexi�n de entrada
    /// </summary>
    /// <param name="inPoint">La conexi�n de entrada</param>
    private void OnClickInPoint(ConnectionPoint inPoint)
    {
        editor.selectedInPoint = inPoint;

        if (editor.selectedOutPoint != null)
        {
            if (editor.selectedOutPoint.node != editor.selectedInPoint.node)
            {
                editor.CreateConnection();
                editor.ClearConnectionSelection();
            }
            else
            {
                editor.ClearConnectionSelection();
            }
        }
    }

    /// <summary>
    /// Funci�n que se llama en una conexi�n de salida
    /// </summary>
    /// <param name="outPoint">Conexi�n de salida</param>
    private void OnClickOutPoint(ConnectionPoint outPoint)
    {
        editor.selectedOutPoint = outPoint;

        if (editor.selectedInPoint != null)
        {
            if (editor.selectedOutPoint.node != editor.selectedInPoint.node)
            {
                editor.CreateConnection();
                editor.ClearConnectionSelection();
            }
            else
            {
                editor.ClearConnectionSelection();
            }
        }
    }
}
