using BurningKnight.assets.particle.custom;
using BurningKnight.entity.component;
using BurningKnight.state;
using ImGuiNET;
using Lens.assets;
using Lens.entity;
using Lens.lightJson;

namespace BurningKnight.entity.item.use {
	public class ModifyLuckUse : ItemUse {
		public int Amount;

		public override void Use(Entity entity, Item item) {
			Run.Luck += Amount;
			TextParticle.Add(entity, Locale.Get("luck"), Amount, true, Amount < 0);
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);
			Amount = settings["amount"].Int(1);
		}

		public static void RenderDebug(JsonValue root) {
			var val = root["amount"].Int(1);

			if (ImGui.InputInt("Amount", ref val)) {
				root["amount"] = val;
			}
		}
	}
}