using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BehaviourNode
{
    public Rect rect;
    public string title;
    [Newtonsoft.Json.JsonIgnore]
    public bool isDragged;
    [Newtonsoft.Json.JsonIgnore]
    public bool isSelected;
    public ConnectionPoint inPoint;
    public ConnectionPoint outPoint;
    public NodeElement nodo;
    private BehaviourEditor editor;

    private GUIStyle style;
    private GUIStyle defaultNodeStyle;
    private GUIStyle selectedNodeStyle;


    public BehaviourNode( Vector2 position, float width, float height, GUIStyle nodeStyle, GUIStyle selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle, TipoNodo tipo)
    {
        editor = EditorWindow.GetWindow<BehaviourEditor>();
        rect = new Rect(position.x, position.y, width, height);
        style = nodeStyle;
        inPoint = new ConnectionPoint(this, ConnectionPointType.In, inPointStyle);
        outPoint = new ConnectionPoint(this, ConnectionPointType.Out, outPointStyle);
        defaultNodeStyle = nodeStyle;
        selectedNodeStyle = selectedStyle;
        nodo = new NodeElement(tipo, null, null);
    }

    public void updateConnectors(GUIStyle nodeStyle, GUIStyle selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle)
    {
        editor = EditorWindow.GetWindow<BehaviourEditor>();
        style = nodeStyle;
        defaultNodeStyle = nodeStyle;
        selectedNodeStyle = selectedStyle;
        inPoint.updateNode(this, inPointStyle);
        outPoint.updateNode(this, outPointStyle);
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
            List<NodeElement> nodos = new List<NodeElement>();
            foreach (Connection conexion in inPoint.conexiones)
            {
                //nodos.Add(conexion.inPoint.node.nodo);
                nodos.Add(conexion.outPoint.node.nodo);
            }
            nodo.padres = nodos;
            //nodo.padres = inPoint.conexiones[0].outPoint.node.nodo;
        }
        else
            nodo.padres = new List<NodeElement>();
        if (outPoint.conexiones.Count > 0)
        {
            List<NodeElement> nodos = new List<NodeElement>();
            foreach (Connection conexion in outPoint.conexiones)
            {
                //nodos.Add(conexion.outPoint.node.nodo);
                nodos.Add(conexion.inPoint.node.nodo);
            }
            nodo.hijos = nodos;
        }
        else
            nodo.hijos = new List<NodeElement>();
    }

    /// <summary>
    /// Función que mueve el nodo
    /// </summary>
    /// <param name="delta">Cantidad que se mueve en el espacio</param>
    public void Drag(Vector2 delta)
    {
        rect.position += delta;
    }

    /// <summary>
    /// Función que dibuja el nodo
    /// </summary>
    public void Draw()
    {
        inPoint.Draw();
        outPoint.Draw();
        string texto = "";
        switch (nodo.estilo)
        {
            case TipoNodo.Sequence:     //->
                texto = "→";
                break;               
            case TipoNodo.Fallback:     //?
                texto = "?";
                break;               
            case TipoNodo.Parallel:     //⇉
                texto = "⇉";
                break;               
            case TipoNodo.Decorator:    //ƍ
                texto = "ƍ";
                break;               
            case TipoNodo.Action:       //Funcion
                texto = "A";
                break;
            case TipoNodo.Condition:    //Booleano
                texto = "C";
                break;
        }
        //GUI.Box(rect, title, style);
        GUI.Label(rect, texto, style);

    }

    public TipoNodo? actualizarTipo(TipoNodo nuevoTipo)
    {
        if (isSelected)
        {
            nodo.estilo = nuevoTipo;
            return nodo.estilo;
        }
        else
            return null;
    }

    public bool ProcessEvents(Event e)
    {
        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 0)
                {
                    if (rect.Contains(e.mousePosition))
                    {
                        isDragged = true;
                        GUI.changed = true;
                        isSelected = true;
                        style = selectedNodeStyle;
                    }
                    else if(e.mousePosition.x < Screen.width / 4 * 3)
                    {
                        GUI.changed = true;
                        isSelected = false;
                        style = defaultNodeStyle;
                    }
                }

                if (e.button == 1 && isSelected && rect.Contains(e.mousePosition))
                {
                    ProcessContextMenu();
                    e.Use();
                }
            break;
            case EventType.MouseUp:
                isDragged = false;
                if (rect.Contains(e.mousePosition))
                {
                    UpdateNode();
                    UpdateInspector();
                }
                break;
            case EventType.MouseDrag:
                if(e.button == 0 && isDragged)
                {
                    Drag(e.delta);
                    e.Use();
                    return true;
                }
            break;
        }

        return false;
    }

    /// <summary>
    /// Menú contextual del nodo
    /// </summary>
    private void ProcessContextMenu()
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Remove node"), false, OnClickRemoveNode);
        genericMenu.ShowAsContext();
    }

    /// <summary>
    /// Función que elimina un nodo
    /// </summary>
    /// <param name="node">Nodo a eliminar</param>
    private void OnClickRemoveNode()
    {
        if (editor.connections != null)
        {
            List<Connection> connectionsToRemove = new List<Connection>();

            for (int i = 0; i < editor.connections.Count; i++)
            {
                if (editor.connections[i].inPoint == this.inPoint || editor.connections[i].outPoint == this.outPoint)
                {
                    connectionsToRemove.Add(editor.connections[i]);
                }
            }

            for (int i = 0; i < connectionsToRemove.Count; i++)
            {
                editor.connections.Remove(connectionsToRemove[i]);
            }

            connectionsToRemove = null;
        }
        while (inPoint.conexiones.Count > 0)
        {
            inPoint.conexiones[0].removeConnection();
        }
        while (outPoint.conexiones.Count > 0)
        {
            outPoint.conexiones[0].removeConnection();
        }
        editor.nodes.Remove(this);
    }

    public void UpdateInspector()
    {
        editor.Actualizar(nodo);
    }
}
