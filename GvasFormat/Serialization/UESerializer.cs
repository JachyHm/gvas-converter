﻿using GvasFormat.Serialization.UETypes;
using GvasFormat.Utils;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace GvasFormat.Serialization
{
    public static partial class UESerializer
    {
        public static Gvas Read(Stream stream)
        {
            using (BinaryReader reader = new BinaryReader(stream, Encoding.ASCII, true))
            {
                byte[] header = reader.ReadBytes(Gvas.Header.Length);
                if (!Gvas.Header.SequenceEqual(header))
                {
                    throw new FormatException($"Invalid header, expected {Gvas.Header.AsHex()}");
                }

                Gvas result = new Gvas
                {
                    SaveGameVersion = reader.ReadInt32(),
                    PackageVersion = reader.ReadInt32()
                };
                result.EngineVersion.Major = reader.ReadInt16();
                result.EngineVersion.Minor = reader.ReadInt16();
                result.EngineVersion.Patch = reader.ReadInt16();
                result.EngineVersion.Build = reader.ReadInt32();
                result.EngineVersion.BuildId = reader.ReadUEString();
                result.CustomFormatVersion = reader.ReadInt32();
                result.CustomFormatData.Count = reader.ReadInt32();
                result.CustomFormatData.Entries = new CustomFormatDataEntry[result.CustomFormatData.Count];
                for (int i = 0; i < result.CustomFormatData.Count; i++)
                {
                    CustomFormatDataEntry entry = new CustomFormatDataEntry
                    {
                        Id = new Guid(reader.ReadBytes(16)),
                        Value = reader.ReadInt32()
                    };
                    result.CustomFormatData.Entries[i] = entry;
                }
                result.SaveGameType = reader.ReadUEString();

                while (UEProperty.Read(reader) is UEProperty prop)
                {
                    result.Properties.Add(prop);
                }

                return result;
            }
        }
    }
}
