using UnityEngine;
using UnityEngine.SceneManagement;

public class EndMethods : MonoBehaviour
{
    public string menuName = "menu";

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene(menuName);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
