using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class State : Cycle
{


    public UnityEvent OnLiveEvent;
    public bool DOFULL; // Sets up our full state on rebuild


    public AudioClip selectionClip;


    public bool hasFallen;
    public bool hasPickedUpBook;

    public int whichStoryLoop;

    public bool monolithParticlesEmitting;
    public int whichMonolithEmitting;

    public List<StorySetter> storiesVisited;
    public List<StorySetter> storiesCompleted;

    public int currentSetter;
    private int oCurrentStory;

    public float lastTimeStoryVisited;

    public bool inStory;
    public bool inPages;
    public bool inBook;
    public bool inBookPages;




    public int currentConnectedMonolith;

    public int startSetter;
    public int startStory;
    public int startPage;

    public bool startInStory;
    public bool startInPages;
    public bool startInBook;
    public bool startInBookPages;

    public bool startWithStoryState;

    // makes it so we can jump in for the fist story instead of transition;
    public bool firstStory;




    public StorySetter setter;
    public StorySetter oSetter;
    public Story story;

    public bool fast;



    public bool isMonolithOn;
    private bool oIsMonolithOn;

    public Transform monolithTarget;
    private Transform oMonolithTarget;

    public string animationState;
    private string oAnimationState;

    public Tutorial tutorial;


    public bool frameShown;

    public bool inPerimeter;
    public bool storyFinished;
    public float perimeterFadeVal;


    public int reincarnationNumber;


    public override void Create()
    {

        currentSetter = startSetter;

        // Regenerating when we create!
        storiesVisited = new List<StorySetter>();
        storiesCompleted = new List<StorySetter>();

        SafeInsert(tutorial);

    }

    public void SetStartToCurrentStory()
    {
        startSetter = currentSetter;
        setter = data.journey.setters[startSetter];
        startStory = setter.currentStory;

        startPage = setter.stories[startStory].currentPage;
    }




    public override void OnLive()
    {

        
        data.book.CloseBook();

        //public string 
        if (startInStory || startInPages)
        {


            // Set our setter to the correct ID
            currentSetter = startSetter;


            // debug
            if (startSetter > data.journey.setters.Length) { print("starting setter greater than total Setters"); }

            // Get our actual setter
            setter = data.journey.setters[startSetter];


            // Set our player Position
            data.player.position = data.journey.setters[currentSetter].transform.position;

            // Set our terrain position to teh correct position
            data.terrainTap.SetTransform(data.journey.setters[currentSetter].transform);


            // As long as we are starting correctly
            if (startStory >= 0)
            {

                if (setter.stories[startStory] != null)
                {
                   

                    // Get our actual story
                    story = setter.stories[startStory];

                    // Set our state to our current story
                    SetStoryState(story);

                    // Enter our perimeter to toggle everything correctly
                    setter.perimeter.EnterOuter();
                    setter.perimeter.EnterInner();

                }
                else
                {
                    Debug.Log("NO SUB STORY!!!");
                }
            }


            //data.journey.setters[data.journey.currentSetter].
        }else{


            if( startWithStoryState ){

                // Set our setter to the correct ID
            currentSetter = startSetter;


            // debug
            if (startSetter > data.journey.setters.Length) { print("starting setter greater than total Setters"); }

            // Get our actual setter
            setter = data.journey.setters[startSetter];

                print("Setting story info");

                // Get our actual story
                story = setter.stories[startStory];

                // Set our state to our current story
                SetStoryState(story);
            }

        }

        inStory = startInStory;
        inPages = startInPages;





        if (hasFallen)
        {
            //print("HAS FALLEN");

            data.playerControls.animator.SetBool("FallAsleep", false);
            data.playerControls.animator.SetBool("Falling", false);
            data.playerControls.animator.SetBool("GetUp", true);
            data.playerControls.animator.Play("Grounded");

            storiesVisited.Add(data.journey.setters[0]);
        }
        else
        {
            //data.playerControls.Fall();
        }


        //data.journey.inStory = startInStory;
        if (startInStory && !startInPages)
        {
            data.cameraControls.SetFollowTarget();
        }


        if (startInPages)
        {


            //        print( data.journey.controller.currentPage );


            // data.journey.controller 
            data.journey.controller.currentPageID = startPage;
            data.journey.controller.currentPage = data.journey.controller.CP;
            data.journey.controller.SetAllEvents();
            firstStory = true;

            data.journey.controller.StartStory();

            if (startPage != 0)
            {


                data.framer.Set(data.journey.controller.currentPage);
            }


        }
        else
        {
            data.journey.controller.started = false;

            // IF we have any old things playing, stop them playing now!
            for (int i = 0; i < data.journey.setters.Length; i++)
            {
                if (data.journey.setters[i].audio.playing) { data.journey.setters[i].audio.Exit(); }
            }

            // Fade in the global sound!
            data.sound.globalLooper.FadeIn();

        }

        firstStory = false;


        if (DOFULL)
        {
            if (startInBook)
            {
                //print("open books");
                data.book.OpenBook();
            }

            if (hasPickedUpBook)
            {
                PickUpBook();
            }
            else
            {
                PutDownBook();
            }


            if (!monolithParticlesEmitting)
            {
                DisconnectMonolith(whichMonolithEmitting);
            }
            else
            {
                ConnectMonolith(whichMonolithEmitting);
            }
        }


        /*   if( startInBookPages ){
           data.book.OpenStory(data.book.currentSetter);
         }*/


        OnLiveEvent.Invoke();
    }



    public override void WhileLiving(float v)
    {

        // TODO
        if (isMonolithOn != oIsMonolithOn) { }
        if (monolithTarget != oMonolithTarget) { }


      
    }





    public void Fall()
    {

    }

    public void Stand()
    {

    }


    public void PutDownBook()
    {

        if (data.playerControls.epiphanyRing.circle.body.mpb == null)
        {
            data.playerControls.epiphanyRing.circle.body.mpb = new MaterialPropertyBlock();
            print("Body material getting recreated");
        }
        data.playerControls.epiphanyRing.circle.body.mpb.SetFloat("_StartTime", Time.time);
        data.playerControls.epiphanyRing.circle.body.mpb.SetFloat("_Setting", 0);
        data.playerControls.epiphanyRing.UnSet();
        data.playerControls.OnGroundBook.SetActive(true);
        data.playerControls.InHandBook.SetActive(false);
        data.state.hasPickedUpBook = false;
    }

    public void PickUpBook()
    {


        //    print("Pick Up Book");
        //data.playerControls.epiphanyRing.Set();
        data.playerControls.OnGroundBook.SetActive(false);
        data.playerControls.InHandBook.SetActive(true);
        data.state.hasPickedUpBook = true;
    }


    public void SetStoryState(Story s)
    {

        if (s.state == null) { s.DebugThis("THIS STORY HAS NO STATE"); }
        else
        {
            hasPickedUpBook = s.state.bookPickedUp;
            hasFallen = s.state.hasFallen;
            monolithParticlesEmitting = s.state.monolithParticlesEmitting;
            whichMonolithEmitting = s.state.whichMonolithEmitting;
            reincarnationNumber = s.state.reincarnationNumber;

            for (int i = 0; i < s.state.storiesNotVisited.Length; i++)
            {
                if (storiesVisited.IndexOf(s.state.storiesNotVisited[i]) >= 0)
                {
                    storiesVisited.Remove(s.state.storiesNotVisited[i]);
                }
            }

            for (int i = 0; i < s.state.storiesVisited.Length; i++)
            {
                if (storiesVisited.IndexOf(s.state.storiesVisited[i]) < 0)
                {
                    storiesVisited.Add(s.state.storiesVisited[i]);
                }
            }



             for (int i = 0; i < s.state.storiesNotCompleted.Length; i++)
            {
                if (storiesCompleted.IndexOf(s.state.storiesNotCompleted[i]) >= 0)
                {
                    storiesCompleted.Remove(s.state.storiesNotCompleted[i]);
                }
            }

            for (int i = 0; i < s.state.storiesCompleted.Length; i++)
            {
                if (storiesCompleted.IndexOf(s.state.storiesCompleted[i]) < 0)
                {
                    storiesCompleted.Add(s.state.storiesCompleted[i]);
                }
            }

        }

    }



    public void ConnectMonolith(int id)
    {


        whichMonolithEmitting = id;
        monolithParticlesEmitting = true;
        //print("Connecting monolith");

        Shader.SetGlobalInt("_ConnectedStory", whichMonolithEmitting);
        data.monolithParticles._Emit = 1;

        // If we pass in negative ID its going to be the ground connected book!
        if (id >= 0)
        {
            data.monolithParticles._EmitterPosition = data.journey.monoSetters[id].monolith.transform.position;
        }
        else
        {
            data.monolithParticles._EmitterPosition = data.playerControls.OnGroundBook.transform.position;
        }

    }

    public void DisconnectMonolith(int id)
    {
        //print("discordingngn");
        whichMonolithEmitting = -10;
        monolithParticlesEmitting = false;
        Shader.SetGlobalInt("_ConnectedStory", -10);
        data.monolithParticles._Emit = 0;
    }


    public void PlaySelection()
    {
        // print("WASsds");
        data.sound.Play(selectionClip, 1f, 1f);
    }



    public Story CS
    {
        get { return setter.stories[setter.currentStory]; }
    }

    public Page CP
    {
        get { return setter.CS.pages[setter.CS.currentPage]; }
    }



    public void SetterEnterOuter(StorySetter s)
    {
        oSetter = setter;
        setter = s;
        lastTimeStoryVisited = Time.time;
        inStory = true;
    }

    public void SetterEnterInner(StorySetter s)
    {

    }

    public void SetterExitOuter(StorySetter s)
    {
        oSetter = setter;
        setter = null;
        inStory = false;
        lastTimeStoryVisited = Time.time;

        if(endAfterSingleStory){
            DoEndFade();
        }

    
        
    }

    public void SetterExitInner(StorySetter s)
    {


    }


    public void SetSetter(StorySetter s)
    {
        oSetter = setter;
        setter = s;
    }


    public void UnsetSetter()
    {
        oSetter = setter;
        setter = null;
    }


    public bool endAfterSingleStory;
    public FadeToBlack ftb;
    public Texture fadeTexture;
    public void DoEndFade(){

        ftb.OnFadeEnd.AddListener(end);
        ftb.FadeToTexture(fadeTexture,3);
        

    }

    public void end(){
        Application.Quit();
        ftb.OnFadeEnd.RemoveListener(end);
    }
}
