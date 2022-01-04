using System;
using UnityEditor;
using UnityEngine;

public class Connection
{
    [Newtonsoft.Json.JsonIgnore]
    public ConnectionPoint inPoint;
    [Newtonsoft.Json.JsonIgnore]
    public ConnectionPoint outPoint;
    private BehaviourEditor editor;
    public ulong Id;

    /// <summary>
    /// Inicializador de la conexión
    /// </summary>
    /// <param name="inPoint">Nodo del que sale</param>
    /// <param name="outPoint">Nodo al que entra</param>
    /// <param name="OnClickRemoveConnection"></param>
    public Connection(ulong identifier, ConnectionPoint inPoint, ConnectionPoint outPoint)
    {
        this.inPoint = inPoint;
        this.outPoint = outPoint;
        Id = identifier;
        editor = EditorWindow.GetWindow<BehaviourEditor>();
    }

    public void UpdateCon(ConnectionPoint In, ConnectionPoint Out)
    {
        inPoint = In;
        outPoint = Out;
        editor = EditorWindow.GetWindow<BehaviourEditor>();
    }

    /// <summary>
    /// Función que dibuja la conexión
    /// </summary>
    public void Draw()
    {
        Handles.DrawBezier(
            inPoint.rect.center,
            outPoint.rect.center,
            inPoint.rect.center + Vector2.down * 50f,
            outPoint.rect.center - Vector2.down * 50f,
            Color.white,
            null,
            2f
        );

        if (Handles.Button((inPoint.rect.center + outPoint.rect.center) * 0.5f, Quaternion.identity, 4, 8, Handles.RectangleHandleCap))
        {
            OnClickRemoveConnection(this);
        }
    }

    /// <summary>
    /// Función para eliminar conexión
    /// </summary>
    /// <param name="connection">La conexión a eliminar</param>
    private void OnClickRemoveConnection(Connection connection)
    {
        //No se eliminan bien si se ha cargado
        connection.inPoint.conexiones.Remove(connection);
        connection.inPoint.node.UpdateNode();
        connection.outPoint.conexiones.Remove(connection);
        connection.outPoint.node.UpdateNode();
        editor.RemoveId(Id);
        editor.connections.Remove(connection);
    }

    public void removeConnection()
    {
        OnClickRemoveConnection(this);
    }
}
