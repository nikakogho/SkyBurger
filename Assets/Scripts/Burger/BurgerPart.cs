using UnityEngine;

public class BurgerPart : MonoBehaviour
{
    public BurgerPartBlueprint blueprint;
    public SpriteRenderer rend;
    public Rigidbody2D rb;
    public PolygonCollider2D col;

    [ContextMenu("Apply")]
    void Apply()
    {
        Apply(blueprint, 1);
    }

    public void Apply(BurgerPartBlueprint blueprint, int index)
    {
        this.blueprint = blueprint;

        rend.sprite = blueprint.sprite;
        name = blueprint.name;

        rend.sortingOrder = index;

        if (col == null) col = gameObject.AddComponent<PolygonCollider2D>();
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (GameMaster.over || transform.position.y < col.transform.position.y) return;

        if(col.collider == Burger.instance.col)
        {
            Burger.instance.CollidingWithBurgerPart(this);
        }
    }
}
