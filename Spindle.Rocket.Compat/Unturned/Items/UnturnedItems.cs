using SDG.Framework.Utilities;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Rocket.Unturned.Items;

[TypeForwardedFrom(RocketCompatIntl.RocketUnturnedAssembly)]
public static class UnturnedItems
{
    public static ItemAsset GetItemAssetByName(string name)
    {
        if (ushort.TryParse(name, out ushort id))
        {
            if (Assets.find(EAssetType.ITEM, id) is ItemAsset itemAsset)
                return itemAsset;
        }

        if (Guid.TryParse(name, out Guid guid))
        {
            return Assets.find<ItemAsset>(guid);
        }

        List<ItemAsset> list = ListPool<ItemAsset>.claim();
        try
        {
            Assets.find(list);
            list.RemoveAll(x => x.itemName == null);
            list.Sort((a, b) => a.itemName.Length.CompareTo(b.itemName.Length));

            return list.Find(i => i.itemName.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                   ?? list.Find(i => !i.isPro && i.itemName.IndexOf(name, StringComparison.InvariantCultureIgnoreCase) >= 0);
        }
        finally
        {
            ListPool<ItemAsset>.release(list);
        }
    }

    public static ItemAsset GetItemAssetById(ushort id)
    {
        return Assets.find(EAssetType.ITEM, id) as ItemAsset;
    }

    public static Item AssembleItem(ushort itemId, byte clipsize,
        Attachment sight, Attachment tactical, Attachment grip, Attachment barrel, Attachment magazine,
        EFiremode firemode = 0, byte amount = 1, byte durability = 100)
    {
        ItemAsset itemAsset = Assets.find(EAssetType.ITEM, itemId) as ItemAsset;
        if (itemAsset is not ItemGunAsset)
        {
            return new Item(itemId, amount, durability, itemAsset?.getState(EItemOrigin.ADMIN) ?? Array.Empty<byte>());
        }

        byte[] metadata = new byte[18];

        metadata[10] = clipsize;
        metadata[11] = (byte)firemode;
        metadata[12] = 1;

        if (sight != null && sight.AttachmentId != 0)
        {
            MemoryMarshal.Write(metadata, ref sight.AttachmentId);
            metadata[13] = sight.Durability;
        }
        
        if (tactical != null && tactical.AttachmentId != 0)
        {
            MemoryMarshal.Write(metadata.AsSpan(2, 2), ref tactical.AttachmentId);
            metadata[14] = tactical.Durability;
        }
        
        if (grip != null && grip.AttachmentId != 0)
        {
            MemoryMarshal.Write(metadata.AsSpan(4, 2), ref grip.AttachmentId);
            metadata[15] = grip.Durability;
        }
        
        if (barrel != null && barrel.AttachmentId != 0)
        {
            MemoryMarshal.Write(metadata.AsSpan(6, 2), ref barrel.AttachmentId);
            metadata[16] = barrel.Durability;
        }

        if (magazine != null && magazine.AttachmentId != 0)
        {
            MemoryMarshal.Write(metadata.AsSpan(8, 2), ref magazine.AttachmentId);
            metadata[17] = magazine.Durability;
        }

        return new Item(itemId, amount, durability, metadata);
    }

    public static Item AssembleItem(ushort itemId, byte amount = 1, byte durability = 100, byte[] metadata = null)
    {
        return new Item(itemId, amount, durability, metadata ?? Array.Empty<byte>());
    }
}