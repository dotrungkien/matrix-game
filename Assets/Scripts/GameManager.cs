using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public struct GridState
{
    public string player_id;
    public string player_nick;
    public int point;
    public string game_id;
    public Dictionary<string, int> grid;

    public GridState(string _player_id, string _player_nick, int _point, string _game_id, Dictionary<string, int> _grid)
    {
        player_id = _player_id;
        player_nick = _player_nick;
        point = _point;
        game_id = _game_id;
        grid = _grid;
    }
}

public class GameManager : MonoBehaviour, IListener
{

    public Color[] colors;
    public Tile tilePrefab;
    public Transform tileSpawnPos;

    public GridBase gridPrefab;
    public Transform grid1SpawnPos;
    public Transform grid2SpawnPos;

    public bool isGameOver = false;

    private string myID;
    private Color myColor;
    public Dictionary<string, GridBase> grids = new Dictionary<string, GridBase>();

    private Dictionary<string, string> nickToID;

    void Start()
    {
        nickToID = new Dictionary<string, string>();
        myID = PlayerPrefs.GetString("myID", "");
        EventManager.GetInstance().AddListener(EVENT_TYPE.SCORE_CHANGE, this);
        EventManager.GetInstance().AddListener(EVENT_TYPE.GAMEOVER, this);
        EventManager.GetInstance().AddListener(EVENT_TYPE.NEW_PIECE, this);
    }

    public void UpdateGrid(string player_id, GridState state)
    {
        bool isMe = (player_id == myID);
        bool isWatching = EventManager.GetInstance().isWatching;
        Transform spawnPosition;
        if (isMe) spawnPosition = grid1SpawnPos;
        else
        {
            if (isWatching && grid1SpawnPos.childCount <= grid2SpawnPos.childCount)
            {
                spawnPosition = grid1SpawnPos;
            }
            else
            {
                spawnPosition = grid2SpawnPos;
            }
        }

        GridBase target = null;
        grids.TryGetValue(player_id, out target);
        if (target == null)
        {
            string player_nick = state.player_nick;
            target = Instantiate(gridPrefab, spawnPosition.position, Quaternion.identity, spawnPosition);
            target.gameObject.name = player_nick;
            if (isMe)
            {
                target.transform.tag = Constants.PLACEABLE_TAG;
                myColor = colors[grids.Count];
            }
            SpriteRenderer render = target.GetComponent<SpriteRenderer>();
            render.color = colors[grids.Count];
            grids[player_id] = target;
            nickToID[player_nick] = player_id;
            if (!isMe) ActiveGrid(player_nick);
        }
        if (isWatching) grids[player_id].UpdateStateFirstTime(state);
        else grids[player_id].UpdateState(state);
    }

    public void ActiveGrid(string player_nick)
    {
        string username = PlayerPrefs.GetString("username", "");
        Transform currentGrid = grids[nickToID[player_nick]].transform;
        if (player_nick == username) return;
        foreach (KeyValuePair<string, GridBase> item in grids)
        {
            string gridName = item.Value.name;
            if (gridName == player_nick)
            {
                item.Value.gameObject.SetActive(true);
            }
            else
            {
                Transform otherGrid = item.Value.transform;
                Debug.Log(string.Format("{0} {1} {2}", currentGrid.parent.name, otherGrid.parent.name, Object.ReferenceEquals(currentGrid.parent, otherGrid.parent)));
                if (gridName != username && currentGrid != null && Object.ReferenceEquals(currentGrid.parent, otherGrid.parent))
                {
                    item.Value.gameObject.SetActive(false);
                }
            }
        }
    }

    public void UpdateGridData(string player_id, Dictionary<string, int> gridData, JToken piece)
    {
        grids[player_id].PlacePiece(piece);
        grids[player_id].UpdateData(gridData);
    }

    public void SpawnNewTiles(int[] piece)
    {
        foreach (Transform child in tileSpawnPos.transform)
        {
            if (child != null) GameObject.Destroy(child.gameObject);
        }
        Tile tile = Instantiate(tilePrefab, tileSpawnPos.position, Quaternion.identity, tileSpawnPos);
        tile.transform.tag = Constants.MOVABLE_TAG;
        tile.GetComponent<SpriteRenderer>().color = myColor;
        tile.SetVal(piece);
    }

    public void GameOver()
    {
        isGameOver = true;
    }

    public void Restart()
    {
        SceneManager.LoadScene("Game");
    }

    public void OnEvent(EVENT_TYPE eventType, Component sender, object param = null)
    {
        switch (eventType)
        {
            case EVENT_TYPE.GAMEOVER:
                GameOver();
                break;
            case EVENT_TYPE.PLAYER_LEFT:
                GameOver();
                break;
            case EVENT_TYPE.RESTART:
                Restart();
                break;
            case EVENT_TYPE.NEW_PIECE:
                int[] pieceVal = (int[])param;
                bool isWatching = EventManager.GetInstance().isWatching;
                if (!isWatching) SpawnNewTiles(pieceVal);
                break;
            default:
                break;
        }
    }
}
