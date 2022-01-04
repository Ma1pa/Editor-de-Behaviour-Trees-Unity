using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class CerebroSimple : MonoBehaviour
{
    public TextAsset arbol;
    public GameObject selfGameObject;
    private List<SimpleNode> nodes;
    private List<SimpleConnection> connections;
    private SimpleNodeElement[] nodos;
    private SimpleNodeElement inicio;
    private Conditions condiciones;
    private Actions acciones;
    
    private void Cargar()
    {
        connections = new List<SimpleConnection>();
        nodes = JsonConvert.DeserializeObject<List<SimpleNode>>(arbol.text);
        foreach (SimpleNode nodo in nodes)
        {
            nodo.updateConnectors();
        }
        updateConnections();
        nodos = new SimpleNodeElement[nodes.Count];
        updateNodes();
    }

    private void updateConnections()
    {
        foreach (SimpleNode nodo in nodes)
        {
            if (nodo.outPoint.conexiones.Count > 0)
            {
                List<SimpleConnection> outs = nodo.outPoint.conexiones;

                for (int j = 0; j < outs.Count; j++)
                {
                    foreach (SimpleNode entradus in nodes)
                    {
                        if (entradus.inPoint.conexiones.Count > 0)
                        {
                            List<SimpleConnection> ins = entradus.inPoint.conexiones;
                            //entradus.inPoint.conexiones = new List<Connection>();
                            for (int i = 0; i < ins.Count; i++)
                            {
                                if (ins[i].Id == outs[j].Id)
                                {
                                    outs[j].UpdateCon(entradus.inPoint, nodo.outPoint);
                                    //entradus.inPoint.conexiones.Add(conexion);
                                    entradus.inPoint.conexiones[i] = outs[j];
                                    connections.Add(outs[j]);
                                }
                            }

                        }
                    }
                }
            }
        }
    }

    private void updateNodes()
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            nodes[i].UpdateNode();
            nodos[i] = nodes[i].nodo;
        }
    }

    private void Awake()
    {
        inicio = null;
        Cargar();
        foreach(SimpleNodeElement nodo in nodos)
        {
            if(nodo.padres.Count == 0)
            {
                inicio = nodo;
                break;
            }
        }
        if(inicio == null)
        {
            inicio = nodos[0];
        }
        //Construir acciones / condiciones
        acciones = new Actions(selfGameObject);
        condiciones = new Conditions(selfGameObject);
    }

    public void Perform()
    {
        bool resultado = EmpezarNavegacion();
        Debug.Log("El resultado total ha sido: " + resultado);
    }

    public bool EmpezarNavegacion()
    {
        Debug.Log("El grafo a iniciar es: " + inicio);
        return NavegarGrafo(inicio);
    }

    private bool NavegarGrafo(SimpleNodeElement nodo)
    {
        //Hacer funci�n recursiva de navegaci�n del grafo
        switch (nodo.estilo)
        {
            case TipoNodo.Sequence:
                return Sequence(nodo);
            case TipoNodo.Fallback:
                return Fallback(nodo);
            case TipoNodo.Parallel:
                return Parallel(nodo);
            case TipoNodo.Decorator:
                return Decorator(nodo);
            case TipoNodo.Action:
                return Action(nodo);
            case TipoNodo.Condition:
                return Condition(nodo);
        }
        return true;
    }

    /// <summary>
    /// Funci�n que ejecuta un nodo secuencia (Comprueba hijos en orden)
    /// </summary>
    /// <param name="nodo">Nodo secuencia</param>
    /// <returns>Resultado del and de los hijos</returns>
    private bool Sequence(SimpleNodeElement nodo)
    {
        bool resultado = true;
        int actual = 0;
        if(nodo.hijos.Count < 1)
            throw new System.Exception("El nodo Secuencia no tiene hijos.");
        else
        {
            while(resultado && actual < nodo.hijos.Count)
            {
                resultado = NavegarGrafo(nodo.hijos[actual]);
                actual++;
            }
            Debug.Log("Hola soy un nodo secuencia con resultado " + resultado);
            return resultado;
        }
    }

    /// <summary>
    /// Funci�n que ejecuta un nodo Fallback (Ejecuta cada hijo hasta que uno devuelva true)
    /// </summary>
    /// <param name="nodo">Nodo Fallback</param>
    /// <returns>Devoluci�n del nodo</returns>
    private bool Fallback(SimpleNodeElement nodo)
    {
        bool devolver = false;
        int actual = 0;
        if (nodo.hijos.Count < 1)
            throw new System.Exception("El nodo Fallback no tiene hijos.");
        else
        {
            while (!devolver && actual < nodo.hijos.Count)
            {
                devolver = NavegarGrafo(nodo.hijos[actual]);
                actual++;
            }
            Debug.Log("Hola soy un nodo fallback con resultado " + devolver);
            return devolver;
        }
    }

    /// <summary>
    /// Funci�n que ejecuta un nodo Paralelo (Ejecuta todos los hijos y devuelve true si alg�n hijo da true)
    /// </summary>
    /// <param name="nodo">Nodo Paralelo</param>
    /// <returns>Or de todos los hijos</returns>
    private bool Parallel(SimpleNodeElement nodo)
    {
        bool[] resultados = new bool[nodo.hijos.Count];
        if (nodo.hijos.Count < 1)
            throw new System.Exception("El nodo Paralelo no tiene hijos.");
        else
        {
            for (int i = 0; i < nodo.hijos.Count; i++)
            {
                resultados[i] = NavegarGrafo(nodo.hijos[i]);
            }
            foreach(bool resultado in resultados)
            {
                if (resultado)
                {
                    Debug.Log("Hola soy un nodo paralelo con resultado " + resultado);
                    return resultado;
                }
            }
            Debug.Log("Hola soy un nodo paralelo con resultado " + false);
            return false;
        }
    }

    /// <summary>
    /// Funci�n que ejecuta un nodo decorador (Negaci�n)
    /// </summary>
    /// <param name="nodo">Nodo decorador</param>
    /// <returns>Inversi�n del resultado del hijo</returns>
    private bool Decorator(SimpleNodeElement nodo)
    {
        if (nodo.hijos.Count != 1)
            throw new System.Exception("el nodo decorador no tiene un �nico hijo.");
        else
        {
            bool respuesta = NavegarGrafo(nodo.hijos[0]);
            Debug.Log("Hola soy un nodo decorador con resultado " + !respuesta);
            return !respuesta;
        }
    }

    private bool Action(SimpleNodeElement nodo)
    {
        bool objeto = acciones.GetMethod(nodo.funcion);

        return objeto;
    }

    private bool Condition(SimpleNodeElement nodo)
    {
        bool objeto = condiciones.GetMethod(nodo.funcion);

        return objeto;
    }

}
