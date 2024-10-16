using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class BallManager : MonoBehaviour
{
    public NumberGenerator numberGenerator;
    public List<GameObject> balls;
    public List<Sprite> ballSprite;
    public GameObject ballPrefab;
    public Transform extraBallParent;

    public GameObject ballOutMachineAnimParent;
    public Image bigBallImg;
    public GameObject ballMachine;
    public GameObject extraBallMachine;
    public List<Sprite> bigBallSprite;
    private List<Sprite> bigBallSpriteSequence = new List<Sprite>();
    public List<GameObject> extraBalls;
    private Vector3[] extraBaStartPos = new Vector3[9]; 
    public float ballAnimSpeed = 0.11f;
    private List<int> ballIndexList = new List<int>();
    private int[] extraBallPosArr = new int[10] {-315, 315, -245, 245 ,-175, 175, -105, 105, -35, 35 };
    private List<GameObject> instantiatedExtraBall = new List<GameObject>();
    private void OnEnable()
    {
        EventManager.OnGenerateBall += GenerateBall;
        EventManager.OnGenerateExtraBall += GenerateExtraBall;
        EventManager.OnPlay += ResetBalls;

        EventManager.OnTapForExtraBall += ShowExtraBallOnTap;

        GetStartPosition_ExtraBalls();

        ballOutMachineAnimParent.SetActive(true);
        bigBallImg.gameObject.SetActive(false);

		if(ballMachine != null)
        	ballMachine.SetActive(false);
        if(extraBallMachine != null)
        	extraBallMachine.SetActive(false);
    }

    private void OnDisable()
    {
        EventManager.OnGenerateBall -= GenerateBall;
        EventManager.OnGenerateExtraBall -= GenerateExtraBall;
        EventManager.OnPlay -= ResetBalls;
        EventManager.OnTapForExtraBall -= ShowExtraBallOnTap;

    }


    void GetStartPosition_ExtraBalls()
    {
        for(int i = 0; i< extraBalls.Count; i++)
        {
            extraBaStartPos[i] = extraBalls[i].transform.localPosition;
            extraBalls[i].SetActive(false);
        }
    }

    void GenerateBall(List<int> _ballIndexList)
    {
        ballIndexList = _ballIndexList;
        ballOutMachineAnimParent.SetActive(false);

        AddRandomBallSprites();
        StartCoroutine(StartBallAnim());
    }

    void ShowExtraBallOnTap(bool isExtraBallLeft)
    {
    	if(extraBallMachine != null)
        	extraBallMachine.SetActive(isExtraBallLeft);
        bigBallImg.gameObject.SetActive(false);
        if(ballMachine != null)
        	ballMachine.SetActive(!isExtraBallLeft);
        if (isExtraBallLeft)
        {
            for (int i = 0; i < 4; i++)
            {

                EventManager.ShowMissingPL(i, true);
            }
        }
    }

    public void ShowExtraBallAfterTap(int index)
    {
        extraBalls[index].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = ballIndexList[ballIndexList.Count - 1].ToString();
    }

    int ballIndex = 0;
    void GenerateExtraBall(List<int> _ballIndexList, bool showExtraBall)
    {
        Debug.Log("GenerateExtraBall : "+ showExtraBall);
        //StartCoroutine(StartExtaBallAnim(ballIndexList, showExtraBall));        //For Auto show 5 extra balls
        //StartCoroutine(StartExtaBallAnim(showExtraBall, ballIndex++));            //For Tap and show extra ball
        ballIndexList = _ballIndexList;
        StartExtaBallAnim(showExtraBall, ballIndex++);
    }

    void AddRandomBallSprites()
    {
        for(int i = 0; i< balls.Count; i++)
        {
            int ballSpriteIndex = Random.Range(0, ballSprite.Count);
            bigBallSpriteSequence.Add(bigBallSprite[ballSpriteIndex]);

            balls[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = ballIndexList[i].ToString();
            balls[i].GetComponent<Image>().sprite = ballSprite[ballSpriteIndex];
        }
    }

 

    IEnumerator StartBallAnim()
    {
        if (!bigBallImg.isActiveAndEnabled)
        {
            if (ballMachine != null)
                ballMachine.SetActive(true);
            if (extraBallMachine != null)
                extraBallMachine.SetActive(false);
            bigBallImg.gameObject.SetActive(true);
        }
        for (int i = 0; i < balls.Count; i++)
        {
            bigBallImg.sprite = bigBallSpriteSequence[i];
            bigBallImg.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = ballIndexList[i].ToString();


            balls[i].transform.localPosition = new Vector2(0, 100);
            balls[i].SetActive(true);
            yield return new WaitForSeconds(numberGenerator.ballAnimSpeed);
            if (i < 15)
                balls[i].transform.DOLocalMoveY(-350, numberGenerator.ballAnimSpeed);
            else
                balls[i].transform.DOLocalMoveY(-280, numberGenerator.ballAnimSpeed);
            
            EventManager.ShowBallOnCard(i);

            if (i == 14 || i == 29)
            {
                yield return null;
            }
            else {
                if (i <= 21)
                {
                    yield return new WaitForSeconds(numberGenerator.ballAnimSpeed);
                    if (i < 7)
                    {
                        balls[i].transform.DOLocalMoveX(70 * ((i % 7) - 7), numberGenerator.ballAnimSpeed);
                    }
                    else if (i >= 7 && i < 14)
                    {
                        balls[i].transform.DOLocalMoveX(70 * (7 - (i % 7)), numberGenerator.ballAnimSpeed);
                    }
                    else if ((i > 14 && i <= 21))
                    {
                        if (i == 21)
                        {
                            balls[i].transform.DOLocalMoveX(-70, numberGenerator.ballAnimSpeed);
                        }
                        else
                        {
                            balls[i].transform.DOLocalMoveX(70 * ((i % 7) - 7 - 1), numberGenerator.ballAnimSpeed);
                        }
                    }


                }
                else
                {
                    yield return new WaitForSeconds(numberGenerator.ballAnimSpeed);
                    if (i == 28)
                    {
                        balls[i].transform.DOLocalMoveX(70, numberGenerator.ballAnimSpeed);
                    }
                    else
                    {
                        balls[i].transform.DOLocalMoveX(70 * (7 + 1 - (i % 7)), numberGenerator.ballAnimSpeed);
                    }

                }
            }
        }
    }

    IEnumerator StartExtaBallAnim(List<int> ballIndexList, bool showExtraBall)
    {
        if(showExtraBall){
            for (int i = 0; i < ballIndexList.Count; i++)
            {
                if (!extraBalls[i].activeInHierarchy)
                {
                    Debug.Log(ballIndexList.Count);
                    //extraBalls[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = ballIndexList[30 + i].ToString(); //NumberGenerator.generatedNO[30+i]
                    extraBalls[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = ballIndexList[ballIndexList.Count-1].ToString(); //NumberGenerator.generatedNO[30+i]

                    extraBalls[i].transform.localPosition = new Vector2(0, 100);
                    extraBalls[i].SetActive(true);
                    extraBalls[i].transform.DOLocalMove(extraBaStartPos[i], ballAnimSpeed);
                    yield return new WaitForSeconds(ballAnimSpeed + 0.5f);
                    EventManager.ShowBallOnCard(30 + i);
                }
            }
        }
        //NumberGenerator.isExtraBallDone = true;
        for (int i = 0; i < 4; i++)
        {
            EventManager.ShowMissingPL(i, true);
        }
    }


    void StartExtaBallAnim(bool showExtraBall, int index)
    {
        if (showExtraBall)
        {
            if (instantiatedExtraBall.Count == 0 || instantiatedExtraBall.Count-1 < index)
            {
                GameObject g = Instantiate(ballPrefab, extraBallParent);
                g.GetComponent<Image>().sprite = ballSprite[Random.Range(0, ballSprite.Count)];
                

                StartCoroutine( ModifyExtraBallPos(g, index));
                instantiatedExtraBall.Add(g);
            }
            else
            {
                StartCoroutine(ModifyExtraBallPos(instantiatedExtraBall[index], index));
            }
            //extraBalls[index].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = ballIndexList[ballIndexList.Count - 1].ToString(); //NumberGenerator.generatedNO[30+i]

            //extraBalls[index].transform.localPosition = new Vector2(0, 100);
            //extraBalls[index].SetActive(true);
            //extraBalls[index].transform.DOLocalMove(extraBaStartPos[index], ballAnimSpeed);
            //yield return new WaitForSeconds(ballAnimSpeed + 0.5f);
            //EventManager.ShowBallOnCard(ballIndexList.Count - 1);


            ////extraBalls[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = ballIndexList[30 + i].ToString(); //NumberGenerator.generatedNO[30+i]
            //extraBalls[index].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = ballIndexList[ballIndexList.Count - 1].ToString(); //NumberGenerator.generatedNO[30+i]

            //extraBalls[index].transform.localPosition = new Vector2(0, 100);
            //extraBalls[index].SetActive(true);
            //extraBalls[index].transform.DOLocalMove(extraBaStartPos[index], ballAnimSpeed);
            //yield return new WaitForSeconds(ballAnimSpeed + 0.5f);
            //EventManager.ShowBallOnCard(ballIndexList.Count-1);

        }
        //NumberGenerator.isExtraBallDone = true;
        // Debug.Log("FA");
        for (int i = 0; i < 4; i++)
        {
       
            EventManager.ShowMissingPL(i, true);
        }
    }

    IEnumerator ModifyExtraBallPos(GameObject g, int index)
    {
        g.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = ballIndexList[ballIndexList.Count - 1].ToString();
        g.transform.localPosition = new Vector2(0, 150);
        g.SetActive(true);

       // yield return new WaitForSeconds(numberGenerator.ballAnimSpeed);

        if (index < 10)
            g.transform.DOLocalMoveY(-164 + 100, numberGenerator.ballAnimSpeed);
        else
            g.transform.DOLocalMoveY(-95 + 100, numberGenerator.ballAnimSpeed);

        yield return new WaitForSeconds(numberGenerator.ballAnimSpeed);
        //Debug.Log(g.transform.localPosition.y);
        //if ((index + 1) % 10 == 0)
        //{
        //    yield return null;
        //}
        //else
        //{
            g.transform.DOLocalMoveX(extraBallPosArr[index % 10], numberGenerator.ballAnimSpeed);

        //}
        yield return new WaitForSeconds(numberGenerator.ballAnimSpeed);
        EventManager.ShowBallOnCard(ballIndexList.Count - 1);
    }

    void ResetBalls()
    {
        if(extraBallMachine != null)
            extraBallMachine.SetActive(false);

        StopAllCoroutines();
        foreach (var e in balls)
        {
            e.SetActive(false);
        }

        foreach (var e in extraBalls)
        {
            e.SetActive(false);
            e.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
        }

        ballIndex = 0;
        foreach (var g in instantiatedExtraBall)
        {
            g.SetActive(false);
        }

      
    }


}
