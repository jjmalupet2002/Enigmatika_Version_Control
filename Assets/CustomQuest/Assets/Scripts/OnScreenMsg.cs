using System.Collections.Generic;
using UnityEngine;

public class OnScreenMsg : MonoBehaviour
{
    #region Field

    [SerializeField, Header("info")]
    private float lifeTime;

    [SerializeField]
    private string msg;

    [SerializeField]
    private int size;

    [SerializeField]
    private Color color;

    [SerializeField]
    private Vector2 msgPosition;

    [SerializeField]
    private string font;

    [SerializeField, Header("Font")]
    private Font hangyabolyFont;

    private GUIStyle myGuiStyle = new GUIStyle();

    #endregion Field

    #region Properties

    public float LifeTime { get { return lifeTime; } set { lifeTime = value; } }

    public string Msg { get { return msg; } set { msg = value; } }

    public int Size { get { return size; } set { size = value; } }

    public Color Color { get { return color; } set { color = value; } }

    public Vector2 MsgPosition { get { return msgPosition; } set { msgPosition = value; } }

    public string Font { get { return font; } set { font = value; } }

    public Font HangyabolyFont { get { return hangyabolyFont; } set { hangyabolyFont = value; } }

    #endregion Properties

    /// <summary>
    /// Use this for initialization
    /// </summary>
    private void Start()
    {
        // Set the font style if the font is available in the resources
        if (HangyabolyFont != null)
        {
            myGuiStyle.font = HangyabolyFont;
        }
        else if (!string.IsNullOrEmpty(Font))
        {
            Font loadedFont = Resources.Load<Font>(Font);
            if (loadedFont != null)
            {
                myGuiStyle.font = loadedFont;
            }
            else
            {
                UnityEngine.Debug.LogWarning("Font not found: " + Font);
            }
        }
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    private void Update()
    {
        LifeTimeCheck();
    }

    /// <summary>
    /// The OnGui logic
    /// </summary>
    private void OnGUI()
    {
        myGuiStyle.fontSize = size;
        myGuiStyle.normal.textColor = color;
        myGuiStyle.alignment = TextAnchor.MiddleCenter;  // Ensure justification
        GUI.depth = 20;

        // Combine Quest Complete message and msg
        string fullMessage = "Quest Complete\n" + msg;

        GUI.Label(new Rect(MsgPosition.x, MsgPosition.y, 200f, 200f), fullMessage, myGuiStyle);
    }

    /*** Private Methods ***/

    /// <summary>
    /// Checks the lifeTime of the OnScreenMsgs and removes it if its under 0.
    /// </summary>
    private void LifeTimeCheck()
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
        {
            List<OnScreenMsg> msgsHolder = GetComponentInParent<OnScreenMsgHandler>().Msgs;
            msgsHolder.Remove(this);
            Destroy(this.gameObject);
        }
    }
}
