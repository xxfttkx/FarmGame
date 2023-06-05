using System.Collections;
using System.Collections.Generic;
using MFarm.Save;
using UnityEngine;
public class Player : Singleton<Player>, ISaveable
{
    private Rigidbody2D rb;

    public float speed;
    private float inputX;
    private float inputY;
    private Vector2 movementInput;

    private Animator[] animators;
    private bool isMoving { get { return !inputDisable && movementInput != Vector2.zero; } }
    public bool inputDisable;

    //使用工具动画
    private float mouseX;
    private float mouseY;
    private bool useTool;
    public GameState gameState;
    private bool canPlayFootSound;
    private float ToPrevPlayFootSoundTime;
    public string GUID => GetComponent<DataGUID>().guid;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        animators = GetComponentsInChildren<Animator>();
        inputDisable = true;
        gameState = GameState.GameEnd;
        canPlayFootSound = true;
    }

    private void Start()
    {
        ISaveable saveable = this;
        saveable.RegisterSaveable();
    }

    private void OnEnable()
    {
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
        EventHandler.MoveToPosition += OnMoveToPosition;
        EventHandler.MouseClickedEvent += OnMouseClickedEvent;
        EventHandler.UpdateGameStateEvent += OnUpdateGameStateEvent;
        EventHandler.StartNewGameEvent += OnStartNewGameEvent;
        EventHandler.EndGameEvent += OnEndGameEvent;
        EventHandler.GamePauseEvent += () => { inputDisable = true; };
        EventHandler.GameResumeEvent += () => { inputDisable = false; };
    }

    private void OnDisable()
    {
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
        EventHandler.MoveToPosition -= OnMoveToPosition;
        EventHandler.MouseClickedEvent -= OnMouseClickedEvent;
        EventHandler.UpdateGameStateEvent -= OnUpdateGameStateEvent;
        EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
        EventHandler.EndGameEvent -= OnEndGameEvent;
        EventHandler.GamePauseEvent -= () => { inputDisable = true; };
        EventHandler.GameResumeEvent -= () => { inputDisable = false; };
    }

    private void Update()
    {
        SwitchAnimation();
        if (canPlayFootSound)
        {

        }
        else
        {
            ToPrevPlayFootSoundTime += Time.deltaTime;
            if (ToPrevPlayFootSoundTime > 0.5f)
            {
                canPlayFootSound = true;
                ToPrevPlayFootSoundTime = 0f;
            }
        }
    }

    private void FixedUpdate()
    {
        if (!inputDisable)
        {
            PlayerInput();
            Movement();
        }
    }

    private void OnStartNewGameEvent(int obj)
    {
        // inputDisable = false;
        transform.position = Settings.playerStartPos;
    }

    private void OnEndGameEvent()
    {
        inputDisable = true;
    }

    private void OnUpdateGameStateEvent(GameState gameState)
    {
        this.gameState = gameState;
        switch (gameState)
        {
            case GameState.Gameplay:
                inputDisable = false;
                break;
            case GameState.Pause:
                inputDisable = true;
                break;
            case GameState.GameEnd:
                inputDisable = true;
                break; 
            default:break;
        }
    }

    private void OnMouseClickedEvent(Vector3 mouseWorldPos, ItemDetails itemDetails)
    {
        if (useTool)
            return;

        if (itemDetails.itemType != ItemType.Seed && itemDetails.itemType != ItemType.Commodity 
            && itemDetails.itemType != ItemType.Furniture && itemDetails.itemType != ItemType.ReturnTool)
        {
            mouseX = mouseWorldPos.x - transform.position.x;
            mouseY = mouseWorldPos.y - (transform.position.y + 0.85f);

            if (Mathf.Abs(mouseX) > Mathf.Abs(mouseY))
                mouseY = 0;
            else
                mouseX = 0;

            StartCoroutine(UseToolRoutine(mouseWorldPos, itemDetails));
        }
        else
        {
            EventHandler.CallExecuteActionAfterAnimation(mouseWorldPos, itemDetails);
        }
    }

    private IEnumerator UseToolRoutine(Vector3 mouseWorldPos, ItemDetails itemDetails)
    {
        useTool = true;
        inputDisable = true;
        yield return null;
        foreach (var anim in animators)
        {
            anim.SetBool("useTool",useTool);
            //人物的面朝方向
            anim.SetFloat("InputX", mouseX);
            anim.SetFloat("InputY", mouseY);
        }
        yield return new WaitForSeconds(0.45f);
        EventHandler.CallExecuteActionAfterAnimation(mouseWorldPos, itemDetails);
        yield return new WaitForSeconds(0.25f);
        //等待动画结束
        useTool = false;
        inputDisable = false;
        foreach (var anim in animators)
        {
            anim.SetBool("useTool",useTool);
            //人物的面朝方向
            anim.SetFloat("InputX", mouseX);
            anim.SetFloat("InputY", mouseY);
        }
    }
    private void OnMoveToPosition(Vector3 targetPosition)
    {
        transform.position = targetPosition;
    }

    private void OnAfterSceneLoadedEvent()
    {
        //inputDisable = false;
    }

    private void OnBeforeSceneUnloadEvent()
    {
        //inputDisable = true;
    }


    private void PlayerInput()
    {
        // if (inputY == 0)
        inputX = Input.GetAxisRaw("Horizontal");
        // if (inputX == 0)
        inputY = Input.GetAxisRaw("Vertical");
        if (inputX != 0 && inputY != 0)
        {
            inputX = inputX * 0.6f;
            inputY = inputY * 0.6f;
        }

        //走路状态速度
        if (Input.GetKey(KeyCode.LeftShift))
        {
            inputX = inputX * 0.5f;
            inputY = inputY * 0.5f;
        }
        movementInput = new Vector2(inputX, inputY);

    }

    private void Movement()
    {
        if(isMoving)
        {
            rb.MovePosition(rb.position + movementInput * speed * Time.deltaTime);
            if (canPlayFootSound)
            {
                EventHandler.CallPlaySoundEvent(SoundName.FootStepHard);
                canPlayFootSound = false;
            }
        } 
    }

    private void SwitchAnimation()
    {
        foreach (var anim in animators)
        {
            anim.SetBool("isMoving", isMoving);
            anim.SetFloat("mouseX", mouseX);
            anim.SetFloat("mouseY", mouseY);

            if (isMoving)
            {
                anim.SetFloat("InputX", inputX);
                anim.SetFloat("InputY", inputY);
            }
        }
    }
    public void SetToScreen()
    {
        foreach (var anim in animators)
        {
            //人物的面朝方向
            anim.SetFloat("InputX", 0);
            anim.SetFloat("InputY", -1);
        }
    }

    public GameSaveData GenerateSaveData()
    {
        GameSaveData saveData = new GameSaveData();
        saveData.characterPosDict = new Dictionary<string, SerializableVector3>();
        saveData.characterPosDict.Add(this.name, new SerializableVector3(transform.position));

        return saveData;
    }

    public void RestoreData(GameSaveData saveData)
    {
        var targetPosition = saveData.characterPosDict[this.name].ToVector3();

        transform.position = targetPosition;
    }
}
