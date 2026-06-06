using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Timer = Godot.Timer;

namespace Kaguya
{
    public class MoonTextData
    {
        public string text { get; set; }
        public float? time { get; set; }
    }

    public partial class KaguyaMoonTextPool : Node2D
    {
        private readonly List<Control> _pool = new();
        private Script _script;

        public override void _Ready()
        {
            _script = PreloadManager.Cache.GetAsset<Script>("res://MoonText/MoonText.gd");
        }

        public Control GetInstance()
        {
            foreach (var node in _pool)
            {
                if (!node.Visible)
                {
                    node.Visible = true;
                    return node;
                }
            }
            var newNode = new Control();
            newNode.Visible = true;
            newNode.Name = $"MoonTextContainer_{_pool.Count}";
            newNode.SetScript(_script ?? PreloadManager.Cache.GetAsset<Script>("res://MoonText/MoonText.gd"));
            AddChild(newNode);
            _pool.Add(newNode);
            return newNode;
        }
    }

    public static class KaguyaMoonText
    {
        private static KaguyaMoonTextPool _pool;

        public static void Setup(NCombatRoom combatRoom)
        {
            if (_pool != null && GodotObject.IsInstanceValid(_pool)) return;
            _pool = combatRoom.GetNodeOrNull<KaguyaMoonTextPool>("KaguyaMoonTextPool");
            if (_pool == null || !GodotObject.IsInstanceValid(_pool))
            {
                _pool = new KaguyaMoonTextPool();
                _pool.Name = "KaguyaMoonTextPool";
                combatRoom.AddChild(_pool);
                _pool._Ready();
            }
        }

        public static void Spawn(string text, Vector2 position)
        {
            if (_pool == null) return;
            var node = _pool.GetInstance();
            node.Set("text", text);
            node.Position = position;
            node.Call("spawn");
        }

        public static void Spawn(string text, Vector2 position,
            float typingDuration, float floatDelay, float floatTime)
        {
            if (_pool == null) return;
            var plainText = Regex.Replace(text, @"\[(\/?[^\]]+)\]", "");
            float charDelay = plainText.Length > 0 ? typingDuration / plainText.Length : 0.05f;
            var node = _pool.GetInstance();
            node.Set("text", text);
            node.Set("char_delay", charDelay);
            node.Set("float_delay", floatDelay);
            node.Set("float_time", floatTime);
            node.Position = position;
            node.Call("spawn");
        }

        public static void SpawnInstant(string text, Vector2 position,
            float floatDelay = 3f, float floatTime = 2f, float jitterStrength = 2f)
        {
            if (_pool == null) return;
            var node = _pool.GetInstance();
            node.Set("text", text);
            node.Set("char_delay", 0f);
            node.Set("float_delay", floatDelay);
            node.Set("float_time", floatTime);
            node.Set("jitter_strength", jitterStrength);
            node.Position = position;
            node.Call("spawn");
        }

        public static void SpawnRandom(string text)
        {
            float x = (float)GD.RandRange(150.0, 1300.0);
            float y = (float)GD.RandRange(200.0, 650.0);
            Spawn(text, new Vector2(x, y));
        }

                public static void StartAuto(NCombatRoom combatRoom, MoonTextData[] data)
        {
            if (data == null || data.Length == 0) return;
            bool allHaveTime = data.All(d => d.time.HasValue);
            bool noneHaveTime = data.All(d => !d.time.HasValue);
            if (allHaveTime)
                StartSequential(combatRoom, data);
            else if (noneHaveTime)
                StartRandom(combatRoom, data);
        }

        public static void StartSequential(NCombatRoom combatRoom, MoonTextData[] data)
        {
            if (data == null || data.Length == 0) return;
            int index = 0;
            var timer = new Timer();
            timer.OneShot = true;
            combatRoom.AddChild(timer);

            void ScheduleNext()
            {
                if (index >= data.Length) return;
                float currentTime = data[index].time!.Value;
                int batchEnd = index;
                while (batchEnd < data.Length && data[batchEnd].time == currentTime)
                    batchEnd++;
                float waitTime;
                if (batchEnd < data.Length)
                    waitTime = data[batchEnd].time!.Value - currentTime;
                else
                    waitTime = 5f;
                if (waitTime > 10f) waitTime = 5f;
                float typingDuration = waitTime / 5f;
                float floatDelay = waitTime * 4f / 5f;
                float floatTime = 2f;
                for (int i = index; i < batchEnd; i++)
                {
                    float x = (float)GD.RandRange(0.0, 1350.0);
                    float y = (float)GD.RandRange(200.0, 650.0);
                    Spawn(data[i].text, new Vector2(x, y),
                        typingDuration, floatDelay, floatTime);
                }
                index = batchEnd;
                if (index < data.Length)
                {
                    timer.WaitTime = data[index].time!.Value - currentTime;
                    timer.Start();
                }
            }
            timer.WaitTime = data[0].time!.Value;
            timer.Timeout += ScheduleNext;
            timer.Start();
        }

        public static void StartRandom(NCombatRoom combatRoom, MoonTextData[] data)
        {
            if (data == null || data.Length == 0) return;
            var timer = new Timer();
            timer.WaitTime = 5f;
            timer.OneShot = false;
            timer.Autostart = true;
            combatRoom.AddChild(timer);
            timer.Timeout += () =>
            {
                float x = (float)GD.RandRange(150.0, 1300.0);
                float y = (float)GD.RandRange(200.0, 650.0);
                var item = data[(int)(GD.Randi() % (uint)data.Length)];
                Spawn(item.text, new Vector2(x, y));
            };
        }
    }
}

[HarmonyPatch(typeof(NCombatRoom), "_Ready")]
public static class KaguyaMoonTextPatch
{
    public static void Postfix(NCombatRoom __instance)
    {
        Kaguya.KaguyaMoonText.Setup(__instance);
    }
}