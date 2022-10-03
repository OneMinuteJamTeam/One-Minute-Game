using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using xPoke.CustomLog;

public class GameController : Singleton<GameController>
{
    public int Player1Points { get; private set; } = 0;
    public int Player2Points { get; private set; } = 0;

    [Header("References")]
    [SerializeField]
    private Timer timer;
    [SerializeField]
    private GameObject pickablesGenerator;

    private GameObject pickablesContainer;
    private bool _isPause = false;
    private int _round = 1;
    private Pumpkin _pumpkin;
    private Scarecrow _scarecrow;
    private bool _rolesSwapped = false;
    private bool _roundOver = false;

    private void Start()
    {
        ResetPoints();
        StartCoroutine(COGetPlayerRef());
        timer.StartTimerAt(60, true);

        pickablesContainer = Instantiate(pickablesGenerator);
        pickablesGenerator.SetActive(false);
    }

    private void Update()
    {
        if(InputManager.Instance.GetApplicationPausePressed())
            SwitchPause();

        if(timer.CurrentTimer >=30 && timer.CurrentTimer < 31 && !_rolesSwapped)
            GivePointToEscapee();
        
        if(timer.CurrentTimer <= 0 && !_roundOver)
            GivePointToEscapee();
    }

    public void GivePoint(int _player) 
    {
        if (_player == 0)
            Player1Points++;
        else if (_player == 1)
            Player2Points++;
        else 
            Debug.LogError("GameController: assigned point to player " + _player + ", that doesn't exist");

        PlayerPrefs.SetInt("P1Points",Player1Points);
        PlayerPrefs.SetInt("P2Points",Player2Points);

        //Debug.Log("P1 "+Player1Points + " - P2 " + Player2Points);

        GameUIManager.Instance.SetScoreText(Player1Points,Player2Points);

        if (_round == 1) 
        {
            _round++;
            SwapRoles();
        }
        else if (_round == 2) 
        {
            _roundOver = true;
            CustomSceneManager.Instance.LoadScene("Results");
        }
    }

    // TO-DO: DELETE
    public void DebugPlayerWins(int playerWhoWins) 
    {
        if (playerWhoWins == 0) 
        {
            Player1Points = Player2Points = 0;
            CustomSceneManager.Instance.LoadScene("Results");
        }
        else if (playerWhoWins == 1)
            Player1Points = 10;
        else
            Player2Points = 10;

        CustomSceneManager.Instance.LoadScene("Results");
    }

    public void ResetPoints()
    {
        PlayerPrefs.DeleteKey("P1Points");
        PlayerPrefs.DeleteKey("P2Points");
        Player1Points = Player2Points = 0;
        _round = 1;
        _roundOver = false;
    }

    private void SwapRoles()
    {
        CustomLog.Log(CustomLog.CustomLogType.GAMEPLAY, "ROLES SWAP");
        _rolesSwapped = true;

        timer.StartTimerAt(30, true);
        timer.StopTimer();
        

        _pumpkin.CanReadInput = false;
        _scarecrow.CanReadInput = false;

        Destroy(_pumpkin.gameObject);
        Destroy(_scarecrow.gameObject);
        Destroy(pickablesContainer);

        GameUIManager.Instance.PlaySwapPanel(() => { timer.ResumeTimer(); });
        StartCoroutine(COSwap());
        pickablesContainer = Instantiate(pickablesGenerator);
        pickablesContainer.SetActive(true);
    }

    private void GivePointToEscapee()
    {
        if (_pumpkin.IsEscaping)
            GivePoint((int)_pumpkin.GetPlayerNumber());
        else if (_scarecrow.IsEscaping)
            GivePoint((int)_scarecrow.GetPlayerNumber());
    }

    private IEnumerator COGetPlayerRef()
    {
        yield return new WaitForSeconds(0.1f);
        _pumpkin = FindObjectOfType<Pumpkin>();
        _scarecrow = FindObjectOfType<Scarecrow>();
    }

    private IEnumerator COSwap()
    {
        yield return new WaitForSeconds(1.0f);

        _pumpkin.SetIsEscaping(!_pumpkin.IsEscaping);
        _scarecrow.SetIsEscaping(!_scarecrow.IsEscaping);

        SpawnManager.Instance.SpawnPlayers();

        _pumpkin = FindObjectOfType<Pumpkin>();
        _scarecrow = FindObjectOfType<Scarecrow>();
    }

    #region Pause Handling
    public void SwitchPause() {
        if (_isPause)
            UnpauseGame();
        else
            PauseGame();
    }

    public void UnpauseGame() {
        if (_isPause)
        {
            Time.timeScale = 1f;
            GameUIManager.Instance.ShowPausePanel(false);
            _isPause = false;
        }
    }
    public void PauseGame() {
        if (!_isPause) 
        {
            Time.timeScale = 0;
            GameUIManager.Instance.ShowPausePanel(true);
            _isPause = true;
        }
    }
    #endregion
}