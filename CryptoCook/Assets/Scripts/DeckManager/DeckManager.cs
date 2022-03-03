using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DeckManager : NetworkBehaviour
{
    [SerializeField]
    private GameObject cardObject;
    [SerializeField]
    private GameObject deckObject;

    [SerializeField]
    private List<ScriptableCard> deckCards;
    [SerializeField]
    private GameObject[] cardsPosition;

    public List<CardBehavior> boardCards = new List<CardBehavior>();

    [SyncVar] public GameObject playerHost;
    [SyncVar] public GameObject playerClient;


    [SyncVar] public int tour = 0;

    // Start is called before the first frame update
    void Start()
    {
        //boardCards.Callback += OnHandUpdated;
        for (int i =0; i < cardsPosition.Length; i++)
        {
            PickupAliment(i);
        }

        /*while(playerHost == null || playerClient == null)
        {

        }*/

        int playerStart = Random.Range(0, 2);
        switch (playerStart)
        {
            case 0:
                break;
            case 1:
                break;
        }
    }

    public void PickupAliment(int emplacement)
    {
        CmdCreateCard(emplacement);
        deckCards.RemoveAt(0);
    }

    [Command(requiresAuthority = false)]
    private void CmdCreateCard(int emplacement)
    {
        Debug.Log("create");
        RpcCreateCard(emplacement);
    }

    [ClientRpc]
    private void RpcCreateCard(int emplacement)
    {
        GameObject cardObj = Instantiate(cardObject, deckObject.transform.position, Quaternion.identity);
        cardObj.GetComponent<CardBehavior>().InitializeCard(deckCards[0]);

        cardObj.transform.position = cardsPosition[emplacement].transform.position; //La position de la carte pioché étant, la taille de la main
        cardObj.transform.rotation = Quaternion.Euler(90, 0, 0);
        boardCards.Add(cardObj.GetComponent<CardBehavior>());
    }

    public void OnHandUpdated(SyncList<CardBehavior>.Operation op, int index, CardBehavior oldItem, CardBehavior newItem)
    {
        switch (op)
        {
            case SyncList<CardBehavior>.Operation.OP_ADD:
                // index is where it was added into the list
                // newItem is the new item
                newItem.cardLogic = deckCards[0];
                break;
            case SyncList<CardBehavior>.Operation.OP_INSERT:
                // index is where it was inserted into the list
                // newItem is the new item
                break;
            case SyncList<CardBehavior>.Operation.OP_REMOVEAT:
                // index is where it was removed from the list
                // oldItem is the item that was removed
                break;
            case SyncList<CardBehavior>.Operation.OP_SET:
                // index is of the item that was changed
                // oldItem is the previous value for the item at the index
                // newItem is the new value for the item at the index
                break;
            case SyncList<CardBehavior>.Operation.OP_CLEAR:
                // list got cleared
                break;
        }
    }

    public bool CanStartMatch()
    {
        if (playerHost == null)
            return false;
        if (playerClient == null)
            return false;



        return true;
    }

    [Command(requiresAuthority =false)]
    public void CmdNextTurn()
    {
        playerHost.GetComponent<PlayerBehavior>().yourTurn = !playerHost.GetComponent<PlayerBehavior>().yourTurn;
        if (playerHost.GetComponent<PlayerBehavior>().yourTurn)
        {

        }

        playerClient.GetComponent<PlayerBehavior>().yourTurn = !playerClient.GetComponent<PlayerBehavior>().yourTurn;

        if (playerClient.GetComponent<PlayerBehavior>().yourTurn)
        {

        }
    }


}
