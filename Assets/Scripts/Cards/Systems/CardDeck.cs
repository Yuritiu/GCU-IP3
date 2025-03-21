using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class CardDeck : MonoBehaviour
{
    //!- Coded By Charlie -!
    public static CardDeck Instance;

    [System.Serializable]
    public class CardPrefab
    {
        [SerializeField] public GameObject cardPrefab;
        [SerializeField] public int quantity;
    }

    [Header("All Card Prefabs")]
    [SerializeField] List<CardPrefab> cardPrefabs;
    [SerializeField] public List<GameObject> deck = new List<GameObject>();

    [Header("Playing Card Pile")]
    [SerializeField] GameObject emptyCardPrefab;
    [SerializeField] Transform deckPosition;
    //Distance Between Cards
    [SerializeField] float cardStackOffset = 0.002f;
    [SerializeField] public float currentDeckStackHeight;

    [Header("Card Count Display")]
    private TextMeshProUGUI cardCount;
    [SerializeField] Camera playerCamera;

    private List<GameObject> visualDeck = new List<GameObject>();
    bool reshuffling;

    [Header("Deck Fan Settings")]
    [SerializeField] Transform fanStartPosition;
    [SerializeField] float fanDuration = 1f;
    [SerializeField] float returnDuration = 1f;
    [SerializeField] float pauseDuration = 5f;
    bool fannedDeck = false;
    bool calledStartDraw = false;
    [HideInInspector] public bool fanAnimationComplete = false;

    public bool animateIntro = false;
    public bool startIntro = false;
    bool introCalled = false;

    void Start()
    {
        GameObject cardCountObject = GameObject.Find("CardCount");

        if (cardCountObject != null)
        {
            cardCount = cardCountObject.GetComponent<TextMeshProUGUI>();
        }
        else
        {
            Debug.LogWarning("CardCount Display Not Found");
        }
    }

    void Awake()
    {
        Instance = this;

        if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }


    }

    public void CallIntro()
    {
        startIntro = true;
        InitializeDeck();
        introCalled = true;
    }

    void Update()
    {
        if (startIntro && !introCalled)
        {
            introCalled = true;
            CallIntro();
        }

        if (deck.Count == 0 && !GameManager.Instance.showddown)
        {
            GameManager.Instance.Showdown();
        }

        if (cardCount != null && playerCamera != null)
        {
            cardCount.transform.LookAt(playerCamera.transform);
            cardCount.transform.Rotate(0, 180, 0);
        }

        if(fannedDeck && !calledStartDraw)
        {
            calledStartDraw = true;
            DrawCard();
        }
    }

    void InitializeDeck()
    {
        if (deck.Count == 0 && reshuffling == true)
        {
            GameManager.Instance.Showdown();
        }

        deck.Clear();

        //Calculate The Total Height Before Stacking
        float totalHeight = (cardPrefabs.Sum(card => card.quantity) - 1) * cardStackOffset;

        foreach (CardPrefab card in cardPrefabs)
        {
            for (int i = 0; i < card.quantity; i++)
            {
                if (card != null)
                {
                    //For Each Card In The Deck Instantiate A Visual Card Prefab At The Proper Position
                    Vector3 playingDeckStackPosition = fanStartPosition.position + new Vector3(90, totalHeight - currentDeckStackHeight, 0);
                    GameObject emptyCard = Instantiate(emptyCardPrefab, playingDeckStackPosition, Quaternion.identity);

                    emptyCard.transform.localPosition = new Vector3(fanStartPosition.position.x, playingDeckStackPosition.y, fanStartPosition.position.z);
                    emptyCard.transform.localRotation = Quaternion.Euler(90, 0, 0);

                    // Add to the list of visual cards
                    visualDeck.Add(emptyCard);

                    // Update the stack height
                    currentDeckStackHeight += cardStackOffset;

                    deck.Add(card.cardPrefab);

                    if (fannedDeck)
                    {
                        UpdateCardCount();
                    }
                }
            }
        }

        ShuffleDeck();
        Debug.Log("Deck Created With " + deck.Count + " Cards.");

        if (!fannedDeck)
        {
            FanCardDeck();
        }
        else
        {
            UpdateCardCount();
        }
    }

    void ShuffleDeck()
    {
        //Debug.Log("Deck Shuffling...");
        for (int i = 0; i < deck.Count; i++)
        {
            int randomIndex = Random.Range(0, deck.Count);
            GameObject temp = deck[i];
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
        //Debug.Log("Deck Shuffled.");
    }

    void ReshuffleDeck()
    {
        InitializeDeck();
        //Debug.Log("Deck Reshuffled. New deck count: " + deck.Count);
        //Might Be A Problem Just Adding A Card
        DrawCard();
    }

    public GameObject DrawCard()
    {
        if (deck.Count == 0)
        {
            ReshuffleDeck();
        }

        //Get First Card From The Deck
        GameObject card = deck[0];

        if (card == null)
        {
            //Debug.LogError("Drawn Card Is Null.");
            return null;
        }

        //Remove The Drawn Card From The Deck
        deck.RemoveAt(0);
        RemoveCards(1);

        if (fannedDeck)
        {
            UpdateCardCount();
        }

        return card;
    }

    void RemoveCards(int numberOfCardsToRemove)
    {
        //Ensure The Number To Remove Doesn't Exceed The Deck Size
        int cardsToRemove = Mathf.Min(numberOfCardsToRemove, deck.Count);

        for (int i = 0; i < cardsToRemove; i++)
        {
            //Get The Top Visual Card Prefab From Playing Deck
            GameObject visualCard = visualDeck[visualDeck.Count - 1];

            Destroy(visualCard);

            currentDeckStackHeight -= cardStackOffset;

            //Remove The Destroyed Card From The List Of Visual Cards
            visualDeck.RemoveAt(visualDeck.Count - 1);
        }
    }

    void UpdateCardCount()
    {
        if (cardCount != null)
        {
            cardCount.text = visualDeck.Count.ToString();
        }
    }

    //CALLED AT START OF GAME TO ANIMATE CARDS
    void FanCardDeck()
    {
        StartCoroutine(AnimateFanIn());
    }

    IEnumerator AnimateFanIn()
    {
        //Stack From Bottom Instead Of Top Of Deck
        float baseY = deckPosition.position.y;

        List<Coroutine> currentCardMoveCoroutines = new List<Coroutine>();

        for (int i = 0; i < visualDeck.Count; i++)
        {
            //Card Slide
            Vector3 slidePosition = new Vector3(deckPosition.position.x, baseY, deckPosition.position.z);
            Quaternion slideRotation = Quaternion.Euler(90, 0, 0);

            //Card Stack
            Vector3 stackPosition = new Vector3(deckPosition.position.x, deckPosition.position.y + (i * cardStackOffset), deckPosition.position.z);
            Quaternion stackRotation = Quaternion.Euler(90, 0, 0);

            currentCardMoveCoroutines.Add(StartCoroutine(MoveCard(visualDeck[i], slidePosition, stackPosition, slideRotation, stackRotation, fanDuration)));

            yield return new WaitForSeconds(0.05f);

            if (cardCount != null)
            {
                cardCount.text = i.ToString();
            }
        }

        foreach (Coroutine coroutine in currentCardMoveCoroutines)
        {
            yield return coroutine;
        }

        fannedDeck = true;
        fanAnimationComplete = true;
    }

    IEnumerator MoveCard(GameObject card, Vector3 slideTarget, Vector3 stackTarget, Quaternion targetSlideRotation, Quaternion targetStackRotation, float duration)
    {
        Vector3 startPosition = card.transform.position;
        Quaternion startRotation = card.transform.rotation;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float progress = elapsedTime / duration;

            card.transform.position = Vector3.Lerp(startPosition, slideTarget, progress);
            card.transform.rotation = Quaternion.Lerp(startRotation, targetSlideRotation, progress);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        card.transform.position = slideTarget;
        elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float progress = elapsedTime / duration;

            card.transform.position = Vector3.Lerp(slideTarget, stackTarget, progress);
            card.transform.rotation = Quaternion.Lerp(startRotation, targetSlideRotation, progress);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        card.transform.position = stackTarget;
    }
}
