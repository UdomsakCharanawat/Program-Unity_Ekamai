using UnityEngine;

public class player : MonoBehaviour
{
    public bool lockPos;
    public float moveSpeed = 5f;

    public Vector2 screenMin = new Vector2(-960, -540);
    public Vector2 screenMax = new Vector2(960, 540);

    [Header("object")]
    public GameObject[] obj_lockPosition;
    public GameObject[] obj_pickuptools;
    public GameObject[] obj_fix;
    [Header("player")]
    public GameObject playerBack;
    public GameObject playerFrontGlow;
    public GameObject playerFront;
    public GameObject playerFrontHand;
    public GameObject pistolGripTool;
    [Header("point")]
    public GameObject[] pointFix;

    public GameObject[] enableFirst;
    public GameObject[] disableFirst;



    private Rigidbody2D rb;
    private Vector2 moveInput;

    public mission missionRef;

    public float driftStrength = 0.5f;
    private Vector2 driftDirection;
    private float driftTimer = 0f;
    private float driftChangeInterval = 2f;

    public void OnEnable()
    {
        for (int i = 0; i < enableFirst.Length; i++)
            enableFirst[i].SetActive(true);
        for (int i = 0; i < disableFirst.Length; i++)
            disableFirst[i].SetActive(false);
        for(int i = 0; i< pointFix.Length; i++)
            pointFix[i].SetActive(true);
    }

    public void useObject(GameObject[] obj, bool status)
    {
        for (int i = 0; i < obj.Length; i++)
            obj[i].SetActive(status);
    }

    void Start() => rb = GetComponent<Rigidbody2D>();

    void Update()
    {
        if (!missionRef._gameManager.gameOver)
        {
            moveInput = new Vector2(
            Input.GetKey(KeyCode.D) ? 1 : Input.GetKey(KeyCode.A) ? -1 : 0,
            Input.GetKey(KeyCode.W) ? 1 : Input.GetKey(KeyCode.S) ? -1 : 0
        );

            if (missionRef.inArea)
            {
                if (Input.GetKeyDown(KeyCode.X) && !lockPos)
                {
                    lockPos = true;
                    useObject(obj_lockPosition, false);
                    useObject(obj_pickuptools, true);

                    playerFrontGlow.SetActive(false);

                    pointFix[missionRef.stayPointFix].SetActive(false);
                }
            }

            if (lockPos)
            {
                if (Input.GetKeyDown(KeyCode.C) && playerFront.activeInHierarchy)
                {
                    useObject(obj_pickuptools, false);
                    useObject(obj_fix, true);

                    playerFront.SetActive(false);
                    playerFrontHand.SetActive(true);
                    pistolGripTool.SetActive(true);
                }

                if (Input.GetKeyDown(KeyCode.X) && pistolGripTool.activeInHierarchy)
                {
                    useObject(obj_fix, false);

                    playerFrontHand.SetActive(false);
                    pistolGripTool.SetActive(false);

                    playerBack.SetActive(true);
                    missionRef.PauseTimer();
                }
            }
        }

    }

    public void fixFinish()
    {
        missionRef.addPointFiexed();

        playerBack.SetActive(false);
        playerFront.SetActive(true);

        lockPos = false;

        missionRef.ResumeTimer();
    }

    void FixedUpdate()
    {
        if (!lockPos)
        {
            rb.AddForce(moveInput.normalized * moveSpeed);

            if (moveInput == Vector2.zero)
            {
                driftTimer += Time.fixedDeltaTime;
                if (driftTimer >= driftChangeInterval)
                {
                    driftDirection = Random.insideUnitCircle.normalized;
                    driftTimer = 0f;
                }

                rb.AddForce(driftDirection * driftStrength);
            }

            Vector3 nextLocalPos = transform.localPosition + (Vector3)(rb.velocity * Time.fixedDeltaTime);

            float clampedX = Mathf.Clamp(nextLocalPos.x, screenMin.x, screenMax.x);
            float clampedY = Mathf.Clamp(nextLocalPos.y, screenMin.y, screenMax.y);

            transform.localPosition = new Vector3(clampedX, clampedY, transform.localPosition.z);
        }
        else
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.Sleep();

            transform.localPosition = transform.localPosition;
        }

    }

}
