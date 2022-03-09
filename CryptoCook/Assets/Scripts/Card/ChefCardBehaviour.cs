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

    public bool isOnBoard = false;

    public Repas repas;
    public int basePoint;
    public int variablePoint;

    [HideInInspector]
    [SyncVar] public int emplacementHand = -1;

    public List<ChefCardScriptable.Cost> currentCost;

    public void InitializeCard(ChefCardScriptable card)
    {
        Debug.Log(card.cardName);
        cardLogic = card;
        basePoint = cardLogic.pointEarn;

        if(cardLogic.cardType == ScriptableCard.CardType.Recette)
        {
            //Text Assignement
            textName.text = cardLogic.cardName;
            currentCost = new List<ChefCardScriptable.Cost>(cardLogic.cost);
            textCost.text = (basePoint + variablePoint).ToString();
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
            textCostFood.text = foodCost;
            textRecipeType.text = cardLogic.recipeType.ToString();
            textCulture.text = cardLogic.recipeCulture.ToString();
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
            textDescription.text = cardLogic.effect.effectDescription;

        }
        

    }

    public override void OnMouseDrag()
    {
        if (hasAuthority && !isOnBoard)
        {
            //Vector3 ScreenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, mZCoord);
            //Vector3 newWorldPosition = Camera.main.ScreenToWorldPoint(ScreenPosition);
            //transform.position = newWorldPosition;

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("DragPlane")))
            {
                transform.position = hit.point;
            }
        }
    }

    public override void OnMouseUp()
    {
        deckManager.dragPlane.SetActive(false);
        if (hasAuthority)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, LayerMask.GetMask("DropCard")))
            {
                if (hit.transform.tag == "Board" && !isOnBoard)
                {
                    PlayerBehavior pl = hit.transform.parent.parent.GetComponent<PlayerBehavior>();
                    if (pl.statePlayer == StatePlayer.PlayCardPhase)
                    {
                        isOnBoard = true;
                        pl.CmdDropCardOnBoard(this, pl.FindBoardPlaces(hit.transform.gameObject));
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

    public void ResetToHand()
    {
        transform.position = basePosition;
        transform.localRotation = Quaternion.Euler(50f, 0, 0);
    }

    public void RefreshEffect()
    {
        variablePoint = 0;
        StartCoroutine(cardLogic.effect.OnBoardChange(this));
    }
}
