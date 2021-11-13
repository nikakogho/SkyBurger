using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameStarter : MonoBehaviour
{
    public GameObject loadPrefab;
    public Transform loadParent;

    public BurgerPartBlueprint[] ingredients;

    public InputField newProfileNameField;

    public string gameSceneName = "main";

    public static GameStarter instance;

    public static List<Profile> profiles;
    public static Profile selectedProfile { get; private set; }

    void Awake()
    {
        instance = this;

        profiles = new List<Profile>();

        int loadAmount = PlayerPrefs.GetInt("Profiles", 0);

        for (int i = 0; i < loadAmount; i++)
        {
            string name = PlayerPrefs.GetString("Profile Name " + i);
            int money = PlayerPrefs.GetInt(name + " Money");
            int level = PlayerPrefs.GetInt(name + " Level");

            Order order = StaticMethods.GetOrder(name + " Order ");

            var profile = new Profile(name, money, level, order);

            profiles.Add(profile);
        }
    }

    public void LoadAllProfiles()
    {
        for(int i = loadParent.childCount - 1; i >= 0; i--)
        {
            Destroy(loadParent.GetChild(i).gameObject);
        }

        if (profiles.Count > 0)
        {
            for (int i = 0; i < profiles.Count; i++)
            {
                var clone = Instantiate(loadPrefab, transform.position, Quaternion.identity, loadParent);

                var load = clone.GetComponent<ProfileUI>();

                load.Apply(profiles[i]);
            }
        }
    }

    public void StartGame(Profile profile)
    {
        selectedProfile = profile;
        LoadGame();
    }

    public void VerifyName(string name)
    {
        bool contains = false;

        foreach(var profile in profiles)
        {
            if(profile.name == name)
            {
                contains = true;
                break;
            }
        }

        if (contains)
        {
            newProfileNameField.text = "";
            var placeHolderText = newProfileNameField.placeholder as Text;

            placeHolderText.color = Color.red;
            placeHolderText.text = "Such Name Already Exists! Please Enter A New One:";
        }
        else
        {
            var profile = new Profile(name, 0, 1, null);

            profiles.Add(profile);

            StartGame(profile);
        }
    }

    public void Exit()
    {
        SaveAll();
        Application.Quit();
    }

    public void DeleteAllProfiles()
    {
        PlayerPrefs.DeleteAll();

        profiles.Clear();
        selectedProfile = null;
    }

    public static void SaveAll()
    {
        PlayerPrefs.DeleteAll();

        int size = profiles.Count;

        PlayerPrefs.SetInt("Profiles", size);

        for(int i = 0; i < size; i++)
        {
            profiles[i].Save();
            PlayerPrefs.SetString("Profile Name " + i, profiles[i].name);
        }
    }

    void LoadGame()
    {
        SaveAll();
        GameMaster.profile = selectedProfile;
        SceneManager.LoadScene(gameSceneName);
    }

    public BurgerPartBlueprint GetBlueprint(string name)
    {
        foreach (var blueprint in ingredients)
        {
            if (blueprint.name == name) return blueprint;
        }

        return null;
    }
}

public class Profile
{
    public readonly string name;

    int money;
    public int Money { get { return money; } set { money = value; Save(); } }
    
    public int level;
    public Order order;

    public Profile(string name, int money, int level, Order order)
    {
        this.name = name;
        this.money = money;
        this.level = level;
        this.order = order;
    }

    public void AssignOrder(Order order)
    {
        this.order = order;

        Save();
    }

    public void Save()
    {
        PlayerPrefs.SetInt(name + " Money", money);
        PlayerPrefs.SetInt(name + " Level", level);

        StaticMethods.SetOrder(this);
    }

    public void Delete()
    {
        PlayerPrefs.DeleteKey(name + " Money");
        PlayerPrefs.DeleteKey(name + " Level");
        GameStarter.profiles.Remove(this);

        GameStarter.SaveAll();
    }
}
