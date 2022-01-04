using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public enum TipoNodo
//{
//    Sequence,   //->
//    Fallback,   //?
//    Parallel,   //⇉
//    Decorator,  //ƍ
//    Action,     //Funcion
//    Condition   //Booleano
//}

public class NodeElement
{
    public TipoNodo estilo;
    [Newtonsoft.Json.JsonIgnore]
    public List<NodeElement> padres;
    [Newtonsoft.Json.JsonIgnore]
    public List<NodeElement> hijos;
    public string funcion = "Patata";
    public NodeElement(TipoNodo tipo, List<NodeElement> anteriores, List<NodeElement> posteriores)
    {
        estilo = tipo;
        padres = anteriores;
        hijos = posteriores;
    }

    //Función que hace comprovaciones según el tipo de nodo

    public override string ToString()
    {
        switch (estilo)
        {
            case TipoNodo.Action:
                return "Tipo: " + estilo + " | Acción: " + funcion;
            case TipoNodo.Condition:
                return "Tipo: " + estilo + " | Acción: " + funcion;
            default:
                return "Tipo: " + estilo;
        }
        
    }
}
