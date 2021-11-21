using System.Collections;
using System.Collections.Generic;
using HollowKnight;
using MikroFramework.Architecture;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace HollowKnight {
    public struct OnPlayerInDialogueRange
    {
        public Vector3 Position;
    }

    public class DialogueViewController : AbstractMikroController<HollowKnight>
    {
        [SerializeField]
        private Dialogue dialogue;


        [SerializeField]
        private bool triggered = false;

        [SerializeField]
        private bool talked = false;

        private Queue<string> sentences;
        private Queue<string> names;
        private Queue<Sprite> avatars;
        private Queue<Sprite> textboxes;

        string name;
        string sentence;
        Sprite avatar;
        Sprite textbox;

        //GameObject player;

        void Start()
        {
            names = new Queue<string>();
            sentences = new Queue<string>();
            avatars = new Queue<Sprite>();
            textboxes = new Queue<Sprite>();


        }
        void Update()
        {

            if (Mathf.Abs(transform.position.x - Player.Singleton.transform.position.x) <= 2.5f)
            {
                if (Mathf.Sign(transform.position.x - Player.Singleton.transform.position.x) == 1)
                {
                    GetComponent<SpriteRenderer>().flipX = true;
                }
                else
                {
                    GetComponent<SpriteRenderer>().flipX = false;
                }
            }

            if (!talked && Mathf.Abs(transform.position.x - Player.Singleton.transform.position.x) <= 2.5f)
            {
                DialogueUIController.Singleton.TurnOnInteractableObj(transform.position);
            }


            if (triggered)
            {

                if (!DialogueUIController.Singleton.textboxSprite. enabled)
                {
                    DialogueUIController.Singleton.TurnOnInteractableObj(transform.position);
                }
                else
                {
                    DialogueUIController.Singleton.TurnOffInteractableObj();
                }


                if (Input.GetKeyDown(KeyCode.E)) {
                   
                    if (DialogueUIController.Singleton.nextButton.enabled || 
                        !DialogueUIController.Singleton.textboxSprite.enabled) {

                        //FindObjectOfType<PlayerMovement>().canMove = false;
                        talked = true;

                        if (sentences.Count == 0)
                        {
                            talked = true;
                            EndDialogue();

                            return;
                        }

                        name = names.Dequeue();
                        sentence = sentences.Dequeue();
                        avatar = avatars.Dequeue();
                        textbox = textboxes.Dequeue();

                        DialogueUIController.Singleton.ShowDialogueWithTypewriter(name,
                            sentence,avatar,textbox);
                    }
                    else if (!DialogueUIController.Singleton.nextButton.enabled &&
                             DialogueUIController.Singleton.textboxSprite.enabled) {
                        DialogueUIController.Singleton.ShowDialogueWithoutTypeWriter(
                            name,sentence,avatar,textbox);
                    }
                }
            }
            else
            {
                DialogueUIController.Singleton.TurnOffInteractableObj();
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

            DialogueUIController.Singleton.EndDialogue();
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

            // FindObjectOfType<PlayerMovement>().canMove = true;

        }

    }

}
