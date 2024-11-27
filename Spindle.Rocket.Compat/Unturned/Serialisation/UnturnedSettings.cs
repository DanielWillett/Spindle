using Rocket.API;
using System;
using System.Xml.Serialization;

namespace Rocket.Unturned.Serialisation;

[TypeForwardedFrom(RocketCompatIntl.RocketUnturnedAssembly)]
public sealed class UnturnedSettings : IDefaultable
{
    [XmlElement("RocketModObservatory")]
    [Obsolete("Observatory is no longer maintained.")]
    public RocketModObservatorySettings RocketModObservatory = new RocketModObservatorySettings();

    [XmlElement("AutomaticSave")]
    public AutomaticSaveSettings AutomaticSave = new AutomaticSaveSettings();

    [XmlElement("CharacterNameValidation")]
    public bool CharacterNameValidation;

    [XmlElement("CharacterNameValidationRule")]
    public string CharacterNameValidationRule = "([\\x00-\\xAA]|[\\w_\\ \\.\\+\\-])+";

    public bool LogSuspiciousPlayerMovement = true;
    public bool EnableItemBlacklist;
    public bool EnableItemSpawnLimit;
    public bool EnableVehicleBlacklist;
    public int MaxSpawnAmount;

    public void LoadDefaults()
    {
        AutomaticSave = new AutomaticSaveSettings();

#pragma warning disable CS0618
        RocketModObservatory = new RocketModObservatorySettings();
#pragma warning restore CS0618

        CharacterNameValidation = true;
        CharacterNameValidationRule = "([\\x00-\\xAA]|[\\w_\\ \\.\\+\\-])+";
        LogSuspiciousPlayerMovement = true;
        EnableItemBlacklist = false;
        EnableItemSpawnLimit = false;
        MaxSpawnAmount = 10;
        EnableVehicleBlacklist = false;
    }
}