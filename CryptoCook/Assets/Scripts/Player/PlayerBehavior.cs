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
    [SerializeField]
    private TextMeshProUGUI textPoint;

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

    public int currentPoint = 0; //Point du joueur
    public int maxPoint = 50; //Point a atteindre pour que le joueur gagne

    [SyncVar(hook = nameof(ShowButtonTurn))] public bool yourTurn = false;

    public enum StatePlayer
    {
        DrawPhase,
        PickupFoodPhase,
        PlayCardPhase,
        EffetPhase,
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
    //Emplacement des cartes, permet de savoir dans quel plats on met les recettes
    public GameObject[] boardCardsEmplacement;
    //Recete sur le plateau
    public List<Repas> boardRepas = new List<Repas>();

    public CardBehavior selectedCard;

    #endregion

    public class Repas
    {
        public List<CardBehavior> allRecipes = new List<CardBehavior>();
        public int points;
    }


    IEnumerator Start()
    {
        if (isServer)
            yield return new WaitUntil(() => allPlayersReady());

        textPoint.gameObject.SetActive(false);
        handCardsPositionIsNotEmpty = new bool[maxCardsInHand];
        handCardsPositionIsNotEmpty.Count(v => (v = false));

        //Inutile je pense
        for (int i = 0; i < boardCardsEmplacement.Length; i++)
        {
            boardRepas.Add(new Repas());
        }

        gameManager = GameObject.Find("GameManager");
        deckManager = gameManager.GetComponent<DeckManager>();
        buttonNextRound.SetActive(false);
        textStatePlayer.gameObject.SetActive(false);

        if (!isClientOnly)
        {
            Debug.Log("Host pos" + deckManager.playerPosition[0].transform.position);
            transform.position = deckManager.playerPosition[0].transform.position;
        }
        else
        {
            Debug.Log("Client pos" + deckManager.playerPosition[1].transform.position);
            transform.position = deckManager.playerPosition[1].transform.position;
        }

        if (hasAuthority)
        {
            ChangePseudo(pseudo, pseudo);
            textStatePlayer.gameObject.SetActive(true);
            textPoint.gameObject.SetActive(true);

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
            textPoint.text = currentPoint.ToString();

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
        cardObj.GetComponent<CardBehavior>().player = this;
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
                card.emplacementHand = i;
                handCardsPositionIsNotEmpty[i] = true;
                return i;
            }
        }

        return -1;
    }

    [Command]
    public void CmdDropCardOnBoard(CardBehavior card,int emplacement)
    {
        RpcDropCardOnBoard(card,emplacement);
    }

    [ClientRpc]
    public void RpcDropCardOnBoard(CardBehavior card,int emplacement)
    {
        boardRepas[emplacement].allRecipes.Add(card);
        card.repas = boardRepas[emplacement];
        handCards.Remove(card);
        handCardsPositionIsNotEmpty[card.emplacementHand] = false;
        StartCoroutine(card.cardLogic.effect.OnUse(card));
        RefreshBoard();
    }

    public int FindBoardPlaces(GameObject card)
    {
        for(int i =0; i< boardCardsEmplacement.Length; i++)
        {
            if(boardCardsEmplacement[i] == card)
            {
                Debug.Log("Empacement sur le board + " + i);
                return i;
            }
        }

        return 0;
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

    public void StartSelectRecipeAlly()
    {
        StartCoroutine(SelectRecipeAlly());
    }

    private IEnumerator SelectRecipeAlly()
    {
        StatePlayer oldState = statePlayer;
        statePlayer = StatePlayer.EffetPhase;
        textStatePlayer.text = "Select Receipe ally";
        
        while (selectedCard == null)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (hit.transform.tag == "Card" && Input.GetMouseButton(0) && hit.transform.GetComponent<CardBehavior>().isOnBoard && hit.transform.GetComponent<NetworkIdentity>().hasAuthority)
                {
                    selectedCard = hit.transform.GetComponent<CardBehavior>();
                    Debug.Log("Carte sélectionné : " + selectedCard);
                }
            }
            yield return new WaitForEndOfFrame();
        }
        statePlayer = oldState;
    }

    public void StartSelectRecipeEnemy()
    {
        StartCoroutine(SelectRecipeEnemy());
    }

    private IEnumerator SelectRecipeEnemy()
    {
        StatePlayer oldState = statePlayer;
        statePlayer = StatePlayer.EffetPhase;
        textStatePlayer.text = "Select Receipe Enemy";

        while (selectedCard == null)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (hit.transform.tag == "Card" && Input.GetMouseButton(0) && hit.transform.GetComponent<CardBehavior>().isOnBoard && !hit.transform.GetComponent<NetworkIdentity>().hasAuthority)
                {
                    selectedCard = hit.transform.GetComponent<CardBehavior>();
                    Debug.Log("Carte sélectionné : " + selectedCard);
                }
            }
            yield return new WaitForEndOfFrame();
        }
        statePlayer = oldState;
    }

    public void RefreshBoard()
    {
        for (int i = 0; i < boardRepas.Count; i++)
        {
            for (int j = 0; j < boardRepas[i].allRecipes.Count; j++)
            {
                boardRepas[i].allRecipes[j].RefreshEffect();
            }
        }

        currentPoint = 0;
        for (int i = 0; i < boardRepas.Count; i++)
        {
            for (int j = 0; j < boardRepas[i].allRecipes.Count; j++)
            {
                currentPoint += boardRepas[i].allRecipes[j].basePoint + boardRepas[i].allRecipes[j].variablePoint;
            }
        }

    }
}
