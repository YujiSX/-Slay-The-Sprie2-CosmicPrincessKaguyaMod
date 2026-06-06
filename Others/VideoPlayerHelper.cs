using Godot;
using System.Threading.Tasks;

namespace Kaguya;

public static class VideoPlayerHelper
{
    private const string OverlayName = "__KaguyaVideoOverlay";

    public static async Task PlayFullscreenVideo(string videoPath)
    {
        var tree = Engine.GetMainLoop() as SceneTree;
        if (tree == null) return;
        var root = tree.Root;
        if (root == null) return;

        var existing = root.GetNodeOrNull<Control>(OverlayName);
        existing?.QueueFree();

        var overlay = new Control
        {
            Name = OverlayName,
            MouseFilter = Control.MouseFilterEnum.Stop
        };
        overlay.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
        root.AddChild(overlay);

        var stream = ResourceLoader.Load<VideoStream>(videoPath);
        if (stream == null)
        {
            overlay.QueueFree();
            GD.PushWarning($"[Kaguya] Failed to load video: {videoPath}");
            return;
        }

        var videoPlayer = new VideoStreamPlayer
        {
            Stream = stream,
            Expand = true,
            Autoplay = false,
            MouseFilter = Control.MouseFilterEnum.Ignore
        };
        videoPlayer.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
        overlay.AddChild(videoPlayer);

        bool closeRequested = false;
        void OnGuiInput(InputEvent e)
        {
            if (e is InputEventMouseButton mb && mb.Pressed && mb.ButtonIndex == MouseButton.Left)
                closeRequested = true;
        }
        overlay.GuiInput += OnGuiInput;

        overlay.Modulate = new Color(1, 1, 1, 0);
        videoPlayer.Play();
        await TweenAlpha(overlay, 1f, 0.3f);

        while (GodotObject.IsInstanceValid(videoPlayer) && videoPlayer.IsPlaying() && !closeRequested)
            await root.ToSignal(tree, SceneTree.SignalName.ProcessFrame);

        if (closeRequested && GodotObject.IsInstanceValid(videoPlayer))
            videoPlayer.Stop();

        if (GodotObject.IsInstanceValid(overlay))
            await TweenAlpha(overlay, 0f, 0.3f);

        overlay.GuiInput -= OnGuiInput;
        overlay.QueueFree();
    }

    private static async Task TweenAlpha(Control target, float toAlpha, float duration)
    {
        if (!GodotObject.IsInstanceValid(target)) return;
        var tween = target.CreateTween();
        tween.TweenProperty(target, "modulate:a", toAlpha, duration);
        await target.ToSignal(tween, Tween.SignalName.Finished);
    }
}