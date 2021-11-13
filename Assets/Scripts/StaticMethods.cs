using UnityEngine;

public static class StaticMethods
{
    public static Order GetOrder(string name)
    {
        bool skyBurger = PlayerPrefs.GetInt(name + "SkyBurger") == 1;

        if (skyBurger) return new Order();

        int size = PlayerPrefs.GetInt(name + "Amount");
        var parts = new Order.Part[size];

        for(int i = 0; i < size; i++)
        {
            int amount = PlayerPrefs.GetInt(name + i + " Amount");
            string blueprintName = PlayerPrefs.GetString(name + i + " Name");
            var blueprint = GameMaster.instance != null ? GameMaster.instance.GetBlueprint(blueprintName) : GameStarter.instance.GetBlueprint(blueprintName);

            parts[i] = new Order.Part(amount, blueprint);
        }

        return new Order(parts);
    }

    public static void SetOrder(Profile profile)
    {
        if (profile.order == null) return;

        bool skyBurger = profile.order.skyBurger;
        string name = profile.name + " Order ";

        PlayerPrefs.SetInt(name + "SkyBurger", skyBurger ? 1 : 0);

        if (skyBurger) return;

        int size = profile.order.parts.Length;
        PlayerPrefs.SetInt(name + "Amount", size);

        for (int i = 0; i < size; i++)
        {
            PlayerPrefs.SetInt(name + i + " Amount", profile.order.parts[i].amount);
            PlayerPrefs.SetString(name + i + " Name", profile.order.parts[i].ingredient.name);
        }
    }
}
