using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class APIManager : MonoBehaviour
{
    public static APIManager instance;
    private const string BASE_URL = "https://bingoapi.codehabbit.com/";
    private string initialApiUrl;
    private string remainingApiUrl;
    public int bonusAMT;
    private List<SlotData> slotDataList = new List<SlotData>();




    private bool isSlotDataFetched = false;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        CallApisForFetchData();
    }

    public void CallApisForFetchData()
    {
        CallRemainingApi();
    }
    public void CallRemainingApi()
    {
        Debug.Log("Remaining API call started.");
        if (GameManager.instance != null)
        {
            int currentBet = GameManager.instance.currentBet;
            remainingApiUrl = $"{BASE_URL}api/v1/slot?bet={currentBet}";
            StartCoroutine(FetchSlotDataFromAPI(remainingApiUrl, false));
        }
        else
        {
            Debug.LogError("GameManager instance is not available.");
        }
    }

    private IEnumerator FetchSlotDataFromAPI(string url, bool isInitialCall)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + request.error);
            }
            else
            {
                string responseText = request.downloadHandler.text;
                //Debug.Log("Raw JSON Response from URL " + url + ": " + responseText);

                if (isInitialCall)
                {
                    RootObject rootObject = JsonUtility.FromJson<RootObject>(responseText);

                    if (rootObject != null && rootObject.data != null)
                    {
                        slotDataList = rootObject.data;
                        // Debug.Log("SlotData List Count: " + slotDataList.Count);
                        isSlotDataFetched = true;
                    }
                    else
                    {
                        Debug.LogError("Failed to parse JSON from URL " + url + ". Please check the JSON format and ensure it matches the class structure.");
                    }
                }
                else
                {
                    RemainingApiResponse remainingApiResponse = JsonUtility.FromJson<RemainingApiResponse>(responseText);

                    if (remainingApiResponse != null && remainingApiResponse.data != null)
                    {
                        SlotData slotData = remainingApiResponse.data;
                        slotDataList.Clear();
                        slotDataList.Add(slotData);
                        //Debug.Log("SlotData added for remaining API call.");
                        isSlotDataFetched = true;
                        StartGameWithBet();
                    }
                    else
                    {
                        Debug.LogError("Failed to parse JSON from URL " + url + ". Please check the JSON format and ensure it matches the class structure.");
                    }
                }
            }
        }
    }

    public void StartGameWithBet()
    {
        if (GameManager.instance != null)
        {
            if (isSlotDataFetched)
            {
                int currentBet = GameManager.instance.currentBet;
                //Debug.Log("Current Bet Value: " + currentBet);

                foreach (SlotData slotData in slotDataList)
                {
                    if (slotData.bet == currentBet)
                    {
                        int FetchNo = GameManager.instance.betlevel + 1;

                        if (FetchNo != 0)
                        {
                            // Debug.Log("Fetched number: " + FetchNo);
                            // Debug.Log("slotData.number: " + slotData.number);
                            FetchNo = slotData.number / FetchNo;
                            // FetchNo = 170;
                            Debug.Log("Original Fetched number: " + slotData.number);
                            NumberManager.instance.num = FetchNo;
                            //Debug.Log("Fetched number: " + FetchNo);

                            // Mark as used
                        }
                        else
                        {
                            NumberManager.instance.num = slotData.number;
                            //NumberManager.instance.num = 170;
                            Debug.Log("Fetched number: " + slotData.number);
                            Debug.Log("GameManager.instance.betlevel is zero. Division by zero is not allowed.");
                        }

                        if (FetchNo > 150)
                        {
                            Debug.Log("Bonus Is Present :::");
                            NumberManager.instance.num = 150;

                            int Bonus = FetchNo - 150;
                            bonusAMT = Bonus;
                            Debug.Log("Bonus Is Present ::: " + Bonus);
                        }


                        NumberManager.instance.DoAvailablePattern();
                        slotData.selected = true;
                    }

                    else
                    {
                        slotData.selected = false;
                    }
                }
            }
            else
            {
                Debug.LogError("Slot data has not been fetched yet.");
            }
        }
        else
        {
            Debug.LogError("GameManager instance is not available.");
        }
    }

    // private IEnumerator SendSlotDataToServer()
    // {
    //     string newServerUrl = BASE_URL + "api/v1/slot/releaseNumbers";  // Replace with your new URL
    //     string jsonBody = JsonUtility.ToJson(new SlotWrapper(slotDataList));
    //     //Debug.Log("JSON Body to send: " + jsonBody);

    //     using (UnityWebRequest request = new UnityWebRequest(newServerUrl, "POST"))
    //     {
    //         byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
    //         request.uploadHandler = new UploadHandlerRaw(bodyRaw);
    //         request.downloadHandler = new DownloadHandlerBuffer();
    //         request.SetRequestHeader("Content-Type", "application/json");

    //         yield return request.SendWebRequest();

    //         Debug.Log("Response Code: " + request.responseCode);

    //         if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
    //         {
    //             Debug.LogError("Error sending slot data to server: " + request.error);
    //             Debug.LogError("Server Response: " + request.downloadHandler.text);
    //         }
    //         else
    //         {
    //             //Debug.Log("Successfully sent slot data to server.");
    //         }
    //     }
    // }

    [System.Serializable]
    private class SlotWrapper
    {
        public List<SlotData> slots;

        public SlotWrapper(List<SlotData> slotDataList)
        {
            this.slots = slotDataList;
        }
    }

    [System.Serializable]
    private class RootObject
    {
        public List<SlotData> data;
        public bool error;
        public string message;
        public int code;
    }

    [System.Serializable]
    private class RemainingApiResponse
    {
        public SlotData data;
        public bool error;
        public string message;
        public int code;
    }

    [System.Serializable]
    private class SlotData
    {
        public string slotId;
        public string numberId;
        public int number;
        public int bet;
        public bool selected;
    }
}
