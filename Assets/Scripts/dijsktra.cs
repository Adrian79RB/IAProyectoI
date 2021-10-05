using System.Collections;
using System.Collections.Generic;
using UnityEngine; 


public class dijsktra
{

    /*Hola. Como hoy sé que no puedo ir os dejo aquí explicado el funcionamiento de este algoritmo
    Básicamente calcula todos los caminos posibles desde un nodo hasta el resto de nodos y devuelve aquellos que son óptimos
    en un array de Int que contiene la posición del nodo anterior por el que se tiene que pasar para llegar al nodo que 
    ocupa la posición en el grafo que corresponde a la de la array. Para mover al personaje habría que hacer una función que 
    cogiera este array, sacara el camino óptimo para el nodo de destino desde el origen y mandara a recorrer al monigote todos
    los nodos. */
    public int[] caminoOptimo(Nodo[] grafo, int inicio){     
        int cantidadDeVertices = grafo.Length;
        int actual = inicio;
        float infinito = float.MaxValue;
        priorityQueue cola = new priorityQueue();
        float[] distancias = new float[cantidadDeVertices];
        int[] padre = new int[cantidadDeVertices];
        //como no he encontrado una forma de inicializar un array automáticamente he hecho este For para inicializar los valores de distancias a infinito y padre a -1
        for(int i = 0; i< cantidadDeVertices; i++){
            if(i== inicio){
                distancias[i]=0;
                padre[i]=-2;//usaremos -1 para los vertices inalcanzables, por lo que he decidido usar el -2 para marcar quien es el nodo inicial
            }
            else{
                distancias[i] = infinito;
                padre[i]=-1;
            }
        }
        //se mete todo en la cola y a empezar el mix
        for(int j=0; j<cantidadDeVertices; j++){
            cola.Insertar(grafo[j],distancias[j]);
        }
        //necesito que se cambie el nodo para que tenga un número asignado que indique su posición en el grafo @Adrián
        while(!cola.Vacia()){
            Nodo chosenNodo = cola.Devolver();
            //actual = chosenNodo.posicion;
            foreach(GameObject nodoActual in chosenNodo.arcs){
                int vecino = 0;// nodoActual.posicion;
                float nuevaDist = distancias[actual] + chosenNodo.weigths[vecino];
                if(nuevaDist < distancias[vecino]){
                    distancias[vecino] = nuevaDist;
                    padre[vecino] = actual;
                    //cola.CambiarPrio(nodoActual,nuevaDist);
                }
            }
        }
        
        return padre;
    }
    
}
