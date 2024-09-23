using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;

namespace Content.Shared.VendingMachines;

public enum SortType : byte
{
    /// <summary>
    /// Use the same order that items were defined in the prototype
    /// </summary>
    Unsorted,

    /// <summary>
    /// Sort alphabetically, based on localized name
    /// </summary>
    Lexographic,
}


[Serializable, NetSerializable, Prototype("vendingMachineInventory")]
public sealed partial class VendingMachineInventoryPrototype : IPrototype
{
    [ViewVariables]
    [IdDataField]
    public string ID { get; private set; } = default!;

    /// <summary>
    /// How the inventory should be sorted when shown to users
    /// </summary>
    [DataField("inventorySort")]
    public SortType SortType { get; private set; } = SortType.Unsorted;

    [DataField("startingInventory", customTypeSerializer:typeof(PrototypeIdDictionarySerializer<uint, EntityPrototype>))]
    public Dictionary<string, uint> StartingInventory { get; private set; } = new();

    [DataField("emaggedInventory", customTypeSerializer:typeof(PrototypeIdDictionarySerializer<uint, EntityPrototype>))]
    public Dictionary<string, uint>? EmaggedInventory { get; private set; }

    [DataField("contrabandInventory", customTypeSerializer:typeof(PrototypeIdDictionarySerializer<uint, EntityPrototype>))]
    public Dictionary<string, uint>? ContrabandInventory { get; private set; }
}
