[System.Serializable]
public class KeyWord
{
    public KeyWordType keyWordType;
    public int keyWordValue;

    public KeyWord(){}

    public KeyWord(KeyWordType KeyWordType, int KeyWordValue)
    {
        keyWordType = KeyWordType;
        keyWordValue = KeyWordValue;
    }
}
