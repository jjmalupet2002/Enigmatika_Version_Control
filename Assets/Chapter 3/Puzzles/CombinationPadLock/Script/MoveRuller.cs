using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class MoveRuller : MonoBehaviour
{
    PadLockPassword _lockPassword;
    PadLockEmissionColor _pLockColor;

    [HideInInspector]
    public List<GameObject> _rullers = new List<GameObject>();
    [HideInInspector]
    public int[] _numberArray = { 0, 0, 0, 0 };

    private GameObject _selectedRuller;
    private Vector2 _startTouchPos;
    private bool _isSwiping = false;

    void Awake()
    {
        _lockPassword = FindObjectOfType<PadLockPassword>();
        _pLockColor = FindObjectOfType<PadLockEmissionColor>();

        _rullers.Add(GameObject.Find("Ruller1"));
        _rullers.Add(GameObject.Find("Ruller2"));
        _rullers.Add(GameObject.Find("Ruller3"));
        _rullers.Add(GameObject.Find("Ruller4"));
    }

    void Update()
    {
        HandleTouchInput();
    }

    void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Ray ray = Camera.main.ScreenPointToRay(touch.position);
            RaycastHit hit;

            switch (touch.phase)
            {
                case UnityEngine.TouchPhase.Began:
                    if (Physics.Raycast(ray, out hit))
                    {
                        if (_rullers.Contains(hit.collider.gameObject))
                        {
                            _selectedRuller = hit.collider.gameObject;
                            HighlightRuller(_selectedRuller, true);
                            _startTouchPos = touch.position;
                            _isSwiping = true;
                        }
                    }
                    break;

                case UnityEngine.TouchPhase.Moved:
                    if (_isSwiping && _selectedRuller != null)
                    {
                        float swipeDelta = touch.position.y - _startTouchPos.y;
                        if (Mathf.Abs(swipeDelta) > 30) // Threshold to detect a swipe
                        {
                            RotateSelectedRuller(swipeDelta > 0 ? 1 : -1);
                            _startTouchPos = touch.position;
                        }
                    }
                    break;

                case UnityEngine.TouchPhase.Ended:
                    _isSwiping = false;
                    break;
            }
        }
    }

    void HighlightRuller(GameObject ruller, bool highlight)
    {
        foreach (GameObject r in _rullers)
        {
            r.GetComponent<PadLockEmissionColor>()._isSelect = (r == ruller && highlight);
            r.GetComponent<PadLockEmissionColor>().BlinkingMaterial();
        }
    }

    void RotateSelectedRuller(int direction)
    {
        if (_selectedRuller != null)
        {
            int index = _rullers.IndexOf(_selectedRuller);
            _selectedRuller.transform.Rotate(-direction * 36, 0, 0, Space.Self);
            _numberArray[index] = (_numberArray[index] + direction + 10) % 10;
        }
    }
}
