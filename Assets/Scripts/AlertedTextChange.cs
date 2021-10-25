using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlertedTextChange : MonoBehaviour
{
    public Transform fantasmas;

    bool undercoverState = true;
    Color undercover = Color.green;
    Color alerted = Color.red;

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < fantasmas.childCount; i++)
        {
            MovimientoFantasmas fantasma = fantasmas.GetChild(i).GetComponent<MovimientoFantasmas>();
            if (undercoverState && fantasma.consultaEstadoFantasma() != EstadoNPC.Patrolling && fantasma.consultaEstadoFantasma() != EstadoNPC.GoingPatrol)
            {
                undercoverState = false;
                Text text = GetComponent<Text>();
                text.text = "Alerted";
                text.color = alerted;
            }
            else if(!undercoverState && (fantasma.consultaEstadoFantasma() == EstadoNPC.Patrolling || fantasma.consultaEstadoFantasma() == EstadoNPC.GoingPatrol))
            {
                undercoverState = true;
                Text text = GetComponent<Text>();
                text.text = "Undercover";
                text.color = undercover;
            }
        }
    }
}
