﻿using System;
using System.Diagnostics;
using System.IO;

namespace GvasFormat.Serialization.UETypes
{
    [DebuggerDisplay("Count = {Items.Length}", Name = "{Name}")]
    public sealed class UEArrayProperty : UEProperty
    {
        public UEArrayProperty() { }

        public UEArrayProperty(BinaryReader reader, long valueLength)
        {
            ItemType = reader.ReadUEString();
            byte terminator = reader.ReadByte();
            if (terminator != 0)
            {
                throw new FormatException($"Offset: 0x{reader.BaseStream.Position - 1:x8}. Expected terminator (0x00), but was (0x{terminator:x2})");
            }

            // valueLength starts here
            int count = reader.ReadInt32();
            Items = new UEProperty[count];

            switch (ItemType)
            {
                case "StructProperty":
                    Items = Read(reader, count);
                    break;
                case "ByteProperty":
                    Items = UEByteProperty.Read(reader, valueLength, count);
                    break;
                case "FloatProperty":
                    Items = UEFloatProperty.Read(reader, valueLength, count);
                    break;
                case "IntProperty":
                    Items = UEIntProperty.Read(reader, valueLength, count);
                    break;
                case "BoolProperty":
                    Items = UEBoolProperty.Read(reader, valueLength, count);
                    break;
                case "TextProperty":
                    Items = UETextProperty.Read(reader, valueLength, count);
                    break;
                default:
                {
                    for (int i = 0; i < count; i++)
                    {
                        Items[i] = UESerializer.Deserialize(null, ItemType, -1, reader);
                    }

                    break;
                }
            }
        }
        public override void Serialize(BinaryWriter writer) { throw new NotImplementedException(); }

        public string ItemType;
        public UEProperty[] Items;
    }
}