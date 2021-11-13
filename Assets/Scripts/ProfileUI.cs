using UnityEngine;
using UnityEngine.UI;

public class ProfileUI : MonoBehaviour {

    public Text nameText, moneyText;
    Profile profile;

    public void Apply(Profile profile)
    {
        this.profile = profile;

        nameText.text = profile.name;
        moneyText.text = "$" + profile.Money;
    }

    public void Click()
    {
        GameStarter.instance.StartGame(profile);
    }

    public void Remove()
    {
        profile.Delete();

        Destroy(gameObject);
    }
}
