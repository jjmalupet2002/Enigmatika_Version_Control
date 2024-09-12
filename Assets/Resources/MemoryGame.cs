// Save point 2 in MemoryGame.cs

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryGame : MonoBehaviour
{
    // Suit and Rank names must match the Free_Playing_Cards naming convention
    string[] kCardSuits = new string[] { "Club", "Diamond", "Spades", "Heart" };
    string[] kCardRanks = new string[] { "2", "3", "4", "5", "6", "7", "8",
                                         "9", "10", "J", "Q", "K", "A" };

    //Thereis only one Memory game at a time
    static public MemoryGame instance;

    //These are local state
    private Card[] cards;
    private Card selectOne;
    private Card selectTwo;
    private double selectTime;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        // Get all cards on GameBoard
        cards = transform.GetComponentsInChildren<Card>();

        // Deal random cards, in pairs
        int n = 0;
        Shuffle(cards);
        for (int m = 0; m < cards.Length / 2; m++)
        {
            // choose a random suit & rank
            string suit = GetRandomFromArray(kCardSuits);
            string rank = GetRandomFromArray(kCardRanks);
            // assign it to two cards
            cards[n++].SetSuitAndRank(suit, rank);
            cards[n++].SetSuitAndRank(suit, rank);
        }
    }

    private void Shuffle<T>(T[] array)
    {
        int n = array.Length;
        while (n > 1)
        {
            int k = (int)Mathf.Floor(Random.value * (n--));
            T temp = array[n];
            array[n] = array[k];
            array[k] = temp;
        }
    }

    private T GetRandomFromArray<T>(T[] array)
    {
        return array[(int)Mathf.Floor(Random.value * array.Length)];
    }

    private bool isSelecting = false; // Add this line

    public void Select(Card card)
    {
        // Prevent further selections if already processing a match
        if (isSelecting) return;

        // If we don't already have two selected cards
        if (selectOne == null)
        {
            selectOne = card;
        }
        else if (selectTwo == null) // Ensure selectTwo is only set if it's null
        {
            selectTwo = card;
            selectTime = Time.time;
            isSelecting = true; // Set the flag to true
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Check for match or mismatch
        if (selectTwo != null && isSelecting) // Only check if we are in selecting state
        {
            // Wait one second so user can see card
            if (Time.time > selectTime + 1.0)
            {
                CheckMatch();
            }
        }
    }

    private void CheckMatch()
    {
        if (selectOne.Matches(selectTwo))
        {
            // Remove cards from the board
            selectOne.Hide();
            selectTwo.Hide();
        }
        else
        {
            // Return cards to face down
            selectOne.Flip();
            selectTwo.Flip();
        }

        // Reset the selection state
        selectOne = null;
        selectTwo = null;
        isSelecting = false; // Reset the flag
    }
}




