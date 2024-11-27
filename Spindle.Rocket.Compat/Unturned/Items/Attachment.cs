namespace Rocket.Unturned.Items;

[TypeForwardedFrom(RocketCompatIntl.RocketUnturnedAssembly)]
public class Attachment
{
    public ushort AttachmentId;
    public byte Durability;

    public Attachment(ushort attachmentId, byte durability)
    {
        AttachmentId = attachmentId;
        Durability = durability;
    }
}