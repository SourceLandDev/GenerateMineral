using LiteLoader.Hook;
using LiteLoader.NET;
using MC;

namespace GenerateMineral;
[PluginMain("GenerateMineral")]
public class Main : IPluginInitializer
{
    public string Introduction => "刷矿机";
    public Dictionary<string, string> MetaData => new();
    public Version Version => new();
    internal static readonly Dictionary<string, int> data = new()
    {
        ["minecraft:stone"] = 64,
        ["minecraft:coal_ore"] = 15,
        ["minecraft:copper_ore"] = 13,
        ["minecraft:iron_ore"] = 11,
        ["minecraft:gold_ore"] = 9,
        ["minecraft:redstone_ore"] = 7,
        ["minecraft:lapis_ore"] = 5,
        ["minecraft:emerald_ore"] = 3,
        ["minecraft:diamond_ore"] = 1
    };
    internal static int sum = data.Values.Sum();
    public void OnInitialize() => Thook.RegisterHook<OnSolidifyHook, OnSolidifyHookDelegate>();
}

internal delegate void OnSolidifyHookDelegate(nint @this, pointer<BlockSource> a2, BlockPos a3, BlockPos a4);
[HookSymbol("?solidify@LiquidBlock@@IEBAXAEAVBlockSource@@AEBVBlockPos@@1@Z")]
internal class OnSolidifyHook : THookBase<OnSolidifyHookDelegate>
{
    public override OnSolidifyHookDelegate Hook =>
        (@this, a2, a3, a4) =>
        {
            Original(@this, a2, a3, a4);
            BlockSource blockSource = a2.Dereference();
            if (blockSource.GetBlock(a3).Name != "minecraft:cobblestone")
            {
                return;
            }
            int randInt = Random.Shared.Next(Main.sum);
            int testedWeight = default;
            foreach ((string block, int weight) in Main.data)
            {
                if (randInt >= (testedWeight += weight))
                {
                    continue;
                }
                if (string.IsNullOrWhiteSpace(block))
                {
                    return;
                }
                Level.SetBlock(a3, blockSource.DimensionId, block, default);
                return;
            }
        };
}
