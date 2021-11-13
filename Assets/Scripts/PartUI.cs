using UnityEngine;
using UnityEngine.UI;

public class PartUI : MonoBehaviour
{
    public Image image;
    public Text amountText;
    public Image tickImage;

    int amount;

    public void ApplySkyBurger(Sprite sprite)
    {
        image.sprite = sprite;
        amountText.text = "?";
    }

    public void Apply(Order.Part part)
    {
        image.sprite = part.ingredient.sprite;
        amount = part.amount;
        amountText.text = amount.ToString();
    }

    public void AddItem()
    {
        amount--;

        amountText.text = amount.ToString();

        if(amount == 0)
        {
            amountText.enabled = false;
            tickImage.enabled = true;
        }
    }
}
