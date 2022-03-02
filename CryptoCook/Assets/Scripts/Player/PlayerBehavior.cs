using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

public class PlayerBehavior : NetworkBehaviour
{

    #region Références
    [SerializeField]
    private GameObject cardObject;
    [SerializeField]
    private GameObject deckObject;

    #endregion

    #region paramètres

    //Variables venant du lobby
    [SyncVar(hook = nameof(ChangePseudo))] public string pseudo;

    //Permet de choisir le deck que l'on a
    [SyncVar] public string deck;

    [SyncVar] public int gamePoint; //variables représentant les points gagnés par le joueur

    //Carte
    private int startHand = 5;
    // Liste de cartes dans la main du joueur
    private List<CardBehavior> handCards;
    // Liste de cartes dans le deck du joueur, dois agir comme une pile
    private List<CardBehavior> deckCards;

    #endregion


    void Start()
    {
        if (hasAuthority)
        {
            for(int i = 0; i< startHand; i++)
            {
                handCards.Add(PickupInDeckCuisine());
            }
        }
    }

    private void ChangePseudo(string oldValue, string newValue)
    {
        gameObject.name = newValue;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private CardBehavior PickupInDeckCuisine()
    {
        if(deckCards.Count > 0)
        {
            CardBehavior pickupCard = deckCards[0];
            deckCards.RemoveAt(0);
            return pickupCard;
        }

        return null;
    }

    private void ShuffleDeckCuisine()
    {
        deckCards.OrderBy(item => Random.Range(0, deckCards.Count));
    }

    private void CreateCard()
    {
        GameObject cardObj = Instantiate(cardObject, deckObject.transform.position,Quaternion.identity);

        NetworkServer.Spawn(cardObj);
    }


}
