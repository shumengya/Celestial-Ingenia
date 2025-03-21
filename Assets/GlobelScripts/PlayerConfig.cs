using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerConfig : MonoBehaviour
{
    // ����ʵ��
    public static PlayerConfig Instance;

    public string playerName = "";
    public int woodNum = 0;
    public int stoneNum = 0;
    public int ironNum = 0;
    public int copperNum = 0;

    public Text woodText;
    public Text stoneText;
    public Text ironText;
    public Text copperText;

    private float timer = 0f;
    private const float updateInterval = 0.5f;

    void Awake()
    {
        // ��鵥��ʵ���Ƿ��Ѿ�����
        if (Instance == null)
        {
            
            // ��������ڣ�����ǰʵ����Ϊ����ʵ��
            Instance = this;
            // ȷ���ö����ڳ����л�ʱ���ᱻ����
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // ����Ѿ����ڣ����ٵ�ǰ����
            Destroy(gameObject);
        }

        // ע�᳡��������ɵ��¼�
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // �ڳ���������ɺ����»�ȡ�ı��������
        FindTextComponents();
    }

    void FindTextComponents()
    {
        // ������Щ Text ����ڳ����п���ͨ�������ҵ�
        woodText = GameObject.Find("Wood_Text")?.GetComponent<Text>();
        stoneText = GameObject.Find("Stone_Text")?.GetComponent<Text>();
        ironText = GameObject.Find("Iron_Ore_Text")?.GetComponent<Text>();
        copperText = GameObject.Find("Copper_Ore_Text")?.GetComponent<Text>();
    }

    void Start()
    {

        // �״μ��س���ʱ��ȡ�ı��������
        FindTextComponents();
    }

    void Update()
    {
        // �ۼӼ�ʱ��
        timer += Time.deltaTime;

        // ����ʱ���ﵽ���¼��ʱ
        if (timer >= updateInterval)
        {
            // �����ı���ʾ
            if (woodText != null) woodText.text = "ľ��: " + woodNum.ToString();
            if (stoneText != null) stoneText.text = "ʯͷ: " + stoneNum.ToString();
            if (ironText != null) ironText.text = "����: " + ironNum.ToString();
            if (copperText != null) copperText.text = "ͭ��: " + copperNum.ToString();

            // ���ü�ʱ��
            timer = 0f;
        }
    }
}