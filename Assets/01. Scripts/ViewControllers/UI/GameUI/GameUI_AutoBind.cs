using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using MikroFramework;
using TMPro;
using MikroFramework.Architecture;

namespace HollowKnight {
	public partial class GameUI : AbstractMikroController<HollowKnight> {
		[SerializeField] private Slider SliderPlayerHealth;
		[SerializeField] private Slider SliderUltCharge;
        [SerializeField] private Slider SliderBossHealth;
		[SerializeField] private Image ImgBackgroundNotFull;
		[SerializeField] private Image ImgBackgroundFull;
		[SerializeField] private Image ImgFill;
	}
}