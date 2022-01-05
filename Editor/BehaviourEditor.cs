using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;

public class BehaviourEditor : EditorWindow
{
    [Newtonsoft.Json.JsonIgnore]
    public List<BehaviourNode> nodes;
    [Newtonsoft.Json.JsonIgnore]
    public List<Connection> connections;
    private GUIStyle nodeStyle;
    private GUIStyle selectedNodeStyle;
    private GUIStyle inPointStyle;
    private GUIStyle outPointStyle;

    [Newtonsoft.Json.JsonIgnore]
    public ConnectionPoint selectedInPoint;
    [Newtonsoft.Json.JsonIgnore]
    public ConnectionPoint selectedOutPoint;

    private Vector2 drag;
    private Vector2 offset;

    private GUIStyle estiloMenu;
    private List<ulong> conIds;

    //Valores Inspector
    private string ubicacionBase = "./Assets/BT_Editor/Functional/jsons//";
    private string nombreArchivo = "";
    private NodeElement Nodo;
    private string funcion = "";
    private float _space = 20f;

    [MenuItem("Arboles Comportamientos/Abrir")]
    public static void OpenEditor()
    {
        GetWindow<BehaviourEditor>("Editor de árboles de comportamientos");

    }

    private void OnEnable()
    {
        nodeStyle = new GUIStyle();
        nodeStyle.normal.background = Resources.Load("Caja") as Texture2D;
        nodeStyle.border = new RectOffset(12, 12, 12, 12);
        nodeStyle.normal.textColor = Color.black;
        nodeStyle.alignment = TextAnchor.MiddleCenter;
        nodeStyle.fontSize = 50;

        selectedNodeStyle = new GUIStyle();
        selectedNodeStyle.normal.background = Resources.Load("CajaSeleccionada") as Texture2D; ;
        selectedNodeStyle.border = new RectOffset(12, 12, 12, 12);
        selectedNodeStyle.normal.textColor = Color.black;
        selectedNodeStyle.alignment = TextAnchor.MiddleCenter;
        selectedNodeStyle.fontSize = 50;

        inPointStyle = new GUIStyle();
        inPointStyle.normal.background = Resources.Load("Extremos") as Texture2D;
        inPointStyle.border = new RectOffset(12, 12, 12, 12);

        outPointStyle = new GUIStyle();
        outPointStyle.normal.background = Resources.Load("Extremos") as Texture2D;
        outPointStyle.border = new RectOffset(12, 12, 12, 12);

        estiloMenu = new GUIStyle();
        estiloMenu.wordWrap = true;
        estiloMenu.normal.textColor = Color.white;
        estiloMenu.normal.background = Texture2D.grayTexture;
        conIds = new List<ulong>();
    }

    private void OnGUI()
    {
        DrawGrid(20, 0.2f, Color.gray);
        DrawGrid(100, 0.2f, Color.gray);

        //Crear dos columnas Izq con el menú y Der con las opciones
        DrawNodes();
        DrawConnections();

        DrawConnectionLine(Event.current);

        ProcessNodeEvents(Event.current);

        ProcessEvents(Event.current);

        GUILayout.BeginArea(new Rect(Screen.width/4*3,10,Screen.width/4, Screen.height), estiloMenu);
        GUILayout.BeginVertical();
        GUILayout.Label("Nombre del archivo:", estiloMenu);
        nombreArchivo = GUILayout.TextField(nombreArchivo, estiloMenu);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Cargar")) this.Cargar();
        //try
        //{
            if (GUILayout.Button("Guardar") && nodes.Count > 0) this.Guardar();
        //}
        //catch (System.Exception)
        //{
        //    throw new System.Exception("No existen nodos en el grafo");
        //}
        
        GUILayout.EndHorizontal();
        GUILayout.Space(_space);
        
        if (Nodo != null) {
            GUILayout.Label("Datos del nodo:", estiloMenu);
            GUILayout.Label("Tipo de nodo:", estiloMenu);
            //Menu de elegir tipo
            if (GUILayout.Button(tomarTipo(Nodo))) this.ElegirTipo();
            GUILayout.Space(_space);
            if (Nodo.estilo > TipoNodo.Decorator)
            {
                GUILayout.Label("Funcion a realizar:", estiloMenu);
                funcion = GUILayout.TextField(funcion, estiloMenu);
                if (GUILayout.Button("Guardar función")) this.SaveFunc();
            }
            GUILayout.Space(_space);

            //Si tiene o no tiene Nodo
            GUILayout.Label("Padres", estiloMenu);
            if (Nodo.padres != null)
            {
                string padres = "";
                foreach (NodeElement objeto in Nodo.padres)
                {
                    padres += objeto + "\n";
                }

                GUILayout.Label(padres, estiloMenu);
            }
            else
                GUILayout.Space(_space);

            //Si tiene o no tiene hijos
            GUILayout.Label("Hijos", estiloMenu);
            if (Nodo.hijos != null)
            {
                string hijos = "";
                foreach (NodeElement hijo in Nodo.hijos)
                {
                    hijos += hijo + "\n";
                }
                
                GUILayout.Label(hijos, estiloMenu);
            }
            else
                GUILayout.Space(_space);
            GUILayout.Space(_space);
        }
        GUILayout.EndVertical();
        GUILayout.EndArea();
        if (GUI.changed) Repaint();
    }

    private void SaveFunc()
    {
        Nodo.funcion = funcion;
    }
    private void ElegirTipo()
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Sequence (→)"), false, () => CambiarTipo(TipoNodo.Sequence));
        genericMenu.AddItem(new GUIContent("Fallback (?)"), false, () => CambiarTipo(TipoNodo.Fallback));
        genericMenu.AddItem(new GUIContent("Parallel (⇉)"), false, () => CambiarTipo(TipoNodo.Parallel));
        genericMenu.AddItem(new GUIContent("Decorator (ƍ)"), false, () => CambiarTipo(TipoNodo.Decorator));
        genericMenu.AddItem(new GUIContent("Action (Function)"), false, () => CambiarTipo(TipoNodo.Action));
        genericMenu.AddItem(new GUIContent("Condition (Bool)"), false, () => CambiarTipo(TipoNodo.Condition));
        genericMenu.ShowAsContext();
    }

    private string tomarTipo(NodeElement nodo)
    {
        switch (nodo.estilo)
        {
            case TipoNodo.Sequence:
                return "Sequence (→)";
            case TipoNodo.Fallback:
                return "Fallback (?)";
            case TipoNodo.Parallel:
                return "Parallel (⇉)";
            case TipoNodo.Decorator:
                return "Decorator (ƍ)";
            case TipoNodo.Action:
                return "Action (Function)";
            case TipoNodo.Condition:
                return "Condition (Bool)";
            default:
                return "Nada";
        }
    }

    private void CambiarTipo(TipoNodo nuevoTipo)
    {
        Nodo.estilo = nuevoTipo;
    }

    private void Guardar()
    {
        string json = "";
        foreach (BehaviourNode nodo in nodes)
        {
            Debug.Log(nodo.nodo.padres);
                json = JsonConvert.SerializeObject(nodes);
        }
        Debug.Log(json);
        if(nombreArchivo != null && nombreArchivo != "")
            System.IO.File.WriteAllText(ubicacionBase + nombreArchivo + ".json", json);
        else    throw new System.Exception("El archivo no tiene nombre");
    }

    private void Cargar()
    {
        connections = new List<Connection>();
        if (File.Exists(ubicacionBase + nombreArchivo + ".json"))
        {
            string json = System.IO.File.ReadAllText(ubicacionBase + nombreArchivo + ".json");
            nodes = JsonConvert.DeserializeObject<List<BehaviourNode>>(json);
            foreach (BehaviourNode nodo in nodes)
            {
                nodo.updateConnectors(nodeStyle, selectedNodeStyle, inPointStyle, outPointStyle);
            }
            updateConnections();
            foreach (Connection conexion in connections)
            {
                Debug.Log(conexion.Id);
            }
            //foreach (BehaviourNode nodo in nodes)
            //{
            //    nodo.UpdateNode();
            //}

            Repaint();
        }
        else throw new System.Exception("El archivo " + nombreArchivo + ".json no existe");
    }

    private void updateConnections()
    {
        foreach (BehaviourNode nodo in nodes)
        {
            if (nodo.outPoint.conexiones.Count > 0)
            {
                List<Connection> outs = nodo.outPoint.conexiones;
                
                for ( int j = 0; j < outs.Count; j++)
                {
                    foreach (BehaviourNode entradus in nodes)
                    {
                        if (entradus.inPoint.conexiones.Count > 0)
                        {
                            List<Connection> ins = entradus.inPoint.conexiones;
                            //entradus.inPoint.conexiones = new List<Connection>();
                            for (int i = 0; i < ins.Count; i++)
                            {
                                if ( ins[i].Id == outs[j].Id)
                                {
                                    outs[j].UpdateCon(entradus.inPoint, nodo.outPoint);
                                    //entradus.inPoint.conexiones.Add(conexion);
                                    entradus.inPoint.conexiones[i] = outs[j];
                                    connections.Add(outs[j]);
                                    conIds.Add(outs[j].Id);
                                    entradus.UpdateInspector();
                                    nodo.UpdateInspector();
                                }
                            }

                        }
                    }
                }
            }
        }
        
    }

    public void Actualizar(NodeElement node)
    {
        Nodo = node;
        if (Nodo.funcion != null && Nodo.funcion != "")
            funcion = Nodo.funcion;
        else
            funcion = "";
        GUI.changed = true;
    }

    /// <summary>
    /// Función que dibuja una rejilla
    /// </summary>
    /// <param name="gridSpacing">Distancia entre líneas</param>
    /// <param name="gridOpacity">Opacidad de las líneas</param>
    /// <param name="gridColor">Color de las líneas</param>
    private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
    {
        int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
        int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

        Handles.BeginGUI();
        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

        offset += drag * 0.5f;
        Vector3 newOffset = new Vector3(offset.x % gridSpacing, offset.y % gridSpacing, 0);

        for (int i = 0; i < widthDivs; i++)
        {
            Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, position.height, 0f) + newOffset);
        }

        for (int j = 0; j < heightDivs; j++)
        {
            Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset, new Vector3(position.width, gridSpacing * j, 0f) + newOffset);
        }

        Handles.color = Color.white;
        Handles.EndGUI();
    }

    /// <summary>
    /// Función que dibuja los nodos
    /// </summary>
    private void DrawNodes()
    {
        if (nodes != null)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].Draw();
            }
        }
    }

    /// <summary>
    /// Función que dibuja las conexiones
    /// </summary>
    private void DrawConnections()
    {
        if(connections != null)
        {
            for(int i = 0; i < connections.Count; i++)
            {
                connections[i].Draw();
            }
        }
    }

    /// <summary>
    /// Función que procesa los eventos del teclado / ratón
    /// </summary>
    /// <param name="e">Evento</param>
    private void ProcessEvents(Event e)
    {
        drag = Vector2.zero;

        switch (e.type)
        {
            case EventType.MouseDown:
                if(e.button == 1)
                {
                    ProcessContextMenu(e.mousePosition);
                }
            break;
            case EventType.MouseDrag:
                if(e.button == 0)
                {
                    OnDrag(e.delta);
                }
            break;
        }
    }

    /// <summary>
    /// Función que procesa los eventos del teclado / ratón para todos los nodos
    /// </summary>
    /// <param name="e">Evento</param>
    private void ProcessNodeEvents(Event e)
    {
        if(nodes != null)
        {
            for(int i = nodes.Count -1; i >= 0; i--)
            {
                bool guiChanged = nodes[i].ProcessEvents(e);

                if (guiChanged)
                {
                    GUI.changed = true;
                }
            }
        }
    }

    /// <summary>
    /// Funcion que crea el menú contextual (Click derecho)
    /// </summary>
    /// <param name="mousePosition">La posición del ratón</param>
    private void ProcessContextMenu(Vector2 mousePosition)
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Add node"), false, () => OnClickAddNode(mousePosition));
        genericMenu.ShowAsContext();
    }

    /// <summary>
    /// Función que añade un nodo al menú
    /// </summary>
    /// <param name="mousePosition">La posición del ratón</param>
    private void OnClickAddNode(Vector2 mousePosition)
    {
        if(nodes == null)
        {
            nodes = new List<BehaviourNode>();
        }

        nodes.Add(new BehaviourNode( mousePosition, 100, 100, nodeStyle, selectedNodeStyle, inPointStyle, outPointStyle, TipoNodo.Sequence));
    }

    /// <summary>
    /// Función que crea una conexión
    /// </summary>
    public void CreateConnection()
    {
        if (connections == null)
        {
            connections = new List<Connection>();
        }
        bool elegido = false;
        ulong newId = 0;
        while (!elegido)
        {
            if (!conIds.Contains(newId))
            {
                elegido = true;
            }
            else
                newId++;
        }
        
        connections.Add(new Connection(newId, selectedInPoint, selectedOutPoint));
        conIds.Add(newId);
        Debug.Log("Id = " + newId);

        selectedInPoint.conexiones.Add(connections[connections.Count-1]);
        selectedInPoint.node.UpdateNode();
        selectedOutPoint.conexiones.Add(connections[connections.Count - 1]);
        selectedOutPoint.node.UpdateNode();
    }

    public void RemoveId(ulong Id)
    {
        conIds.Remove(Id);
    }

    /// <summary>
    /// Función para deseleccionar una conexión
    /// </summary>
    public void ClearConnectionSelection()
    {
        selectedInPoint = null;
        selectedOutPoint = null;
    }

    private void OnDrag(Vector2 delta)
    {
        drag = delta;

        if (nodes != null)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].Drag(delta);
            }
        }

        GUI.changed = true;
    }

    private void DrawConnectionLine(Event e)
    {
        if (selectedInPoint != null && selectedOutPoint == null)
        {
            Handles.DrawBezier(
                selectedInPoint.rect.center,
                e.mousePosition,
                selectedInPoint.rect.center + Vector2.left * 50f,
                e.mousePosition - Vector2.left * 50f,
                Color.white,
                null,
                2f
            );

            GUI.changed = true;
        }

        if (selectedOutPoint != null && selectedInPoint == null)
        {
            Handles.DrawBezier(
                selectedOutPoint.rect.center,
                e.mousePosition,
                selectedOutPoint.rect.center - Vector2.left * 50f,
                e.mousePosition + Vector2.left * 50f,
                Color.white,
                null,
                2f
            );

            GUI.changed = true;
        }
    }
}
