using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
 private Rigidbody rb; 
 private int count;
 private float movementX;
 private float movementY;
 public float speed = 0;
 public TextMeshProUGUI countText;
 public GameObject winTextObject;
 public AudioSource PickUpAudioSource;
 public AudioSource GameOverAudioSource;
public GameObject pickupFX;
public GameObject deathFX;
public GameObject winFX;
private bool hasWon = false;
private bool exitUnlocked  = false;
private GameObject currentWinFX;
public GameObject playAgainButton;
private Vector3 targetPos;
[SerializeField] private bool isMoving = false;
[SerializeField] private int coinsNeededToEscape = 25;
public GameObject treeGate;
public GameObject exitOpenText;


 void Start()
    {
        rb = GetComponent<Rigidbody>();
        count = 0;
        SetCountText();

        winTextObject.SetActive(false);
        exitOpenText.SetActive(false);
    }
 
 void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x; 
        movementY = movementVector.y; 
    }

 private void FixedUpdate() 
    {
        Vector3 movement = new Vector3 (movementX, 0.0f, movementY);
        rb.AddForce(movement * speed); 

        if (isMoving)
        {
        // Move the player towards the target position
            Vector3 direction = targetPos - rb.position;
            direction.Normalize();
            rb.AddForce(direction * speed);
        }
        // Stop moving the player if it is close to the target position
        if (Vector3.Distance(rb.position, targetPos) < 0.5f)
        {
            isMoving = false;
        }
    }

private void OnCollisionEnter(Collision collision)
       {
              if(collision.gameObject.CompareTag("Enemy") && !hasWon)
              {
                     Instantiate(deathFX, transform.position, Quaternion.identity);

                     winTextObject.gameObject.SetActive(true);
                     winTextObject.GetComponent<TextMeshProUGUI>().text = "You Lose!";

                     if (GameOverAudioSource != null)
                        {
                            GameOverAudioSource.Play();
                        }

                     collision.gameObject.transform.parent.GetComponentInChildren<Animator>().SetFloat("speed_f", 0);

                     Destroy(gameObject);

                     playAgainButton.SetActive(true);
              }
       }

 
 void OnTriggerEnter(Collider other) 
    {
    if (other.gameObject.CompareTag("PickUp")) 
        {
            var currentPickupFX = Instantiate(pickupFX, other.transform.position, Quaternion.identity);
            
            Destroy(currentPickupFX, 3);
            
            other.gameObject.SetActive(false);

            if (PickUpAudioSource != null)
            {
                PickUpAudioSource.Play();
            }
            count = count + 1;
            SetCountText();
        }
        else if (other.gameObject.CompareTag("Exit") && !hasWon)
        {
            if (exitUnlocked)
            {
                Escape();
            }
            else
            {
                int coinsRemaining = coinsNeededToEscape - count;

                winTextObject.SetActive(true);
                winTextObject.GetComponent<TextMeshProUGUI>().text = 
                "Collect " + coinsRemaining + " more coins to unlock the exit!";
            }
        }
    }

    void SetCountText()
        {
            countText.text = "Count: " + count.ToString();

            if (count >= coinsNeededToEscape && !exitUnlocked)
            {
                exitUnlocked = true;

                if (exitOpenText != null)
            {
                exitOpenText.SetActive(true);
                exitOpenText.GetComponent<TextMeshProUGUI>().text = "The exit is now open!";
            }

                if (treeGate != null)
                {
                    treeGate.SetActive(false);
                }
            }
        }


    private void Escape()
    {
        hasWon = true;

        if (exitOpenText != null)
        {
            exitOpenText.SetActive(false);
        }

        winTextObject.SetActive(true);
        winTextObject.GetComponent<TextMeshProUGUI>().text = 
        "You escaped! You Win!";

        currentWinFX = Instantiate(
            winFX, transform.position, Quaternion.identity
            );

        GameObject enemyBody = GameObject.FindGameObjectWithTag("Enemy");

        if (enemyBody != null)
        {
            if (enemyBody.transform.parent != null)
            {
                Destroy(enemyBody.transform.parent.gameObject);
            }
            else
            {
                Destroy(enemyBody);
            }
        }

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;

        playAgainButton.SetActive(true);
    }

    void Update()
    {
        if (currentWinFX != null)
        {
            currentWinFX.transform.position = transform.position;
            currentWinFX.transform.rotation = Quaternion.identity;
        }

        /*
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); 
        Debug.DrawRay(ray.origin, ray.direction * 50, Color.yellow);

        RaycastHit hit; // Define variable to hold raycast hit information

        if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                {
                    targetPos = hit.point;
                    isMoving = true;
                }
                else
                {
                    isMoving = false;
                }
            }
        else
        {
            isMoving = false;
        }

        */
    }
}