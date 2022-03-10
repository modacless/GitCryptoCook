using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;
using System.Linq;


public class PlayerBehavior : NetworkBehaviour
{

    #region R�f�rences
    [Header("R�f�rences")]

    [SerializeField]
    private GameObject buttonNextRound;
    [SerializeField]
    private GameObject cardObjectRecipe;
    [SerializeField]
    private GameObject cardObjectEffect;
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

    public List<TextMeshPro> repasScores;

    #endregion

    #region param�tres

    [Header("Paramètres")]
    //Variables venant du lobby
    [HideInInspector]
    [SyncVar(hook = nameof(ChangePseudo))] public string pseudo;

    //Permet de choisir le deck que l'on veut joueur
    [SyncVar] public string deck;
    public Decks DeckChoosen;

    public float repasRecipeStackingOffset;
    public float repasRecipeStartOffset;
    public float alimentReserveDuplicataOffset;

    [HideInInspector]
    [SyncVar] public int gamePoint; //variables repr�sentant les points gagn�s par le joueur

    //Carte pioch� au d�but de la partie
    private int startHand = 5;

    private int maxCardsInHand = 7;

    [HideInInspector]
    public int currentPoint = 0; //Point du joueur
    public int maxPoint = 50; //Point a atteindre pour que le joueur gagne

    [HideInInspector]
    [SyncVar(hook = nameof(ShowButtonTurn))] public bool yourTurn = false;

    private bool isPickingInDeck = false;

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
    /// Stockage des diff�rentes cartes
    /// </summary>

    // Liste de cartes dans la main du joueur
    [HideInInspector]
    public List<ChefCardBehaviour> handCards = new List<ChefCardBehaviour>();
    [HideInInspector]
    public bool[] handCardsPositionIsNotEmpty;
    // Liste de cartes dans le deck du joueur, dois agir comme une pile
    [SerializeField]
    private List<ChefCardScriptable> chefDeck;
    //Liste de toutes les cartes aliments
    [HideInInspector]
    public List<AlimentBehavior> reserveCards;
    //Emplacement des cartes, permet de savoir dans quel plats on met les recettes
    public GameObject[] boardCardsEmplacement;
    //Recete sur le plateau
    [HideInInspector]
    public List<Repas> boardRepas = new List<Repas>();
    //Emplacement des aliment dans la réserve
    public GameObject[] reservesSlotsPositions;

    public List<GameObject> repasHighlight;
    private List<List<AlimentBehavior>> reserveSlots;

    [HideInInspector]
    public ChefCardBehaviour selectedChefCard;
    [HideInInspector]
    public AlimentBehavior selectedAliment;
    [HideInInspector]
    public Repas selectMeal;

    public List<AlimentBehavior> engagedAliment;

    public List<ScriptableEffect> effectActiveThisTurn;

    #endregion


    public class Repas
    {
        public List<ChefCardBehaviour> allRecipes = new List<ChefCardBehaviour>();
        public GameObject sceneObject;
        public int basePoint;
        public int variablePoint;
        public GameObject highlight;

        public Repas(GameObject _highlight)
        {
            highlight = _highlight;
        }

    }


    IEnumerator Start()
    {
        if (isServer)
            yield return new WaitUntil(() => allPlayersReady());

        DeckChoosen.decks.TryGetValue(deck, out List<ChefCardScriptable> deckFind);
        chefDeck = deckFind;

        textPoint.gameObject.SetActive(false);
        handCardsPositionIsNotEmpty = new bool[maxCardsInHand];
        handCardsPositionIsNotEmpty.Count(v => (v = false));

        //Inutile je pense
        for (int i = 0; i < boardCardsEmplacement.Length; i++)
        {
            Repas r = new Repas(repasHighlight[i]);
            r.sceneObject = boardCardsEmplacement[i];
            boardRepas.Add(r);

        }

        gameManager = GameObject.Find("GameManager");
        deckManager = gameManager.GetComponent<DeckManager>();
        buttonNextRound.SetActive(false);
        textStatePlayer.gameObject.SetActive(false);
        engagedAliment = new List<AlimentBehavior>();
        reserveSlots = new List<List<AlimentBehavior>>();

        yield return new WaitUntil(() => deckManager != null);

        if (!isClientOnly)
        {
            //Debug.Log("Host pos" + deckManager.playerPosition[0].transform.position);
            transform.position = deckManager.playerPosition[0].transform.position;
        }
        else
        {
            //Debug.Log("Client pos" + deckManager.playerPosition[1].transform.position);
            transform.position = deckManager.playerPosition[1].transform.position;
        }

        if (hasAuthority)
        {
            deckManager.authorityPlayer = this;
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
                Camera.main.transform.rotation = deckManager.posCamP1.transform.rotation;
                Camera.main.transform.position = deckManager.posCamP1.transform.position;
            }

            if (isClientOnly)
            {
                statePlayer = StatePlayer.EnnemyPhase;
                CmdInitializeDeckClient();
                //CmdChangePlayerPosition(deckManager.playerPosition[1].transform.position);
                transform.localRotation = Quaternion.Euler(0, 180, 0);

                Camera.main.transform.rotation = deckManager.posCamP2.transform.rotation;
                Camera.main.transform.position = deckManager.posCamP2.transform.position;
            }

            //On attend que les joueurs se positionnent correctement
            yield return new WaitForSeconds(1f);

            for (int i = 0; i < startHand; i++)
            {
                PickupInDeckCuisine();
                yield return new WaitUntil(() => isPickingInDeck == false);
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
            if (Input.GetKeyDown(KeyCode.R))
            {
                Debug.Log("r");
                StartSelectTableIngredient(AlimentScriptable.Gout.Salé);
            }

            textPoint.text = currentPoint.ToString();

            for (int i = 0; i < boardRepas.Count; i++)
            {
                repasScores[i].text = (boardRepas[i].basePoint + boardRepas[i].variablePoint).ToString();
                if (boardRepas[i].allRecipes.Count > 0)
                {
                    repasScores[i].transform.parent.gameObject.SetActive(true);
                }
                else
                {
                    repasScores[i].transform.parent.gameObject.SetActive(false);
                }
            }

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

            CardZoom();
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
            deckManager.UpdateAlimentCardHalo(true);
        }
    }

    public void PickupInDeckCuisine()
    {
        if(chefDeck.Count > 0)
        {
            isPickingInDeck = true;
            CmdCreateCard(this.gameObject);
        }
    }

    private void ShuffleDeckCuisine()
    {
        chefDeck.OrderBy(item => Random.Range(0, chefDeck.Count));
    }

    //Permet de cr�er une carte, c�t� client et server
    [Command]
    private void CmdCreateCard(GameObject playerobj)
    {
        
        //RpcCreateCard();
        GameObject cardToChoose = null;
        if(chefDeck[0].cardType == ScriptableCard.CardType.Effet)
        {
            cardToChoose = cardObjectEffect;
            Debug.Log("EFFECT " + chefDeck[0].cardName);
        }
        else if(chefDeck[0].cardType == ScriptableCard.CardType.Recette)
        {
            cardToChoose = cardObjectRecipe;
            Debug.Log("Recette " + chefDeck[0].cardName);
        }
        GameObject cardObj = Instantiate(cardToChoose, deckObject.transform.position, Quaternion.identity);
        int emplacement = FindPlaceInHand(cardObj.GetComponent<ChefCardBehaviour>());
        if (emplacement != -1)
        {
            //Debug.Log("Create Card");
            cardObj.transform.position = cardPosition[emplacement].transform.position; //La position de la carte pioch� �tant, la taille de la main
            
            float angle = Vector3.Angle(new Vector3(cardObj.transform.position.x, 0, cardObj.transform.position.z), new Vector3(Camera.main.transform.position.x, 0, Camera.main.transform.position.z));
            if(cardObj.transform.position.x <= Camera.main.transform.position.x)
            {
                angle = -angle;
            }
            cardObj.transform.localRotation = Quaternion.Euler(50, angle, 0);
            
            cardObj.GetComponent<ChefCardBehaviour>().SetCurrentPosAsBase();
            NetworkServer.Spawn(cardObj, playerobj);
            RpcCreateCard(cardObj,emplacement);
        }
        else
        {
            //Discard
        }
    }

    //Permet de r�pliquer la cr�ation de la carte
    [ClientRpc]
    private void RpcCreateCard(GameObject cardObj,int emplacement)
    {
        cardObj.GetComponent<ChefCardBehaviour>().player = this;
        cardObj.GetComponent<ChefCardBehaviour>().InitializeCard(chefDeck[0], this);
        handCards.Add(cardObj.GetComponent<ChefCardBehaviour>());
        chefDeck.RemoveAt(0);
        isPickingInDeck = false;
    }

    [ClientRpc]
    private void RpcDestroyCard()
    {
        chefDeck.RemoveAt(0);
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

    private int FindPlaceInHand(ChefCardBehaviour card)
    {
        for(int i = 0; i< handCardsPositionIsNotEmpty.Length; i++)
        {
            //Debug.Log(i);
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
    public void CmdDropCardOnBoard(ChefCardBehaviour card,int emplacement)
    {
        RpcDropCardOnBoard(card,emplacement);
    }

    [ClientRpc]
    public void RpcDropCardOnBoard(ChefCardBehaviour card,int emplacement)
    {
        boardRepas[emplacement].allRecipes.Add(card);
        card.repas = boardRepas[emplacement];
        handCards.Remove(card);
        handCardsPositionIsNotEmpty[card.emplacementHand] = false;

        card.transform.position = boardCardsEmplacement[emplacement].transform.GetChild(0).position + Vector3.forward * repasRecipeStartOffset + Vector3.back * repasRecipeStackingOffset * boardRepas[emplacement].allRecipes.Count;
        card.transform.rotation = Quaternion.Euler(85, 0, 0);
        card.SetCurrentPosAsBase();

        if (card.cardLogic.effect != null && card.isEffectActive)
            StartCoroutine(card.cardLogic.effect.OnUse(card));

        if(card.cardLogic.cardType == ScriptableCard.CardType.Recette)
        {
            OnNewCardRefreshBoard(card);
        }
        else
        {
            card.DestroyCard();
        }


        RefreshBoard();
    }

    public int FindBoardPlaces(GameObject card)
    {
        for(int i =0; i< boardCardsEmplacement.Length; i++)
        {
            if(boardCardsEmplacement[i] == card)
            {
                //Debug.Log("Empacement sur le board + " + i);
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

    #region Sélection des cartes (effets)

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
    //Permet de sélectionner une recette sur la planche Allié
    private IEnumerator SelectRecipeAlly()
    {
        StatePlayer oldState = statePlayer;
        statePlayer = StatePlayer.EffetPhase;
        textStatePlayer.text = "Select Receipe ally";
        
        while (selectedChefCard == null)
        {
            RaycastHit hit;
            int layerMask = LayerMask.GetMask("Card");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                if (hit.transform.tag == "Card" && Input.GetMouseButton(0) && hit.transform.GetComponent<ChefCardBehaviour>().isOnBoard && hit.transform.GetComponent<NetworkIdentity>().hasAuthority)
                {
                    selectedChefCard = hit.transform.GetComponent<ChefCardBehaviour>();
                    Debug.Log("Carte s�lectionn� : " + selectedChefCard);
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

    //Permet de sélectionner une recette sur la planche énemie
    private IEnumerator SelectRecipeEnemy()
    {
        StatePlayer oldState = statePlayer;
        statePlayer = StatePlayer.EffetPhase;
        textStatePlayer.text = "Select Receipe Enemy";

        while (selectedChefCard == null)
        {
            RaycastHit hit;
            int layerMask = LayerMask.GetMask("Card");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                if (hit.transform.tag == "Card" && Input.GetMouseButton(0) && hit.transform.GetComponent<ChefCardBehaviour>().isOnBoard && !hit.transform.GetComponent<NetworkIdentity>().hasAuthority)
                {
                    selectedChefCard = hit.transform.GetComponent<ChefCardBehaviour>();
                    Debug.Log("Carte s�lectionn� : " + selectedChefCard);
                }
            }
            yield return new WaitForEndOfFrame();
        }
        statePlayer = oldState;
    }
    //Permet de sélectionner un repas sur la planche allié
    public void StartSelectMealAlly()
    {
        StartCoroutine(SelecMealAlly());
    }

    private IEnumerator SelecMealAlly()
    {
        StatePlayer oldState = statePlayer;
        statePlayer = StatePlayer.EffetPhase;
        textStatePlayer.text = "Select Meal Ally";

        while (selectMeal == null)
        {
            RaycastHit hit;
            int layerMask = LayerMask.GetMask("DropCard");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                if (hit.transform.tag == "Board" && Input.GetMouseButton(0))
                {
                    GameObject root = hit.transform.parent.parent.gameObject;
                    if (root.transform.GetComponent<NetworkIdentity>().hasAuthority) {
                        for (int i = 0; i < boardRepas.Count; i++)
                        {
                            if (boardRepas[i].sceneObject == hit.transform.gameObject)
                            {
                                selectMeal = boardRepas[i];
                            }
                        }

                    }
                }
            }
            yield return new WaitForEndOfFrame();
        }
        selectMeal = null;
        statePlayer = oldState;
    }

    public void StartSelectMealEnemy()
    {
        StartCoroutine(SelecMealEnemy());
    }
    //Permet de sélectionner un repas sur la planche énemie
    private IEnumerator SelecMealEnemy()
    {
        StatePlayer oldState = statePlayer;
        statePlayer = StatePlayer.EffetPhase;
        textStatePlayer.text = "Select Meal Enemy";

        while (selectMeal == null)
        {
            RaycastHit hit;
            int layerMask = LayerMask.GetMask("DropCard");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                if (hit.transform.tag == "Board" && Input.GetMouseButton(0))
                {
                    GameObject root = hit.transform.parent.parent.gameObject;
                    if (!root.GetComponent<NetworkIdentity>().hasAuthority)
                    {
                        Debug.Log("board Enemie");
                        for (int i = 0; i < root.GetComponent<PlayerBehavior>().boardRepas.Count; i++)
                        {
                            if (root.GetComponent<PlayerBehavior>().boardRepas[i].sceneObject == hit.transform.gameObject)
                            {
                                selectMeal = root.GetComponent<PlayerBehavior>().boardRepas[i];
                            }
                        }
                    }
                }
            }
            yield return new WaitForEndOfFrame();
        }
        statePlayer = oldState;
    }

    public void StartSelectTableIngredient(AlimentScriptable.Gout flavor)
    {
        StartCoroutine(SelectTableIngredient(flavor));
    }

    private IEnumerator SelectTableIngredient(AlimentScriptable.Gout flavor)
    {
        StatePlayer oldState = statePlayer;
        statePlayer = StatePlayer.EffetPhase;
        textStatePlayer.text = "Select Food On table";

        while (selectedChefCard == null)
        {
            RaycastHit hit;
            int layerMask = LayerMask.GetMask("Card");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                if (hit.transform.tag == "Card" && Input.GetMouseButton(0))
                {
                    if(hit.transform.GetComponent<AlimentBehavior>() != null)
                    {
                        AlimentBehavior alim = hit.transform.GetComponent<AlimentBehavior>();
                        if (alim.alimentLogic.cardType == ScriptableCard.CardType.Aliment && hit.transform.GetComponent<NetworkIdentity>().hasAuthority && !alim.isInReserve && flavor == alim.alimentLogic.gout)
                        {
                            selectedAliment = hit.transform.GetComponent<AlimentBehavior>();
                            Debug.Log("Carte s�lectionn� : " + selectedAliment);
                        }
                    }
                }
            }
            yield return new WaitForEndOfFrame();
        }
        statePlayer = oldState;
    }

    public void StartSelectIngredientAlly()
    {
        StartCoroutine(SelectIngredientAlly());
    }

    private IEnumerator SelectIngredientAlly()
    {
        StatePlayer oldState = statePlayer;
        statePlayer = StatePlayer.EffetPhase;
        textStatePlayer.text = "Select Food On ally Reserve";

        while (selectedChefCard == null)
        {
            RaycastHit hit;
            int layerMask = LayerMask.GetMask("Card");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                if (hit.transform.tag == "Card" && Input.GetMouseButton(0))
                {
                    if (hit.transform.GetComponent<AlimentBehavior>() != null)
                    {
                        AlimentBehavior alim = hit.transform.GetComponent<AlimentBehavior>();
                        if (alim.alimentLogic.cardType == ScriptableCard.CardType.Aliment && hit.transform.GetComponent<NetworkIdentity>().hasAuthority && alim.isInReserve)
                        {
                            selectedAliment = hit.transform.GetComponent<AlimentBehavior>();
                            Debug.Log("Carte s�lectionn� : " + selectedAliment);
                        }
                    }
                }
            }
            yield return new WaitForEndOfFrame();
        }
        statePlayer = oldState;
    }

    public void StartSelectIngredientEnemy()
    {
        StartCoroutine(SelectIngredientEnemy());
    }

    private IEnumerator SelectIngredientEnemy()
    {
        StatePlayer oldState = statePlayer;
        statePlayer = StatePlayer.EffetPhase;
        textStatePlayer.text = "Select Food On Enemy Reserve";

        while (selectedChefCard == null)
        {
            RaycastHit hit;
            int layerMask = LayerMask.GetMask("Card");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                if (hit.transform.tag == "Card" && Input.GetMouseButton(0))
                {
                    if (hit.transform.GetComponent<AlimentBehavior>() != null)
                    {
                        AlimentBehavior alim = hit.transform.GetComponent<AlimentBehavior>();
                        if (alim.alimentLogic.cardType == ScriptableCard.CardType.Aliment && !hit.transform.GetComponent<NetworkIdentity>().hasAuthority && alim.isInReserve)
                        {
                            selectedAliment = hit.transform.GetComponent<AlimentBehavior>();
                            Debug.Log("Carte s�lectionn� : " + selectedAliment);
                        }
                    }
                }
            }
            yield return new WaitForEndOfFrame();
        }
        statePlayer = oldState;
    }

    #endregion

    public void OnNewCardRefreshBoard(ChefCardBehaviour newChefCard)
    {
        for (int i = 0; i < effectActiveThisTurn.Count; i++)
        {
            StartCoroutine(effectActiveThisTurn[i].OnNewCardPlayed(null, newChefCard));
        }

        for (int i = 0; i < boardRepas.Count; i++)
        {
            for (int j = 0; j < boardRepas[i].allRecipes.Count; j++)
            {
                StartCoroutine(boardRepas[i].allRecipes[j].cardLogic.effect.OnNewCardPlayed(boardRepas[i].allRecipes[j], newChefCard));
            }
        }
    }

    public void RefreshBoard()
    {
        currentPoint = 0;

        for (int k = 0; k < effectActiveThisTurn.Count; k++)
        {
            StartCoroutine(effectActiveThisTurn[k].OnBoardChange(null));
        }

        for (int i = 0; i < boardRepas.Count; i++)
        {
            for (int j = 0; j < boardRepas[i].allRecipes.Count; j++)
            {
                boardRepas[i].allRecipes[j].RefreshEffect();
            }

            boardRepas[i].variablePoint = 0;

            for (int j = 0; j < boardRepas[i].allRecipes.Count; j++)
            {
                boardRepas[i].variablePoint += boardRepas[i].allRecipes[j].basePoint + boardRepas[i].allRecipes[j].variablePoint;
            }

            currentPoint += boardRepas[i].variablePoint + boardRepas[i].basePoint;
        }
    }

    public void NewTurn()
    {
        statePlayer = StatePlayer.DrawPhase;
        effectActiveThisTurn.Clear();

        for (int i = 0; i < reserveCards.Count; i++)
        {
            reserveCards[i].ResetForTurn();
        }
    }

    private bool[] alimentUsedInCost;
    public bool CanPlayCard(ChefCardBehaviour card)
    {
        bool canPlayCard = false;
        alimentUsedInCost = new bool[reserveCards.Count];
        if(engagedAliment.Count == card.cardLogic.cost.Count)
        {
            if (TestCost(0, card))
            {
                canPlayCard = true;
            }
            else
            {
                Debug.Log("The aliments engaged do not correspond to the cost of the card");
            }
        }
        else
        {
            if(engagedAliment.Count > card.cardLogic.cost.Count)
            {
                Debug.Log("There is too much aliment engaged");
            }
            else
            {
                Debug.Log("There is not enough aliment engaged");
            }
        }

        return canPlayCard;
    }

    public void UseEngagedAliment()
    {
        for (int i = 0; i < engagedAliment.Count; i++)
        {
            engagedAliment[i].UseToPlayCard();
        }

        engagedAliment.Clear();
    }

    /// <summary>
    /// The recursive fonction of hell, ask antoine if you want to understand
    /// </summary>
    /// <param name="costIndexToTest"></param>
    /// <param name="card"></param>
    /// <returns></returns>
    private bool TestCost(int costIndexToTest, ChefCardBehaviour card)
    {
        if(costIndexToTest > 0)
        {
            ChefCardScriptable.Cost costToTest = card.cardLogic.cost[costIndexToTest];
            int currentCostIndex = costIndexToTest;
            for (int i = 0; i < engagedAliment.Count; i++)
            {
                if (!alimentUsedInCost[i])
                {
                    bool isValid = false;
                    switch (costToTest.costType)
                    {
                        case ChefCardScriptable.Cost.CostType.Gout:
                            if (engagedAliment[i].alimentLogic.gout == costToTest.goutCost)
                            {
                                isValid = true;
                            }
                            break;

                        case ChefCardScriptable.Cost.CostType.AlimentType:
                            if (engagedAliment[i].alimentLogic.alimentType == costToTest.alimentTypeCost)
                            {
                                isValid = true;
                            }
                            break;

                        case ChefCardScriptable.Cost.CostType.Specific:
                            if (engagedAliment[i].alimentLogic == costToTest.specificCost)
                            {
                                isValid = true;
                            }
                            break;

                        default:
                            break;
                    }

                    if (isValid)
                    {
                        alimentUsedInCost[i] = true;
                        if (currentCostIndex == card.cardLogic.cost.Count - 1)
                        {
                            return true;
                        }
                        else
                        {
                            currentCostIndex++;
                            if (TestCost(currentCostIndex, card))
                            {
                                return true;
                            }
                            else
                            {
                                alimentUsedInCost[i] = false;
                                currentCostIndex--;
                            }
                        }
                    }
                }
                else
                {
                    //Debug.Log("! aliment " + i + " " + engagedAliment[i].alimentLogic.cardName + " is blocked");
                }
            }
            return false;
        }
        return true;
    }

    [HideInInspector] public bool cardIsZoom = false;
    [HideInInspector] public bool isDraggingCard = false;
    [HideInInspector] public CardBehavior zoomedCard = null;
    public void CardZoom()
    {
        if (Input.GetMouseButtonDown(1)) //Récupère la carte sur laquelle le joueur clique
        {

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (cardIsZoom == false && !isDraggingCard)
            {
                if (Physics.Raycast(ray, out hit, 100, deckManager.cardMask))
                {
                    zoomedCard = hit.transform.GetComponent<CardBehavior>();
                    zoomedCard.SetCurrentPosAsBase();

                    hit.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 10.5f; 
                        //new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y - 10f, Camera.main.transform.position.z + 6f);

                    hit.transform.rotation = Camera.main.transform.rotation;

                    cardIsZoom = true;

                }
            }
            else if (cardIsZoom == true)
            {
                zoomedCard.ResetPos();
                cardIsZoom = false;

            }
        }
    }

    public void PlaceAlimentInReserve(AlimentBehavior newAliment)
    {
        newAliment.CmdSetInReserve(true);
        reserveCards.Add(newAliment);
        deckManager.CmdPickOnTable(newAliment);
        statePlayer = StatePlayer.PlayCardPhase;

        bool alimentTypeAlreadyInReserve = false;
        int reserveSlotIndex = 0;
        int reserveSlotDuplicataIndex = 0;
        for (int i = 0; i < reserveSlots.Count; i++)
        {
            if(reserveSlots[i][0].alimentLogic == newAliment.alimentLogic)
            {
                alimentTypeAlreadyInReserve = true;
                reserveSlotIndex = i;
                reserveSlotDuplicataIndex = reserveSlots[i].Count;
                reserveSlots[i].Add(newAliment);
            }
        }

        if(!alimentTypeAlreadyInReserve)
        {
            reserveSlots.Add(new List<AlimentBehavior>());
            reserveSlots[reserveSlots.Count - 1].Add(newAliment);
            reserveSlotIndex = reserveSlots.Count - 1;
            reserveSlotDuplicataIndex = reserveSlots[reserveSlots.Count - 1].Count - 1;
        }

        newAliment.transform.position = reservesSlotsPositions[reserveSlotIndex].transform.position + Vector3.left * reserveSlotDuplicataIndex * alimentReserveDuplicataOffset;
    }

    [Command(requiresAuthority = false)]
    public void CmdDestroyCardFromBoard(ChefCardBehaviour chefCardBehaviour)
    {
        //chefCardBehaviour.repas.allRecipes.Remove(chefCardBehaviour);
        RpcDestroyCardFromBoard(chefCardBehaviour);
        NetworkServer.Destroy(chefCardBehaviour.gameObject);
        RefreshBoard();
    }

    [ClientRpc]
    public void RpcDestroyCardFromBoard(ChefCardBehaviour chefCardBehaviour)
    {
        chefCardBehaviour.repas.allRecipes.Remove(chefCardBehaviour);

    }

    [Command(requiresAuthority = false)]
    public void CmdDestroyMeal(ChefCardBehaviour chefCardBehaviour)
    {
        List<ChefCardBehaviour> ca = new List<ChefCardBehaviour>(chefCardBehaviour.repas.allRecipes);
        //RpcDestroyMeal(chefCardBehaviour);
        chefCardBehaviour.repas.allRecipes.Clear(); //Problème à résoudre
        for (int i =0; i< ca.Count; i++)
        {
            NetworkServer.Destroy(ca[i].gameObject);
        }
    }

    [ClientRpc]
    public void RpcDestroyMeal(ChefCardBehaviour chefCardBehaviour)
    {
        chefCardBehaviour.repas.allRecipes.Clear();
        RefreshBoard();
    }



    public void HighlightRepas(int index, bool active)
    {
        LowLightAllRepas();
        boardRepas[index].highlight.SetActive(active);
    }

    public void LowLightAllRepas()
    {
        for (int i = 0; i < boardRepas.Count; i++)
        {
            boardRepas[i].highlight.SetActive(false);
        }
    }
}
