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
    private int cantidad;

    public priorityQueue(){
        raiz = null;
    }

    void Insertar(Nodo nodoNew, float prioridad){
        NodoPQ nuevo = new NodoPQ();
        nuevo.nodo = nodoNew;
        nuevo.prioridad = prioridad;
        if(raiz == null){
            raiz = nuevo;
            cantidad++;
        }
        else{
            if(raiz.prioridad > nuevo.prioridad){
                NodoPQ auxiliar = raiz;
                raiz = nuevo;
                raiz.siguiente = auxiliar;
                cantidad++;
            }
            else{
                for(NodoPQ nodo = raiz; nodo != null; nodo = nodo.siguiente){
                    if(nuevo.prioridad < nodo.prioridad && nodo.anterior.prioridad < nuevo.prioridad){
                        nodo.anterior.siguiente = nuevo;
                        nuevo.anterior = nodo.anterior;
                        nodo.anterior = nuevo;
                        nuevo.siguiente = nodo;
                        cantidad++;

                    }
                }
            }
        }
    }
    Nodo Devolver(){
        if(raiz != null){
            NodoPQ primero = raiz;
            raiz = raiz.siguiente;
            cantidad--;
            return primero.nodo;
        }
        return null;
    }

    void CambiarPrio(Nodo nodoComp, float nuevaPrio){
        NodoPQ nodoaux = new NodoPQ();
        for(NodoPQ nodo = raiz; nodo != null; nodo = nodo.siguiente){
            if(nodo.nodo == nodoComp){
                nodo.anterior.siguiente = nodo.siguiente;
                nodo.siguiente.anterior = nodo.anterior;
                Insertar(nodoComp, nuevaPrio);
                return;
            }
        }

    }
    bool Vacia(){
        if(cantidad == 0){
            return true;
        }
        return false;
    }

}
