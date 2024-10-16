using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using System;
using System.Reflection;
using System.Net.NetworkInformation;
using DG.Tweening;
//using UnityEditor.IMGUI.Controls;

public class NumberGenerator : MonoBehaviour
{
    public GameObject extraBallObj;
    public GameObject bonusMainObj;
    public SlotController slotController;
    public TextMeshProUGUI autoSpinRemainingPlayText;
    public TextMeshProUGUI extraBallCountText;
    public List<int> random = new List<int>();
    public List<int> generatedNO = new List<int>();
    public CardClass[] cardClasses = new CardClass[4];
    //public Sprite[] ballSprites;
    //public GameObject[] displayNo;
    //public GameObject[] extraBalls;
    public List<Patterns> patternList;
    public Material matchedMat;
    public Material unMatchedMat;
    private int totalActiveCard = 4;
    private int totalNumInEachCard = 15;
    public int autoSpinCount = 0;
    private int totalExtraBallCount = 0;

    public static bool isExtraBallDone = false;

    private PaylineManager paylineManager;
    private BallManager ballManager;
    float startingGameTime;
    DateTime startingGameDate;
    private float secondsOfARealDay = 24 * 60 * 60;
    public double elapsedRealTime;

    private void OnEnable()
    {
        EventManager.OnPlay += StartGame;
        EventManager.OnAutoSpinStart += StartAutoSpin;
        EventManager.OnBallDisplay += CheckSelectedNumb;
        //EventManager.OnExtraBallCompletion += ShowMissingImg;
        EventManager.OnBonusOver += NextPlay;
    }

    private void OnDisable()
    {
        EventManager.OnPlay -= StartGame;
        EventManager.OnAutoSpinStart -= StartAutoSpin;
        EventManager.OnBallDisplay -= CheckSelectedNumb;
        //EventManager.OnExtraBallCompletion -= ShowMissingImg;
        EventManager.OnBonusOver -= NextPlay;
    }

    // Start is called before the first frame update
    void Start()
    {
        paylineManager = new PaylineManager(this);
        ballManager = new BallManager();
        extraBallObj.SetActive(false);
        autoSpinRemainingPlayText.text = "";
    }


    public void NextPlay()
    {
        //Debug.Log(getGameTime(3600).Second.ToString());
        //Debug.Log(autoSpinCount);
        EventManager.isPlayOver = true;

        if (autoSpinCount > 0)
        {
        	if (getGameTime(3600).Second < 30)
                Invoke(nameof(CheckRemainingPlay), 30 - getGameTime(3600).Second);
            else
                Invoke(nameof(CheckRemainingPlay), 0);

        }
    }

    void CheckRemainingPlay()
    {
        autoSpinCount -= 1;
        StartAutoSpin(autoSpinCount);
        Debug.Log(autoSpinCount);
    }

    private void StartAutoSpin(int count)
    {
        autoSpinCount = count;
        isBonusSelected = false;
        if (autoSpinCount == 0)
        {
            EventManager.isAutoSpinStart = false;
            EventManager.AutoSpinOver(true);
            autoSpinRemainingPlayText.text = "";

            return;
        }
        else
        {
            if (autoSpinCount - 1 != 0)
                autoSpinRemainingPlayText.text = (autoSpinCount - 1).ToString();
            else
                autoSpinRemainingPlayText.text = "";
        }
        ResetNumb();
        count--;
        EventManager.Play();
        startingGameTime = Time.time;
        startingGameDate = DateTime.Now;
        //StartGame();
        //Invoke(nameof(StartGame), 2);
    }

    private void StartGame()
    {
        EventManager.isPlayOver = false;

        RandomNumberGenerator();
        for (int i = 0; i < totalActiveCard; i++)
        {
            for (int a = 0; a < totalNumInEachCard; a++)
            {
                cardClasses[i].num_text[a].text = cardClasses[i].numb[a].ToString();
            }
        }

        generatedNO = Numbgen(generatedNO, 30);
        EventManager.GenerateBall(generatedNO);
        //StartCoroutine(DisplayNo(displayNo));
    }

    void RandomNumberGenerator()
    {
        int i_no = 0;
        int i = 0;
        while (random.Count != 60)
        {
            i_no = UnityEngine.Random.Range(1, 61);

            if (!random.Contains(i_no))
            {
                random.Add(i_no);

                if (i < totalActiveCard)
                {
                    if (cardClasses[i].numb.Count == totalNumInEachCard)
                    {
                        i++;
                    }
                    cardClasses[i].numb.Add(i_no);
                    cardClasses[i].numb.Sort();

                }
            }

        }
    }

    public void ShowExtraBallOnTap(int extraballCount)
    {
        Debug.Log("ShowExtraBallOnTap");
        //if (isExtraBallDone) { return; }
        totalExtraBallCount = extraballCount;
        
        extraBallCountText.text = extraballCount.ToString();
        EventManager.TapForExtraBall(true);
        
        //isExtraBallDone = true;
    }

    public void TapOnExtraBall()
    {
        generatedNO = Numbgen(generatedNO, generatedNO.Count + 1);
        if (totalExtraBallCount > 0)
        {
            if (totalExtraBallCount == 1)
            {
                EventManager.TapForExtraBall(false);
            }
            EventManager.GenerateExtraBall(generatedNO, true);
        }
       
        totalExtraBallCount--;
        slotController.extraBallCount--;
        extraBallCountText.text = totalExtraBallCount.ToString();
    }

    public void ShowExtraBalls(int extraballCount)
    {
        if (isExtraBallDone) { return; }
        //extraBalls[0].transform.parent.gameObject.SetActive(true);
        //StartCoroutine(DisplayNo(extraBalls));

        //generatedNO = Numbgen(generatedNO, 35);
        
        generatedNO = Numbgen(generatedNO, 30+ extraballCount);
        extraBallCountText.text = extraballCount.ToString();
        EventManager.GenerateExtraBall(generatedNO, isMissingPattern);

        //isExtraBallDone = true;
    }

    //IEnumerator GenerateBalls(GameObject[] _balls) {

    //    generatedNO = Numbgen(generatedNO, 30);
    //    for (int i = 0; i < _balls.Length; i++)
    //    {
    //        Debug.Log("aaaaaaaaaaaaaaaaaaaaaaaaa");

    //        _balls[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = generatedNO[i].ToString();
    //        _balls[i].GetComponent<UnityEngine.UI.Image>().sprite = ballSprites[UnityEngine.Random.Range(0, ballSprites.Length)];
    //        _balls[i].SetActive(true);
    //        _balls[i].transform.localPosition = new Vector2(0, 70);
    //        yield return new WaitForSeconds(0.2f);
    //        _balls[i].transform.localPosition = new Vector2(0, 350);
    //        yield return new WaitForSeconds(0.2f);
    //        if(i < 7 && )
    //        CheckSelectedNumb(i);

    //    } 
    //}

    //IEnumerator DisplayNo(GameObject[] _balls)
    //{
    //    yield return new WaitForSeconds(1f);
    //    int ballCount = 30;
    //    int startCount = 0;
    //    if(_balls.Length == 5)
    //    {
    //        ballCount = 35;
    //        startCount = 30;
    //    }
    //    generatedNO = Numbgen(generatedNO, ballCount);

    //    for (int i = 0; i < _balls.Length; i++)
    //    {
    //        Debug.Log("aaaaaaaaaaaaaaaaaaaaaaaaa");
    //        _balls[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = generatedNO[startCount+i].ToString();
    //        if (_balls.Length == 30)
    //        {
    //            _balls[i].GetComponent<UnityEngine.UI.Image>().sprite = ballSprites[UnityEngine.Random.Range(0, ballSprites.Length)];
    //        }
    //        _balls[i].SetActive(true);
    //        CheckSelectedNumb(startCount + i);
    //        yield return new WaitForSeconds(0.5f);
    //        for (int j = 0; j < totalActiveCard; j++)
    //        {
    //            if (_balls.Length == 5 && i == _balls.Length - 1)
    //                ShowMissingImg(j);
    //        }
    //    }
    //}

    public static List<int> Numbgen(List<int> numblist, int loopCount)
    {
        int no = 0;
        while (numblist.Count != loopCount)
        {
            no = UnityEngine.Random.Range(1, 61);

            if (!numblist.Contains(no))
            {
                numblist.Add(no);
            }
        }

        return numblist;
    }

    public void CheckSelectedNumb(int num)
    {
        for (int i = 0; i < totalActiveCard; i++)
        {
            if (cardClasses[i].numb.Contains(generatedNO[num]))
            {
                int index = cardClasses[i].numb.IndexOf(generatedNO[num]);
                cardClasses[i].selectionImg[index].SetActive(true);
                cardClasses[i].payLinePattern[index] = 1;
                CheckPayLineMatch(i);
            }
            if (num == 29)
            {

                //Invoke(nameof(ShowExtraBalls), 1);
                Invoke(nameof(StartExtraBallScreen), 2);
            }
        }
    }

    void StartExtraBallScreen()
    {
        if (isExtraBallDone) { return; }

        if (totalExtraBallCount < 20)
            extraBallObj.SetActive(true);
        else
            ShowExtraBallOnTap(totalExtraBallCount);
        EndGame();
        isExtraBallDone = true;
    }

    public void CheckPayLineMatch(int cardNo)
    {

        for (int i = 0; i < cardClasses[cardNo].paylineObj.Count; i++)
        {
            cardClasses[cardNo].paylineindex.Add(false);
        }

        for (int i = 0; i < patternList.Count; i++)
        {
            if (!cardClasses[cardNo].paylineindex[i])
            {

                int count = 0;
                if (selectedIndex != null) selectedIndex.Clear();
                for (int a = 0; a < totalNumInEachCard; a++)
                {
                    if (patternList[i].pattern[a] == 1 && cardClasses[cardNo].payLinePattern[a] == 1)
                    {
                        count++;
                        if (!cardClasses[cardNo].selectedPayLineCanBe.ContainsKey(i))
                        {
                            cardClasses[cardNo].selectedPayLineCanBe.Add(i, count);
                        }
                        else
                        {
                            cardClasses[cardNo].selectedPayLineCanBe[i] = count;
                        }
                        selectedIndex.Add(a);
                        selectedCard.Add(cardNo);
                        if (count == patternList[i].totalCountOfTrue)
                        {
                            cardClasses[cardNo].paylineindex[i] = true;
                            //cardClasses[cardNo].paylineObj[i].SetActive(true);
                            EventManager.ShowMatchedPattern(i, true);
                            for (int m = 0; m < selectedIndex.Count; m++)
                            {
                                cardClasses[cardNo].matchPatternImg[selectedIndex[m]].SetActive(true);
                                ballAnimSpeed = 0.11f; 
                                Debug.Log("m : " + m + " selectedIndex[m] : " + selectedIndex[m]);
                                EventManager.ShowMissingPattern(i, selectedIndex[m], false);
                            }

                            if (i < 10)
                            {
                                isMissingPattern = true;
                            }

                            if (i == 1)
                            {
                                isBonusSelected = true;
                            }
                            else
                            {
                                EventManager.AddWinAmt(i);
                            }
                        }
						else if (count == patternList[i].totalCountOfTrue - 1)
                        {
                            for (int b = 0; b < totalNumInEachCard; b++)
                            {
                                if (patternList[i].pattern[b] == 1 && cardClasses[cardNo].payLinePattern[b] == 0)
                                {
                                    cardClasses[cardNo].paylineObj[i].SetActive(true);

                                    cardClasses[cardNo].missingPatternImg[b].SetActive(true);
                                    EventManager.ShowMissingPattern(i, b, true);
                                }
                            }

                        } 
                        else
                        {
                            if(ballAnimSpeed <0.3f)
                                ballAnimSpeed += 0.0005f;
                        }
                    }
                }
            }
        }
    }

  

    public float ballAnimSpeed = 0.11f;
    List<int> selectedIndex = new List<int>();
    bool isMissingPattern;
    List<int> selectedCard = new List<int>();
    void ShowMissingImg(int cardNo, bool extraBallDone)
    {

        for (int i = 0; i < patternList.Count; i++)
        {
            if (cardClasses[cardNo].selectedPayLineCanBe.ContainsKey(i))
            {
                //Debug.Log(cardClasses[cardNo].selectedPayLineCanBe[i] + " " + patternList[i].totalCountOfTrue);
                if (cardClasses[cardNo].selectedPayLineCanBe[i] == patternList[i].totalCountOfTrue - 1)
                {
                    cardClasses[cardNo].paylineObj[i].SetActive(true);
                    // Debug.Log("ONE");
                    foreach (Transform l in cardClasses[cardNo].paylineObj[i].transform)
                    {
                        // l.GetComponent<LineRenderer>().material = unMatchedMat;
                    }
                    for (int a = 0; a < totalNumInEachCard; a++)
                    {
                        if (patternList[i].pattern[a] == 1 && cardClasses[cardNo].payLinePattern[a] == 0)
                        {
                            cardClasses[cardNo].missingPatternImg[a].SetActive(true);
                            // if(isExtraBallDone)
                            EventManager.ShowMissingPattern(i, a, true);
                            //isMissingPattern = true;
                            //cardClasses[cardNo].paylineObj[a].GetComponent<LineRenderer>().material.color = Color.red;
                        }
                    }
                }
            }
        }

        /*if (cardNo == 3)
        {
            // if (extraBallDone || !isMissingPattern)
            // {
            //Debug.Log("Card no : " + cardNo);

            if (isBonusSelected)
            {
                Invoke(nameof(StartBonus), 1);
            }
            else
            {
                NextPlay();
            }
            // }
            // else
            // {
            //     Debug.Log("ShowExtraBalls");
            //     if(isMissingPattern)
            //         Invoke(nameof(ShowExtraBalls), 1);

            // }
        } */

    }
	private void EndGame()
    {
    
        if (isBonusSelected)
        {
            Invoke(nameof(StartBonus), 1);
        }
        else
        {
            NextPlay();
        }
    }

    bool isBonusSelected;

    void StartBonus()
    {

        bonusMainObj.SetActive(true);
    }


    void ResetNumb()
    {
        isMissingPattern = false;
        StopAllCoroutines();
        if (random.Count != 0) random.Clear();
        if (generatedNO.Count != 0) generatedNO.Clear();

        for (int i = 0; i < cardClasses.Length; i++)
        {
            if (cardClasses[i].numb.Count != 0) cardClasses[i].numb.Clear();
            cardClasses[i].selectionImg.ForEach(p => p.SetActive(false));
            cardClasses[i].missingPatternImg.ForEach(p => p.SetActive(false));
            cardClasses[i].matchPatternImg.ForEach(p => p.SetActive(false));
            cardClasses[i].paylineObj.ForEach(p => p.SetActive(false));
            if (cardClasses[i].paylineindex.Count != 0) cardClasses[i].paylineindex.Clear();
            for (int j = 0; j < cardClasses[i].paylineObj.Count; j++)
            {
                foreach (Transform l in cardClasses[i].paylineObj[j].transform)
                {
                    //l.GetComponent<LineRenderer>().material = matchedMat;
                }
            }

            for (int j = 0; j < cardClasses[i].payLinePattern.Count; j++)
            {
                cardClasses[i].payLinePattern[j] = 0;
            }
            if (cardClasses[i].selectedPayLineCanBe.Count != 0) cardClasses[i].selectedPayLineCanBe.Clear();
        }
        //extraBalls[0].transform.parent.gameObject.SetActive(false);


        ballAnimSpeed = 0.1f;
        isExtraBallDone = false;
        //EventManager.Play();
    }


    DateTime getGameTime(int secodsDayDurationInGame)
    {
        float scaledElapsedSecondInGame = secondsOfARealDay / secodsDayDurationInGame; // second equivalent in your game 
        //float elapsedRealTime = Time.time - startingGameTime; // uncomment to calculate with elapsed real time.
        DateTime gateDateTime = startingGameDate.AddSeconds(elapsedRealTime * scaledElapsedSecondInGame);

        return gateDateTime;
    }
}



[System.Serializable]
public class CardClass
{
    public bool isCardActive;
    public List<int> numb = new List<int>();
    public List<TextMeshProUGUI> num_text = new List<TextMeshProUGUI>();
    public List<GameObject> selectionImg = new List<GameObject>();
    public List<GameObject> missingPatternImg = new List<GameObject>();
    public List<GameObject> matchPatternImg = new List<GameObject>();
    public List<byte> payLinePattern = new List<byte>(15);
    public Dictionary<int, int> selectedPayLineCanBe = new Dictionary<int, int>();
    public List<GameObject> paylineObj;
    public List<bool> paylineindex = new List<bool>();
}