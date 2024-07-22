using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneReloader : MonoBehaviour
{
    // Этот метод перезагружает текущую сцену
    public void ReloadScene()
    {
        // Получаем текущую активную сцену
        Scene currentScene = SceneManager.GetActiveScene();

        // Перезагружаем текущую сцену
        SceneManager.LoadScene(currentScene.name);
    }
}
