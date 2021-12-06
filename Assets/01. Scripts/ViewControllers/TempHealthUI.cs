using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.Event;
using TMPro;
using UnityEngine;

namespace HollowKnight
{
    public class TempHealthUI : AbstractMikroController<HollowKnight>
    {
        private TMP_Text text;

        private void Awake()
        {
            text = GetComponent<TMP_Text>();
        }

        private void Start()
        {
            this.GetModel<IPlayerModel>().Health.RegisterOnValueChaned(OnHealthChange).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void OnHealthChange(float old, float newHealth)
        {
            text.text = $"Player Health: {newHealth}/{this.GetModel<IPlayerConfigurationModel>().MaxHealth}";

        }
    }
}