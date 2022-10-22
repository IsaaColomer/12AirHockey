using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void Host(string sceneHost)
    {
        SceneManager.LoadScene(sceneHost);
    }
    public void Join(string sceneJoin)
    {
        SceneManager.LoadScene(sceneJoin);
    }
}
