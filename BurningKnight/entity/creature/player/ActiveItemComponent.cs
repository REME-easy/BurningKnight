using System;
using BurningKnight.assets;
using BurningKnight.assets.input;
using BurningKnight.assets.items;
using BurningKnight.debug;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using BurningKnight.level.biome;
using BurningKnight.save;
using BurningKnight.state;
using BurningKnight.ui.dialog;
using Lens.assets;
using Lens.entity;
using Lens.entity.component.logic;
using Lens.input;
using Lens.util.timer;

namespace BurningKnight.entity.creature.player {
	public class ActiveItemComponent : ItemComponent {
		public override void PostInit() {
			base.PostInit();

			if (Item != null && Run.Depth < 1) {
				Item.Done = true;
				Item = null;
			}
		}

		public override bool HandleEvent(Event e) {
			if (e is RoomClearedEvent) {
				if (Item != null && Item.UseTime > 0.02f) {
					var o = Item.Delay;
					Item.Delay = Math.Max(Item.Delay - 1, 0f);

					if (Math.Abs(o) >= 0.01f && Math.Abs(Item.Delay) < 0.01f) {
						Audio.PlaySfx("item_active_charged");
					}
				}
			}
			
			return base.HandleEvent(e);
		}

		protected override void OnItemSet(Item previous) {
			base.OnItemSet(previous);
			
			if (Run.Depth > 0 && GlobalSave.IsFalse("control_active") && GetComponent<DialogComponent>().Dialog?.Str != null) {
				var dialog = GetComponent<DialogComponent>();
				
				dialog.Dialog.Str.ClearIcons();
				dialog.Dialog.Str.AddIcon(CommonAse.Ui.GetSlice(Controls.FindSlice(Controls.Active, false)));

				if (GamepadComponent.Current != null && GamepadComponent.Current.Attached) {
					dialog.Dialog.Str.AddIcon(CommonAse.Ui.GetSlice(Controls.FindSlice(Controls.Active, true)));
				}
				
				dialog.StartAndClose("control_6", 5);
			}

			if (Item.Id == "bk:snow_bucket" && !(Run.Level.Biome is IceBiome)) {
				Timer.Add(() => {
					var i = Item;
				
					Drop();
					i.Done = true;

					Entity.GetComponent<InventoryComponent>().Pickup(Items.CreateAndAdd("bk:water_bucket", Entity.Area));
				}, 3f);
			}
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (Run.Depth > 0 && Item != null && !Item.Done && Input.WasPressed(Controls.Active, GetComponent<InputComponent>())) {
				if (GetComponent<PlayerInputComponent>().InDialog) {
					return;
				}
			
				if (GetComponent<StateComponent>().StateInstance is Player.SleepingState) {
					GetComponent<StateComponent>().Become<Player.IdleState>();
				}
				
				if (Item.Use((Player) Entity)) {
					if (CheatWindow.InfiniteActive) {
						Item.Delay = 0;
					}
					
					if (Run.Depth > 0 && GlobalSave.IsFalse("control_active")) {
						Entity.GetComponent<DialogComponent>().Close();
						GlobalSave.Put("control_active", true);
					}
					
					Audio.PlaySfx("item_active");

					if (Item.SingleUse && !CheatWindow.InfiniteActive) {
						Item.Done = true;
					}
				}
			}
		}

		public void Clear() {
			Item = null;
		}

		protected override bool ShouldReplace(Item item) {
			return item.Type == ItemType.Active;
		}
		
		public bool IsFullOrEmpty() {
			return Item == null || Item.Delay <= 0.01f;
		}

		public void Charge(int amount) {
			if (Item != null && Item.UseTime > 0.02f) {
				Item.Delay = Math.Max(Item.Delay - amount, 0f);
			}
		}
	}
}