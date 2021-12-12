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

    public class DialogueViewController : AbstractMikroController<HollowKnight> {
        [SerializeField] private DialogueUIController uiController;

        [SerializeField]
        private Dialogue dialogue;


        [SerializeField]
        private bool triggered = false;

        [SerializeField]
        private bool talked = false;

        private Queue<string> sentences;
        private Queue<string> names;
        private Queue<Sprite> avatars;
        
        private Queue<UnityEvent> callbacks;


        string name;
        string sentence;
        Sprite avatar;
        
        private UnityEvent callback;

        [SerializeField] private bool triggerWhenEnter = true;
        private bool triggerWhenEnterTriggered = false;

        private bool hasTalked = false;
        private bool isTalking = false;

        [SerializeField]
        private bool freezePlayer = false;

        [SerializeField] private GameObject interactable;

        [SerializeField] private bool canRepeat = false;

        //GameObject player;

        void Start() {
            callbacks = new Queue<UnityEvent>();
            names = new Queue<string>();
            sentences = new Queue<string>();
            avatars = new Queue<Sprite>();
           
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

            if (!talked && Mathf.Abs(transform.position.x - Player.Singleton.transform.position.x) <= 2.5f && !triggerWhenEnter)
            {
                TurnOnInteractableObj();
            }


            if (triggered)
            {

                if (uiController.textboxSprite.color.a == 0 && !triggerWhenEnter)
                {
                    //Debug.Log("Turn on");
                    TurnOnInteractableObj();
                }
                else
                {
                    TurnOffInteractableObj();
                }


                if ((Input.GetKeyDown(KeyCode.F) || (triggerWhenEnter && !triggerWhenEnterTriggered)) &&
                    (!hasTalked || isTalking || canRepeat)) {

                    if (freezePlayer) {
                        Player.Singleton.FrozePlayer(true);
                    }

                    hasTalked = true;
                    isTalking = true;

                    triggerWhenEnterTriggered = true;
                    if (uiController.nextButton.enabled || 
                        uiController.textboxSprite.color.a == 0) {

                        //FindObjectOfType<PlayerMovement>().canMove = false;
                        talked = true;

                        if (sentences.Count == 0) {
                            talked = true;
                            isTalking = false;
                            EndDialogue();
                            Player.Singleton.FrozePlayer(false);
                            return;
                        }

                        
                        name = names.Dequeue();
                        sentence = sentences.Dequeue();
                        avatar = avatars.Dequeue();
                        
                        callback = callbacks.Dequeue();

                        uiController.ShowDialogueWithTypewriter(name,
                            sentence,avatar, callback);
                    }
                    else if (!uiController.nextButton.enabled &&
                             uiController.textboxSprite.color.a == 1) {
                        uiController.ShowDialogueWithoutTypeWriter(
                            name,sentence,avatar, callback);
                    }
                }
            }
            else
            {
                 TurnOffInteractableObj();
            }
        }


        private void TurnOnInteractableObj()
        {
            interactable.SetActive(true);
           
        }

        private void TurnOffInteractableObj()
        {
            interactable.SetActive(false);
        }


        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                triggered = true;
                triggerWhenEnterTriggered = false;

                names.Clear();
                sentences.Clear();
                avatars.Clear();
               
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

            uiController.EndDialogue();
            names.Clear();
            sentences.Clear();
            avatars.Clear();
            
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

            
            foreach (UnityEvent callback in dialogue.callbacks)
            {
                callbacks.Enqueue(callback);
            }

            // FindObjectOfType<PlayerMovement>().canMove = true;

        }

    }

}
