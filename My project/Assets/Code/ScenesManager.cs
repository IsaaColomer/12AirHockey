using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    public enum UserType
    {
        NONE,
        CLIENT,
        HOST
    }
    public ScenesManager instance;
    public UserType type;

    private void Start()
    {
        if(instance != null)
            Destroy(instance);
        else
            instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void Host(string sceneHost)
    {
        type = UserType.HOST;
        SceneManager.LoadScene(sceneHost);
    }
    public void Join(string sceneJoin)
    {
        type = UserType.CLIENT;
        SceneManager.LoadScene(sceneJoin);
    }
}


