using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameMaster : MonoBehaviour
{
    public BurgerPartBlueprint[] ingredients;
    public BurgerPartBlueprint top;
    public static GameMaster instance;

    public const float SinglePartValue = 0.6f;

    public static int level;
    public static Profile profile;

    public static bool skyBurger = false;

    public static float minX, maxX;

    public static bool over;

    public GameObject partUIPrefab;
    public Transform partUIParent;

    public Dictionary<BurgerPartBlueprint, PartUI> partUIs = new Dictionary<BurgerPartBlueprint, PartUI>();

    public GameObject winUI, loseUI, endPanel;
    public Text restartOrNextText;

    void Awake()
    {
        instance = this;
        over = false;

        level = profile.level;

        Cursor.visible = false;

        if(profile.order == null)
        {
            NewOrder();
        }

        skyBurger = profile.order.skyBurger;

        if (skyBurger)
        {
            foreach(var ingredient in ingredients)
            {
                var partUI = Instantiate(partUIPrefab, transform.position, Quaternion.identity, partUIParent).GetComponent<PartUI>();

                partUI.ApplySkyBurger(ingredient.sprite);
            }
        }
        else
        {
            foreach(var part in profile.order.parts)
            {
                var partUI = Instantiate(partUIPrefab, transform.position, Quaternion.identity, partUIParent).GetComponent<PartUI>();

                partUI.Apply(part);

                partUIs.Add(part.ingredient, partUI);
            }
        }
    }

    void NewOrder()
    {
        Order order;

        bool skyBurger = IsSkyBurger();

        if (skyBurger)
        {
            order = new Order();
        }
        else
        {
            int total = GetTotalFromLevel();

            List<Order.Part> parts = new List<Order.Part>();

            for(int i = 0; i < total; i++)
            {
                var ingredient = ingredients[Random.Range(0, ingredients.Length)];

                bool contains = false;

                foreach(var part in parts)
                {
                    if(part.ingredient == ingredient)
                    {
                        contains = true;
                        part.amount++;
                        break;
                    }
                }

                if (!contains)
                {
                    parts.Add(ingredient);
                }
            }

            order = new Order(parts.ToArray());
        }

        profile.AssignOrder(order);
    }

    bool IsSkyBurger()
    {
        int rand = Random.Range(0, level);

        return rand > 4 && rand > Random.Range(0, level);
    }

    int GetTotalFromLevel()
    {
        float curveAngle = level * 37;
        curveAngle *= Mathf.Deg2Rad;

        float variation = 1 + Mathf.Sin(curveAngle);

        float answer = level * 2 * variation;

        return (int)answer;
    }

    public BurgerPartBlueprint GetBlueprint(string name)
    {
        foreach(var blueprint in ingredients)
        {
            if (blueprint.name == name) return blueprint;
        }

        return null;
    }

    #region End

    void Win()
    {
        winUI.SetActive(true);
        restartOrNextText.text = "NEXT";

        profile.level++;

        int totalSize = 0;

        if (profile.order.skyBurger)
        {
            totalSize = Burger.instance.transform.childCount - 2;
        }
        else
        {
            foreach(var part in profile.order.parts)
            {
                totalSize += part.amount;
            }
        }

        profile.Money += totalSize * (1 + Burger.tip / 100);

        NewOrder();
    }

    void Lose()
    {
        loseUI.SetActive(true);
        restartOrNextText.text = "RESTART";

        profile.level = 0;
    }

    public void EndGame(bool won)
    {
        over = true;
        Cursor.visible = true;
        endPanel.SetActive(true);

        if (won)
        {
            Win();
        }
        else
        {
            Lose();
        }
    }

    #endregion
}

public class Order
{
    public class Part
    {
        public int amount;
        public BurgerPartBlueprint ingredient;

        public Part(int amount, BurgerPartBlueprint ingredient)
        {
            this.amount = amount;
            this.ingredient = ingredient;
        }

        public static implicit operator Part(BurgerPartBlueprint blueprint)
        {
            return new Part(1, blueprint);
        }
    }

    public Part[] parts;
    public bool skyBurger = false;

    public Order()
    {
        skyBurger = true;
        parts = null;
    }

    public Order(Part[] parts)
    {
        this.parts = parts;
        skyBurger = false;
    }
}
