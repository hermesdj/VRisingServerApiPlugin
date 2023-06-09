#nullable enable
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace VRisingServerApiPlugin.players;

public class EquipmentSlotConverter : JsonConverter<EquipmentSlot>
{
    public override EquipmentSlot Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, EquipmentSlot value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(Enum.GetName(typeof(EquipmentSlot), value));
    }
}