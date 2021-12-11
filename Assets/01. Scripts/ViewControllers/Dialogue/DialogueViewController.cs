using System.Collections;
using System.Collections.Generic;
using HollowKnight;
using MikroFramework.Architecture;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
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
        private Queue<UnityEvent> callbacks;


        string name;
        string sentence;
        Sprite avatar;
        Sprite textbox;
        private UnityEvent callback;

        //GameObject player;

        void Start() {
            callbacks = new Queue<UnityEvent>();
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


                if (Input.GetKeyDown(KeyCode.W)) {
                   
                    if (DialogueUIController.Singleton.nextButton.enabled || 
                        !DialogueUIController.Singleton.textboxSprite.enabled) {

                        //FindObjectOfType<PlayerMovement>().canMove = false;
                        talked = true;

                        if (sentences.Count == 0) {
                            talked = true;
                            EndDialogue();

                            return;
                        }

                        name = names.Dequeue();
                        sentence = sentences.Dequeue();
                        avatar = avatars.Dequeue();
                        textbox = textboxes.Dequeue();
                        callback = callbacks.Dequeue();

                        DialogueUIController.Singleton.ShowDialogueWithTypewriter(name,
                            sentence,avatar,textbox, callback);
                    }
                    else if (!DialogueUIController.Singleton.nextButton.enabled &&
                             DialogueUIController.Singleton.textboxSprite.enabled) {
                        DialogueUIController.Singleton.ShowDialogueWithoutTypeWriter(
                            name,sentence,avatar,textbox, callback);
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
                callbacks.Clear();

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

                foreach (UnityEvent call in dialogue.callbacks)
                {
                    callbacks.Enqueue(call);
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
            callbacks.Clear();

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

            foreach (UnityEvent callback in dialogue.callbacks)
            {
                callbacks.Enqueue(callback);
            }

            // FindObjectOfType<PlayerMovement>().canMove = true;

        }

    }

}
