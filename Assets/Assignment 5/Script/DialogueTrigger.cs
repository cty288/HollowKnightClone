using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;
    public TMP_Text nameText;
    public TMP_Text dialogueText;
    public Image avatarSprite;
    public Image textboxSprite;
    public GameObject Interactable;

    public bool triggered = false;
    public bool talked = false;

    private Queue<string> sentences;
    private Queue<string> names;
    private Queue<Sprite> avatars;
    private Queue<Sprite> textboxes;

    //GameObject player;

    void Start()
    {
        names = new Queue<string>();
        sentences = new Queue<string>();
        avatars = new Queue<Sprite>();
        textboxes = new Queue<Sprite>();
        Interactable = GameObject.Find("Interactable");
        //avatarSprite.gameObject.SetActive(false);
        //textboxSprite.gameObject.SetActive(false);
        avatarSprite.enabled = false;
        textboxSprite.enabled = false;
    }
    void Update()
    {

        if (Mathf.Abs(transform.position.x - GameObject.Find("Madeline").transform.position.x) <= 2.5f)
        {
            if (Mathf.Sign(transform.position.x - GameObject.Find("Madeline").transform.position.x) == 1)
            {
                GetComponent<SpriteRenderer>().flipX = true;
            }
            else
            {
                GetComponent<SpriteRenderer>().flipX = false;
            }
        }

            if (!talked && Mathf.Abs(transform.position.x - GameObject.Find("Madeline").transform.position.x) <= 2.5f)
        {
            Interactable.GetComponent<MeshRenderer>().enabled = true;
            Interactable.transform.position = new Vector3(transform.position.x, transform.position.y + 3, transform.position.z);
        }
        else if(Mathf.Abs(transform.position.x - GameObject.Find("Madeline").transform.position.x) <= 10)
        {
            Interactable.GetComponent<MeshRenderer>().enabled = false;
        }

        if (triggered)
        {
            

            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                //FindObjectOfType<playerMovement>().canMove = false;
                talked = true;

                if (sentences.Count == 0)
                {
                    talked = true;
                    EndDialogue();
                    return;
                }

                string name = names.Dequeue();
                string sentence = sentences.Dequeue();
                Sprite avatar = avatars.Dequeue();
                Sprite textbox = textboxes.Dequeue();
                avatarSprite.enabled = true;
                textboxSprite.enabled = true;   //show image
                //avatarSprite.gameObject.SetActive(true);
                //textboxSprite.gameObject.SetActive(true);
                nameText.text = name;
                dialogueText.text = sentence;
                avatarSprite.GetComponent<Image>().sprite = avatar;
                textboxSprite.GetComponent<Image>().sprite = textbox;
                //Debug.Log(name);
                //Debug.Log(sentence);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {   
            triggered = true;

            names.Clear();
            sentences.Clear();
            avatars.Clear();
            textboxes.Clear();

            foreach (string sentence in dialogue.sentences)
            {
                sentences.Enqueue(sentence);
            }

            foreach (string name in dialogue.names)
            {
                names.Enqueue(name);
            }

            foreach (Sprite avatar in dialogue.avatars)
            {
                avatars.Enqueue(avatar);
            }

            foreach (Sprite textbox in dialogue.textboxs)
            {
                textboxes.Enqueue(textbox);
            }

            //Debug.Log("Trigger conversation " + names.Peek());

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            triggered = false;
            EndDialogue();

        }
    }

    void EndDialogue()
    {
        //Debug.Log("End conversation ");
        avatarSprite.enabled = false;
        textboxSprite.enabled = false;
        //avatarSprite.gameObject.SetActive(false);
        //textboxSprite.gameObject.SetActive(false);
        nameText.text = "";
        dialogueText.text = "";
        avatarSprite.GetComponent<Image>().sprite = null;
        textboxSprite.GetComponent<Image>().sprite = null;
        names.Clear();  
        sentences.Clear();
        avatars.Clear();
        textboxes.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        foreach (string name in dialogue.names)
        {
            names.Enqueue(name);
        }

        foreach (Sprite avatar in dialogue.avatars)
        {
            avatars.Enqueue(avatar);
        }

        foreach (Sprite textbox in dialogue.textboxs)
        {
            textboxes.Enqueue(textbox);
        }

        //FindObjectOfType<playerMovement>().canMove = true;

    }

}
