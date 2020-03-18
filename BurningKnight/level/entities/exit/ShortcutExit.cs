using BurningKnight.assets;
using BurningKnight.level.biome;
using BurningKnight.state;
using ImGuiNET;
using Lens;
using Lens.assets;
using Lens.graphics;
using Lens.util.file;

namespace BurningKnight.level.entities.exit {
	public class ShortcutExit : Exit {
		private byte id;
    		
    protected override void Descend() {
      Run.StartNew(id);
    }

    protected override string GetFxText() {
      return Locale.Get(BiomeRegistry.GenerateForDepth(id).Id);
    }

    public override void Load(FileReader stream) {
      base.Load(stream);
      id = stream.ReadByte();
    }

    public override void Save(FileWriter stream) {
      base.Save(stream);
      stream.WriteByte(id);
    }

    public override void RenderImDebug() {
      base.RenderImDebug();
      var v = (int) id;

      if (ImGui.InputInt("To depth", ref v)) {
    	  id = (byte) v;
      }
    }

    public override void Render() {
      base.Render();

      if (Engine.EditingLevel) {
    	  Graphics.Print(id.ToString(), Font.Small, Position);
      }
    }
	}
}