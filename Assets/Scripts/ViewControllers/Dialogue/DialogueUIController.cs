using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.Event;
using MikroFramework.Singletons;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HollowKnight
{
    public class DialogueUIController : AbstractMikroController<HollowKnight>, ISingleton
    {
        [SerializeField]
        private TMP_Text nameText;
        [SerializeField]
        private TMP_Text dialogueText;
        [SerializeField]
        private Image avatarSprite;
        [SerializeField]
        public Image textboxSprite;
        [SerializeField]
        public Image nextButton;
        [SerializeField]
        private GameObject Interactable;

        public static DialogueUIController Singleton
        {
            get
            {
                return SingletonProperty<DialogueUIController>.Singleton;
            }

        }

        private void Awake()
        {
            avatarSprite.enabled = false;
            textboxSprite.enabled = false;
            nextButton.enabled = false;
        }


        public void TurnOnInteractableObj(Vector3 position)
        {
            Interactable.GetComponent<MeshRenderer>().enabled = true;
            Interactable.transform.position = new Vector3(position.x, position.y + 3, position.z);
        }

        public void TurnOffInteractableObj()
        {
            Interactable.GetComponent<MeshRenderer>().enabled = false;
        }

        public void ShowDialogueWithTypewriter(string name, string text, Sprite avatarSprite, Sprite textboxSprite)
        {
            StopAllCoroutines();
            this.avatarSprite.enabled = true;
            this.textboxSprite.enabled = true;

            nameText.text = name;
            StartCoroutine(TypeSentence(text));

            this.avatarSprite.GetComponent<Image>().sprite = avatarSprite;
            this.textboxSprite.GetComponent<Image>().sprite = textboxSprite;
        }

        public void ShowDialogueWithoutTypeWriter(string name, string text, Sprite avatarSprite, Sprite textboxSprite)
        {
            StopAllCoroutines();
            this.avatarSprite.enabled = true;
            this.textboxSprite.enabled = true;

            nameText.text = name;
            dialogueText.text = text;
            nextButton.enabled = true;

            this.avatarSprite.GetComponent<Image>().sprite = avatarSprite;
            this.textboxSprite.GetComponent<Image>().sprite = textboxSprite;
        }

        public void EndDialogue()
        {
            avatarSprite.enabled = false;
            textboxSprite.enabled = false;
            nextButton.enabled = false;

            nameText.text = "";
            dialogueText.text = "";

            avatarSprite.GetComponent<Image>().sprite = null;
            textboxSprite.GetComponent<Image>().sprite = null;
        }

        IEnumerator TypeSentence(string sentence)
        {
            dialogueText.text = "";
            int charCount = 0;
            foreach (char letter in sentence.ToCharArray())
            {
                charCount++;
                dialogueText.text += letter;
                yield return null;

                if (charCount >= sentence.ToCharArray().Length)
                {
                    nextButton.enabled = true;
                }
                else
                {
                    nextButton.enabled = false;
                }
            }

        }
        public void OnSingletonInit()
        {

        }
    }
}
