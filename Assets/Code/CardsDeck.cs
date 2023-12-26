using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Random = UnityEngine.Random;
using UnityEngine;
using UnityEngine.UI;

public class CardsDeck : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private TMP_Text winnerLable;
    
    private List<GameCard> _deck = new List<GameCard>();
    private int _deckOrder = 0;
    
    public static CardsDeck Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        winnerLable.gameObject.SetActive(false);
    }

    public void StartGame()
    {
        if (Table.Instance.GetPlayers().Count <= 1) 
        {
            Debug.Log("Player created automatically.");
            Table.Instance.SetPlayer(Table.Instance.CreatePlayer());
        }
        else
        {
            if (Table.Instance.GetMainPlayer() == null)
            {
                int randomSeatIndex = Random.Range(1, Table.Instance.GetPlayers().Count);
                Table.Instance.SetPlayer(Table.Instance.GetPlayers()[randomSeatIndex]);
            }
        }

        if (_deck.Count > 0)
        {
            StartCoroutine(DistributeOpeningCards()); 
            return;
        }
        
        CreateDeck(); 
        ShuffleDeck(); 
        AssignBotLogic(); 

        StartCoroutine(DistributeOpeningCards()); 
    }

    private IEnumerator DistributeOpeningCards()
    {
        Player dealer = Table.Instance.GetDealer();
        winnerLable.gameObject.SetActive(false);

        SelectPlayer(dealer); 
        PlaceCard(_deck[_deckOrder++], dealer, true); 
        yield return new WaitForSeconds(1f); 

        foreach (Player player in Table.Instance.GetPlayers()) 
        {
            if (player != dealer) 
            {
                SelectPlayer(player); 
                PlaceCard(_deck[_deckOrder++], player);  
                yield return new WaitForSeconds(1f); 
            }
        }

        SelectPlayer(dealer); 
        PlaceCard(_deck[_deckOrder++], dealer);
        yield return new WaitForSeconds(1f);

        foreach (Player player in Table.Instance.GetPlayers())
        {
            if (player != dealer)
            {
                SelectPlayer(player);
                PlaceCard(_deck[_deckOrder++], player);  
                yield return new WaitForSeconds(1f);
            }
        }

        StartCoroutine(PlayRound()); 
    }

    private IEnumerator PlayRound()
    {
        foreach (Player player in Table.Instance.GetPlayers()) 
        {
            SelectPlayer(player); 

            if (player == Table.Instance.GetMainPlayer()) 
            {
                yield return StartCoroutine(HandleHumanActions()); 
            } 
            else if (player == Table.Instance.GetDealer()) 
            {
                continue;     
            }
            else 
            {
                player.GetComponent<Bot>().MakeAMove(); 
                yield return new WaitForSeconds(1f); 
            }
        }

        Player dealer = Table.Instance.GetDealer();
        
        SelectPlayer(dealer); 
        ShowDealerHiddenCard(); 
        yield return new WaitForSeconds(1f); 
        Table.Instance.GetDealer().GetComponent<Bot>().MakeAMove();
        yield return new WaitForSeconds(1f); 
        CalculateWinners();
        yield return new WaitForSeconds(5f); 
        ResetGame(); 
    }

    private IEnumerator HandleHumanActions()
    {
        if (PlayerInterfaceManager.Instance == null)
        {
            Debug.LogError("No Player Interface Instance was found!");
            yield break;
        }

        bool decisionMade = false; 
        PlayerInterfaceManager.Instance.OnDecisionMade += () => { decisionMade = true; };

        yield return new WaitUntil(() => decisionMade); 
    }

    private void CalculateWinners()
    {
        Player dealer = Table.Instance.GetDealer();

        foreach (Player player in Table.Instance.GetPlayers())
        {
            Balance balance = player.GetPlayerBalance();
            int betAmount = balance.GetBetAmount();
            
            if (player == dealer) continue;

            if (player.GetOverallValue() > dealer.GetOverallValue() && player.GetOverallValue() <= 21)
            {
                if (player.GetOverallValue() != 21)
                {
                    balance.ModifyBalance(betAmount * 2);
                }
                else
                {
                    balance.ModifyBalance(Mathf.RoundToInt(betAmount * 2.5f));
                }

                balance.SetBetAmount(0);

                if (player == Table.Instance.GetMainPlayer())
                {
                    winnerLable.text = "You won!";
                }
            }
            else if (dealer.GetOverallValue() > 21 && player.GetOverallValue() <= 21)
            {
                if (player.GetOverallValue() != 21)
                {
                    balance.ModifyBalance(betAmount * 2);
                }
                else
                {
                    balance.ModifyBalance(Mathf.RoundToInt(betAmount * 2.5f));
                }

                balance.SetBetAmount(0);

                if (player == Table.Instance.GetMainPlayer())
                {
                    winnerLable.text = "You won!";
                }
            }
            else if (player.GetOverallValue() == dealer.GetOverallValue() && player.GetOverallValue() <= 21)
            {
                balance.ModifyBalance(betAmount);
                balance.SetBetAmount(0);

                if (player == Table.Instance.GetMainPlayer())
                {
                    winnerLable.text = "Draw.";
                }
            }
            else
            {
                balance.SetBetAmount(0);

                if (player == Table.Instance.GetMainPlayer())
                {
                    winnerLable.text = "Dealer won!";
                }
            }
        }
        
        winnerLable.gameObject.SetActive(true);
    }
    
    private void ResetGame()
    {
        _deckOrder = 0; 

        foreach (Player player in Table.Instance.GetPlayers()) 
        {
            player.ClearDeck();    
        }

        BetPanel.Instance.TogglePanel(true);
    }

    private void CreateDeck()
    {
        for (int k = 0; k < 6; k++) 
        {
            foreach (CardSuit suit in Enum.GetValues(typeof(CardSuit))) 
            {
                for (int i = 1; i <= 13; i++) 
                {
                    GameCard card = CreateCard(suit, i); 
                    _deck.Add(card); 
                }
            }
        }

        Debug.Log($"Deck created, expected amount of cards: {6 * 52}, amount of cards in the deck: {_deck.Count}");
    }

    private void ShuffleDeck()
    {
        for (int i = 0; i < _deck.Count; i++) 
        {
            GameCard temporaryCard = _deck[i]; 
            int randomIndex = Random.Range(i, _deck.Count); 
            _deck[i] = _deck[randomIndex]; 
            _deck[randomIndex] = temporaryCard; 
        }  
    }

    private void AssignBotLogic()
    {
        foreach (Player player in Table.Instance.GetPlayers()) 
        {
            if (player == Table.Instance.GetMainPlayer()) 
            {
                continue;
            }

            player.gameObject.AddComponent<Bot>().Initialize(); 

            if (player == Table.Instance.GetDealer()) 
            {
                player.GetComponent<Bot>().SetTargetValue(17); 
            }
        }
    }

    private void PlaceCard(GameCard card, Player owner, bool hidden = false)
    {
        GameObject prefab = Instantiate(cardPrefab, owner.GetPlayerLocalDeckGrid()); 
        prefab.GetComponent<Image>().sprite = card.GetCardImage(); 

        if (hidden == true)
        {       
            prefab.GetComponent<Image>().color = Color.black; 
        }

        Debug.Log($"{card.GetCardName()} was given to {owner}");
        _deck.Remove(card); 
        card.Hide(hidden); 
        owner.AddCard(card); 
    }

    private void SelectPlayer(Player player)
    {
        player.Select(); 
        foreach (Player other in Table.Instance.GetPlayers())
        {
            if (other != player)
            {
                other.Deselect(); 
            }
        }
    }

    private void ShowDealerHiddenCard()
    {
        Player dealer = Table.Instance.GetDealer();
        List<GameCard> temporaryDeck = dealer.GetLocalDeck();
        dealer.ClearDeck();
        foreach (GameCard card in temporaryDeck)
        {
            PlaceCard(card, dealer);   
        }
        dealer.RefreshCards();
    }

    private GameCard CreateCard(CardSuit suit, int value)
    {
        GameCard card = new GameCard(); 
        card.SetCardSuit(suit); 

        if (value >= 1 && value <= 10) 
        {
            card.SetCardValue(value); 
        }
        else if (value > 10)
        {
            card.SetCardValue(10); 
        }

        string cardName = card.GetCardName(); 
        Sprite cardImage = Resources.Load<Sprite>(cardName); 
        card.SetCardImage(cardImage); 

        return card; 
    }

    public void CreatePlayerOnClick()
    {
        Table.Instance.CreatePlayer();
    }

    public void GetCardFromTheDeck(Player player)
    {
        GameCard card = _deck[_deckOrder++];
        PlaceCard(card, player);    
    }
}