using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

public class PlayerBehavior : NetworkBehaviour
{

    #region R�f�rences
    [Header("R�f�rences")]
    [SerializeField]
    private GameObject cardObject;
    [SerializeField]
    private GameObject deckObject;
    [SerializeField]
    private GameObject[] cardPosition;

    #endregion

    #region param�tres

    [Header("Param�tres")]
    //Variables venant du lobby
    [SyncVar(hook = nameof(ChangePseudo))] public string pseudo;

    //Permet de choisir le deck que l'on a
    [SyncVar] public string deck;

    [SyncVar] public int gamePoint; //variables repr�sentant les points gagn�s par le joueur

    //Carte pioch� au d�but de la partie
    private int startHand = 5;

    // Liste de cartes dans la main du joueur
    [SerializeField]
    private List<CardBehavior> handCards;
    // Liste de cartes dans le deck du joueur, dois agir comme une pile
    [SerializeField]
    private List<ScriptableCard> deckCards;

    #endregion


    void Start()
    {
        if (hasAuthority)
        {
           
        }

        for (int i = 0; i < startHand; i++)
        {
            handCards.Add(PickupInDeckCuisine());
        }

        transform.position = new Vector3(0, 0, 0);
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
            CardBehavior pickupCard = new CardBehavior();
            pickupCard.cardLogic = deckCards[0];
            CreateCard(pickupCard);
            deckCards.RemoveAt(0);
            return pickupCard;
        }

        return null;
    }

    private void ShuffleDeckCuisine()
    {
        deckCards.OrderBy(item => Random.Range(0, deckCards.Count));
    }

    private void CreateCard(CardBehavior logic)
    {
        GameObject cardObj = Instantiate(cardObject, deckObject.transform.position,Quaternion.identity);
        cardObj.GetComponent<CardBehavior>().InitializeCard(logic);
        cardObj.transform.position = cardPosition[handCards.Count].transform.localPosition; //La position de la carte pioch� �tant, la taille de la main
        cardObj.transform.rotation = Quaternion.Euler(90, 0, 0);
        NetworkServer.Spawn(cardObj);
    }


}
