using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Actions
{
    public GameObject selfGameObject;

    //Constructor
    public Actions(GameObject gameObject)
    {
        this.selfGameObject = gameObject;
    }

    //Este deberia ser la clase en la que se guardan las funciones que ejecutarian los nodos del tipo Action
    public bool GetMethod(string metodo)
    {
        MethodInfo mi = typeof(Actions).GetMethod(metodo);
        //Invoke the method
        // (null- no parameter for the method call
        // or you can pass the array of parameters...)
        object objeto = mi.Invoke(this, null);
        return (bool)objeto;
    }

    public bool Test()
    {
        Debug.Log("Has llamado a la funcion Test del acciones");
        return true;
    }
    
    ////Introducir acciones a realizar
    //Las funciones deberian devolver siempre un booleano, el cual deberia ser verdadero si se ha podido ejecutar

}
