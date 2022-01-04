using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
//using Unit;

public class Conditions 
{
    public GameObject selfGameObject;

    //Constructor
    public Conditions(GameObject gameObject)
    {
        selfGameObject = gameObject;
    }

    //Este deberia ser la clase en la que se guardan las funciones que ejecutarian los nodos del tipo Condicion
    public bool GetMethod(string metodo)
    {
        MethodInfo mi = typeof(Conditions).GetMethod(metodo);
        //Invoke the method
        // (null- no parameter for the method call
        // or you can pass the array of parameters...)
        object objeto = mi.Invoke(this, null);
        return (bool)objeto;
    }

    public bool Test()
    {
        Debug.Log("Has llamado a la funcion Test del condicional");
        return true;
    }

    //Introducir funciones a comprovar.
    //Las funciones deberian devolver siempre un booleano, el cual dependeria de la condicion.
}
