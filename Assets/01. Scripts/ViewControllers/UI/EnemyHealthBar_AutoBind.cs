using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using MikroFramework;
using TMPro;
using MikroFramework.Architecture;

namespace HollowKnight {
	public partial class EnemyHealthBar : AbstractMikroController<HollowKnight> {
		[SerializeField] private Slider SliderEnemyHealth;
		[SerializeField] private Image ImgBackground;
		[SerializeField] private Image ImgFill;
	}
}