using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class CardBehavior : NetworkBehaviour
{
    public ScriptableCard cardLogic;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitializeCard(ScriptableCard card)
    {
        cardLogic = card;
    }
}
