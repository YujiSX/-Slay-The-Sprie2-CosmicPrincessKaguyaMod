using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;

namespace Kaguya;

[HarmonyPatch(typeof(NMainMenuBg))]
public static class MainMenuLogoPatch
{
	private const string CustomLogoPath = "res://images/my_logo.png";
	private const float LogoScale = 0.85f;
	private const float MarginTop = 150f;
	private const float MarginBottom = 320f;

	private static TextureRect _customRect;

	[HarmonyPatch(nameof(NMainMenuBg._Ready))]
	[HarmonyPostfix]
	static void ReadyPostfix(NMainMenuBg __instance)
	{
		var tex = GD.Load<Texture2D>(CustomLogoPath);
		if (tex == null) return;

		var logo = __instance.GetNode("%Logo");
		if (logo == null) return;

		if (logo is CanvasItem canvas)
			canvas.Visible = false;

		_customRect = new TextureRect
		{
			Texture = tex,
			ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
			StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
			MouseFilter = Control.MouseFilterEnum.Ignore,

			Scale = new Vector2(LogoScale, LogoScale)
		};
		_customRect.SetAnchorsPreset(Control.LayoutPreset.FullRect);
		_customRect.OffsetLeft = 150f;
		_customRect.OffsetTop = MarginTop;
		_customRect.OffsetRight = 0f;
		_customRect.OffsetBottom = -MarginBottom;
		_customRect.AnchorRight = 1f;
		_customRect.AnchorBottom = 1f;
		_customRect.GrowHorizontal = Control.GrowDirection.Both;
		_customRect.GrowVertical = Control.GrowDirection.Both;
		_customRect.Name = "CustomLogo";

		__instance.AddChild(_customRect);
	}

	[HarmonyPatch(nameof(NMainMenuBg.HideLogo))]
	[HarmonyPostfix]
	static void HideLogoPostfix()
	{
		if (_customRect != null && GodotObject.IsInstanceValid(_customRect))
			_customRect.Visible = false;
	}

	[HarmonyPatch(nameof(NMainMenuBg.ShowLogo))]
	[HarmonyPostfix]
	static void ShowLogoPostfix()
	{
		if (_customRect != null && GodotObject.IsInstanceValid(_customRect))
			_customRect.Visible = true;
	}
}
