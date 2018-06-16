using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using KMHelper;
using System;
using UnityEngine;

public class subways : MonoBehaviour {

    public KMAudio newAudio;
    public KMBombModule module;
    public KMBombInfo info;

    public GameObject nyc;
    public GameObject london;
    public GameObject paris;

    public GameObject nameScrn;
    public GameObject dayScrn;
    public GameObject timeScrn;
    public GameObject ampmScrn;
    public String[] names = new String[6];
    public String[] days = new String[5];
    public String nameOnScreen = "Bryan";
    public String day = "Mo";
    public int time = 12;
    public bool isAM = false;
    public int map; //0=nyc, 1=london, 2=paris
    public int[] stops = new int[3] { -1,-1,-1 };

    public String[] stopNamesNYC;
    public KMSelectable[] stopBtnsNYC;
    public GameObject[] stopHiLitesNYC;
    public String[] stopNamesLondon;
    public KMSelectable[] stopBtnsLondon;
    public GameObject[] stopHiLitesLondon;
    public String[] stopNamesParis;
    public KMSelectable[] stopBtnsParis;
    public GameObject[] stopHiLitesParis;

    public Material[] stopMats;
    private int stopIndexCurrent = 0;

    public int correctTime;
    public bool correctAMPM;
    public int[] correctStopsIndex = new int[3];

    public KMSelectable timeUpBtn;
    public KMSelectable timeDownBtn;
    public KMSelectable ampmUpBtn;
    public KMSelectable ampmDownBtn;
    public KMSelectable submitBtn;

    private static int _moduleIdCounter = 1;
    private int _moduleId = 0;
    private bool _isSolved = false, _lightsOn = false;

    void Start () {
        _moduleId = _moduleIdCounter++;
        module.OnActivate += Activate;
    }

    void Activate()
    {
        Init();
        _lightsOn = true;
    }

    private void Awake()
    {
        timeUpBtn.OnInteract += delegate ()
        {
            timeUpPress();
            return false;
        };
        timeDownBtn.OnInteract += delegate ()
        {
            timeDownPress();
            return false;
        };
        ampmUpBtn.OnInteract += delegate ()
        {
            ampmPress(true);
            return false;
        };
        ampmDownBtn.OnInteract += delegate ()
        {
            ampmPress(false);
            return false;
        };
        submitBtn.OnInteract += delegate ()
        {
            submitPress();
            return false;
        };
        for(int i = 0; i < 21; i++)
        {
            int j = i;
            stopBtnsNYC[i].OnInteract += delegate ()
            {
                handlePressNYC(j);
                return false;
            };
        }
        for (int i = 0; i < 9; i++)
        {
            int j = i;
            stopBtnsLondon[i].OnInteract += delegate ()
            {
                handlePressLondon(j);
                return false;
            };
        }
        for (int i = 0; i < 11; i++)
        {
            int j = i;
            stopBtnsParis[i].OnInteract += delegate ()
            {
                handlePressParis(j);
                return false;
            };
        }
    }

    void clearHighlights()
    {
        for(int i = 0; i<21; i++)
        {
            stopHiLitesNYC[i].SetActive(false);
        }
        for (int i = 0; i < 9; i++)
        {
            stopHiLitesLondon[i].SetActive(false);
        }
        for (int i = 0; i < 11; i++)
        {
            stopHiLitesParis[i].SetActive(false);
        }
    }

    void handlePressNYC(int btnIndex)
    {
        newAudio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, stopBtnsNYC[btnIndex].transform);
        if (!_lightsOn || _isSolved || !(map == 0)) return;
        switch (stopIndexCurrent)
        {
            case 0:
                stops[0] = btnIndex;
                stopHiLitesNYC[btnIndex].SetActive(true);
                stopHiLitesNYC[btnIndex].GetComponent<MeshRenderer>().material = stopMats[0];
                stopIndexCurrent++;
                break;
            case 1:
                if (stops[0] == btnIndex)
                {
                    stopIndexCurrent = 0;
                    stopHiLitesNYC[stops[0]].SetActive(false);
                    stops[0] = -1;
                    break;
                }
                stops[1] = btnIndex;
                stopHiLitesNYC[btnIndex].SetActive(true);
                stopHiLitesNYC[btnIndex].GetComponent<MeshRenderer>().material = stopMats[1];
                stopIndexCurrent++;
                break;
            case 2:
                if (stops[0] == btnIndex)
                {
                    stopHiLitesNYC[stops[0]].SetActive(false);
                    stopHiLitesNYC[stops[1]].GetComponent<MeshRenderer>().material = stopMats[0];
                    stops[0] = stops[1];
                    stops[1] = -1;
                    stopIndexCurrent = 1;
                    break;
                } else if(stops[1] == btnIndex)
                {
                    stopHiLitesNYC[stops[1]].SetActive(false);
                    stops[1] = -1;
                    stopIndexCurrent = 1;
                    break;
                }
                stops[2] = btnIndex;
                stopHiLitesNYC[btnIndex].SetActive(true);
                stopHiLitesNYC[btnIndex].GetComponent<MeshRenderer>().material = stopMats[2];
                stopIndexCurrent++;
                break;
            case 3:
                if (stops[0] == btnIndex)
                {
                    stopHiLitesNYC[stops[0]].SetActive(false);
                    stopHiLitesNYC[stops[1]].GetComponent<MeshRenderer>().material = stopMats[0];
                    stopHiLitesNYC[stops[2]].GetComponent<MeshRenderer>().material = stopMats[1];
                    stops[0] = stops[1];
                    stops[1] = stops[2];
                    stops[2] = -1;
                    stopIndexCurrent = 2;
                    break;
                }
                else if (stops[1] == btnIndex)
                {
                    stopHiLitesNYC[stops[1]].SetActive(false);
                    stopHiLitesNYC[stops[2]].GetComponent<MeshRenderer>().material = stopMats[1];
                    stops[1] = stops[2];
                    stops[2] = -1;
                    stopIndexCurrent = 2;
                    break;
                }
                else if (stops[2] == btnIndex)
                {
                    stopHiLitesNYC[stops[2]].SetActive(false);
                    stops[2] = -1;
                    stopIndexCurrent = 2;
                    break;
                } else
                {
                    stopHiLitesNYC[stops[0]].SetActive(false);
                    stopHiLitesNYC[stops[1]].GetComponent<MeshRenderer>().material = stopMats[0];
                    stopHiLitesNYC[stops[2]].GetComponent<MeshRenderer>().material = stopMats[1];
                    stopHiLitesNYC[btnIndex].SetActive(true);
                    stopHiLitesNYC[btnIndex].GetComponent<MeshRenderer>().material = stopMats[2];
                    stops[0] = stops[1];
                    stops[1] = stops[2];
                    stops[2] = btnIndex;
                    stopIndexCurrent = 3;
                    break;
                }
        }
    }

    void handlePressLondon(int btnIndex)
    {
        newAudio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, stopBtnsLondon[btnIndex].transform);
        if (!_lightsOn || _isSolved || !(map == 1)) return;
        switch (stopIndexCurrent)
        {
            case 0:
                stops[0] = btnIndex;
                stopHiLitesLondon[btnIndex].SetActive(true);
                stopHiLitesLondon[btnIndex].GetComponent<MeshRenderer>().material = stopMats[0];
                stopIndexCurrent++;
                break;
            case 1:
                if (stops[0] == btnIndex)
                {
                    stopIndexCurrent = 0;
                    stopHiLitesLondon[stops[0]].SetActive(false);
                    stops[0] = -1;
                    break;
                }
                stops[1] = btnIndex;
                stopHiLitesLondon[btnIndex].SetActive(true);
                stopHiLitesLondon[btnIndex].GetComponent<MeshRenderer>().material = stopMats[1];
                stopIndexCurrent++;
                break;
            case 2:
                if (stops[0] == btnIndex)
                {
                    stopHiLitesLondon[stops[0]].SetActive(false);
                    stopHiLitesLondon[stops[1]].GetComponent<MeshRenderer>().material = stopMats[0];
                    stops[0] = stops[1];
                    stops[1] = -1;
                    stopIndexCurrent = 1;
                    break;
                }
                else if (stops[1] == btnIndex)
                {
                    stopHiLitesLondon[stops[1]].SetActive(false);
                    stops[1] = -1;
                    stopIndexCurrent = 1;
                    break;
                }
                stops[2] = btnIndex;
                stopHiLitesLondon[btnIndex].SetActive(true);
                stopHiLitesLondon[btnIndex].GetComponent<MeshRenderer>().material = stopMats[2];
                stopIndexCurrent++;
                break;
            case 3:
                if (stops[0] == btnIndex)
                {
                    stopHiLitesLondon[stops[0]].SetActive(false);
                    stopHiLitesLondon[stops[1]].GetComponent<MeshRenderer>().material = stopMats[0];
                    stopHiLitesLondon[stops[2]].GetComponent<MeshRenderer>().material = stopMats[1];
                    stops[0] = stops[1];
                    stops[1] = stops[2];
                    stops[2] = -1;
                    stopIndexCurrent = 2;
                    break;
                }
                else if (stops[1] == btnIndex)
                {
                    stopHiLitesLondon[stops[1]].SetActive(false);
                    stopHiLitesLondon[stops[2]].GetComponent<MeshRenderer>().material = stopMats[1];
                    stops[1] = stops[2];
                    stops[2] = -1;
                    stopIndexCurrent = 2;
                    break;
                }
                else if (stops[2] == btnIndex)
                {
                    stopHiLitesLondon[stops[2]].SetActive(false);
                    stops[2] = -1;
                    stopIndexCurrent = 2;
                    break;
                }
                else
                {
                    stopHiLitesLondon[stops[0]].SetActive(false);
                    stopHiLitesLondon[stops[1]].GetComponent<MeshRenderer>().material = stopMats[0];
                    stopHiLitesLondon[stops[2]].GetComponent<MeshRenderer>().material = stopMats[1];
                    stopHiLitesLondon[btnIndex].SetActive(true);
                    stopHiLitesLondon[btnIndex].GetComponent<MeshRenderer>().material = stopMats[2];
                    stops[0] = stops[1];
                    stops[1] = stops[2];
                    stops[2] = btnIndex;
                    stopIndexCurrent = 3;
                    break;
                }
        }
    }

    void handlePressParis(int btnIndex)
    {
        newAudio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, stopBtnsParis[btnIndex].transform);
        if (!_lightsOn || _isSolved || !(map == 2)) return;
        switch (stopIndexCurrent)
        {
            case 0:
                stops[0] = btnIndex;
                stopHiLitesParis[btnIndex].SetActive(true);
                stopHiLitesParis[btnIndex].GetComponent<MeshRenderer>().material = stopMats[0];
                stopIndexCurrent++;
                break;
            case 1:
                if (stops[0] == btnIndex)
                {
                    stopIndexCurrent = 0;
                    stopHiLitesParis[stops[0]].SetActive(false);
                    stops[0] = -1;
                    break;
                }
                stops[1] = btnIndex;
                stopHiLitesParis[btnIndex].SetActive(true);
                stopHiLitesParis[btnIndex].GetComponent<MeshRenderer>().material = stopMats[1];
                stopIndexCurrent++;
                break;
            case 2:
                if (stops[0] == btnIndex)
                {
                    stopHiLitesParis[stops[0]].SetActive(false);
                    stopHiLitesParis[stops[1]].GetComponent<MeshRenderer>().material = stopMats[0];
                    stops[0] = stops[1];
                    stops[1] = -1;
                    stopIndexCurrent = 1;
                    break;
                }
                else if (stops[1] == btnIndex)
                {
                    stopHiLitesParis[stops[1]].SetActive(false);
                    stops[1] = -1;
                    stopIndexCurrent = 1;
                    break;
                }
                stops[2] = btnIndex;
                stopHiLitesParis[btnIndex].SetActive(true);
                stopHiLitesParis[btnIndex].GetComponent<MeshRenderer>().material = stopMats[2];
                stopIndexCurrent++;
                break;
            case 3:
                if (stops[0] == btnIndex)
                {
                    stopHiLitesParis[stops[0]].SetActive(false);
                    stopHiLitesParis[stops[1]].GetComponent<MeshRenderer>().material = stopMats[0];
                    stopHiLitesParis[stops[2]].GetComponent<MeshRenderer>().material = stopMats[1];
                    stops[0] = stops[1];
                    stops[1] = stops[2];
                    stops[2] = -1;
                    stopIndexCurrent = 2;
                    break;
                }
                else if (stops[1] == btnIndex)
                {
                    stopHiLitesParis[stops[1]].SetActive(false);
                    stopHiLitesParis[stops[2]].GetComponent<MeshRenderer>().material = stopMats[1];
                    stops[1] = stops[2];
                    stops[2] = -1;
                    stopIndexCurrent = 2;
                    break;
                }
                else if (stops[2] == btnIndex)
                {
                    stopHiLitesParis[stops[2]].SetActive(false);
                    stops[2] = -1;
                    stopIndexCurrent = 2;
                    break;
                }
                else
                {
                    stopHiLitesParis[stops[0]].SetActive(false);
                    stopHiLitesParis[stops[1]].GetComponent<MeshRenderer>().material = stopMats[0];
                    stopHiLitesParis[stops[2]].GetComponent<MeshRenderer>().material = stopMats[1];
                    stopHiLitesParis[btnIndex].SetActive(true);
                    stopHiLitesParis[btnIndex].GetComponent<MeshRenderer>().material = stopMats[2];
                    stops[0] = stops[1];
                    stops[1] = stops[2];
                    stops[2] = btnIndex;
                    stopIndexCurrent = 3;
                    break;
                }
        }
    }

    void timeUpPress()
    {
        newAudio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, timeUpBtn.transform);
        if (!_lightsOn || _isSolved) return;
        time++;
        if(time == 13)
        {
            time = 1;
        }
        renderTimeScreens();
    }

    void timeDownPress()
    {
        newAudio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, timeDownBtn.transform);
        if (!_lightsOn || _isSolved) return;
        time--;
        if (time == 0)
        {
            time = 12;
        }
        renderTimeScreens();
    }

    void ampmPress(bool up)
    {
        if (up)
        {
            newAudio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, ampmUpBtn.transform);
        }
        else
        {
            newAudio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, ampmDownBtn.transform);
        }
        if (!_lightsOn || _isSolved) return;
        if (isAM)
        {
            isAM = false;
        }
        else
        {
            isAM = true;
        }
        renderTimeScreens();
    }

    void submitPress()
    {
        newAudio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, submitBtn.transform);
        if (!_lightsOn || _isSolved) return;
        if (time == correctTime && isAM == correctAMPM && stops[0] == correctStopsIndex[0] && stops[1] == correctStopsIndex[1] && stops[2] == correctStopsIndex[2])
        {
            module.HandlePass();
        }
        else
        {
            module.HandleStrike();
            if(time != correctTime)
            {
                Debug.LogFormat("[Subways #{0}] Time Selected: {1}. Correct time: {2}", _moduleId, time, correctTime);
            }
            if(isAM != correctAMPM)
            {
                Debug.LogFormat("[Subways #{0}] AMPM Selected: {1}. Correct ampm: {2}", _moduleId, isAM, correctAMPM);
            }
            if(stops[0] != correctStopsIndex[0])
            {
                switch (map)
                {
                    case 0:
                        Debug.LogFormat("[Subways #{0}] Stop 1 Selected: {1}. Correct stop 1: {2}", _moduleId, stopNamesNYC[stops[0]], stopNamesNYC[correctStopsIndex[0]]);
                        break;
                    case 1:
                        Debug.LogFormat("[Subways #{0}] Stop 1 Selected: {1}. Correct stop 1: {2}", _moduleId, stopNamesLondon[stops[0]], stopNamesLondon[correctStopsIndex[0]]);
                        break;
                    case 2:
                        Debug.LogFormat("[Subways #{0}] Stop 1 Selected: {1}. Correct stop 1: {2}", _moduleId, stopNamesParis[stops[0]], stopNamesParis[correctStopsIndex[0]]);
                        break;
                }
            }
            if (stops[1] != correctStopsIndex[1])
            {
                switch (map)
                {
                    case 0:
                        Debug.LogFormat("[Subways #{0}] Stop 2 Selected: {1}. Correct stop 2: {2}", _moduleId, stopNamesNYC[stops[1]], stopNamesNYC[correctStopsIndex[1]]);
                        break;
                    case 1:
                        Debug.LogFormat("[Subways #{0}] Stop 2 Selected: {1}. Correct stop 2: {2}", _moduleId, stopNamesLondon[stops[1]], stopNamesLondon[correctStopsIndex[1]]);
                        break;
                    case 2:
                        Debug.LogFormat("[Subways #{0}] Stop 2 Selected: {1}. Correct stop 2: {2}", _moduleId, stopNamesParis[stops[1]], stopNamesParis[correctStopsIndex[1]]);
                        break;
                }
            }
            if (stops[2] != correctStopsIndex[2])
            {
                switch (map)
                {
                    case 0:
                        Debug.LogFormat("[Subways #{0}] Stop 3 Selected: {1}. Correct stop 3: {2}", _moduleId, stopNamesNYC[stops[2]], stopNamesNYC[correctStopsIndex[2]]);
                        break;
                    case 1:
                        Debug.LogFormat("[Subways #{0}] Stop 3 Selected: {1}. Correct stop 3: {2}", _moduleId, stopNamesLondon[stops[2]], stopNamesLondon[correctStopsIndex[2]]);
                        break;
                    case 2:
                        Debug.LogFormat("[Subways #{0}] Stop 3 Selected: {1}. Correct stop 3: {2}", _moduleId, stopNamesParis[stops[2]], stopNamesParis[correctStopsIndex[2]]);
                        break;
                }
            }
            Debug.LogFormat("[Subways #{0}] If you feel that this strike is an error, please contact AAces as soon as possible so we can get this error sorted out. Have a copy of this log file handy. Discord: AAces#0908", _moduleId);
        }
    }

    void Init()
    {
        getRandomNameDay();
        setRandomMap();
        renderDefaultScreens();
        renderTimeScreens();
        getCorrectAnswer();
    }

    private int nameInt;
    private int dayInt;

    void getRandomNameDay()
    {
        int name = UnityEngine.Random.Range(0, 6);
        nameInt = name;
        nameOnScreen = names[name];
        Debug.LogFormat("[Subways #{0}] Name Selected: {1}", _moduleId, nameOnScreen);
        int dayRnd = UnityEngine.Random.Range(0, 5);
        day = days[dayRnd];
        dayInt = dayRnd;
        Debug.LogFormat("[Subways #{0}] Day Selected: {1}", _moduleId, day);
    }

    void setRandomMap()
    {
        map = UnityEngine.Random.Range(0,3);
        switch (map)
        {
            case 0:
                nyc.SetActive(true);
                london.SetActive(false);
                paris.SetActive(false);
                Debug.LogFormat("[Subways #{0}] Map Selected: NYC", _moduleId);
                break;
            case 1:
                nyc.SetActive(false);
                london.SetActive(true);
                paris.SetActive(false);
                Debug.LogFormat("[Subways #{0}] Map Selected: London", _moduleId);
                break;
            case 2:
                nyc.SetActive(false);
                london.SetActive(false);
                paris.SetActive(true);
                Debug.LogFormat("[Subways #{0}] Map Selected: Paris", _moduleId);
                break;
        }
    }

    void renderDefaultScreens()
    {
        nameScrn.GetComponentInChildren<TextMesh>().text = nameOnScreen;
        dayScrn.GetComponentInChildren<TextMesh>().text = day;
    }

    void renderTimeScreens()
    {
        timeScrn.GetComponentInChildren<TextMesh>().text = time.ToString();
        if (isAM)
        {
            ampmScrn.GetComponentInChildren<TextMesh>().text = "AM";
        } else
        {
            ampmScrn.GetComponentInChildren<TextMesh>().text = "PM";
        }
    }

    void getCorrectAnswer()
    {
        correctStopsIndex = getCorrectStops(map, nameInt, dayInt);
        correctTime = getCorrectTime(map, nameInt, dayInt);
        correctAMPM = getCorrectAMPM(map, nameInt, dayInt);
        if (correctAMPM)
        {
            Debug.LogFormat("[Subways #{0}] Correct time: {1} AM.", _moduleId, correctTime);
        } else
        {
            Debug.LogFormat("[Subways #{0}] Correct time: {1} PM.", _moduleId, correctTime);
        }
        switch (map)
        {
            case 0:
                Debug.LogFormat("[Subways #{0}] Correct stops (in order): {1} > {2} > {3}", _moduleId, stopNamesNYC[correctStopsIndex[0]], stopNamesNYC[correctStopsIndex[1]], stopNamesNYC[correctStopsIndex[2]]);
                break;
            case 1:
                Debug.LogFormat("[Subways #{0}] Correct stops (in order): {1} > {2} > {3}", _moduleId, stopNamesLondon[correctStopsIndex[0]], stopNamesLondon[correctStopsIndex[1]], stopNamesLondon[correctStopsIndex[2]]);
                break;
            case 2:
                Debug.LogFormat("[Subways #{0}] Correct stops (in order): {1} > {2} > {3}", _moduleId, stopNamesParis[correctStopsIndex[0]], stopNamesParis[correctStopsIndex[1]], stopNamesParis[correctStopsIndex[2]]);
                break;
        }
    }

    int[] getCorrectStops(int cityMap, int nameScrn, int dayOWeek)
    {
        int[] answer = new int[3] { 0, 1, 2 };
        switch (cityMap)
        {
            case 0:
                switch (nameInt)
                {
                    case 0:
                        switch (dayOWeek)
                        {
                            case 0:
                                answer = new int[3] { 0, 3, 4 }; //Rt1
                                break;
                            case 1:
                                answer = new int[3] { 17, 12, 20 }; //Rt8
                                break;
                            case 2:
                                answer = new int[3] { 19, 11, 2 }; //Rt4
                                break;
                            case 3:
                                answer = new int[3] { 2, 7, 15 }; //Rt3
                                break;
                            case 4:
                                answer = new int[3] { 13, 8, 4 }; //Rt6
                                break;
                        }
                        break;
                    case 1:
                        switch (dayOWeek)
                        {
                            case 0:
                                answer = new int[3] { 13, 8, 4 }; //Rt6
                                break;
                            case 1:
                                answer = new int[3] { 0, 3, 4 }; //Rt1
                                break;
                            case 2:
                                answer = new int[3] { 3, 14, 18 }; //Rt2
                                break;
                            case 3:
                                answer = new int[3] { 9, 1, 5 }; //Rt7
                                break;
                            case 4:
                                answer = new int[3] { 2, 7, 15 }; //Rt3
                                break;
                        }
                        break;
                    case 2:
                        switch (dayOWeek)
                        {
                            case 0:
                                answer = new int[3] { 9, 1, 5 }; //Rt7
                                break;
                            case 1:
                                answer = new int[3] { 3, 14, 18 }; //Rt2
                                break;
                            case 2:
                                answer = new int[3] { 6, 10, 16 }; //Rt5
                                break;
                            case 3:
                                answer = new int[3] { 17, 12, 20 }; //Rt8
                                break;
                            case 4:
                                answer = new int[3] { 19, 11, 2 }; //Rt4
                                break;
                        }
                        break;
                    case 3:
                        switch (dayOWeek)
                        {
                            case 0:
                                answer = new int[3] { 17, 12, 20 }; //Rt8
                                break;
                            case 1:
                                answer = new int[3] { 3, 14, 18 }; //Rt2
                                break;
                            case 2:
                                answer = new int[3] { 0, 3, 4 }; //Rt1
                                break;
                            case 3:
                                answer = new int[3] { 2, 7, 15 }; //Rt3
                                break;
                            case 4:
                                answer = new int[3] { 6, 10, 16 }; //Rt5
                                break;
                        }
                        break;
                    case 4:
                        switch (dayOWeek)
                        {
                            case 0:
                                answer = new int[3] { 9, 1, 5 }; //Rt7
                                break;
                            case 1:
                                answer = new int[3] { 0, 3, 4 }; //Rt1
                                break;
                            case 2:
                                answer = new int[3] { 19, 11, 2 }; //Rt4
                                break;
                            case 3:
                                answer = new int[3] { 13, 8, 4 }; //Rt6
                                break;
                            case 4:
                                answer = new int[3] { 3, 14, 18 }; //Rt2
                                break;
                        }
                        break;
                    case 5:
                        switch (dayOWeek)
                        {
                            case 0:
                                answer = new int[3] { 6, 10, 16 }; //Rt5
                                break;
                            case 1:
                                answer = new int[3] { 9, 1, 5 }; //Rt7
                                break;
                            case 2:
                                answer = new int[3] { 2, 7, 15 }; //Rt3
                                break;
                            case 3:
                                answer = new int[3] { 17, 12, 20 }; //Rt8
                                break;
                            case 4:
                                answer = new int[3] { 19, 11, 2 }; //Rt4
                                break;
                        }
                        break;
                }
                break;
            case 1:
                switch (nameInt)
                {
                    case 0:
                        switch (dayOWeek)
                        {
                            case 0:
                                answer = new int[3] { 2, 3, 4 }; //Rt9
                                break;
                            case 1:
                                answer = new int[3] { 6, 1, 2 }; //Rt14
                                break;
                            case 2:
                                answer = new int[3] { 1, 6, 8 }; //Rt13
                                break;
                            case 3:
                                answer = new int[3] { 7, 4, 2 }; //Rt10
                                break;
                            case 4:
                                answer = new int[3] { 7, 3, 2 }; //Rt15
                                break;
                        }
                        break;
                    case 1:
                        switch (dayOWeek)
                        {
                            case 0:
                                answer = new int[3] { 1, 6, 8 }; //Rt13
                                break;
                            case 1:
                                answer = new int[3] { 1, 5, 7 }; //Rt11
                                break;
                            case 2:
                                answer = new int[3] { 7, 4, 2 }; //Rt10
                                break;
                            case 3:
                                answer = new int[3] { 8, 6, 2 }; //Rt16
                                break;
                            case 4:
                                answer = new int[3] { 6, 1, 2 }; //Rt14
                                break;
                        }
                        break;
                    case 2:
                        switch (dayOWeek)
                        {
                            case 0:
                                answer = new int[3] { 2, 3, 4 }; //Rt9
                                break;
                            case 1:
                                answer = new int[3] { 8, 6, 2 }; //Rt16
                                break;
                            case 2:
                                answer = new int[3] { 6, 5, 4 }; //Rt12
                                break;
                            case 3:
                                answer = new int[3] { 1, 5, 7 }; //Rt11
                                break;
                            case 4:
                                answer = new int[3] { 7, 3, 2 }; //Rt15
                                break;
                        }
                        break;
                    case 3:
                        switch (dayOWeek)
                        {
                            case 0:
                                answer = new int[3] { 1, 6, 8 }; //Rt13
                                break;
                            case 1:
                                answer = new int[3] { 2, 3, 4 }; //Rt9
                                break;
                            case 2:
                                answer = new int[3] { 7, 4, 2 }; //Rt10
                                break;
                            case 3:
                                answer = new int[3] { 8, 6, 2 }; //Rt16
                                break;
                            case 4:
                                answer = new int[3] { 6, 5, 4 }; //Rt12
                                break;
                        }
                        break;
                    case 4:
                        switch (dayOWeek)
                        {
                            case 0:
                                answer = new int[3] { 1, 6, 8 }; //Rt13
                                break;
                            case 1:
                                answer = new int[3] { 8, 6, 2 }; //Rt16
                                break;
                            case 2:
                                answer = new int[3] { 1, 5, 7 }; //Rt11
                                break;
                            case 3:
                                answer = new int[3] { 2, 3, 4 }; //Rt9
                                break;
                            case 4:
                                answer = new int[3] { 6, 5, 4 }; //Rt12
                                break;
                        }
                        break;
                    case 5:
                        switch (dayOWeek)
                        {
                            case 0:
                                answer = new int[3] { 6, 5, 4 }; //Rt12
                                break;
                            case 1:
                                answer = new int[3] { 6, 1, 2 }; //Rt14
                                break;
                            case 2:
                                answer = new int[3] { 2, 3, 4 }; //Rt9
                                break;
                            case 3:
                                answer = new int[3] { 1, 6, 8 }; //Rt13
                                break;
                            case 4:
                                answer = new int[3] { 8, 6, 2 }; //Rt16
                                break;
                        }
                        break;
                }
                break;
            case 2:
                switch (nameInt)
                {
                    case 0:
                        switch (dayOWeek)
                        {
                            case 0:
                                answer = new int[3] { 0, 1, 2 }; //Rt17
                                break;
                            case 1:
                                answer = new int[3] { 3, 4, 5 }; //Rt18
                                break;
                            case 2:
                                answer = new int[3] { 2, 1, 0 }; //Rt21
                                break;
                            case 3:
                                answer = new int[3] { 5, 4, 3 }; //Rt22
                                break;
                            case 4:
                                answer = new int[3] { 10, 9, 3 }; //Rt19
                                break;
                        }
                        break;
                    case 1:
                        switch (dayOWeek)
                        {
                            case 0:
                                answer = new int[3] { 6, 7, 8 }; //Rt20
                                break;
                            case 1:
                                answer = new int[3] { 10, 9, 3 }; //Rt19
                                break;
                            case 2:
                                answer = new int[3] { 3, 9, 10 }; //Rt23
                                break;
                            case 3:
                                answer = new int[3] { 3, 4, 5 }; //Rt18
                                break;
                            case 4:
                                answer = new int[3] { 5, 4, 3 }; //Rt22
                                break;
                        }
                        break;
                    case 2:
                        switch (dayOWeek)
                        {
                            case 0:
                                answer = new int[3] { 6, 7, 8 }; //Rt20
                                break;
                            case 1:
                                answer = new int[3] { 2, 1, 0 }; //Rt21
                                break;
                            case 2:
                                answer = new int[3] { 3, 9, 10 }; //Rt23
                                break;
                            case 3:
                                answer = new int[3] { 3, 4, 5 }; //Rt18
                                break;
                            case 4:
                                answer = new int[3] { 8, 7, 6 }; //Rt24
                                break;
                        }
                        break;
                    case 3:
                        switch (dayOWeek)
                        {
                            case 0:
                                answer = new int[3] { 0, 1, 2 }; //Rt17
                                break;
                            case 1:
                                answer = new int[3] { 5, 4, 3 }; //Rt22
                                break;
                            case 2:
                                answer = new int[3] { 8, 7, 6 }; //Rt24
                                break;
                            case 3:
                                answer = new int[3] { 3, 4, 5 }; //Rt18
                                break;
                            case 4:
                                answer = new int[3] { 6, 7, 8 }; //Rt20
                                break;
                        }
                        break;
                    case 4:
                        switch (dayOWeek)
                        {
                            case 0:
                                answer = new int[3] { 10, 9, 3 }; //Rt19
                                break;
                            case 1:
                                answer = new int[3] { 2, 1, 0 }; //Rt21
                                break;
                            case 2:
                                answer = new int[3] { 3, 9, 10 }; //Rt23
                                break;
                            case 3:
                                answer = new int[3] { 8, 7, 6 }; //Rt24
                                break;
                            case 4:
                                answer = new int[3] { 0, 1, 2 }; //Rt17
                                break;
                        }
                        break;
                    case 5:
                        switch (dayOWeek)
                        {
                            case 0:
                                answer = new int[3] { 10, 9, 3 }; //Rt19
                                break;
                            case 1:
                                answer = new int[3] { 0, 1, 2 }; //Rt17
                                break;
                            case 2:
                                answer = new int[3] { 6, 7, 8 }; //Rt20
                                break;
                            case 3:
                                answer = new int[3] { 2, 1, 0 }; //Rt21
                                break;
                            case 4:
                                answer = new int[3] { 3, 9, 10 }; //Rt23
                                break;
                        }
                        break;
                }
                break;
        }
        return answer;
    } 

    int batteryTime()
    {
        int bat = info.GetBatteryCount();
        if(bat == 0)
        {
            return 12;
        } else if(bat > 12)
        {
            return (bat - 12);
        } else
        {
            return bat;
        }
    }

    bool batteryAMPM()
    {
        int bat = info.GetBatteryCount();
        return (bat < 12);
    }

    int getCorrectTime(int cityMap, int nameScrn, int dayOWeek)
    {
        switch (cityMap)
        {
            case 0:
                switch (nameInt)
                {
                    case 0:
                        switch (dayOWeek)
                        {
                            case 0:
                                return 8;
                            case 1:
                                return 7;
                            case 2:
                                return 4;
                            case 3:
                                return 11;
                            case 4:
                                return 12;
                        }
                        break;
                    case 1:
                        switch (dayOWeek)
                        {
                            case 0:
                                return 7;
                            case 1:
                                return 2;
                            case 2:
                                return 1;
                            case 3:
                                return batteryTime();
                            case 4:
                                return 4;
                        }
                        break;
                    case 2:
                        switch (dayOWeek)
                        {
                            case 0:
                                return batteryTime();
                            case 1:
                                return 3;
                            case 2:
                                return 6;
                            case 3:
                                return 9;
                            case 4:
                                return batteryTime();
                        }
                        break;
                    case 3:
                        switch (dayOWeek)
                        {
                            case 0:
                                return 8;
                            case 1:
                                return 1;
                            case 2:
                                return 2;
                            case 3:
                                return batteryTime();
                            case 4:
                                return 11;
                        }
                        break;
                    case 4:
                        switch (dayOWeek)
                        {
                            case 0:
                                return 6;
                            case 1:
                                return batteryTime();
                            case 2:
                                return 3;
                            case 3:
                                return 5;
                            case 4:
                                return 5;
                        }
                        break;
                    case 5:
                        switch (dayOWeek)
                        {
                            case 0:
                                return 12;
                            case 1:
                                return 10;
                            case 2:
                                return batteryTime();
                            case 3:
                                return 10;
                            case 4:
                                return 9;
                        }
                        break;
                }
                break;
            case 1:
                switch (nameInt)
                {
                    case 0:
                        switch (dayOWeek)
                        {
                            case 0:
                                return 1;
                            case 1:
                                return batteryTime();
                            case 2:
                                return 5;
                            case 3:
                                return 5;
                            case 4:
                                return 6;
                        }
                        break;
                    case 1:
                        switch (dayOWeek)
                        {
                            case 0:
                                return batteryTime();
                            case 1:
                                return 12;
                            case 2:
                                return 2;
                            case 3:
                                return 4;
                            case 4:
                                return 9;
                        }
                        break;
                    case 2:
                        switch (dayOWeek)
                        {
                            case 0:
                                return 8;
                            case 1:
                                return 7;
                            case 2:
                                return batteryTime();
                            case 3:
                                return 9;
                            case 4:
                                return 11;
                        }
                        break;
                    case 3:
                        switch (dayOWeek)
                        {
                            case 0:
                                return 11;
                            case 1:
                                return 4;
                            case 2:
                                return 3;
                            case 3:
                                return 1;
                            case 4:
                                return batteryTime();
                        }
                        break;
                    case 4:
                        switch (dayOWeek)
                        {
                            case 0:
                                return 7;
                            case 1:
                                return 2;
                            case 2:
                                return 12;
                            case 3:
                                return batteryTime();
                            case 4:
                                return 10;
                        }
                        break;
                    case 5:
                        switch (dayOWeek)
                        {
                            case 0:
                                return batteryTime();
                            case 1:
                                return 8;
                            case 2:
                                return 6;
                            case 3:
                                return 3;
                            case 4:
                                return 10;
                        }
                        break;
                }
                break;
            case 2:
                switch (nameInt)
                {
                    case 0:
                        switch (dayOWeek)
                        {
                            case 0:
                                return batteryTime();
                            case 1:
                                return 9;
                            case 2:
                                return 8;
                            case 3:
                                return 2;
                            case 4:
                                return 7;
                        }
                        break;
                    case 1:
                        switch (dayOWeek)
                        {
                            case 0:
                                return 3;
                            case 1:
                                return 10;
                            case 2:
                                return batteryTime();
                            case 3:
                                return 10;
                            case 4:
                                return 12;
                        }
                        break;
                    case 2:
                        switch (dayOWeek)
                        {
                            case 0:
                                return 5;
                            case 1:
                                return batteryTime();
                            case 2:
                                return 11;
                            case 3:
                                return 8;
                            case 4:
                                return 4;
                        }
                        break;
                    case 3:
                        switch (dayOWeek)
                        {
                            case 0:
                                return 12;
                            case 1:
                                return 1;
                            case 2:
                                return 9;
                            case 3:
                                return 6;
                            case 4:
                                return batteryTime();
                        }
                        break;
                    case 4:
                        switch (dayOWeek)
                        {
                            case 0:
                                return 5;
                            case 1:
                                return 3;
                            case 2:
                                return 6;
                            case 3:
                                return batteryTime();
                            case 4:
                                return 11;
                        }
                        break;
                    case 5:
                        switch (dayOWeek)
                        {
                            case 0:
                                return 2;
                            case 1:
                                return batteryTime();
                            case 2:
                                return 7;
                            case 3:
                                return 1;
                            case 4:
                                return 4;
                        }
                        break;
                }
                break;
        }
        return 12;
    }

    bool getCorrectAMPM(int cityMap, int nameScrn, int dayOWeek)
    {
        bool n = false;
        bool y = true;
        switch (cityMap)
        {
            case 0:
                switch (nameInt)
                {
                    case 0:
                        switch (dayOWeek)
                        {
                            case 0:
                                return y;
                            case 1:
                                return n;
                            case 2:
                                return y;
                            case 3:
                                return y;
                            case 4:
                                return n;
                        }
                        break;
                    case 1:
                        switch (dayOWeek)
                        {
                            case 0:
                                return y;
                            case 1:
                                return y;
                            case 2:
                                return n;
                            case 3:
                                return batteryAMPM();
                            case 4:
                                return n;
                        }
                        break;
                    case 2:
                        switch (dayOWeek)
                        {
                            case 0:
                                return batteryAMPM();
                            case 1:
                                return y;
                            case 2:
                                return n;
                            case 3:
                                return y;
                            case 4:
                                return batteryAMPM();
                        }
                        break;
                    case 3:
                        switch (dayOWeek)
                        {
                            case 0:
                                return n;
                            case 1:
                                return y;
                            case 2:
                                return n;
                            case 3:
                                return batteryAMPM();
                            case 4:
                                return n;
                        }
                        break;
                    case 4:
                        switch (dayOWeek)
                        {
                            case 0:
                                return y;
                            case 1:
                                return batteryAMPM();
                            case 2:
                                return n;
                            case 3:
                                return y;
                            case 4:
                                return n;
                        }
                        break;
                    case 5:
                        switch (dayOWeek)
                        {
                            case 0:
                                return y;
                            case 1:
                                return n;
                            case 2:
                                return batteryAMPM();
                            case 3:
                                return y;
                            case 4:
                                return n;
                        }
                        break;
                }
                break;
            case 1:
                switch (nameInt)
                {
                    case 0:
                        switch (dayOWeek)
                        {
                            case 0:
                                return y;
                            case 1:
                                return batteryAMPM();
                            case 2:
                                return n;
                            case 3:
                                return y;
                            case 4:
                                return n;
                        }
                        break;
                    case 1:
                        switch (dayOWeek)
                        {
                            case 0:
                                return batteryAMPM();
                            case 1:
                                return n;
                            case 2:
                                return y;
                            case 3:
                                return y;
                            case 4:
                                return y;
                        }
                        break;
                    case 2:
                        switch (dayOWeek)
                        {
                            case 0:
                                return y;
                            case 1:
                                return n;
                            case 2:
                                return batteryAMPM();
                            case 3:
                                return n;
                            case 4:
                                return n;
                        }
                        break;
                    case 3:
                        switch (dayOWeek)
                        {
                            case 0:
                                return y;
                            case 1:
                                return n;
                            case 2:
                                return y;
                            case 3:
                                return n;
                            case 4:
                                return batteryAMPM();
                        }
                        break;
                    case 4:
                        switch (dayOWeek)
                        {
                            case 0:
                                return y;
                            case 1:
                                return n;
                            case 2:
                                return y;
                            case 3:
                                return batteryAMPM();
                            case 4:
                                return y;
                        }
                        break;
                    case 5:
                        switch (dayOWeek)
                        {
                            case 0:
                                return batteryAMPM();
                            case 1:
                                return n;
                            case 2:
                                return y;
                            case 3:
                                return y;
                            case 4:
                                return n;
                        }
                        break;
                }
                break;
            case 2:
                switch (nameInt)
                {
                    case 0:
                        switch (dayOWeek)
                        {
                            case 0:
                                return batteryAMPM();
                            case 1:
                                return y;
                            case 2:
                                return n;
                            case 3:
                                return n;
                            case 4:
                                return y;
                        }
                        break;
                    case 1:
                        switch (dayOWeek)
                        {
                            case 0:
                                return y;
                            case 1:
                                return n;
                            case 2:
                                return batteryAMPM();
                            case 3:
                                return y;
                            case 4:
                                return y;
                        }
                        break;
                    case 2:
                        switch (dayOWeek)
                        {
                            case 0:
                                return n;
                            case 1:
                                return batteryAMPM();
                            case 2:
                                return y;
                            case 3:
                                return y;
                            case 4:
                                return y;
                        }
                        break;
                    case 3:
                        switch (dayOWeek)
                        {
                            case 0:
                                return n;
                            case 1:
                                return n;
                            case 2:
                                return n;
                            case 3:
                                return n;
                            case 4:
                                return batteryAMPM();
                        }
                        break;
                    case 4:
                        switch (dayOWeek)
                        {
                            case 0:
                                return y;
                            case 1:
                                return n;
                            case 2:
                                return y;
                            case 3:
                                return batteryAMPM();
                            case 4:
                                return n;
                        }
                        break;
                    case 5:
                        switch (dayOWeek)
                        {
                            case 0:
                                return y;
                            case 1:
                                return batteryAMPM();
                            case 2:
                                return n;
                            case 3:
                                return y;
                            case 4:
                                return n;
                        }
                        break;
                }
                break;
        }
        return n;
    }

    private string TwitchHelpMessage = "Set time using !{0} set time 10 pm. Set route using !{0} set route oxford, holborn, green. Alternatively submit all using !{0} submit 10 pm, South Ferry 1, City Hall 4-5-6, Canal St ACE.";

    private KMSelectable[] ProcessTwitchCommand(string command)
    {
        List<KMSelectable> selectables = new List<KMSelectable>();

        command = command.ToLowerInvariant();
        //Each command should start with either set or submit
        bool set = command.StartsWith("set");
        bool submit = command.StartsWith("submit");
        //For easier comparisons, remove submit and set from the input.
        command = command.Replace("set ", "").Replace("submit ", "");
        //splice input for time calculations. This will be used for routes later
        var split = command.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
        //Make sure that the input matches a format similar to "time 1am" or "12 pm" 
        var Match = Regex.Match(split[0], @"^(?:time |)(\d{1,2})(?: |)((?:a|p)m)$");
        //To set the time directly, we'll need to know what is currently present on the module.
        var screenTime = timeScrn.GetComponentInChildren<TextMesh>().text;
        var screenAMPM = ampmScrn.GetComponentInChildren<TextMesh>().text;

        if (Match.Success)
        {
            //time matches (\d{1,2}) in Match
            string time = Match.Groups[1].Value;
            //ampm matches ((?:a|p)m) in Match
            string ampm = Match.Groups[2].Value;
            /*Since we are sending the selectable at the end of the TP interaction,
            we cannot use a while statement to add selectables to the list.
            So instead, we'll create a new value based on the screen time,
            and increase that instead.*/
            var i = int.Parse(screenTime);
            while (int.Parse(time) != i)
            {
                i++;
                if (i > 12) i = 1;
                selectables.Add(timeUpBtn);
            }
            //Since AMPM only contains two values, a while statement is not necessary.
            if (ampm != screenAMPM.ToLowerInvariant()) selectables.Add(ampmDownBtn);
        }

        //Take out the time command for the submit case, so that the route code can be used
        //for both set and submit
        if (Match.Success && submit && split.Count() > 1)
        {
            command = command.Replace(Match.Value + ", ", "");
        }

        bool route = command.StartsWith("route");
        command = command.Replace("route ", "");
        //This should be accessible for !# set route or !# submit [time], [route] only.
        if (route || submit && command != "submit")
        {
            //If stops has already been used, deselect previously selected buttons.
            //This has to be here, rather than at the beginning, due to the time command
            if (!stops.All(x => x == 0))
            {
                switch (map)
                {
                    case 0:
                        selectables.AddRange(new[] { stopBtnsNYC[stops[2]], stopBtnsNYC[stops[1]], stopBtnsNYC[stops[0]] });
                        break;
                    case 1:
                        selectables.AddRange(new[] { stopBtnsLondon[stops[2]], stopBtnsLondon[stops[1]], stopBtnsLondon[stops[0]] });
                        break;
                    case 2:
                        selectables.AddRange(new[] { stopBtnsParis[stops[2]], stopBtnsParis[stops[1]], stopBtnsParis[stops[0]] });
                        break;
                }
            }
            //resplit command, in case time was also set. Otherwise, ignore this.
            split = command.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
            //Create a new list for route selectables to check its count after the for statement
            List<KMSelectable> Set = new List<KMSelectable>(); 
            //Go through the inputs 3 times. One for each stop. Probably best to do all three at once.
            for (int i = 0; i < 3; i++)
            {
                //Compare the first word of the stop to the one on the module.
                //Using a split for ease, though only the first one is used.
                var split2 = split[i].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
                //Commands are only valid on the map that is chosen
                switch (map)
                {
                    case 0:
                        foreach (string s in stopNamesNYC)
                        {
                            //Make some inputs unambiguous
                            switch (split[i])
                            {
                                case "canal":
                                case "canal st":
                                case "canal street":
                                case "chambers":
                                case "chambers st":
                                case "chambers street":
                                case "city":
                                case "city hall":
                                case "wall":
                                case "wall st":
                                case "wall street":
                                case "rector":
                                case "rector st":
                                case "rector street":
                                case "south":
                                case "south ferry":
                                    throw new FormatException(split[i] + " cannot be condensed. Please use the full stop name.");
                            }
                            //split the name to compare it with the command split
                            var name = Regex.Replace(s, "-", "").ToLowerInvariant();
                            split[i] = Regex.Replace(split[i], "-", "");
                            var nameS = name.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
                            //At least require the first word to match completely
                            if (split2[0].Equals(nameS[0]) && name.StartsWith(split[i]))
                            {
                                //Disallow the same button from being pressed more than once
                                if (Set.Contains(stopBtnsNYC[Array.IndexOf(stopNamesNYC, s)])) return null;
                                Set.Add(stopBtnsNYC[Array.IndexOf(stopNamesNYC, s)]);
                            }
                        }
                        break;
                    case 1:
                        foreach (string s in stopNamesLondon)
                        {
                            //Allow both st. and st inputs.
                            var name = s.ToLowerInvariant().Replace(".", "").Replace("'", "");
                            if (split2.Contains("st.")) split[i] = Regex.Replace(split[i], ".", "");
                            split[i] = split[i].Replace("'", "");
                            var nameS = name.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
                            if (split2[0].Equals(nameS[0]) && name.StartsWith(split[i]))
                            {
                                if (Set.Contains(stopBtnsLondon[Array.IndexOf(stopNamesLondon, s)])) return null;
                                Set.Add(stopBtnsLondon[Array.IndexOf(stopNamesLondon, s)]);
                            }
                        }
                        break;
                    case 2:
                        foreach (string s in stopNamesParis)
                        {
                            if (split[i] == "pont")
                            {
                                throw new FormatException(split[i] + " cannot be condensed. Please use the full stop name.");
                            }
                            //é inputs from chat and in the module are replaced with e. The dash from St-Michel is also optional.
                            var name = Regex.Replace(s, "é", "e").Replace("-", " ").ToLowerInvariant();
                            split[i] = Regex.Replace(split[i], "é", "e").Replace("-", " ");
                            var nameS = name.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
                            if (split2[0].Equals(nameS[0]) && name.StartsWith(split[i]))
                            {
                                if (Set.Contains(stopBtnsParis[Array.IndexOf(stopNamesParis, s)])) return null;
                                Set.Add(stopBtnsParis[Array.IndexOf(stopNamesParis, s)]);
                            }
                        }
                        break;
                }
            }
            //Ignore the command if less than or more than three routes were added
            if (Set.Count == 3) selectables.AddRange(Set);
            else return null;
        }

        //Add submit if command started with submit
        if (submit) selectables.Add(submitBtn);

        //If the command didn't start with set or submit, ignore everything.
        if (!set && !submit) return null;
        return selectables.ToArray();
    }
}
