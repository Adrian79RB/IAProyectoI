using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class priorityQueue:
{
    //esta clase almacena un nodo del grafo en una lista doblemente enlazada con prioridad.
    public class NodoPQ{

        public Nodo nodo;
        public NodoPQ anterior, siguiente;
        public float prioridad;
    }
    private NodoPQ raiz;
    public priorityQueue{
        raiz = null;
    }

    void Insertar(Nodo nodo, float prioridad){
        NodoPQ nuevo = new NodoPQ();
        nuevo.nodo = nodo;
        nuevo.prioridad = prioridad;
        if(raiz == null){
            raiz = nuevo;
        }
        else{
            if(raiz.prioridad > nuevo.prioridad){
                NodoPQ auxiliar = raiz;
                raiz = nuevo;
                raiz.siguiente = auxiliar;
            }
            else{
                for(NodoPQ nodo = raiz; nodo.siguiente != null; nodo = nodo.siguiente){
                    if(nuevo.prioridad < nodo.prioridad && nodo.anterior.prioridad < nuevo.prioridad){
                        nodo.anterior.siguiente = nuevo;
                        nuevo.anterior = nodo.anterior;
                        nodo.anterior = nuevo;
                        nuevo.siguiente = nodo;

                    }
                }
            }
        }
    }
    Nodo Devolver(){
        if(raiz != null){
            Nodo primero = raiz;
            raiz = raiz.siguiente;
            return primero;
        }
        return null;
        
    }

}
