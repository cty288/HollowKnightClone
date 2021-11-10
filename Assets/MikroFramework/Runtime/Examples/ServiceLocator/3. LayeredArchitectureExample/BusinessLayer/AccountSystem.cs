using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Examples;
using NHibernate.Cache;
using UnityEngine;

namespace MikroFramework.Examples.ServiceLocator
{
    public interface IAccountSystem : ISystem {
        bool IsLogined { get; }
        void Login(string username, string password, Action<bool> onLogin);
        void Logout(Action onLogout);
    }
    public class AccountSystem : IAccountSystem
    {
        public bool IsLogined { get; private set; }


        public void Login(string username, string password, Action<bool> onLogin) {
            PlayerPrefs.SetString("username",username);
            PlayerPrefs.SetString("password",password);
            IsLogined = true;
            onLogin(true);
        }

        public void Logout(Action onLogout) {
            PlayerPrefs.SetString("username", string.Empty);
            PlayerPrefs.SetString("password", string.Empty);
            IsLogined = false;
            onLogout.Invoke();
        }
    }
}
