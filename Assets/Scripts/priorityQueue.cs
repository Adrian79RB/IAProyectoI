using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class priorityQueue
{
    //esta clase almacena un nodo del grafo en una lista doblemente enlazada con prioridad.
    public class NodoPQ{

        public Nodo nodo;
        public NodoPQ anterior, siguiente;
        public float prioridad;
    }
    private NodoPQ raiz;
    private int length;

    public priorityQueue(){
        raiz = null;
    }

    public void Insertar(Nodo nodoNew, float prioridad){
        length++;
        NodoPQ nuevo = new NodoPQ();
        nuevo.nodo = nodoNew;
        nuevo.prioridad = prioridad;
        if(raiz == null){ //Si la cola est� vacia
            raiz = nuevo;
            raiz.anterior = null;
            raiz.siguiente = null;
        }
        else{
            if(raiz.prioridad > nuevo.prioridad){ //Si el nuevo nodo sustituye al primero
                NodoPQ auxiliar = raiz;
                nuevo.siguiente = auxiliar;
                raiz = nuevo;
                auxiliar.anterior = raiz;
            }
            else{
                for(NodoPQ nodo = raiz; nodo != null; nodo = nodo.siguiente){ // Si el nuevo nodo se situa entre dos
                    if (nodo.anterior != null && nuevo.prioridad < nodo.prioridad && nuevo.prioridad > nodo.anterior.prioridad){
                        nodo.anterior.siguiente = nuevo;
                        nuevo.anterior = nodo.anterior;
                        nodo.anterior = nuevo;
                        nuevo.siguiente = nodo;
                        return;
                    }
                    else if(nodo.siguiente == null && nodo.prioridad < nuevo.prioridad) //Si llegamos al �ltimo nodo y siguen siendo m�s peque�os
                    {
                        nodo.siguiente = nuevo;
                        nuevo.anterior = nodo;
                        nuevo.siguiente = null;
                    }
                }
            }
        }
    }

    //Devolver el nodo raiz con menor prioridad de la lista
    public Nodo Devolver(){
        if(raiz != null){
            NodoPQ primero = raiz;
            raiz = raiz.siguiente;

            if (raiz != null)
                raiz.anterior = null;

            primero.siguiente = null;
            length--;
            return primero.nodo;
        }
        return null;
    }
    //Consultar si la cola está vacia
    public bool Vacia(){
        if(length == 0){
            return true;
        }
        return false;
    }

    //Cambiar la prioridad de un nodo de la cola
    public void CambiarPrio(Nodo nodoComp, float nuevaPrio){

        for(NodoPQ nodo = raiz; nodo != null; nodo = nodo.siguiente){
            if(nodo.nodo == nodoComp){
                if(nodo.anterior != null)
                    nodo.anterior.siguiente = nodo.siguiente;
                if(nodo.siguiente != null)
                    nodo.siguiente.anterior = nodo.anterior;
                nodo.anterior = null;
                nodo.siguiente = null;
                Insertar(nodoComp, nuevaPrio);
                return;
            }
        }

    }

    //Comprobamos si un nodo esta en la cola de prioridad
    public bool EncontrarNodo(Nodo nodoBuscado)
    {
        for(NodoPQ nodo = raiz; nodo != null; nodo = nodo.siguiente)
        {
            if (nodo.nodo == nodoBuscado)
                return true;
        }
        return false;
    }

    //Devolvemos un nodo que hemos buscado
    public Nodo ConsultarNodo(Nodo nodoBuscado)
    {
        for (NodoPQ nodo = raiz; nodo != null; nodo = nodo.siguiente)
        {
            if (nodo.nodo == nodoBuscado)
                return nodo.nodo;
        }

        return null;
    }

    public void EliminarNodo(Nodo nodoEliminado)
    {
        if (length > 0)
        {
            for (NodoPQ nodo = raiz; nodo != null; nodo = nodo.siguiente)
            {
                if (nodo.nodo == nodoEliminado)
                {
                    if (nodo == raiz)
                    {
                        raiz = nodo.siguiente;
                        nodo.siguiente.anterior = null;
                        nodo.siguiente = null;
                    }
                    else if (nodo.siguiente == null)
                    {
                        nodo.anterior.siguiente = null;
                        nodo.anterior = null;
                    }
                    else
                    {
                        nodo.anterior.siguiente = nodo.siguiente;
                        nodo.siguiente.anterior = nodo.anterior;
                        nodo.siguiente = null;
                        nodo.anterior = null;
                    }
                    length--;
                }
            }
        }
    }

    public int getLegth()
    {
        return length;
    }

    public void MostrarContenido()
    {
        int i = 1;
        for (NodoPQ nodo = raiz; nodo != null; nodo = nodo.siguiente)
        {
            Debug.Log("Nodo " + i + ": " + nodo.nodo.name);
            i++;
        }
    }

}
