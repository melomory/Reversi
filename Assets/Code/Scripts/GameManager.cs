using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Camera _camera;

    [SerializeField]
    private Disc _discBlackUp;

    [SerializeField]
    private Disc _discWhiteUp;

    [SerializeField]
    private GameObject _highlightPrefab;

    [SerializeField]
    private UIManager _uiManager;

    private Dictionary<Player, Disc> _discPrefabs = new();

    private GameState _gameState = new();

    private Disc[,] _discs = new Disc[8, 8];

    private List<GameObject> _highlights = new();

    private void Start()
    {
        _discPrefabs[Player.Black] = _discBlackUp;
        _discPrefabs[Player.White] = _discWhiteUp;

        AddStartDiscs();
        ShowLegalMoves();
        _uiManager.SetPlayerText(_gameState.CurrentPlayer);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                Vector3 impact = hitInfo.point;
                Position boardPosition = SceneToBoardPosition(impact);
                OnBoardClicked(boardPosition);
            }
        }
    }

    private void ShowLegalMoves()
    {
        foreach (var boardPosition in _gameState.LegalMoves.Keys)
        {
            Vector3 scenePosition = BoardToScenePosition(boardPosition) + Vector3.up * 0.01f;
            var highlight = Instantiate(_highlightPrefab, scenePosition, Quaternion.identity);
            _highlights.Add(highlight);
        }
    }

    private void HideLegalMoves()
    {
        _highlights.ForEach(Destroy);
        _highlights.Clear();
    }

    private void OnBoardClicked(Position boardPosition)
    {
        if (_gameState.MakeMove(boardPosition, out MoveInfo moveInfo))
        {
            StartCoroutine(OnMoveMade(moveInfo));
        }
    }

    private IEnumerator OnMoveMade(MoveInfo moveInfo)
    {
        HideLegalMoves();
        yield return ShowMove(moveInfo);
        yield return ShowTurnOutcome(moveInfo);
        ShowLegalMoves();
    }

    private Position SceneToBoardPosition(Vector3 scenePosition)
    {
        int column = (int)(scenePosition.x - 0.25f);
        int row = 7 - (int)(scenePosition.z - 0.25f);
        return new Position(row, column);
    }

    private Vector3 BoardToScenePosition(Position boardPosition)
    {
        return new Vector3(boardPosition.Column + 0.75f, 0, 7 - boardPosition.Row + 0.75f);
    }

    private void SpawnDisc(Disc prefab, Position boardPosition)
    {
        Vector3 scenePosition = BoardToScenePosition(boardPosition) + Vector3.up * 0.1f;
        _discs[boardPosition.Row, boardPosition.Column] = Instantiate(prefab, scenePosition, Quaternion.identity);
    }

    private void AddStartDiscs()
    {
        foreach (var boardPosition in _gameState.OccupiedPositions())
        {
            Player player = _gameState.Board[boardPosition.Row, boardPosition.Column];
            SpawnDisc(_discPrefabs[player], boardPosition);
        }
    }

    private void FlipDiscs(List<Position> positions)
    {
        foreach (var position in positions)
        {
            _discs[position.Row, position.Column].Flip();
        }
    }

    private IEnumerator ShowMove(MoveInfo moveInfo)
    {
        SpawnDisc(_discPrefabs[moveInfo.Player], moveInfo.Position);
        yield return new WaitForSeconds(0.33f);
        FlipDiscs(moveInfo.Outflanked);
        yield return new WaitForSeconds(0.83f);
    }

    private IEnumerator ShowTurnSkipped(Player skippedPlayer)
    {
        _uiManager.SetSkippedText(skippedPlayer);
        yield return _uiManager.AnimateTopText();
    }

    private IEnumerator ShowGameOver(Player winner)
    {
        _uiManager.SetTopText("Neither Player Can Move");
        yield return _uiManager.AnimateTopText();
        yield return _uiManager.ShowScoreText();
        yield return new WaitForSeconds(0.5f);
        yield return ShowCounting();
        _uiManager.SetWinnerText(winner);
        yield return _uiManager.ShowEndScreen();
    }

    private IEnumerator ShowTurnOutcome(MoveInfo moveInfo)
    {
        if (_gameState.GameOver)
        {
            yield return ShowGameOver(_gameState.Winner);
            yield break;
        }
        Player currentPlayer = _gameState.CurrentPlayer;
        if (currentPlayer == moveInfo.Player)
        {
            yield return ShowTurnSkipped(currentPlayer.Opponent());
        }

        _uiManager.SetPlayerText(currentPlayer);
    }

    private IEnumerator ShowCounting()
    {
        int black = 0;
        int white = 0;

        foreach (var position in _gameState.OccupiedPositions())
        {
            var player = _gameState.Board[position.Row, position.Column];

            if (player == Player.Black)
            {
                black++;
                _uiManager.SetBlackScoreText(black);
            }
            else //if (player == Player.White)
            {
                white++;
                _uiManager.SetWhiteScoreText(white);
            }
            _discs[position.Row, position.Column].Twitch();
            yield return new WaitForSeconds(0.05f);
        }
    }

    private IEnumerator RestartGame()
    {
        yield return _uiManager.HideEndScreen();
        Scene activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.name);
    }

    public void OnPlayAgainClicked()
    {
        StartCoroutine(RestartGame());
    }
}
