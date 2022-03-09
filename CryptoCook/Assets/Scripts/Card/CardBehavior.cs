using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using static ScriptableCard;
using static PlayerBehavior;
using UnityEngine;
using Mirror;


public abstract class CardBehavior : NetworkBehaviour
{
    public LayerMask cardMask;

    #region r�f�rences
    public DeckManager deckManager;

    public GameObject cardHalo;
    #endregion

    #region Drag parameters
    protected float mZCoord;
    protected Vector3 basePosition;
    #endregion

    public PlayerBehavior player;

    private UnityEvent OnUse;

    

    void Start()
    {
        deckManager = GameObject.Find("GameManager").GetComponent<DeckManager>();
        mZCoord = Camera.main.WorldToScreenPoint(transform.position).z;

        cardHalo.SetActive(false);
    }

    public virtual void OnMouseDown()
    {
        basePosition = transform.position;
        deckManager.dragPlane.SetActive(true);
        transform.localRotation = Quaternion.Euler(90, 180, 0);

        if (isServer)
        {
            transform.localRotation = Quaternion.Euler(90, 0, 0);
        }
    }

    public abstract void OnMouseDrag();

    public abstract void OnMouseUp();


    protected Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mZCoord;
        return Camera.main.ScreenToViewportPoint(mousePoint);
    }
    
}
