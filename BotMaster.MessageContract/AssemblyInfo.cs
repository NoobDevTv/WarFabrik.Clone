using NonSucking.Framework.Serialization;

[assembly: NoosonConfiguration(
    GenerateDeserializeExtension = false,
    DisableWarnings = true,
    GenerateStaticDeserializeWithCtor = true,
    GenerateDeserializeOnInstance = false,
    GenerateStaticSerialize = true,
    GenerateStaticDeserializeIntoInstance = true)]