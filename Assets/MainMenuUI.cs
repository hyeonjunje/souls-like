using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Text clearCountText;
    [SerializeField] private Text deadCountText;

    private void Start()
    {
        Cursor.visible = true;

        clearCountText.text = "Ŭ���� Ƚ�� : " + DataManager.instance.clearCount;
        deadCountText.text = "���� Ƚ�� : " + DataManager.instance.deadCount;
    }

    public void NewStart()
    {
        GameManager.instance.NewStart();
    }


    public void Continue()
    {
        GameManager.instance.Continue();
    }


    public void ExitGame()
    {
        GameManager.instance.ExitGame();
    }
}
