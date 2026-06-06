using BaseLib.Abstracts;
using BaseLib.Audio;
using Godot;
using HarmonyLib;
using Kaguya.CardPools;
using Kaguya.Cards;
using Kaguya.Characters;
using Kaguya.PotionPools;
using Kaguya.RelicPools;
using Kaguya.Relics;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Models.Potions;
using MegaCrit.Sts2.Core.Models.Relics;
using System.Collections.Generic;
using System.Linq;

namespace Kaguya.Characters
{
    public class SakayoriIroha : PlaceholderCharacterModel
    {
        // 角色名称颜色
        public override Color NameColor => new(0.5f, 0.5f, 1f);
        // 能量图标轮廓颜色
        public override Color EnergyLabelOutlineColor => new(0.1f, 0.1f, 1f);

        // 人物性别（男女中立）
        public override CharacterGender Gender => CharacterGender.Feminine;

        // 初始血量
        public override int StartingHp => 68;

        // 人物模型tscn路径。要自定义见下。
        public override string CustomVisualPath => "res://scenes/creature_visuals/sakayori_iroha.tscn";
        // 卡牌拖尾场景。
        public override string CustomTrailPath => "res://scenes/vfx/card_trail_sakayori_iroha.tscn";
        // 人物头像路径。
        public override string CustomIconTexturePath => "res://images/ui/top_panel/character_icon_sakayori_iroha.png";
        // 人物头像2号。
        public override string CustomIconPath => "res://scenes/ui/character_icons/sakayori_iroha_icon.tscn";
        // 能量表盘tscn路径。要自定义见下。
        public override string CustomEnergyCounterPath => "res://scenes/combat/energy_counters/sakayori_iroha_energy_counter.tscn";
        // 篝火休息场景。
        public override string CustomRestSiteAnimPath => "res://scenes/rest_site/characters/sakayori_iroha_rest_site.tscn";
        // 商店人物场景。
        public override string CustomMerchantAnimPath => "res://scenes/merchant/characters/sakayori_iroha_merchant.tscn";
        // 多人模式-手指。
         public override string CustomArmPointingTexturePath => "res://images/ui/hands/sakayori_iroha_hand1.png";
        // 多人模式剪刀石头布-石头。
         public override string CustomArmRockTexturePath => "res://images/ui/hands/sakayori_iroha_hand3.png";
        // 多人模式剪刀石头布-布。
         public override string CustomArmPaperTexturePath => "res://images/ui/hands/sakayori_iroha_hand2.png";
        // 多人模式剪刀石头布-剪刀。
         public override string CustomArmScissorsTexturePath => "res://images/ui/hands/sakayori_iroha_had3.png";

        // 人物选择背景。
        public override string CustomCharacterSelectBg => "res://scenes/screens/char_select/char_select_bg_sakayori_iroha.tscn";
        // 人物选择图标。
        public override string CustomCharacterSelectIconPath => "res://images/packed/character_select/char_select_sakayori_iroha.png";
        // 人物选择图标-锁定状态。
        public override string CustomCharacterSelectLockedIconPath => "res://images/packed/character_select/char_select_sakayori_iroha_locked.png";
        // 人物选择过渡动画。
        public override string CustomCharacterSelectTransitionPath => "res://materials/transitions/sakayori_iroha_transition_mat.tres";
        // 地图上的角色标记图标、表情轮盘上的角色头像
        //public override string CustomMapMarkerPath => "res://images/atlases/ui_atlas.sprites/map/icons/map_marker_sakayori_iroha.tres";
        // 攻击音效
        public override string CustomAttackSfx
        {
            get
            {
                ModAudio.PlaySound("res://audios/attack.ogg");
                return "";
            }
        }
        // 施法音效
        // public override string CustomCastSfx => null;
        // 死亡音效
        public override string CustomDeathSfx
        {
            get
            {
                ModAudio.PlaySound("res://audios/death.ogg", volumeAdd: 15f);
                return "";
            }
        }
        // 角色选择音效
        public override string CharacterSelectSfx
        {
            get
            {
                // 播放自定义音效
                ModAudio.PlaySound("res://audios/sakayori_iroha_select.ogg", volumeAdd: 12f);
                // 返回空，防止 FMOD 系统尝试播放无效路径
                return "";
            }
        }
        // 过渡音效。这个不能删。
        public override string CharacterTransitionSfx => "event:/sfx/ui/wipe_silent";

        public override CardPoolModel CardPool => ModelDb.CardPool<KaguyaCardPool>();
        public override RelicPoolModel RelicPool => ModelDb.RelicPool<KaguyaRelicPool>();
        public override PotionPoolModel PotionPool => ModelDb.PotionPool<KaguyaPotionPool>();

        // 初始卡组
        public override IEnumerable<CardModel> StartingDeck => [
            ModelDb.Card<StrikeIroha>(),
            ModelDb.Card<StrikeIroha>(),
            ModelDb.Card<StrikeIroha>(),
            ModelDb.Card<StrikeIroha>(),
            ModelDb.Card<StrikeIroha>(),
            ModelDb.Card<DefendIroha>(),
            ModelDb.Card<DefendIroha>(),
            ModelDb.Card<DefendIroha>(),
            ModelDb.Card<DefendIroha>(),
            ModelDb.Card<TsukuyomiNight>(),
            ModelDb.Card<PartTimeJob>(),
    ];

        // 初始遗物
        public override IReadOnlyList<RelicModel> StartingRelics => [
            ModelDb.Relic<RundownApartment>(),
    ];

        // 攻击建筑师的攻击特效列表
        public override List<string> GetArchitectAttackVfx() => [
            "vfx/vfx_attack_blunt",
        "vfx/vfx_heavy_blunt",
        "vfx/vfx_attack_slash",
        "vfx/vfx_bloody_impact",
        "vfx/vfx_rock_shatter"
        ];
    }
}