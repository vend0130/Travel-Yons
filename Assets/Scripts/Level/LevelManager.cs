using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private List<string> _coinsList = new List<string>();
    private int _idStar;
    private int _coinInLevel;
    private bool _respawn;
    private Vector2 _positionRespawn;
    private int _direction;

    [HideInInspector] public List<string> coinsList { get => _coinsList; set => _coinsList = value; }
    [HideInInspector] public int idStar { get => _idStar; set => _idStar = value; }
    [HideInInspector] public int coinsInLevel { get => _coinInLevel; set => _coinInLevel = value; }
    [HideInInspector] public bool respawn { get => _respawn; set => _respawn = value; }
    [HideInInspector] public Vector2 positionRespawn { get => _positionRespawn; set => _positionRespawn = value; }
    [HideInInspector] public int direction { get => _direction; set => _direction = value; }
    [SerializeField] private Sprite[] checkPointSprites;
    [HideInInspector] public SpriteRenderer currentCheckPoint;

    [SerializeField] private ParticleSystem checkPointParticle;

    private void Awake()
    {
        I.levelManager = this;
        Load();

        I.audioManager.PlayTheme("BackgroundLevel");
    }

    private void Start()
    {
        Firebase.Analytics.FirebaseAnalytics.LogEvent("scene_level");
        _positionRespawn = I.c_Controller.transform.position;
    }

    private void Load()
    {
        GameObject level = Instantiate(I.gm.levels[I.currentLevel].prefab);
        _idStar = 0;
        _coinInLevel = 0;
        _respawn = false;
        _positionRespawn = Vector2.zero;
        if (I.dataLevels[I.currentLevel].stars == 3 && !I.full)
        {
            foreach (Transform ObjectLevel in level.transform)
            {
                if (ObjectLevel.name == "Coins")
                {
                    foreach (Transform coin in ObjectLevel)
                    {
                        for (int i = 0; i < I.dataLevels[I.currentLevel].coins.Count; i++)
                        {
                            if (I.dataLevels[I.currentLevel].coins[i] == coin.name)
                            {
                                coin.gameObject.SetActive(false);
                            }
                        }
                        continue;
                    }
                }
            }
        }
        Firebase.Analytics.FirebaseAnalytics.LogEvent("level_" + (I.currentLevel + 1) + "_start");
    }

    public void CoinAddInList(string _value)
    {
        _coinsList.Add(_value);
    }

    public void CheckPoint(GameObject _go, int getDirection)
    {
        if (!_respawn)
            _respawn = true;

        if(currentCheckPoint == null || currentCheckPoint.transform.position != _go.transform.position)
        {
            I.audioManager.Play("Checkpoint");
            checkPointParticle.Stop();
            Vector2 newPositionParticle = _go.transform.position;
            newPositionParticle.y += .3f;
            checkPointParticle.transform.position = newPositionParticle;
            checkPointParticle.Play();
        }

        _positionRespawn = _go.transform.position;
        _direction = getDirection;

        if (currentCheckPoint != null)
        {
            currentCheckPoint.sprite = checkPointSprites[0];
        }

        currentCheckPoint = _go.GetComponentInChildren<SpriteRenderer>();
        currentCheckPoint.sprite = checkPointSprites[1];


    }
}
