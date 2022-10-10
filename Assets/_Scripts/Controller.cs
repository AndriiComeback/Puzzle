using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {
    private static Controller m_instance;
    [SerializeField] private Color[] m_tokenColors;
    public Color[] TokenColors {
        get { return m_tokenColors; }
        set { m_tokenColors = value; }
    }
    private List<List<Token>> m_tokensByTypes;
    public List<List<Token>> TokensByTypes {
        set { m_tokensByTypes = value; }
        get { return m_tokensByTypes; }
    }
    private Field m_field;
    public Field Field {
        set { m_field = value; }
        get { return m_field; }
    }
    private LevelParameters m_level;
    public LevelParameters Level {
        set { m_level = value; }
        get { return m_level; }
    }
    private int m_currentLevel;
    public int CurrentLevel {
        set { m_currentLevel = value; }
        get { return m_currentLevel; }
    }
    [SerializeField] private Score m_score;
    public Score Score {
        set { m_score = value; }
        get { return m_score; }
    }
    [SerializeField] private Audio m_audio = new Audio();
    public Audio Audio {
        set { m_audio = value; }
        get { return m_audio; }
    }

    public static Controller Instance {
        get {
            if (m_instance == null) {
                var controller =
                Instantiate(Resources.Load("Prefabs/Controller")) as GameObject;
                m_instance = controller.GetComponent<Controller>();
            }
            return m_instance;
        }
    }
    private void Awake() {
        if (m_instance == null) {
            m_instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            if (m_instance != this)
                Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        MakeColors(3);
        Audio.SourceMusic = gameObject.AddComponent<AudioSource>();
        Audio.SourceRandomPitchSFX = gameObject.AddComponent<AudioSource>();
        Audio.SourceSFX = gameObject.AddComponent<AudioSource>();
    }
    private void Start() {
        InitializeLevel();
        TurnCountHelper.Count();
        Audio.PlayMusic(true);
    }
    private Color[] MakeColors(int count) {
        Color[] result = new Color[count];
        float colorStep = 1f / (count + 1);
        float hue = 0f;
        float saturation = 0.5f;
        float value = 1f;
        for (int i = 0; i < count; i++) {
            float newHue = hue + (colorStep * i);
            result[i] = Color.HSVToRGB(newHue, saturation, value);
        }
        return result;
    }
    public void TurnDone() {
        Audio.PlaySound("Drop");
        if (IsAllTokensConnected()) {
            //Debug.Log("Win!");
            Audio.PlaySound("Victory");
            Score.AddLevelBonus();
            m_currentLevel++;
            Destroy(m_field.gameObject);
            InitializeLevel();
        } else {
            //Debug.Log("Continue...");
        }

    }
    public bool IsAllTokensConnected() {
        for (var i = 0; i < TokensByTypes.Count; i++) {
            if (IsTokensConnected(TokensByTypes[i]) == false) {
                return false;
            }
        }
        return true;
    }
    private bool IsTokensConnected(List<Token> tokens) {
        if (tokens.Count == 0) {
            return true;
        }
        List<Token> connectedTokens = new List<Token>();
        connectedTokens.Add(tokens[0]);
        bool moved = true;
        while (moved) {
            moved = false;
            for (int i = 0; i < connectedTokens.Count; i++) {
                for (int j = 0; j < tokens.Count; j++) {
                    if (IsTokensNear(tokens[j], connectedTokens[i])) {
                        if (connectedTokens.Contains(tokens[j]) == false) {
                            connectedTokens.Add(tokens[j]);
                            moved = true;
                        }
                    }
                }
            }
        }
        if (tokens.Count == connectedTokens.Count) {
            return true;
        }
        return false;
    }
    private bool IsTokensNear(Token first, Token second) {
        if ((int)first.transform.position.x == (int)second.transform.position.x + 1 ||
        (int)first.transform.position.x == (int)second.transform.position.x - 1) {
            if ((int)first.transform.position.y == (int)second.transform.position.y) {
                return true;
            }
        }
        if ((int)first.transform.position.y == (int)second.transform.position.y + 1 ||
        (int)first.transform.position.y == (int)second.transform.position.y - 1) {
            if ((int)first.transform.position.x == (int)second.transform.position.x) {
                return true;
            }
        }
        return false;
    }
    public void InitializeLevel() {
        m_level = new LevelParameters(m_currentLevel);
        TokenColors = MakeColors(m_level.TokenTypes);
        TokensByTypes = new List<List<Token>>();
        for (int i = 0; i < m_level.TokenTypes; i++) {
            TokensByTypes.Add(new List<Token>());
        }
        m_field = Field.Create(m_level.FieldSize, m_level.FreeSpace);
    }
}
