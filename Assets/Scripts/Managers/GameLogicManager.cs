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

    private EnemySpawner _enemySpawner;

    private Player _player;
    private Color _gameOverPanelColor;
    private Color _youDiedColor;
    private bool _readyToReStart = false;

    public bool isBossFight = false;

    private void Start()
    {
        _enemySpawner = FindObjectOfType<EnemySpawner>();
        _enemySpawner.SpawnEnemy();

        _player = GameObject.Find("Player").GetComponent<Player>();

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
    }

    public void GameOver()
    {
        gameOverPanel.gameObject.SetActive(true);
        gameOverPanel.DOFade(0, 2f).From();
        youDied.DOFade(0, 4f).From().OnComplete(() => ReStart());
    }

    public void ReStart()
    {
        gameOverPanel.DOFade(1, 2f).OnStart(() =>
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
        });
    }

    private void Respawn()
    {
        // 최근 저장된 위치에서 부활
        _player.transform.position = DataManager.instance.lastPosition;
        _player.transform.rotation = DataManager.instance.lastRotation;

        // 부활 애니메이션
        _player.Revive();

        // 적 초기화
        _enemySpawner.SpawnEnemy();
    }
}
