using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class GameOverScreen : MonoBehaviour
{

  public void RestartGame()
  {
    SceneManager.LoadScene("CampoMinado");
  }

  public void ShowText(string text)
  {
    GetComponentInChildren<Text>().text = text;
  }
}
