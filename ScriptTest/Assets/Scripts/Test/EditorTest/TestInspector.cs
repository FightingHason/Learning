using UnityEngine;

public class TestInspector : MonoBehaviour {

    [HideInInspector][SerializeField]
    Rect pRectValue;
    public Rect mRectValue
    {
        get { return pRectValue; }
        set { pRectValue = value; }
    }

    [HideInInspector][SerializeField]
    Texture pTexture;
    public Texture mTexture
    {
        get { return pTexture; }
        set { pTexture = value; }
    }

    [HideInInspector][SerializeField]
    int width;
    public int mWidth
    {
        get { return width; }
        set { width = value; }
    }
}
