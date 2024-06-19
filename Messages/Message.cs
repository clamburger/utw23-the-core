using System.Text.Json.Serialization;

namespace UbertweakNfcReaderWeb.Messages;

public enum ScannerState
{
    Disabled = 1,
    Ready = 2,
    InvalidCard = 3,
    ReadyToSelect = 4,
    OptionSelected = 5,
}

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(derivedType: typeof(Heartbeat), typeDiscriminator: 1)]
[JsonDerivedType(derivedType: typeof(CardRead), typeDiscriminator: 2)]
[JsonDerivedType(derivedType: typeof(OptionSelected), typeDiscriminator: 3)]
[JsonDerivedType(derivedType: typeof(SetNumberOfOptions), typeDiscriminator: 101)]
[JsonDerivedType(derivedType: typeof(SetOptionTitle), typeDiscriminator: 102)]
[JsonDerivedType(derivedType: typeof(SetOptionEnabled), typeDiscriminator: 103)]
[JsonDerivedType(derivedType: typeof(UserDetails), typeDiscriminator: 104)]
[JsonDerivedType(derivedType: typeof(SetState), typeDiscriminator: 105)]
public abstract class Message
{
}

public abstract class ClientMessage : Message
{
    [JsonPropertyName("station")]
    public required int StationId { get; set; }
}

public abstract class ServerMessage : Message
{
}

public class Heartbeat : ClientMessage
{
    [JsonPropertyName("state")] 
    public required ScannerState State { get; set; }
}

public class CardRead : ClientMessage
{
    [JsonPropertyName("len")]
    public required int Length { get; set; }
    [JsonPropertyName("uid")]
    public required string Uid { get; set; }
}

public class OptionSelected : ClientMessage
{
    [JsonPropertyName("uid")]
    public required string Uid { get; set; }
    [JsonPropertyName("option")]
    public required int OptionNumber { get; set; }
}

public class SetNumberOfOptions : ServerMessage
{
    [JsonPropertyName("num_options")]
    public required int OptionCount { get; set; }
}

public class SetOptionTitle : ServerMessage
{
    [JsonPropertyName("option")]
    public required int OptionNumber { get; set; }
    [JsonPropertyName("text")]
    public required string Text { get; set; }
}

public class SetOptionEnabled : ServerMessage
{
    [JsonPropertyName("option")]
    public required int OptionNumber { get; set; }
    [JsonPropertyName("enable")]
    public required int Enabled { get; set; }
}

public class UserDetails : ServerMessage
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }
}

public class SetState : ServerMessage
{
    [JsonPropertyName("state")]
    public required ScannerState State { get; set; }
}