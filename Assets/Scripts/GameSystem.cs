using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSystem : MonoBehaviour
{
    public Sprite[] sprites;
    public Text playerCoinText, aiCoinText, betCoinText;
    public GameObject panel;
    Place place = new Place();
    Turn turn = new Turn();
    Card[] cards = new Card[5];
    List<GameObject> playerCards = new List<GameObject>(); // プレイヤの手札
    List<GameObject> aiCards = new List<GameObject>(); // AIの手札
    private GameObject playerFieldCard; //プレイヤの場札
    private GameObject aiFieldCard; //AIの場札
    List<GameObject> playerDiscardedCards = new List<GameObject>(); // プレイヤの捨て札
    List<GameObject> aiDiscardedCards = new List<GameObject>(); // AIの捨て札

    private int placeCount = 0;
    private int playerCoin, aiCoin, betCoin;

    // Start is called before the first frame update
    void Start()
    {
        cards[0] = new Card("slave", 0);
        cards[1] = new Card("commoner", 1);
        cards[2] = new Card("knight", 2);
        cards[3] = new Card("noble", 3);
        cards[4] = new Card("king", 4);

        panel.SetActive(false);
        initGame();
    }

    // Update is called once per frame
    void Update()
    {
        if (turn.currentState() == "set_coin")
        {
            betCoin = Random.Range(1, 100);
            betCoinText.text = "×" + betCoin;
            turn.proceed();
        }
    }

    public void screenClicked()
    {
        if (turn.currentState() == "open")
        {
            open(playerFieldCard);
            open(aiFieldCard);
            int playerPower = playerFieldCard.GetComponent<CardController>().card.id;
            int aiPower = aiFieldCard.GetComponent<CardController>().card.id;
            if (playerPower == 4 && aiPower == 0)   //勝敗の判定
            {
                aiCoin += betCoin;
            }
            else if (playerPower == 0 && aiPower == 4)
            {
                playerCoin += betCoin;
            }
            else if (playerPower > aiPower)
            {
                playerCoin += betCoin;
            }
            else if (playerPower < aiPower)
            {
                aiCoin += betCoin;
            }
            playerCoinText.text = "×" + playerCoin;
            aiCoinText.text = "×" + aiCoin;
            turn.proceed();
        }
        else if (turn.currentState() == "discard")
        {
            if (playerCards.Count > 0)
            {
                discard();
            }
            turn.proceed();
        }
    }

    public void cardClicked(GameObject cardObject)
    {
        switch (turn.currentState())
        {
            case "player_select":
                playerSelect(cardObject);
                Debug.Log(turn.currentState());
                turn.proceed();
                break;
            case "ai_select":
                aiSelect(cardObject);
                Debug.Log(turn.currentState());
                turn.proceed();
                break;
            default:
                Debug.Log(turn.currentState());
                break;
        }
        //Debug.Log(cardObject.name);
    }

    public void fakeClicked()
    {
        panel.SetActive(true);
    }

    void initGame()
    {
        for (int i = 0; i < 5; i++)
        {
            // 場にプレイヤのカードを出す
            GameObject playerCard = summonCard(cards[i].name, cards[i], place.playerPlace[i]);
            playerCards.Add(playerCard);

            // 場にAIのカードを出す
            GameObject aiCard = summonCard("back", cards[i], place.aiPlace[i]);
            aiCards.Add(aiCard);
        }
    }

    void playerSelect(GameObject cardObject)
    {
        CardController cardScript = cardObject.GetComponent<CardController>();
        int cardId = cardScript.card.id;

        playerFieldCard = summonCard("back", cardScript.card, place.playerFieldPlace);//場に出す
        removeCard(playerCards, cardId);     // 手札から除く
        Destroy(cardObject);
        if (playerCards.Count % 2 == 1)
        {
            placeCount++;
        }
        for (int i = 0; i < playerCards.Count; i++)
        {
            playerCards[i].transform.position = place.playerPlace[i + placeCount];
        }
    }

    void aiSelect(GameObject cardObject)
    {
        CardController cardScript = cardObject.GetComponent<CardController>();
        int cardId = cardScript.card.id;

        aiFieldCard = summonCard("back", cardScript.card, place.aiFieldPlace);//場に出す
        removeCard(aiCards, cardId);     // 手札から除く
        Destroy(cardObject);
        for (int i = 0; i < aiCards.Count; i++)
        {
            aiCards[i].transform.position = place.aiPlace[i + placeCount];
        }
    }

    void open(GameObject cardObject)    //場のカードを表にする
    {
        SpriteRenderer renderer = cardObject.GetComponent<SpriteRenderer>();
        CardController cardScript = cardObject.GetComponent<CardController>();
        int cardId = cardScript.card.id;
        renderer.sprite = sprites[cardId];
    }

    void discard()
    {
        playerDiscardedCards.Add(playerFieldCard);  //プレイヤの捨て札に加える
        aiDiscardedCards.Add(aiFieldCard);          //AIの捨て札に加える
        int i = playerDiscardedCards.Count - 1;
        playerDiscardedCards[i].transform.position = place.playerDiscardPlace[i];
        playerDiscardedCards[i].transform.localScale = new Vector3(0.5f, 0.5f, 1);
        aiDiscardedCards[i].transform.position = place.aiDiscardPlace[i];
        aiDiscardedCards[i].transform.localScale = new Vector3(0.5f, 0.5f, 1);
    }

    void removeCard(List<GameObject> cards, int cardId)
    {
        foreach (var card in cards)
        {
            CardController cardScript = card.GetComponent<CardController>();
            if (cardScript.card.id == cardId)
            {
                cards.Remove(card);
                break;
            }
        }
    }

    void printCards(List<Card> cards, string label)
    {
        string printString = "";
        foreach (Card card in cards)
        {
            printString += card.name + ":" + card.id + " ";
        }
        Debug.Log(label + "[ " + printString + "]");
    }

    GameObject summonCard(string prefabName, Card card, Vector3 place)
    {
        GameObject cardPrefab = (GameObject)Resources.Load("Prefabs/" + prefabName);
        GameObject summonedCard = Instantiate(cardPrefab, place, Quaternion.identity);
        CardController cardScript = summonedCard.GetComponent<CardController>();
        cardScript.card = card;
        return summonedCard;
    }
}

public class Card
{
    public string name;
    public int id;

    public Card(string cardName, int cardId)
    {
        name = cardName;
        id = cardId;
    }
}

public class Place
{
    public Vector3[] playerPlace = new Vector3[5]
    {
        new Vector3(-4.0f, -3.4f, 0),
        new Vector3(-2.0f, -3.4f, 0),
        new Vector3(-0.0f, -3.4f, 0),
        new Vector3(2.0f, -3.4f, 0),
        new Vector3(4.0f, -3.4f, 0)
    };

    public Vector3[] aiPlace = new Vector3[5]
    {
        new Vector3(-4.0f, 3.4f, 0),
        new Vector3(-2.0f, 3.4f, 0),
        new Vector3(-0.0f, 3.4f, 0),
        new Vector3(2.0f, 3.4f, 0),
        new Vector3(4.0f, 3.4f, 0)
    };

    public Vector3 playerFieldPlace = new Vector3(-1.7f, 0, 0);
    public Vector3 aiFieldPlace = new Vector3(1.7f, 0, 0);

    public Vector3[] playerDiscardPlace = new Vector3[4]
    {
        new Vector3(-8.0f, -1.5f, 0),
        new Vector3(-7.2f, -1.5f, 0),
        new Vector3(-6.4f, -1.5f, 0),
        new Vector3(-5.6f, -1.5f, 0)
    };

    public Vector3[] aiDiscardPlace = new Vector3[4]
    {
        new Vector3(-8.0f, 1.5f, 0),
        new Vector3(-7.2f, 1.5f, 0),
        new Vector3(-6.4f, 1.5f, 0),
        new Vector3(-5.6f, 1.5f, 0)
    };
}

public class Turn
{
    public int current_state = 0;
    public string[] states = new string[5]
    {
        "set_coin",
        "player_select",
        "ai_select",
        "open",
        "discard"
    };

    public string currentState()
    {
        return states[current_state];
    }

    public string proceed()
    {
        current_state = (current_state + 1) % 5;
        //Debug.Log("proceed: " + currentState());
        return states[current_state];
    }
}