using Godot;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;

namespace Kaguya.Utils;

public partial class UpdatePopup : Control, IScreenContext
{
    private const string ScenePath = "res://scenes/ui/vertical_popup.tscn";

    private NVerticalPopup? _verticalPopup;
    private string _releaseUrl = string.Empty;
    private string _currentVersion = string.Empty;
    private string _latestVersion = string.Empty;

    public Control? DefaultFocusedControl => null;

    public static UpdatePopup? Create(string currentVersion, string latestVersion, string releaseUrl)
    {
        var scene = GD.Load<PackedScene>(ScenePath);
        if (scene == null) return null;

        var popup = new UpdatePopup();
        popup._releaseUrl = releaseUrl;
        popup._currentVersion = currentVersion;
        popup._latestVersion = latestVersion;
        popup.SetAnchorsPreset(Control.LayoutPreset.FullRect);

        popup._verticalPopup = scene.Instantiate<NVerticalPopup>(PackedScene.GenEditState.Disabled);
        if (popup._verticalPopup == null) return null;

        popup.AddChild(popup._verticalPopup);
        return popup;
    }

    public override void _Ready()
    {
        base._Ready();
        SetupContent();
    }

    private void SetupContent()
    {
        if (_verticalPopup == null) return;

        var titleLoc = new LocString("settings_ui", "KAGUYA-UPDATE_POPUP.title");
        var bodyLoc = new LocString("settings_ui", "KAGUYA-UPDATE_POPUP.body");

        string rawBody = bodyLoc.GetRawText();
        string bodyText = rawBody
            .Replace("{0}", _currentVersion)
            .Replace("{1}", _latestVersion);

        _verticalPopup.SetText(titleLoc.GetRawText(), bodyText);

        _verticalPopup.InitYesButton(
            new LocString("settings_ui", "KAGUYA-UPDATE_POPUP.download"),
            OnDownloadPressed
        );

        _verticalPopup.InitNoButton(
            new LocString("settings_ui", "KAGUYA-UPDATE_POPUP.close"),
            OnClosePressed
        );
    }

    private void OnDownloadPressed(NButton _)
    {
        OS.ShellOpen(_releaseUrl);
        ClosePopup();
    }

    private void OnClosePressed(NButton _)
    {
        ClosePopup();
    }

    private void ClosePopup()
    {
        NModalContainer.Instance?.Clear();
        this.QueueFreeSafely();
    }
}