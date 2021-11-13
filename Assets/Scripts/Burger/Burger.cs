using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Burger : MonoBehaviour
{
    public BurgerPart top { get; private set; }

    public Text tipText;
    public Collider2D col;

    List<Order.Part> collectedParts;

    int directionX;

    public float startSpeed = 20;

    float speed;

    float burgerLength;

    public static int tip;

    Renderer rend;

    public static Burger instance;

    void Awake()
    {
        instance = this;

        tip = 0;
        directionX = 0;
        SetPosToTargetPos();
        speed = startSpeed;

        collectedParts = new List<Order.Part>();

        rend = GetComponentInChildren<Renderer>();

        burgerLength = rend.bounds.extents.x;

        maxWidth = MaxWidth();

        GameMaster.maxX = maxWidth;
        GameMaster.minX = -maxWidth;
    }

    void Update()
    {
        float targetX = Mathf.Clamp(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, GameMaster.minX, GameMaster.maxX);
        float direction = targetX - transform.position.x;

        directionX = direction > 0 ? 1 : (direction < 0 ? -1 : 0);
    }

    float maxWidth;

    float MaxWidth()
    {
        Vector3 topCorner = new Vector3(Screen.width, Screen.height, 0);
        Vector3 targetWidth = Camera.main.ScreenToWorldPoint(topCorner);

        burgerLength = rend.bounds.extents.x;

        return targetWidth.x - burgerLength;
    }
    /*
    void FixedUpdate()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 targetPosition = new Vector3(mousePosition.x, transform.position.y, transform.position.z);

        float targetWidth = Mathf.Clamp(targetPosition.x, -maxWidth, maxWidth);

        targetPosition.x = targetWidth;

        transform.position += targetPosition;
    }*/

    void FixedUpdate()
    {
        float moveX = directionX * speed * Time.fixedDeltaTime;
        
        transform.position += Vector3.right * moveX;

        float targetX = Mathf.Clamp(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, GameMaster.minX, GameMaster.maxX);
        float direction = targetX - transform.position.x;

        float newDirectionX = direction > 0 ? 1 : (direction < 0 ? -1 : 0);

        if(directionX != newDirectionX)
        {
            SetPosToTargetPos();
        }
    }

    void SetPosToTargetPos()
    {
        Vector3 targetPos = transform.position;
        targetPos.x = Mathf.Clamp(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, GameMaster.minX, GameMaster.maxX);

        transform.position = targetPos;
    }

    void AssignNewTop(BurgerPart top, bool partNeeded)
    {
        this.top = top;
        top.transform.parent = transform;
        top.transform.rotation = Quaternion.identity;
        top.transform.position = col.transform.position + Vector3.up * 0.6f;

        if (top.transform.position.y > 0)
        {
            Camera cam = Camera.main;
            Vector3 newPos = cam.transform.position;
            newPos.y = top.transform.position.y;

            cam.transform.position = newPos;

            
        }

        speed *= 0.94f;

        Destroy(top.rb);

        Destroy(col);
        col = top.col;

        bool collected = false;

        foreach(var part in collectedParts)
        {
            if(part.ingredient == top.blueprint)
            {
                collected = true;
                part.amount++;
                break;
            }
        }

        if (!collected)
        {
            collectedParts.Add(top.blueprint);
        }

        if (GameMaster.profile.order.skyBurger || !partNeeded) return;

        var partUI = GameMaster.instance.partUIs[top.blueprint];

        partUI.AddItem();
    }

    bool PartNeeded(BurgerPartBlueprint blueprint)
    {
        Order order = GameMaster.profile.order;

        if (order.skyBurger) return blueprint != GameMaster.instance.top;

        if (blueprint == GameMaster.instance.top)
        {
            foreach (var part in order.parts)
            {
                bool isCollected = false;

                foreach (var collectedPart in collectedParts)
                {
                    if (collectedPart.ingredient == part.ingredient)
                    {
                        if (collectedPart.amount < part.amount) return false;

                        isCollected = true;
                        break;
                    }
                }

                if (!isCollected) return false;
            }

            return true;
        }

        foreach (var part in order.parts)
        {
            if(blueprint == part.ingredient)
            {
                foreach(var collectedPart in collectedParts)
                {
                    if(collectedPart.ingredient == blueprint)
                    {
                        return collectedPart.amount < part.amount;
                    }
                }

                return true;
            }
        }

        return false;
    }

    bool CheckIfAllCollected()
    {
        foreach (var part in GameMaster.profile.order.parts)
        {
            bool isCollected = false;

            foreach (var collectedPart in collectedParts)
            {
                if (collectedPart.ingredient == part.ingredient)
                {
                    if (collectedPart.amount < part.amount) return false;

                    isCollected = true;
                    break;
                }
            }

            if (!isCollected) return false;
        }

        return true;
    }

    public void CollidingWithBurgerPart(BurgerPart part)
    {
        float distance = Mathf.Abs(part.transform.position.x - transform.position.x);
        
        bool partNeeded = PartNeeded(part.blueprint);
        
        if (distance < burgerLength)
        {
            if(part.blueprint == GameMaster.instance.top)
            {
                bool won = CheckIfAllCollected();
                GameMaster.instance.EndGame(won);
                part.transform.position = col.transform.position + Vector3.up * 0.6f;
                
                return;
            }

            AssignNewTop(part, partNeeded);

            if (partNeeded) tip++;
            else if(tip > 0) tip--;
        }
        else
        {
            if (partNeeded && tip > 0)
            {
                tip--;
            }
        }

        TipTextUpdate();
        Destroy(part);
    }

    public void TipTextUpdate()
    {
        tipText.text = "Tip : " + tip + "%";
    }
}
