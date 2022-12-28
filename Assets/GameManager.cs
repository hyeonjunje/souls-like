using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public Player player;

    public void NewStart()
    {
        DataManager.instance.ClearSaveData();
        SceneManager.LoadScene("InGame");

        Cursor.visible = false;
    }


    public void Continue()
    {
        SceneManager.LoadScene("InGame");

        Cursor.visible = false;
    }


    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // 어플리케이션 종료
#endif
    }


    public void ReturnMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }


    public void ConnectPlayer()
    {
        StartCoroutine(CoStartGame());
    }


    private void Update()
    {
        if (player != null)
            Debug.Log(player.transform.position);
    }

    IEnumerator CoStartGame()
    {
        player.GetComponent<PlayerController>().isControllable = false;

        float timer = 0f;
        while(true)
        {
            InfiniteLoopDetector.Run();
            timer += Time.deltaTime;

            player.transform.position = DataManager.instance.lastPosition;
            player.transform.rotation = DataManager.instance.lastRotation;

            if (timer > 1.5f)
                break;

            yield return null;
        }

        player.GetComponent<PlayerController>().isControllable = true;
    }
}
