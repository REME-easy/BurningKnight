using BurningKnight.entity.component;
using BurningKnight.physics;
using Lens.entity;

namespace BurningKnight.level {
	public class Chasm : Entity, CollisionFilterEntity {
		public Level Level;

		public override void AddComponents() {
			base.AddComponents();
			AddComponent(new ChasmBodyComponent());
		}

		public bool ShouldCollide(Entity entity) {
			// fixme: makes it impossible to pass to platform from top
			return !entity.TryGetComponent<SupportableComponent>(out var t) || t.Supports.Count == 0;
		}
	}
}