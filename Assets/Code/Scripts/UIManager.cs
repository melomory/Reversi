using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Linq;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _topText;

    [SerializeField]
    private TextMeshProUGUI _blackScoreText;

    [SerializeField]
    private TextMeshProUGUI _whiteScoreText;

    [SerializeField]
    private TextMeshProUGUI _winnerText;

    [SerializeField]
    private Image _blackOverlay;

    [SerializeField]
    private RectTransform _playAgainButton;

    [SerializeField]
    private ToggleGroup _whiteBlackRadioOptions;

    [SerializeField]
    private RectTransform _playButton;

    public void SetPlayerText(Player currentPlayer)
    {
        if (currentPlayer == Player.Black)
        {
            _topText.text = "Black's Turn <sprite name=DiscBlackUp>";
        }
        else if (currentPlayer == Player.White)
        {
            _topText.text = "White's Turn <sprite name=DiscWhiteUp>";
        }
    }

    public void SetSkippedText(Player skippedPlayer)
    {
        if (skippedPlayer == Player.Black)
        {
            _topText.text = "Black Cannot Move! <sprite name=DiscBlackUp>";
        }
        else if (skippedPlayer == Player.White)
        {
            _topText.text = "White Cannot Move! <sprite name=DiscWhiteUp>";
        }
    }

    public void SetTopText(string message)
    {
        _topText.text = message;
    }

    public IEnumerator AnimateTopText()
    {
        _topText.transform.LeanScale(Vector3.one * 1.2f, 0.25f).setLoopPingPong(3);
        yield return new WaitForSeconds(2);
    }

    private IEnumerator ScaleDown(RectTransform rect)
    {
        rect.LeanScale(Vector3.zero, 0.2f);
        yield return new WaitForSeconds(0.2f);
        rect.gameObject.SetActive(false);
    }

    private IEnumerator ScaleUp(RectTransform rect)
    {
        rect.gameObject.SetActive(true);
        rect.localScale = Vector3.zero;
        rect.LeanScale(Vector3.one, 0.2f);
        yield return new WaitForSeconds(0.2f);
    }

    public IEnumerator ShowScoreText()
    {
        yield return ScaleDown(_topText.rectTransform);
        yield return ScaleUp(_blackScoreText.rectTransform);
        yield return ScaleUp(_whiteScoreText.rectTransform);
    }

    public void SetBlackScoreText(int score)
    {
        _blackScoreText.text = $"<sprite name=DiscBlackUp> {score}";
    }

    public void SetWhiteScoreText(int score)
    {
        _whiteScoreText.text = $"<sprite name=DiscWhiteUp> {score}";
    }

    private IEnumerator ShowOverlay()
    {
        _blackOverlay.gameObject.SetActive(true);
        _blackOverlay.color = Color.clear;
        _blackOverlay.rectTransform.LeanAlpha(0.8f, 1);
        yield return new WaitForSeconds(1);
    }

    private IEnumerator HideOverlay()
    {
        _blackOverlay.rectTransform.LeanAlpha(0, 1);
        yield return new WaitForSeconds(1);
        _blackOverlay.gameObject.SetActive(false);
    }

    private IEnumerator MoveScoresDown()
    {
        _blackScoreText.rectTransform.LeanMoveY(0, 0.5f);
        _whiteScoreText.rectTransform.LeanMoveY(0, 0.5f);
        yield return new WaitForSeconds(0.5f);
    }

    public void SetWinnerText(Player winner)
    {
        switch (winner)
        {
            case Player.Black:
                _winnerText.text = "Black Won!";
                break;
            case Player.White:
                _winnerText.text = "White Won!";
                break;
            case Player.None:
                _winnerText.text = "It's a Tie!";
                break;
        }
    }

    public IEnumerator ShowEndScreen()
    {
        yield return ShowOverlay();
        yield return MoveScoresDown();
        yield return ScaleUp(_winnerText.rectTransform);
        yield return ScaleUp(_playAgainButton);
    }

    public IEnumerator HideEndScreen()
    {
        StartCoroutine(ScaleDown(_winnerText.rectTransform));
        StartCoroutine(ScaleDown(_blackScoreText.rectTransform));
        StartCoroutine(ScaleDown(_whiteScoreText.rectTransform));
        StartCoroutine(ScaleDown(_playAgainButton));
        yield return new WaitForSeconds(0.5f);
        yield return HideOverlay();
    }

    public IEnumerator ShowCurrentScoreText()
    {
        _blackScoreText.rectTransform.LeanMove(new Vector3(-600, 300, 0), 0.5f);
        _whiteScoreText.rectTransform.LeanMove(new Vector3(600, 300, 0), 0.5f);
        yield return ScaleUp(_blackScoreText.rectTransform);
        yield return ScaleUp(_whiteScoreText.rectTransform);
        yield return new WaitForSeconds(0.5f);
    }

    public IEnumerator HideCurrentScoreText()
    {
        _blackScoreText.rectTransform.LeanMove(new Vector3(-150, 475, 0), 0.5f);
        _whiteScoreText.rectTransform.LeanMove(new Vector3(150, 475, 0), 0.5f);
        yield return ScaleDown(_blackScoreText.rectTransform);
        yield return ScaleDown(_whiteScoreText.rectTransform);
        yield return new WaitForSeconds(0.5f);
    }

    public void ShowStartOverlay()
    {
        _blackOverlay.gameObject.SetActive(true);
        _blackOverlay.color = new Color(0, 0, 0, 0.8f);
    }

    public void ShowStartDialog()
    {
        _whiteBlackRadioOptions.gameObject.SetActive(true);
        _playButton.gameObject.SetActive(true);
    }

    public void HideStartDialog()
    {
        _whiteBlackRadioOptions.gameObject.SetActive(false);
        _playButton.gameObject.SetActive(false);
    }

    public void ShowTopText()
    {
        _topText.gameObject.SetActive(true);
    }

    public void ShowStartScreen()
    {
        ShowStartOverlay();
        ShowStartDialog();
    }

    public string GetPlayerSide()
    {
        return _whiteBlackRadioOptions.ActiveToggles().FirstOrDefault().GetComponentInChildren<Text>().text;
    }

    public IEnumerator HideStartScreen()
    {
        HideStartDialog();
        yield return new WaitForSeconds(0.5f);
        yield return HideOverlay();
    }
}
