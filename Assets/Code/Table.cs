using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour
{
    [SerializeField] private GameObject playersGrid;
    [SerializeField] private Player playerPrefab;
    [SerializeField] private Player dealer;

    private List<Player> _players = new List<Player>();
    private Player _mainPlayer;

    public static Table Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        SetPlayer(CreatePlayer());
        AddPlayerToTheTable(dealer); // adding the dealer as the first player as soon as the scene launched
    }

    private void AddPlayerToTheTable(Player player)
    {
        _players.Add(player);
    }

    private void RemovePlayerFromTheTable(Player player)
    {
        if (_players.Contains(player) == false)
        {
            Debug.LogError($"{player.name} does not exists in the players list!");
            return;
        }

        _players.Remove(player);
    }

    public Player CreatePlayer()
    {
        Player newPlayer = Instantiate(playerPrefab, playersGrid.transform); // instantiate a player assign it to the players grid
        AddPlayerToTheTable(newPlayer); // add the created player to the table

        return newPlayer;
    }

    public void SetPlayer(Player user)
    {
        Sprite icon = Resources.Load<Sprite>("ZPlayerIcon");
        _mainPlayer = user;
        _mainPlayer.SetupIcon(icon, Color.white);
    }

    public Player GetMainPlayer()
    {
        return _mainPlayer;
    }

    public List<Player> GetPlayers()
    {
        return _players;
    }

    public Player GetDealer()
    {
        return dealer;
    }
}