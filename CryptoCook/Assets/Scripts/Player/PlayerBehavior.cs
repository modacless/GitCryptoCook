using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    // Liste de cartes dans la main du joueur
    public List<CardBehavior> handCards = new List<CardBehavior>();
    // Liste de cartes dans le deck du joueur, dois agir comme une pile
    [SerializeField]
    private List<ScriptableCard> deckCards;

    public int actualPoint = 0; //Point du joueur
    public int maxPoint = 50; //Point a atteindre pour que le joueur gagne

    [HideInInspector]
    [SyncVar(hook = nameof(ShowButtonTurn))] public bool yourTurn = false;

    #endregion


    void Start()
    {

        gameManager = GameObject.Find("GameManager");
        deckManager = gameManager.GetComponent<DeckManager>();
        buttonNextRound.SetActive(false);

        if (hasAuthority)
        {

            if (isServer)
            {
                yourTurn = true;
                buttonNextRound.SetActive(true);
                CmdInitializeDeckHost();
            }

            if (isClientOnly)
            {
                CmdInitializeDeckClient();
                Camera.main.transform.rotation = Quaternion.Euler(90, 180, 0);
            }

            transform.position = new Vector3(0, 0, -4);
            for (int i = 0; i < startHand; i++)
            {
                PickupInDeckCuisine();
            }
        }
        else
        {
            transform.position = new Vector3(0, 0, 4);
        }

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
            SelectCard();
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

    [Command]
    private void CmdCreateCard(GameObject playerobj)
    {
        //RpcCreateCard();
        GameObject cardObj = Instantiate(cardObject, deckObject.transform.position, Quaternion.identity);
        cardObj.GetComponent<CardBehavior>().InitializeCard(deckCards[0]);
        if (handCards.Count < 8)
        {
            cardObj.transform.position = cardPosition[handCards.Count].transform.position; //La position de la carte pioché étant, la taille de la main
            cardObj.transform.rotation = Quaternion.Euler(90, 0, 0);
            handCards.Add(cardObj.GetComponent<CardBehavior>());
            deckCards.RemoveAt(0);
            NetworkServer.Spawn(cardObj, playerobj);
        }
        else
        {
            //Discard
        }
    }

    [ClientRpc(includeOwner = false)]
    private void RpcCreateCard()
    {
        GameObject cardObj = Instantiate(cardObject, deckObject.transform.position, Quaternion.identity);
        cardObj.GetComponent<CardBehavior>().InitializeCard(deckCards[0]);
        if (handCards.Count < 8)
        {
            cardObj.transform.position = cardPosition[handCards.Count].transform.position; //La position de la carte pioché étant, la taille de la main
            cardObj.transform.rotation = Quaternion.Euler(90, 0, 0);
            handCards.Add(cardObj.GetComponent<CardBehavior>());
            deckCards.RemoveAt(0);
        }
        else
        {
            //Discard
        }
    }

    [Command]
    public void CmdInitializeDeckClient()
    {
        deckManager.playerClient = this.gameObject;
    }

    [Command]
    public void CmdInitializeDeckHost()
    {
        deckManager.playerHost = this.gameObject;
    }

    private int FindPlaceInHand()
    {
        for(int i = 0; i< handCards.Count; i++)
        {
            if (handCards[i] == null)
            {
                return i;
            }
        }

        return -1;
    }

    public void OnPressedNextTurn()
    {
        deckManager.CmdNextTurn();

        if (hasAuthority)
        {
            PickupInDeckCuisine();
        }
    }

    private void ShowButtonTurn(bool oldValue, bool newValue)
    {
        if (hasAuthority)
        {
            buttonNextRound.SetActive(newValue);
        }

    }

    private void SelectCard()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit,100))
            {
                if(hit.transform.gameObject.GetComponent<CardBehavior>() != null)
                {
                    if (hit.transform.gameObject.GetComponent<CardBehavior>().hasAuthority)
                    {
                        Debug.Log(hit.transform.gameObject);
                    }
                }
            }
            Debug.DrawRay(Input.mousePosition, Camera.main.transform.forward);
        }
    }


}
