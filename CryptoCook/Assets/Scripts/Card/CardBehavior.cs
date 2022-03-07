using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using static ScriptableCard;
using static PlayerBehavior;
using UnityEngine;
using Mirror;


public class CardBehavior : NetworkBehaviour
{
    public ScriptableCard cardLogic;
    public DeckManager deckManager;

    #region Drag parameters
    private float mZCoord;
    private Vector3 basePosition;

    public bool isInReserve = false;
    public bool isOnBoard = false;

    public PlayerBehavior player;
    public Repas repas;
    public int basePoint;
    public int variablePoint;

    [HideInInspector]
    [SyncVar] public int emplacementHand = -1;
    [SyncVar] public int emplacementFood = -1;

    private UnityEvent OnUse;


    #endregion

    void Start()
    {
        deckManager = GameObject.Find("GameManager").GetComponent<DeckManager>();
        mZCoord = Camera.main.WorldToScreenPoint(transform.position).z;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitializeCard(ScriptableCard card)
    {
        cardLogic = card;
        basePoint = cardLogic.pointEarn;
    }

    public void OnMouseDown()
    {
        basePosition = transform.position;
    }

    public void OnMouseDrag()
    {
        if (hasAuthority && !isInReserve && !isOnBoard)
        {
            Vector3 ScreenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, mZCoord);
            Vector3 newWorldPosition = Camera.main.ScreenToWorldPoint(ScreenPosition);
            transform.position = newWorldPosition;
        }
    }

    public void OnMouseUp()
    {
        if (hasAuthority)
        {
            RaycastHit hit;
            int emplacementMask = LayerMask.GetMask("DropCard");
            if (Physics.Raycast(transform.position,Vector3.down,out hit,Mathf.Infinity, emplacementMask))
            {
                if (hit.transform.tag == "Reserve" && !isInReserve && cardLogic.cardType == CardType.Aliment)
                {
                    PlayerBehavior pl = hit.transform.parent.parent.GetComponent<PlayerBehavior>();
                    Debug.Log(pl.pseudo);
                    if ( pl.statePlayer == PlayerBehavior.StatePlayer.PickupFoodPhase)
                    {
                        isInReserve = true;
                        pl.reserveCards.Add(this);
                        deckManager.CmdPickInReserve(this);                                                                                                                                                                                                 
                        pl.statePlayer = PlayerBehavior.StatePlayer.PlayCardPhase;
                    }
                    else
                    {
                        transform.position = basePosition;
                    }
                }
                else
                {
                    if (hit.transform.tag == "Board" && !isOnBoard && (cardLogic.cardType == CardType.Recette || cardLogic.cardType == CardType.Effet))
                    {
                        PlayerBehavior pl = hit.transform.parent.parent.GetComponent<PlayerBehavior>();
                        if(pl.statePlayer == PlayerBehavior.StatePlayer.PlayCardPhase)
                        {
                            isOnBoard = true;
                            pl.CmdDropCardOnBoard(this, pl.FindBoardPlaces(hit.transform.gameObject));
                        }
                        else
                        {
                            transform.position = basePosition;
                        }
                        
                    }
                    else
                    {
                        transform.position = basePosition;
                    }
                }
            }
            else
            {
                transform.position = basePosition;
            }
        }
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mZCoord;
        return Camera.main.ScreenToViewportPoint(mousePoint);
    }
    
    public void RefreshEffect()
    {
        StartCoroutine(cardLogic.effect.OnBoardChange(this));
    }
}
