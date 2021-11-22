using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class OnTriggerEnterForPackForTrip : MonoBehaviour
{



    public string ObjectTag;
    public GameObject Collector;
    public DragAndDrop Ref;
    public GameObject Next_Btn;
    [HideInInspector]
    public bool CanPutInside = false;
    GameObject Temp;
    //public GameObject CoverLid;
    public GameObject[] ObjectsToAppearInCollider;
    public int[] ObjectsToAppearInCollider_Index;
    //ActiveNewObjectsForPetStore NewObjectsInstance;
    GameManager Manager;
    public Animator InfoBtn;
    bool CanPlayInfoSound = false;
    public DragAndDrop mode;


    bool NewEnvelopes = false;
    //public int LevelIndex;
    //public GameObject[] AlreadyPlaced;
    bool IsLevelEnd = false;

    //By Aleem

    [HideInInspector]
    public bool CannotPlace = true;
    public GameObject LevelEndAnimation;

    [HideInInspector]
    public int ObjectsAppearInCollider_Index = 0;
    public int AmountCollectedForLevelEnd;

    public int LevelEndVoiceIndex;

    public GameObject[] Tags;
    public Transform[] ReferencePositions;
    //public int[] ObjectNameSounds;

    public float WaitToPlayAndClick;
    public float PlaySoundInfo = 3.2f;

    public float WaitToPlayRightSound;  //Delay To Play Right Sound After Placing a Correct Item

    public float WaitToPlayOtherObjectSound;  //Delay To Play Other Object's Sound After Placing a Correct Item
    public int[] WrongSounds;
    //public bool IsObjectInteractable = true;
    public GameObject InvisiblePanel;

    RaycastHit hit;
    public bool IsAuntsBirthday = false;

    public bool NowPlayOnlyObjectSound = false;


    // Use this for initialization
    void Start()
    {
        //NewObjectsInstance = transform.GetComponentInParent<ActiveNewObjectsForPetStore> ();
        Manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        StartCoroutine(WaitToPlaySound(PlaySoundInfo));


    }
    void OnEnable()
    {
        Invoke("EnableBool", 6.5f);

        if (GameManager.Instance.Accessibilty)
        {
            AccessibilityManager.instance.DestinationSelected += check;
            AccessibilityManager.instance.levelCompleted += LevleCompletedAccessibility;
            StartCoroutine(EnableAfterDelay()); //By Humza
        }

    }

    void EnableBool()
    {
        if (AccessibilityManager.instance != null)
        {
            AccessibilityManager.instance.CanCheckInfo = true;
        }

    }
    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        if (GameManager.Instance.Accessibilty)
        {
            AccessibilityManager.instance.DestinationSelected -= check;
            AccessibilityManager.instance.levelCompleted -= LevleCompletedAccessibility;
        }

    }
    public void LevleCompletedAccessibility(int counter)
    {
        print("level is completed");

    }
    public void PlaySound(int ind)  //play sound function with both unity audio source and html audio play
    {

        Manager.Audio.clip = Manager.Sounds[ind];
        Debug.Log("GameManager->FadeOnClickBtn(" + ind + ")");
        CloseCaption.CCManager.instance.CreateCaption(ind, Manager.Audio.clip.length);
        Manager.Audio.Play();



    }




    //By Aleem
    public IEnumerator LevleComplete()
    {
        if(EventController.instance != null && !External.Instance.Preview)
            EventController.instance.levelCounter++;

        if (AccessibilityManager.instance != null)
        {
            AccessibilityManager.instance.GreenBox.SetActive(false);//By Humza
            AccessibilityManager.instance.ShowPausePanel = false;//By Humza
            Debug.Log("Good Job.. Its Level End");
            AccessibilityManager.instance.ToggleNaviagtion(false);
            FreezeControlsHandler.Instance.FreezeControlls();
            
        }
        // if(EventController.instance != null && !External.Instance.Preview)
        //     EventController.instance.currentGamePercentage();
        //Debug.Log (Manager.CurrentLevelIndex);
        //EventController.ins.completionPercentage(10);
        //StartCoroutine (mode.WaitToClick (WaitToPlayOtherObjectSound));
        InvisiblePanel.SetActive(true);
        if (AccessibilityManager.instance != null)
        {
            AccessibilityManager.instance.CanCheckInfo = false;
        }
        Debug.Log("Gg");
        yield return new WaitForSeconds(3.5f);
        Debug.Log("Gh");
        //EventController.ins.PrintReport();
        LevelEndAnimation.SetActive(true);
        Debug.Log("Gk");
        if (AccessibilityManager.instance != null)
        {
            AccessibilityManager.instance.ToggleNaviagtion(false);
        }
        //yield return new WaitForSeconds (1.5f);
        PlaySound(LevelEndVoiceIndex);
        if (AccessibilityManager.instance != null)
        {
            AccessibilityManager.instance.ToggleNaviagtion(false);
        }
        Debug.Log("Gkn");
        yield return new WaitForSeconds(1.5f);
        if (AccessibilityManager.instance != null)
        {
            AccessibilityManager.instance.ToggleNaviagtion(false);
        }
        PlaySound(LevelEndVoiceIndex + 1);
        yield return new WaitForSeconds(2.5f);
        Fade mod = Manager.Panels_List[Manager.CurrentLevelIndex].GetComponent<Fade>();

        yield return new WaitForSeconds(1.0f);
        mod.Fadeout = true;
        yield return new WaitForSeconds(1.5f);
        if (AccessibilityManager.instance != null)
        {
            AccessibilityManager.instance.GreenBox.SetActive(true);//By Humza
        }
        InvisiblePanel.SetActive(false);
        Manager.Panels_List[Manager.CurrentLevelIndex].SetActive(false);
        Manager.Panels_List[Manager.CurrentLevelIndex + 1].SetActive(true);
        Manager.CurrentLevelIndex++;
        //		StartCoroutine (EnableAfterDelay ()); //By Humza
        if (AccessibilityManager.instance != null)
        {
            AccessibilityManager.instance.ToggleNaviagtion(false);
        }
        if (GameManager.Instance.Accessibilty)
        {
            AccessibilityManager.instance.changeTarget();
            AccessibilityManager.instance.changeDestination();
        }


    }
    IEnumerator EnableAfterDelay() //By Humza
    {
        yield return new WaitForSeconds(4.5f);
        if (AccessibilityManager.instance != null)
        {
            AccessibilityManager.instance.ShowPausePanel = true;//By Humza
        }
    }

    void Update()
    {

        if (AmountCollectedForLevelEnd == PlayerPrefs.GetInt("AmountCollected"))
        {
            if (AccessibilityManager.instance != null)
            {
                AccessibilityManager.instance.ToggleNaviagtion(false);
            }
            print("In the amount collected condition");
            mode.CanTouchOrClick = false;
            this.gameObject.GetComponent<BoxCollider>().isTrigger = false;

            StartCoroutine(mode.WaitToClick(4.0f));
            StartCoroutine(LevleComplete());
            PlayerPrefs.SetInt("AmountCollected", 0);
        }

        if ((Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)) && CanPutInside)
        {
            if (EventController.instance != null && !External.Instance.Preview)
                EventController.instance.correctOptionSelectionCounter++;
            print("In right place");
            Temp.gameObject.SetActive(false);
            CanPlayInfoSound = false;
            StartCoroutine(WaitToPlaySound(Manager.Sounds[int.Parse(mode.ObjectToDrag.name)].length + 1.3f));
            mode.CanTouchOrClick = false;
            StartCoroutine(mode.WaitToClick(Manager.Sounds[int.Parse(mode.ObjectToDrag.name)].length + 1.3f));

            PlayerPrefs.SetInt("AmountCollected", PlayerPrefs.GetInt("AmountCollected") + 1);
            Debug.Log("in right");

            for (int i = 0; i < ObjectsToAppearInCollider_Index.Length; i++)
            {
                if (int.Parse(mode.ObjectToDrag.name) == ObjectsToAppearInCollider_Index[i])
                {
                    ObjectsToAppearInCollider[i].SetActive(true);
                }
            }

            ObjectsAppearInCollider_Index++;

            PlaySound(int.Parse(mode.ObjectToDrag.name));
            StartCoroutine(WaitToPlayTrueSound(Manager.Sounds[int.Parse(mode.ObjectToDrag.name)].length + 1.0f));

            Ref.CanSetParent = true;
            Ref.DraggingMode = false;
            CanPutInside = false;
            StartCoroutine(wait());
        }

        else if ((Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)) && !CannotPlace)
        {

            Debug.Log("In Wrong");
            if (EventController.instance != null && !External.Instance.Preview)
                EventController.instance.wrongOptionSelectionCounter++;
            if (mode.ObjectToDrag.gameObject.tag != "NotTagged")
            {
                Debug.Log("In Not Place");

                PlaySound(WrongSounds[int.Parse(mode.ObjectToDrag.name)]);
                mode.ObjectToDrag.gameObject.tag = "NotTagged";
                StartCoroutine(NowMakeUntagged(Manager.Sounds[WrongSounds[int.Parse(mode.ObjectToDrag.name)]].length));

            }
            CanPlayInfoSound = false;
            StartCoroutine(WaitToPlaySound(Manager.Sounds[WrongSounds[int.Parse(mode.ObjectToDrag.name)]].length));
            mode.CanTouchOrClick = false;
            StartCoroutine(mode.WaitToClick(Manager.Sounds[WrongSounds[int.Parse(mode.ObjectToDrag.name)]].length));


            for (int i = 0; i < Tags.Length; i++)
            {
                Tags[i].gameObject.transform.position = ReferencePositions[i].transform.position;
            }
            StartCoroutine(wait());

        }

        else if ((Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)))
        {
            Debug.Log("in ending phase");

            for (int i = 0; i < Tags.Length; i++)
            {
                Tags[i].gameObject.transform.position = ReferencePositions[i].transform.position;

            }
        }
    }
    //arslan
    public void check(GameObject dragableObject)
    {

        if (dragableObject.tag == ObjectTag) //right place
        {
            if (GameManager.Instance.Accessibilty)
            {
                AccessibilityManager.instance.ToggleNaviagtion(false);
                // FreezeControlsHandler.Instance.FreezeControlls();
                if (TextToSpeech.ins != null)
                    TextToSpeech.ins.StopAudio();
            }
            if (EventController.instance != null && !External.Instance.Preview)
                EventController.instance.correctOptionSelectionCounter++;
            print("In right place");
            dragableObject.gameObject.SetActive(false);
            CanPlayInfoSound = false;
            StartCoroutine(WaitToPlaySound(Manager.Sounds[int.Parse(dragableObject.name)].length + 1.3f));
            mode.CanTouchOrClick = false;
            StartCoroutine(mode.WaitToClick(Manager.Sounds[int.Parse(dragableObject.name)].length + 1.3f));

            PlayerPrefs.SetInt("AmountCollected", PlayerPrefs.GetInt("AmountCollected") + 1);
            Debug.Log("in right");
            if (AccessibilityManager.instance != null)
            {
                AccessibilityManager.instance.ShowPausePanel = false;
            }

            for (int i = 0; i < ObjectsToAppearInCollider_Index.Length; i++)
            {
                if (int.Parse(dragableObject.name) == ObjectsToAppearInCollider_Index[i])
                {
                    ObjectsToAppearInCollider[i].SetActive(true);
                }
            }

            ObjectsAppearInCollider_Index++;

            PlaySound(int.Parse(dragableObject.name));
            StartCoroutine(WaitToPlayTrueSound(Manager.Sounds[int.Parse(dragableObject.name)].length + 1.0f));

            Ref.CanSetParent = true;
            Ref.DraggingMode = false;
            CanPutInside = false;
            StartCoroutine(wait());
            // if (GameManager.Instance.Accessibilty)
            //     AccessibilityManager.instance.ToggleNaviagtion(true);
            //  AccessibilityManager.instance.SwitchToPreviousState();
            if (GameManager.Instance.Accessibilty)
                StartCoroutine(playAccessiblrSound());
        }
        else
        {
            if (GameManager.Instance.Accessibilty)
            {
                AccessibilityManager.instance.ToggleNaviagtion(false);
                if (TextToSpeech.ins != null)
                    TextToSpeech.ins.StopAudio();
            }
            Debug.Log("In Wrong");
            if (AccessibilityManager.instance != null)
            {
                AccessibilityManager.instance.ShowPausePanel = false;
            }

            if (EventController.instance != null && !External.Instance.Preview)
                EventController.instance.wrongOptionSelectionCounter++;
            if (dragableObject.tag != "NotTagged")
            {
                Debug.Log("In Not Place");

                PlaySound(WrongSounds[int.Parse(dragableObject.name)]);
                dragableObject.tag = "NotTagged";
                StartCoroutine(NowMakeUntagged(Manager.Sounds[WrongSounds[int.Parse(dragableObject.name)]].length));

            }
            CanPlayInfoSound = false;
            StartCoroutine(WaitToPlaySound(Manager.Sounds[WrongSounds[int.Parse(dragableObject.name)]].length));
            mode.CanTouchOrClick = false;
            StartCoroutine(mode.WaitToClick(Manager.Sounds[WrongSounds[int.Parse(dragableObject.name)]].length));


            for (int i = 0; i < Tags.Length; i++)
            {
                Tags[i].gameObject.transform.position = ReferencePositions[i].transform.position;
            }
            StartCoroutine(wait());
            if (GameManager.Instance.Accessibilty)
                StartCoroutine(playAccessiblrSound());
        }
    }

    public void callInfo()
    {
        if (AccessibilityManager.instance != null)
        {
            AccessibilityManager.instance.Info();
        }
        Invoke("EnablePause", 1.5f);
    }

    void EnablePause()
    {
        if (AccessibilityManager.instance != null)
        {
            AccessibilityManager.instance.ShowPausePanel = true;
        }
    }
    IEnumerator playAccessiblrSound()
    {

        yield return new WaitForSeconds(5f);
        AccessibilityManager.instance.resetCurrentState();
        AccessibilityManager.instance.ToggleNaviagtion(true);
        // yield return new WaitForSeconds(1f);
        // FreezeControlsHandler.Instance.UnFreezeControlls();
    }

    void OnTriggerEnter(Collider Col) //on object enter inside the container
    {

        if (Col.gameObject.tag == ObjectTag)
        {
            Temp = Col.gameObject;

            CanPutInside = true;
            NewEnvelopes = true;

        }
        if (Col.gameObject.tag != ObjectTag)
        {
            Temp = Col.gameObject;
            CannotPlace = false;
        }
    }

    void OnTriggerExit(Collider Col) //on object exit from the container
    {
        if (Col.gameObject.tag == ObjectTag)
        {
            Ref.CanSetParent = false;
            CanPutInside = false;
            NewEnvelopes = false;
            if (Input.GetMouseButtonUp(0))
            {
                PlaySound(5);
            }
        }

        if (Col.gameObject.tag != ObjectTag)
        {
            Temp = Col.gameObject;
            CannotPlace = true;
        }
    }

    IEnumerator wait()
    {
        yield return new WaitForSeconds(0.3f);
        Ref.CanSetParent = false;


    }

    public void LevelEnd() //called at end of each level
    {
        PlayerPrefs.SetInt("AmountCollected", 0);
        //ObjectsToAppear.SetActive (false);
        Next_Btn.SetActive(false);
    }

    //for opening and closing info button
    public void OpenInfo(int ind)
    {
        if (CanPlayInfoSound)
        {

            Manager.Audio.clip = Manager.Sounds[ind];
            Debug.Log("GameManager->FadeOnClickBtn(" + ind + ")");
            CloseCaption.CCManager.instance.CreateCaption(ind, Manager.Audio.clip.length);
            Manager.Audio.Play();


            CanPlayInfoSound = false;
            StartCoroutine(WaitToPlaySound(WaitToPlayAndClick));
            mode.CanTouchOrClick = false;
            StartCoroutine(mode.WaitToClick(WaitToPlayAndClick));

        }
        InfoBtn.gameObject.SetActive(true);
        InfoBtn.SetBool("CloseInfo", false);
        InfoBtn.SetBool("OpenInfo", true);
    }

    public void CloseInfo()
    {

        InfoBtn.SetBool("CloseInfo", true);
        InfoBtn.SetBool("OpenInfo", false);
        StartCoroutine(WaitSec());


    }

    public IEnumerator WaitToPlaySound(float sec)
    {
        yield return new WaitForSeconds(sec);
        CanPlayInfoSound = true;

    }

    IEnumerator WaitSec()
    {
        yield return new WaitForSeconds(0.5f);
        InfoBtn.gameObject.SetActive(false);

    }

    IEnumerator WaitToPlayTrueSound(float nowPlay)
    {
        yield return new WaitForSeconds(nowPlay);
        //Debug.Log ("Thing Name");
        Debug.Log("INFO IS ENABLED");
        Invoke("callInfo", 2f);
        if (IsAuntsBirthday)
        {
            PlaySound(28);
        }
        else
        {
            PlaySound(22);
        }


    }

    IEnumerator NowMakeUntagged(float s)
    {
        yield return new WaitForSeconds(s);
        Debug.Log("INFO IS ENABLED");
        Invoke("callInfo", 2f);
        CannotPlace = true;
    }
}