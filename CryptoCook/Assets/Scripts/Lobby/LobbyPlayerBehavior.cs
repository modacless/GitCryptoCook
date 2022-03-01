using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class LobbyPlayerBehavior : NetworkBehaviour
{
    [Header("Références")]
    #region Références
    public TextMeshProUGUI pseudoText;
    #endregion

    #region paramètres
    [Header("SyncVar")]
    [SyncVar(hook = nameof(UpdatePseudoText))]
    public string pseudo;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdatePseudoText(string oldText, string newText)
    {
        pseudoText.text = newText;
    }


}
