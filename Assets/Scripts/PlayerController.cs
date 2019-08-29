﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int speed;
    public int playerType;
    public int pickUpsLen;
    public int totalItems;

    public Vector2 rotationAngle;

    public Queue pickUps;
    public List<string> choppedItems;

    public GameObject pickedItemsCanvas;
    public GameObject choppedItemsCanvas;

    public GameObject vegPicked;
    public GameObject vegChopped;

    public bool chopping;


    public GameObject firepoint;
    // Start is called before the first frame update
    void Start() {
        pickedItemsCanvas.SetActive(false);
        choppedItemsCanvas.SetActive(false);
        pickUps = new Queue();
        rotationAngle = Vector2.up;
    }

    private void FixedUpdate() {
        totalItems = pickUps.Count + choppedItems.Count;
    }
    // Update is called once per frame
    void Update() {
        if (playerType == 1) {
            if (!chopping) {
                if (Input.GetKey(KeyCode.W)) {
                    transform.Translate(Vector2.up * speed * Time.deltaTime);
                }
                if (Input.GetKey(KeyCode.S)) {
                    transform.Translate(Vector2.down * speed * Time.deltaTime);
                }
                if (Input.GetKey(KeyCode.A)) {
                    transform.Translate(Vector2.left * speed * Time.deltaTime);
                }
                if (Input.GetKey(KeyCode.D)) {
                    transform.Translate(Vector2.right * speed * Time.deltaTime);
                }

                if (Input.GetKeyDown(KeyCode.Q)) {
                    Pick();
                }

                if (Input.GetKeyDown(KeyCode.E)) {
                    Drop();
                }
            }

        }
        else {
            if (Input.GetKey(KeyCode.UpArrow)) {
                transform.Translate(Vector2.up * speed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.DownArrow)) {
                transform.Translate(Vector2.down * speed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.LeftArrow)) {
                transform.Translate(Vector2.left * speed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.RightArrow)) {
                transform.Translate(Vector2.right * speed * Time.deltaTime);
            }

        }
            
    }

    void Pick() {
        Debug.DrawRay(firepoint.transform.position, rotationAngle, Color.blue, 10000);

        RaycastHit raycastHit;

        if (Physics.Raycast(firepoint.transform.position, rotationAngle , out raycastHit, 10000f)) {
            if (raycastHit.collider.gameObject.tag.Equals("Veg")) {
                if (!pickedItemsCanvas.activeSelf)
                    pickedItemsCanvas.SetActive(true);

                if (pickUps.Count < 2) {
                    pickUps.Enqueue(raycastHit.collider.gameObject.name);
                    pickUpsLen = pickUps.Count;

                    GameObject vPicked = Resources.Load<GameObject>("Prefabs/" + raycastHit.collider.gameObject.name);
                    vegPicked = Instantiate(vPicked, transform.position, Quaternion.identity);
                    vegPicked.transform.SetParent(GameObject.Find("Picked").transform);
                }
            }
            
            Debug.Log(pickUps.Count);
            foreach (var item in pickUps) {
                Debug.Log(item);
            }
        }
    }

    void Drop() {
        Debug.DrawRay(firepoint.transform.position, rotationAngle, Color.red, 10000);

        RaycastHit raycastHit;

        if (Physics.Raycast(firepoint.transform.position, rotationAngle, out raycastHit, 10000f)) {
            if (raycastHit.collider.gameObject.tag.Equals("ChopBoard")) {
                Debug.Log(raycastHit.collider.gameObject.tag);
                if (pickUps.Count > 0) {
                    StartCoroutine(Chopping(pickUps.Dequeue().ToString()));
                    if (pickUps.Count == 0) {
                        pickedItemsCanvas.SetActive(false);
                    }
                }
            }             
        }

        if (Physics.Raycast(firepoint.transform.position, rotationAngle, out raycastHit, 10000f)) {
            if (raycastHit.collider.gameObject.tag.Equals("Plate")) {
                if (raycastHit.collider.gameObject.GetComponent<Plate>().itemPlaced == "") {
                    raycastHit.collider.gameObject.SendMessage("PlaceItem", pickUps.Dequeue());
                }
            }
        }

        if (Physics.Raycast(firepoint.transform.position, rotationAngle, out raycastHit, 10000f)) {
            if (raycastHit.collider.gameObject.tag.Equals("Trash")) {                
                choppedItems.Clear();
            }
        }

        if (Physics.Raycast(firepoint.transform.position, rotationAngle, out raycastHit, 10000f)) {
            if (raycastHit.collider.gameObject.tag.Equals("Customer")) {
                
            }
        }

    }

    void Throw() {
    }

    private void OnTriggerEnter(Collider col) {
        if (col.gameObject.name.Equals("Left")) {
            rotationAngle = Vector2.left;
            firepoint.transform.localPosition = new Vector2(-0.34f, 0);
        }

        if (col.gameObject.name.Equals("Right")) {
            rotationAngle = Vector2.right;
            firepoint.transform.localPosition = new Vector2(0.34f, 0);
        }

        if (col.gameObject.name.Equals("Down")) {
            rotationAngle = Vector2.down;
            firepoint.transform.localPosition = new Vector2(0, -0.34f);
        }

        if (col.gameObject.name.Equals("Up")) {
            rotationAngle = Vector2.up;
            firepoint.transform.localPosition = new Vector2(0, 0.34f);
        }
    }

    //private void OnCollisionStay2D(Collision2D col) {
    //    if (Input.GetKeyDown(KeyCode.C)) {
    //        if (col.gameObject.tag.Equals("ChopBoard")) {
    //            StartCoroutine(Chopping());
    //        }
    //    }
    //}


    IEnumerator Chopping(string item) {
        Debug.Log("chopping");
        chopping = true;
        yield return new WaitForSeconds(2f);
        choppedItems.Add(item);
        if (choppedItems.Count == 1) {
            choppedItemsCanvas.SetActive(true);
        }

        GameObject cPicked = Resources.Load<GameObject>("Prefabs/" + item + "chop");
        vegChopped = Instantiate(cPicked, transform.position, Quaternion.identity);
        vegChopped.transform.SetParent(GameObject.Find("Chopped").transform);

        chopping = false;
    }
}
