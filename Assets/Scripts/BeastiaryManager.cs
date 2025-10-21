using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BeastiaryManager : MonoBehaviour
{
    public static BeastiaryManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    [Header("Pages")]
    [SerializeField] RectTransform[] journalPages;

    private TMP_Text GetTitle(RectTransform page)
    {
        return page.Find("Title").GetComponent<TMP_Text>();
    }

    void OnEnable()
    {
        EventManager.Instance.OnDayStart += HidePages;
    }

    void OnDisable()
    {
        EventManager.Instance.OnDayStart -= HidePages;
    }

    private TMP_Text GetEntry(RectTransform page)
    {
        if (page.Find("Text") == null)
        {
            return page.Find("Content").GetComponent<TMP_Text>();
        }
    
        return page.Find("Text").GetComponent<TMP_Text>();
    }

    public RectTransform GetPageFromAnomalyName(String anomalyName)
    {
        /*
        0: Page 2 front
        floating skull

        1: Page 2 back
        new door

        2: Page 3 front
        weather bewilder

        3: Page 3 back
        topsy table

        4: Page 4 front
        cat -> dog

        5: Page 4 back
        chair stack

        6: Page 5 front
        radio on roof

        7: Page 5 back
        cat photo swap

        8: Page 6 front
        toilet on roof

        9: Page 6 back
        bathroom window swap

        10: Page 7 front
        time twisting
        */

        int pageIndex = -1;

        switch (anomalyName)
        {
            case "Floating Skull":
                pageIndex = 0;
                break;

            case "Door to Nowhere":
                pageIndex = 1;
                break;

            case "Weather Bewilder":
                pageIndex = 2;
                break;

            case "Topsy Table":
                pageIndex = 3;
                break;

            case "Dogcat/Catdog":
                pageIndex = 4;
                break;

            case "Chair Ikon":
                pageIndex = 5;
                break;

            case "Upside Down Radio":
                pageIndex = 6;
                break;

            case "Picture Misplacement":
                pageIndex = 7;
                break;

            case "Plumbing mis,,,Plumbing":
                pageIndex = 8;
                break;

            case "Bathroom Time":
                pageIndex = 9;
                break;

            case "Time Twisting":
                pageIndex = 10;
                break;
        }

        return journalPages[pageIndex];
    }

    private void HidePages(int dayNumber)
    {
        foreach (RectTransform page in journalPages)
        {
            GetTitle(page).text = "<i>Unknown Anomaly</i>";
            GetEntry(page).text = "I haven't encountered this anomaly yet!";
        }
    }

    private void SetAnomalyPage(Anomaly anomaly)
    {
        RectTransform page = GetPageFromAnomalyName(anomaly.journalPageTitle);
        GetTitle(page).text = anomaly.journalPageTitle;
        GetEntry(page).text = anomaly.journalPageEntry;
    }

    public void UpdatePages(List<Anomaly> anomalies)
    {
        foreach (Anomaly anomaly in anomalies)
        {
            SetAnomalyPage(anomaly);
        }
    }
}