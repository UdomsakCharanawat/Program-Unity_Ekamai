using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class dronePlayerFire : MonoBehaviour
{
    [Header("Movement Settings")]
    public float horizontalForce = 10f;
    public float verticalForce = 8f;
    public float maxSpeed = 5f;

    [Header("Scale setting")]
    public float maxScale = 1f;
    public float minScale = 0.4f;
    public GameObject obj_scale;

    [Header("Movement Bounds")]
    public float minX = -470f;
    public float maxX = 470f;
    public float minY = -95f;
    public float maxY = 95f;

    private Rigidbody rb;
    private Vector3 inputDirection;

    private List<GameObject> detectedEnemies = new List<GameObject>();
    private Dictionary<GameObject, Coroutine> enemyTimers = new Dictionary<GameObject, Coroutine>();
    [Header("Scale Curve")]
    public AnimationCurve scaleCurve = AnimationCurve.Linear(0f, 1f, 1f, 0.5f);


    public GameObject obj_enemyActive;
    public GameObject obj_graphicPoint;
    public GameObject obj_panelCapture;

    public fireDrone _fireDrone;
    public fireRandom _fireRandom;

    private void OnEnable()
    {
        obj_enemyActive.SetActive(false);
        obj_graphicPoint.SetActive(false);
        obj_panelCapture.SetActive(false);

        rb.position = new Vector3(0, -130, 714);
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    void Update()
    {
        if (_fireDrone.play)
        {
            float h = Input.GetKey(KeyCode.A) ? -1 : Input.GetKey(KeyCode.D) ? 1 : 0;
            float y = Input.GetKey(KeyCode.W) ? 1 : Input.GetKey(KeyCode.S) ? -1 : 0;
            float z = Input.GetKey(KeyCode.Space) ? 1 : Input.GetKey(KeyCode.LeftShift) ? -1 : 0;

            inputDirection = new Vector3(h, y, z).normalized;

            if (Input.GetKeyDown(KeyCode.Q))
            {
                foreach (GameObject enemy in detectedEnemies)
                {
                    Transform point = enemy.transform.Find("point");
                    if (point != null && point.gameObject.activeSelf)
                    {
                        point.gameObject.SetActive(false);
                        obj_panelCapture.SetActive(true);
                        obj_enemyActive.SetActive(false);
                        obj_graphicPoint.SetActive(false);

                        _fireDrone.getCapture++;
                        _fireDrone.progress[_fireDrone.getCapture - 1].SetActive(true);

                        StartCoroutine(_fireRandom.waitRandomFirst());

                        if (_fireDrone.getCapture >= 12)
                        {
                            _fireDrone.play = false;
                            Debug.Log("game winner");
                            _fireDrone.statusEndGame = "win";
                            _fireDrone.checkEndGame();

                        }
                    }
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (!_fireDrone.play)
        {
            Vector3 target = new Vector3(0f, -415f, rb.position.z);
            Vector3 direction = (target - rb.position).normalized;

            float distance = Vector3.Distance(rb.position, target);

            
            if (distance > 0.1f)
            {
                Vector3 force = new Vector3(direction.x * horizontalForce, direction.y * verticalForce, 0f);
                rb.AddForce(force, ForceMode.Acceleration);
            }
            else
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            return; 
        }


        Vector3 forceInput = new Vector3(inputDirection.x * horizontalForce, inputDirection.y * verticalForce, inputDirection.z * horizontalForce);

        if (rb.velocity.magnitude < maxSpeed)
        {
            rb.AddForce(forceInput, ForceMode.Acceleration);
        }

        Vector3 targetPosition = rb.position;
        targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);

        rb.MovePosition(targetPosition);
        
        float t = Mathf.InverseLerp(maxY, minY, targetPosition.y); // t = 0 → ล่างสุด, t = 1 → บนสุด
        float currentScale = scaleCurve.Evaluate(t);

        if (obj_scale != null)
        {
            obj_scale.transform.localScale = Vector3.one * currentScale;
        }


        float tRot = Mathf.InverseLerp(minX, maxX, targetPosition.x); // t = 0 → ซ้ายสุด, t = 1 → ขวาสุด
        float yRotation = Mathf.Lerp(165f, 195f, tRot); 

        if (obj_scale != null)
        {
            Quaternion targetRotation = Quaternion.Euler(-25f, yRotation, 0f);
            obj_scale.transform.localRotation = Quaternion.Euler(0f, yRotation, 0f);

        }


    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("dropping") && !enemyTimers.ContainsKey(other.gameObject))
        {
            Coroutine timer = StartCoroutine(WaitAndActivateEnemy(other.gameObject));
            enemyTimers.Add(other.gameObject, timer);

            Transform point = other.transform.Find("point");
            if (point != null && point.gameObject.activeSelf)
            {
                obj_graphicPoint.SetActive(true);
            }
        }
    }


    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("dropping"))
        {
            if (enemyTimers.TryGetValue(other.gameObject, out Coroutine timer))
            {
                StopCoroutine(timer);
                enemyTimers.Remove(other.gameObject);
            }

            detectedEnemies.Remove(other.gameObject);
            obj_enemyActive.SetActive(false);
            obj_graphicPoint.SetActive(false);
        }
    }

    private IEnumerator WaitAndActivateEnemy(GameObject enemy)
    {
        yield return new WaitForSeconds(1.5f);

        if (!detectedEnemies.Contains(enemy))
        {
            detectedEnemies.Add(enemy);

            Transform point = enemy.transform.Find("point");
            if (point != null && point.gameObject.activeSelf)
            {
                obj_enemyActive.SetActive(true);
            }
        }

        enemyTimers.Remove(enemy);
    }


}
