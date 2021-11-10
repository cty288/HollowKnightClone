using System.Collections;
using System.Collections.Generic;
using MikroFramework.AudioKit;
using MikroFramework.Managers;
using UnityEngine;

namespace MikroFramework.Examples {
    public class AudioManagerExample : MikroBehavior {
        protected override void OnBeforeDestroy() {
            
        }
#if UNITY_EDITOR
        [UnityEditor.MenuItem("MikroFramework/Examples/AudioKit/01. BasicExample", false,1)]
        static void MenuClicked() {
            UnityEditor.EditorApplication.isPlaying = true;
            new GameObject("AudioExample").AddComponent<AudioManagerExample>();
        }
#endif

        

        void Start() {
            AudioManager.PlayAudio("Lose");
            AudioManager.PlayBGM("War loop",true);
            Delay(5f, () => {
                AudioManager.StopBGM();
                Delay(2f, () => {
                    AudioManager.PlayBGM("War loop", true);
                    Delay(3f, () => {
                        AudioManager.PauseBGM();
                        Delay(1f, () => {
                            AudioManager.ResumeBGM();
                            Delay(2f, () => {
                                AudioManager.BGMMute();
                                Delay(2f, () => { AudioManager.BGMOn(); });
                            });
                            
                        });

                    });

                });


            });

        }
    }
}

