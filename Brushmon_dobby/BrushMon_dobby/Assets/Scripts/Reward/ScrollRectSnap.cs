using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScrollRectSnap : MonoBehaviour
{

    public RectTransform panelRectTransform;
    public GameObject panel;
    public GameObject[] packages;
    public RectTransform center;


    private float[] distance;
    private bool dragging = false;
    private int bttnDistance;
    private int minButtonNum;

    public float value;

    void Start()
    {
        //  packages = GameObject.FindGameObjectsWithTag("Package");


    }

    internal void initailize(){
        GameObject package = Instantiate(Resources.Load("Prefabs/Reward/Package")) as GameObject;
        GameObject locked = Instantiate(Resources.Load("Prefabs/Reward/Locked")) as GameObject;
        Vector2 size = panel.GetComponent<RectTransform>().sizeDelta;
        package.transform.SetParent(panel.transform, false);
        package.GetComponent<RectTransform>().anchoredPosition = new Vector2((940f + 20f) * 0f, 0f);
        locked.transform.SetParent(panel.transform, false);
        locked.GetComponent<RectTransform>().anchoredPosition = new Vector2((940f + 20f) * 1f, 0f);

        packages = new GameObject[2];
        packages[0] = package;
        packages[1] = locked;
        int bttnLength = packages.Length;
        distance = new float[bttnLength];

        bttnDistance = (int)Mathf.Abs(packages[1].GetComponent<RectTransform>().anchoredPosition.x - packages[0].GetComponent<RectTransform>().anchoredPosition.x);

        AudioManager.Instance.playEffect("reward_collection_pop");

    }

    void Update()
    {
        if (packages.Length > 0)
        {
            for (int i = 0; i < packages.Length; i++)
            {

                distance[i] = Mathf.Abs(center.transform.position.x - packages[i].transform.position.x);

            }

            float minDistance = Mathf.Min(distance);

            for (int a = 0; a < packages.Length; a++)
            {

                if (minDistance == distance[a])
                {

                    minButtonNum = a;

                }

            }

            if (!dragging)
            {

                LerpToBttn(minButtonNum * -bttnDistance);

            }
        }
    }

    void LerpToBttn(int position)
    {
        float newX = Mathf.Lerp(panelRectTransform.anchoredPosition.x, position, Time.deltaTime * 5f);
        Vector2 newPosition = new Vector2(newX, panelRectTransform.anchoredPosition.y);
        panelRectTransform.anchoredPosition = newPosition;

    }

    public void StartDrag()
    {

        dragging = true;

    }

    public void EndDrag()
    {

        dragging = false;

    }
}