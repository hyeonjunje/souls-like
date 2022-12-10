using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class GameLogicManager : MonoBehaviour
{
    public static GameLogicManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    [SerializeField] private Image gameOverPanel;
    [SerializeField] private Text youDied;

    private Player _player;
    private Color _gameOverPanelColor;
    private Color _youDiedColor;
    private bool _readyToReStart = false;

    public static Vector3 respawnPos = new Vector3(2, 0.5f, 0);

    private void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();

        _player.transform.position = respawnPos;

        Color tempColor = gameOverPanel.color;

        _gameOverPanelColor = gameOverPanel.color;
        _youDiedColor = youDied.color;

        // 시작은 어둡게
        tempColor.a = 1f;
        gameOverPanel.color = tempColor;

        // 판넬만 활성화
        gameOverPanel.gameObject.SetActive(true);
        youDied.gameObject.SetActive(false);


        // 점점 밝아지면서 필요한 값 초기화
        gameOverPanel.DOFade(0, 1f).OnComplete(() =>
        {
            gameOverPanel.gameObject.SetActive(false);
            youDied.gameObject.SetActive(true);
            gameOverPanel.color = _gameOverPanelColor;
            youDied.color = _youDiedColor;
        });
        //_player.Revive();
    }

    public void GameOver()
    {
        _player.GetComponent<PlayerController>().enabled = false;

        gameOverPanel.gameObject.SetActive(true);
        gameOverPanel.DOFade(0, 2f).From();
        youDied.DOFade(0, 4f).From().OnComplete(() => ReStart());
    }

    public void ReStart()
    {
        SceneManager.LoadScene(0);


        /*gameOverPanel.DOFade(1, 2f).OnStart(() =>
        {
            youDied.gameObject.SetActive(false);
        })
            .OnComplete(() =>
        {
            Respawn();
            gameOverPanel.DOFade(0, 1f).OnComplete(() =>
            {
                gameOverPanel.gameObject.SetActive(false);
                youDied.gameObject.SetActive(true);
                gameOverPanel.color = _gameOverPanelColor;
                youDied.color = _youDiedColor;
            });
        });*/
    }

/*    private void Respawn()
    {
        // 지금은 임시, 나중에는 모닥불 같은거에서 부활해야 함
        // 모든 적들도 초기화

        _player.Revive();
    }*/
}
