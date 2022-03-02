using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerBehavior : NetworkBehaviour
{

    #region R�f�rences

    #endregion

    #region param�tres

    //Variables venant du lobby
    [SyncVar(hook = nameof(ChangePseudo))] public string pseudo;
    [SyncVar] public string deck;

    [SyncVar] public int gamePoint; //variables repr�sentant les points gagn�s par le joueur

    #endregion


    void Start()
    {

    }

    private void ChangePseudo(string oldValue, string newValue)
    {
        gameObject.name = newValue;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
