using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    private string suit;
    private string rank;
    private bool faceUp = false;


    public void SetSuitAndRank(string inSuit, string inRank)
    {
        suit = inSuit;
        rank = inRank;

        // set the graphics for this suit & rank
        string path = "Free_Playing_Cards/PlayingCards_" + rank + suit;
        GetComponent<MeshFilter>().mesh = Resources.Load<Mesh>(path);
        // add collision so we can detect mouse clicks
        gameObject.AddComponent<MeshCollider>();
    }

    public bool Matches(Card otherCard)
    {
        return (rank == otherCard.rank) && (suit == otherCard.suit);
    }

    public void Flip()
    {
        faceUp = !faceUp;
        transform.rotation = Quaternion.LookRotation(-transform.forward, Vector3.up);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform == transform)
                {
                    if (!faceUp)
                    {
                        MemoryGame.instance.Select(this);
                    }
                }
            }
        }
    }
}
