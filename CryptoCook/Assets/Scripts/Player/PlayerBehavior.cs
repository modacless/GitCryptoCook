using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;
using System.Linq;

public class PlayerBehavior : NetworkBehaviour
{

    #region Références
    [Header("Références")]

    [SerializeField]
    private GameObject buttonNextRound;
    [SerializeField]
    private GameObject cardObject;
    [SerializeField]
    private GameObject deckObject;
    [SerializeField]
    private GameObject[] cardPosition;

    [SerializeField]
    private TextMeshProUGUI textStatePlayer;

    private GameObject gameManager;
    private DeckManager deckManager;

    #endregion

    #region paramètres

    [Header("Paramètres")]
    //Variables venant du lobby
    [SyncVar(hook = nameof(ChangePseudo))] public string pseudo;

    //Permet de choisir le deck que l'on a
    [SyncVar] public string deck;

    [SyncVar] public int gamePoint; //variables représentant les points gagnés par le joueur

    //Carte pioché au début de la partie
    private int startHand = 5;

    private int maxCardsInHand = 7;

    public int actualPoint = 0; //Point du joueur
    public int maxPoint = 50; //Point a atteindre pour que le joueur gagne

    [SyncVar(hook = nameof(ShowButtonTurn))] public bool yourTurn = false;

    public enum StatePlayer
    {
        DrawPhase,
        PickupFoodPhase,
        PlayCardPhase,
        EnnemyPhase
    }

    [SyncVar] public StatePlayer statePlayer;

    /// <summary>
    /// Stockage des différentes cartes
    /// </summary>

    // Liste de cartes dans la main du joueur
    public List<CardBehavior> handCards = new List<CardBehavior>();
    public bool[] handCardsPositionIsNotEmpty;
    // Liste de cartes dans le deck du joueur, dois agir comme une pile
    [SerializeField]
    private List<ScriptableCard> deckCards;
    //Liste de toutes les cartes aliments
    public List<CardBehavior> reserveCards;

    public List<List<CardBehavior>> boardCards = new List<List<CardBehavior>>(5);

    #endregion


    IEnumerator Start()
    {
        if (isServer)
            yield return new WaitUntil(() => allPlayersReady());

        handCardsPositionIsNotEmpty = new bool[maxCardsInHand];
        handCardsPositionIsNotEmpty.Count(v => (v = false));

        //Inutile je pense
        for (int i = 0; i < boardCards.Count; i++)
        {
            boardCards[i] = new List<CardBehavior>();
        }

        gameManager = GameObject.Find("GameManager");
        deckManager = gameManager.GetComponent<DeckManager>();
        buttonNextRound.SetActive(false);
        textStatePlayer.gameObject.SetActive(false);

        if (!isClientOnly)
        {
            transform.position = deckManager.playerPosition[0].transform.position;
        }
        else
        {
            transform.position = deckManager.playerPosition[1].transform.position;
        }

        if (hasAuthority)
        {
            ChangePseudo(pseudo, pseudo);
            Debug.Log(deckManager);
            textStatePlayer.gameObject.SetActive(true);

            if (isServer)
            {
                CmdChangeTurn(true);
                buttonNextRound.SetActive(true);
                CmdInitializeDeckHost();
                //CmdChangePlayerPosition(deckManager.playerPosition[0].transform.position);
                deckManager.ChangeBoardAuthority(netIdentity.connectionToClient);
                
            }

            if (isClientOnly)
            {
                statePlayer = StatePlayer.EnnemyPhase;
                CmdInitializeDeckClient();
                //CmdChangePlayerPosition(deckManager.playerPosition[1].transform.position);
                transform.localRotation = Quaternion.Euler(0, 180, 0);
                Camera.main.transform.rotation = Quaternion.Euler(90, 0, 180);
            }

            //On attend que les joueurs se positionnent correctement
            yield return new WaitForSeconds(1f);

            for (int i = 0; i < startHand; i++)
            {
                PickupInDeckCuisine();
            }
        }
        else
        {
            if (isServer)
            {

            }
        }

        yield return null;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
    }

    private void ChangePseudo(string oldValue, string newValue)
    {
        if (!hasAuthority)
        {
            gameObject.name = newValue;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (hasAuthority)
        {
            if (statePlayer == StatePlayer.DrawPhase)
            {
                textStatePlayer.text = "Draw phase";

                //Check mouse is tuching deck
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    if (hit.transform.tag == "Deck" && Input.GetMouseButtonDown(0))
                    {
                        CmdDrawCard();
                    }
                }
            }

            if (statePlayer == StatePlayer.PickupFoodPhase)
            {
                textStatePlayer.text = "Pick a food";
            }
            if (statePlayer == StatePlayer.PlayCardPhase)
            {
                textStatePlayer.text = "Play your cards";
            }
            if (statePlayer == StatePlayer.EnnemyPhase)
            {
                textStatePlayer.text = "Ennemy Turn";
            }
        }

        if (!isClientOnly)
        {
            transform.position = deckManager.playerPosition[0].transform.position;
        }
        else
        {
            transform.position = deckManager.playerPosition[1].transform.position;
        }
    }

    #region Deck Interraction

    [Command]
    public void CmdDrawCard()
    {
        RpcDrawCard();
    }

    [ClientRpc]
    public void RpcDrawCard()
    {
        statePlayer = StatePlayer.PickupFoodPhase;
        if (hasAuthority)
        {
            PickupInDeckCuisine();
        }
    }

    private void PickupInDeckCuisine()
    {
        if(deckCards.Count > 0)
        {
            CmdCreateCard(this.gameObject);
        }
    }

    private void ShuffleDeckCuisine()
    {
        deckCards.OrderBy(item => Random.Range(0, deckCards.Count));
    }

    //Permet de créer une carte, côté client et server
    [Command]
    private void CmdCreateCard(GameObject playerobj)
    {
        //RpcCreateCard();
        GameObject cardObj = Instantiate(cardObject, deckObject.transform.position, Quaternion.identity);
        cardObj.GetComponent<CardBehavior>().InitializeCard(deckCards[0]);
        int emplacement = FindPlaceInHand(cardObj.GetComponent<CardBehavior>());
        if (emplacement != -1)
        {
            Debug.Log("Create Card");
            cardObj.transform.position = cardPosition[emplacement].transform.position; //La position de la carte pioché étant, la taille de la main
            cardObj.transform.rotation = Quaternion.Euler(90, 0, 0);
            NetworkServer.Spawn(cardObj, playerobj);
            RpcCreateCard(cardObj,emplacement);
        }
        else
        {
            //Discard
        }
    }

    //Permet de répliquer la création de la carte
    [ClientRpc]
    private void RpcCreateCard(GameObject cardObj,int emplacement)
    {
        cardObj.GetComponent<CardBehavior>().InitializeCard(deckCards[0]);
        handCards.Add(cardObj.GetComponent<CardBehavior>());
        deckCards.RemoveAt(0);
    }

    #endregion

    #region initialise
    [Command]
    public void CmdInitializeDeckClient()
    {
        deckManager.playerClient = this.gameObject;
        //RpcInitializeDeckClient();
    }

    [ClientRpc]
    public void RpcInitializeDeckClient()
    {
        deckManager.playerClient = this.gameObject;
    }

    [Command]
    public void CmdInitializeDeckHost()
    {
        deckManager.playerHost = this.gameObject;
        //RpcInitializeDeckHost();
        deckManager.GetComponent<NetworkIdentity>().AssignClientAuthority(netIdentity.connectionToClient);
    }

    [ClientRpc]
    public void RpcInitializeDeckHost()
    {
        deckManager.playerHost = this.gameObject;
    }

    #endregion

    #region hand and Card

    private int FindPlaceInHand(CardBehavior card)
    {
        for(int i = 0; i< handCardsPositionIsNotEmpty.Length; i++)
        {
            Debug.Log(i);
            if (handCardsPositionIsNotEmpty[i] == false)
            {
                card.emplacement = i;
                handCardsPositionIsNotEmpty[i] = true;
                return i;
            }
        }

        return -1;
    }

    [Command]
    public void CmdDropCardOnBoard(CardBehavior card)
    {
        RpcDropCardOnBoard(card);
    }

    [ClientRpc]
    public void RpcDropCardOnBoard(CardBehavior card)
    {
        Debug.Log(card.emplacement);
        handCards.Remove(card);
        handCardsPositionIsNotEmpty[card.emplacement] = false;
    }



    #endregion

    #region Turn logic

    [Command]
    public void CmdChangeTurn(bool turn)
    {
        yourTurn = turn;
    }

    public void OnPressedNextTurn()
    {
        deckManager.CmdNextTurn();
        statePlayer = StatePlayer.EnnemyPhase;
    }

    private void ShowButtonTurn(bool oldValue, bool newValue)
    {
        if (hasAuthority)
        {
            buttonNextRound.SetActive(newValue);
        }

    }

    #endregion

    [ServerCallback]
    private bool allPlayersReady()
    {
        foreach (NetworkConnection nt in NetworkServer.connections.Values)
        {
            if (!nt.isReady)
            {
                Debug.Log(nt);
                return false;
            }
        }

        return true;
    }

    [Command]
    public void CmdChangePlayerPosition(Vector3 newPosition)
    {
        RpcChangePlayerPosition(newPosition);
    }

    [ClientRpc]
    public void RpcChangePlayerPosition(Vector3 newPosition)
    {
        transform.position = newPosition;
    }
}
