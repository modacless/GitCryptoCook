using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using static PlayerBehavior;

public class ChefCardBehaviour : CardBehavior
{
    public TextMeshPro textName;
    public TextMeshPro textCost;
    public TextMeshPro textCostFood;
    public TextMeshPro textRecipeType;
    public TextMeshPro textCulture;
    public TextMeshPro textDescription;
    public SpriteRenderer spriteBackgroud;

    public ChefCardScriptable cardLogic;

    [SyncVar] public bool isOnBoard = false;

    public Repas repas;
    public int basePoint;
    public int variablePoint;

    public bool isEffectActive;

    [HideInInspector]
    [SyncVar] public int emplacementHand = -1;

    public List<ChefCardScriptable.Cost> currentCost;


    public void InitializeCard(ChefCardScriptable card, PlayerBehavior _player)
    {
        
        isEffectActive = true;
        player = _player;
        cardLogic = card;
        basePoint = cardLogic.pointEarn;

        if(cardLogic.cardType == ScriptableCard.CardType.Recette)
        {
            textName.text = cardLogic.cardName;
            currentCost = new List<ChefCardScriptable.Cost>(cardLogic.cost);
            textCost.text = (basePoint + variablePoint).ToString();
            string foodCost = "";
            for (int i = 0; i < currentCost.Count; i++)
            {
                if (currentCost[i].costType == ChefCardScriptable.Cost.CostType.AlimentType)
                {
                    foodCost += currentCost[i].alimentTypeCost.ToString();
                }

                if (currentCost[i].costType == ChefCardScriptable.Cost.CostType.Gout)
                {
                    foodCost += currentCost[i].goutCost.ToString();
                }

                if (currentCost[i].costType == ChefCardScriptable.Cost.CostType.Specific)
                {
                    foodCost += currentCost[i].specificCost.cardName;
                }

                if(i < currentCost.Count - 1)
                {
                    foodCost += " + ";
                }

            }
            textCostFood.text = foodCost;
            textRecipeType.text = cardLogic.recipeType.ToString();
            textCulture.text = cardLogic.recipeCulture.ToString();
            if(cardLogic.effect != null)
                textDescription.text = cardLogic.effect.effectDescription;
        }

        if(cardLogic.cardType == ScriptableCard.CardType.Effet)
        {

            if(cardLogic.sprite != null)
            {
                spriteBackgroud.sprite = cardLogic.sprite;
            }
            
            string foodCost = "";
            for (int i = 0; i < currentCost.Count; i++)
            {
                if (currentCost[i].costType == ChefCardScriptable.Cost.CostType.AlimentType)
                {
                    foodCost = currentCost[i].alimentTypeCost.ToString() + " + ";
                }


                if (currentCost[i].costType == ChefCardScriptable.Cost.CostType.Gout)
                {
                    foodCost = currentCost[i].goutCost.ToString() + " + ";
                }

                if (currentCost[i].costType == ChefCardScriptable.Cost.CostType.Specific)
                {
                    foodCost = currentCost[i].specificCost.cardName + " + ";
                }


            }
            textName.text = cardLogic.cardName;
            textCostFood.text = foodCost;
            if (cardLogic.effect != null)
                textDescription.text = cardLogic.effect.effectDescription;

        }
        

    }

    public override void OnMouseDrag()
    {
        if (hasAuthority && !isOnBoard && !deckManager.authorityPlayer.cardIsZoom)
        {
            //Vector3 ScreenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, mZCoord);
            //Vector3 newWorldPosition = Camera.main.ScreenToWorldPoint(ScreenPosition);
            //transform.position = newWorldPosition;
            deckManager.authorityPlayer.isDraggingCard = true;
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("DragPlane")))
            {
                transform.position = hit.point;
            }

            if (Physics.Raycast(transform.position + Vector3.up * 20f, Vector3.down, out hit, 200f, LayerMask.GetMask("DropCard")))
            {
                if (hit.transform.tag == "Board")
                {
                    if (player.statePlayer == StatePlayer.PlayCardPhase)
                    {
                        player.HighlightRepas(player.FindBoardPlaces(hit.transform.gameObject), true);
                    }
                }
            }
            else
            {
                player.LowLightAllRepas();
            }
        }
    }

    public override void OnMouseUp()
    {
        deckManager.dragPlane.SetActive(false);
        if (hasAuthority)
        {
            player.LowLightAllRepas();
            deckManager.authorityPlayer.isDraggingCard = false;
            RaycastHit hit;
            if (Physics.Raycast(transform.position + Vector3.up * 20f, Vector3.down, out hit, 200f, LayerMask.GetMask("DropCard")))
            {
                if (hit.transform.tag == "Board" && !isOnBoard)
                {
                    if (player.statePlayer == StatePlayer.PlayCardPhase)
                    {
                        if(player.CanPlayCard(this))
                        {
                            CmdPutOnBoard(true);
                            player.CmdDropCardOnBoard(this, player.FindBoardPlaces(hit.transform.gameObject));
                            player.UseEngagedAliment();
                        }
                        else
                        {
                            ResetToHand();
                        }
                    }
                    else
                    {
                        ResetToHand();
                    }

                }
                else
                {
                    ResetToHand();
                }
            }
            else
            {
                ResetToHand();
            }
        }
    }

    [Command]
    public void CmdPutOnBoard(bool isOn)
    {
        isOnBoard = isOn;
    }

    public void ResetToHand()
    {
        ResetPos();
    }

    public void RefreshEffect()
    {
        variablePoint = 0;
        if(cardLogic.effect != null && isEffectActive)
            StartCoroutine(cardLogic.effect.OnBoardChange(this));
    }

    public void DestroyCard()
    {
        player.CmdDestroyCardFromBoard(this);
    }

    public void AddCost(ChefCardScriptable.Cost addedCost)
    {
        currentCost.Add(addedCost);
        // refersh affichage
    }
}
