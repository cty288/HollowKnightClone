using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Singletons;
using UnityEngine;

namespace MikroFramework.Examples
{
    public class GameManager : MonoBehaviour, ISingleton
    {

        private static GameManager instance
        {
            get { return SingletonProperty<GameManager>.Singleton; }
        }


        private int score = 0;
        private int health = 100;

        public static void Init()
        {
            instance.score = 0;
            instance.health = 0;
        }

        public static void Play(int addScore, int addHealth)
        {
            instance.score += addScore;
            instance.health += addHealth;
            Debug.Log($"Score: {instance.score}   Health: {instance.health}");
        }

        public static void Pause() { }

        public static void Resume() { }

        public static void Gameover() { }

        void ISingleton.OnSingletonInit()
        {

        }
    }

    public class SingletonBestPractice : MonoBehaviour {

        

        private void Start() {
            GameManager.Init();
            GameManager.Play(5,5);
        }
    }
}
