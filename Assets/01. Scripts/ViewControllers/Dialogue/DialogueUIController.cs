using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MikroFramework.Architecture;
using MikroFramework.Event;
using MikroFramework.Singletons;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
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

       

        private void Awake()
        {
            avatarSprite.enabled = false;
            TurnOffTextBox();
            nextButton.enabled = false;
            
        }

        private void ShowTextBox() {
          
            DOTween.To(() => textboxSprite.color, x => textboxSprite.color = x, new Color(1,1,1,1), 1);
        }

        private void TurnOffTextBox() {
            DOTween.To(() => textboxSprite.color, x => textboxSprite.color = x, new Color(1, 1, 1, 0), 1);
        }


       

        public void ShowDialogueWithTypewriter(string name, string text, Sprite avatarSprite,
            UnityEvent callback)
        {
            StopAllCoroutines();
            this.avatarSprite.enabled = true;
            ShowTextBox();

            nameText.text = name;
            StartCoroutine(TypeSentence(text, callback));

            this.avatarSprite.GetComponent<Image>().sprite = avatarSprite;
            

        }

        public void ShowDialogueWithoutTypeWriter(string name, string text, Sprite avatarSprite, 
            UnityEvent callback)
        {
            StopAllCoroutines();
            this.avatarSprite.enabled = true;
            ShowTextBox();

            nameText.text = name;
            dialogueText.text = text;
            nextButton.enabled = true;

            this.avatarSprite.GetComponent<Image>().sprite = avatarSprite;
           
            callback?.Invoke();
        }

        public void EndDialogue()
        {
            avatarSprite.enabled = false;
            TurnOffTextBox();
            nextButton.enabled = false;

            nameText.text = "";
            dialogueText.text = "";

            avatarSprite.GetComponent<Image>().sprite = null;
            
        }

        IEnumerator TypeSentence(string sentence, UnityEvent callback)
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

            callback?.Invoke();

        }
        public void OnSingletonInit()
        {

        }
    }
}
