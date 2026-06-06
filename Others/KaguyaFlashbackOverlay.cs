using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.Audio.Debug;

namespace Kaguya;

public partial class KaguyaFlashbackOverlay : CanvasLayer
{
    private const float FadeInDuration = 0.3f;
    private const float HoldDuration = 0.7f;
    private const float FadeOutDuration = 0.3f;
    private const float GapDuration = 0.2f;

    private const float TextFadeInDuration = 0.5f;
    private const float TextFadeOutDuration = 0.5f;
    private const float TextCharsPerSecond = 5f;
    private const float TextShakeIntensity = 6f;
    private const float TextHoldAfterTyping = 0.5f;

    private const float FlashExpandDuration = 0.25f;
    private const float FlashFadeOutDuration = 0.25f;
    private const float FlashMaxScale = 1.5f;

    private const float ImagesAudioVolume = 3f;
    private const float TextAudioVolume = 3f;

    private ColorRect _blocker = null!;
    private ColorRect _flashRect = null!;
    private TextureRect[] _imageRects = null!;
    private Label _tailLabel = null!;
    private readonly string[] _imagePaths;
    private readonly string _tailText;

    public KaguyaFlashbackOverlay(string[] imagePaths, string tailText = "")
    {
        _imagePaths = imagePaths;
        _tailText = tailText;
    }

    public override void _Ready()
    {
        Layer = 130;
        ProcessMode = ProcessModeEnum.Always;

        _blocker = new ColorRect
        {
            Color = Colors.Black,
            MouseFilter = Control.MouseFilterEnum.Ignore
        };
        _blocker.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        _blocker.OffsetLeft = _blocker.OffsetTop = _blocker.OffsetRight = _blocker.OffsetBottom = 0f;
        _blocker.AnchorRight = 1f;
        _blocker.AnchorBottom = 1f;
        AddChild(_blocker);

        _flashRect = new ColorRect
        {
            Color = Colors.White,
            MouseFilter = Control.MouseFilterEnum.Ignore,
            Visible = false,
            Scale = Vector2.Zero
        };
        _flashRect.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        _flashRect.OffsetLeft = _flashRect.OffsetTop = _flashRect.OffsetRight = _flashRect.OffsetBottom = 0f;
        _flashRect.AnchorRight = 1f;
        _flashRect.AnchorBottom = 1f;
        AddChild(_flashRect);

        _imageRects = new TextureRect[_imagePaths.Length];
        for (int i = 0; i < _imagePaths.Length; i++)
        {
            var texRect = new TextureRect
            {
                Texture = GD.Load<Texture2D>(_imagePaths[i]),
                StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
                MouseFilter = Control.MouseFilterEnum.Ignore,
                Modulate = new Color(1, 1, 1, 0)
            };
            texRect.SetAnchorsPreset(Control.LayoutPreset.FullRect);
            texRect.OffsetLeft = texRect.OffsetTop = texRect.OffsetRight = texRect.OffsetBottom = 0f;
            texRect.AnchorRight = 1f;
            texRect.AnchorBottom = 1f;
            AddChild(texRect);
            _imageRects[i] = texRect;
        }

        _tailLabel = new Label
        {
            Text = "",
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            MouseFilter = Control.MouseFilterEnum.Ignore,
            Modulate = new Color(1, 1, 1, 0)
        };
        _tailLabel.AddThemeFontSizeOverride("font_size", 48);
        _tailLabel.AddThemeColorOverride("font_color", Colors.White);
        _tailLabel.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        _tailLabel.OffsetLeft = _tailLabel.OffsetTop = _tailLabel.OffsetRight = _tailLabel.OffsetBottom = 0f;
        _tailLabel.AnchorRight = 1f;
        _tailLabel.AnchorBottom = 1f;
        AddChild(_tailLabel);
    }

    public async Task RunFlashbackAsync()
    {
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

        await PlayFlashTransition();

        var neiHandle = NDebugAudioManager.Instance.Play("nei.mp3", ImagesAudioVolume);

        for (int i = 0; i < _imageRects.Length; i++)
        {
            if (!GodotObject.IsInstanceValid(this))
            {
                NDebugAudioManager.Instance.Stop(neiHandle, 0f);
                return;
            }

            await TweenAlpha(_imageRects[i], 0f, 1f, FadeInDuration);
            await ToSignal(GetTree().CreateTimer(HoldDuration), SceneTreeTimer.SignalName.Timeout);
            await TweenAlpha(_imageRects[i], 1f, 0f, FadeOutDuration);

            if (i < _imageRects.Length - 1)
                await ToSignal(GetTree().CreateTimer(GapDuration), SceneTreeTimer.SignalName.Timeout);
        }

        NDebugAudioManager.Instance.Stop(neiHandle, 0f);

        if (!string.IsNullOrEmpty(_tailText))
        {
            if (!GodotObject.IsInstanceValid(this))
                return;

            await ToSignal(GetTree().CreateTimer(0.3f), SceneTreeTimer.SignalName.Timeout);

            var dasukiHandle = NDebugAudioManager.Instance.Play("dasuki.mp3", TextAudioVolume);

            await TweenAlpha(_tailLabel, 0f, 1f, TextFadeInDuration);
            await TypewriterWithShake(_tailLabel, _tailText, TextCharsPerSecond, TextShakeIntensity);
            await ToSignal(GetTree().CreateTimer(TextHoldAfterTyping), SceneTreeTimer.SignalName.Timeout);
            await TweenAlpha(_tailLabel, 1f, 0f, TextFadeOutDuration);

            NDebugAudioManager.Instance.Stop(dasukiHandle, 0f);

            await ToSignal(GetTree().CreateTimer(0.3f), SceneTreeTimer.SignalName.Timeout);
        }

        if (GodotObject.IsInstanceValid(this))
            QueueFree();
    }

    private async Task PlayFlashTransition()
    {
        var viewportSize = GetViewport().GetVisibleRect().Size;
        _flashRect.PivotOffset = viewportSize / 2f;

        _flashRect.Visible = true;
        _flashRect.Scale = Vector2.Zero;
        _flashRect.Modulate = Colors.White;

        var tween = CreateTween();
        tween.TweenProperty(_flashRect, "scale", new Vector2(FlashMaxScale, FlashMaxScale), FlashExpandDuration)
            .SetEase(Tween.EaseType.In)
            .SetTrans(Tween.TransitionType.Cubic);
        await ToSignal(tween, Tween.SignalName.Finished);

        _flashRect.Scale = new Vector2(FlashMaxScale, FlashMaxScale);

        await TweenAlpha(_flashRect, 1f, 0f, FlashFadeOutDuration);

        _flashRect.Visible = false;
    }

    private async Task TweenAlpha(CanvasItem target, float from, float to, float duration)
    {
        var tween = CreateTween();
        target.Modulate = new Color(1, 1, 1, from);
        tween.TweenProperty(target, "modulate", new Color(1, 1, 1, to), duration);
        await ToSignal(tween, Tween.SignalName.Finished);
    }

    private async Task TypewriterWithShake(Label label, string fullText, float charsPerSecond, float shakeIntensity)
    {
        var originalPos = label.Position;
        var delay = 1f / charsPerSecond;

        for (int n = 0; n <= fullText.Length; n++)
        {
            if (!GodotObject.IsInstanceValid(this))
                return;

            label.Text = n == 0 ? "" : fullText.Substring(0, n);

            float x = (float)GD.RandRange(-shakeIntensity, shakeIntensity);
            float y = (float)GD.RandRange(-shakeIntensity, shakeIntensity);
            label.Position = originalPos + new Vector2(x, y);

            if (n < fullText.Length)
                await ToSignal(GetTree().CreateTimer(delay), SceneTreeTimer.SignalName.Timeout);
        }

        label.Position = originalPos;
    }

    public static async Task PlayAsync(Node caller, string[] imagePaths, string tailText = "")
    {
        var overlay = new KaguyaFlashbackOverlay(imagePaths, tailText);
        caller.GetTree().Root.AddChild(overlay);
        await overlay.RunFlashbackAsync();
    }

    public static async Task PlayNumberedAsync(Node caller, string prefix, string suffix, int count, string tailText = "", int startIndex = 1, int digits = 2)
    {
        var paths = new string[count];
        for (int i = 0; i < count; i++)
            paths[i] = prefix + (startIndex + i).ToString("D" + digits) + suffix;
        await PlayAsync(caller, paths, tailText);
    }
}
