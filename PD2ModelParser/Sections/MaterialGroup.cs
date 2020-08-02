﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PD2ModelParser.Sections
{
    [SectionId(Tags.material_group_tag)]
    class MaterialGroup : AbstractSection, ISection
    {
        private UInt32 size;
        private List<UInt32> itemIds = new List<UInt32>();

        public UInt32 Count => (uint)itemIds.Count;
        public List<Material> Items { get; set; } = new List<Material>();
        public byte[] remaining_data = null;

        public MaterialGroup(uint sec_id, Material mat)
        {
            this.SectionId = sec_id;
            this.size = 0;
            this.Items.Add(mat);
        }

        public MaterialGroup(uint sec_id, IEnumerable<Material> mats)
        {
            this.SectionId = sec_id;
            this.size = 0;
            this.Items = mats.ToList();
        }

        public MaterialGroup(BinaryReader instream, SectionHeader section)
        {
            this.SectionId = section.id;
            this.size = section.size;
            
            var count = instream.ReadUInt32();
            for (int x = 0; x < count; x++)
            {
                this.itemIds.Add(instream.ReadUInt32());
            }
            byte[] remaining_data = null;
            if ((section.offset + 12 + section.size) > instream.BaseStream.Position)
            {
                remaining_data = instream.ReadBytes((int)((section.offset + 12 + section.size) - instream.BaseStream.Position));
                Log.Default.Info($"Read a {nameof(MaterialGroup)} with remaining data of size {remaining_data.Length}");
            }
        }

        public override void PostLoad(uint id, Dictionary<uint, ISection> sections)
        {
            base.PostLoad(id, sections);
            foreach(var itemid in itemIds)
            {
                if(sections.TryGetValue(itemid, out var value)) {
                    if(value is Material mat)
                    {
                        this.Items.Add(mat);
                    }
                    else { throw new Exception($"Couldn't load {nameof(MaterialGroup)} {this.SectionId}: Section {value.SectionId} is not a Material"); }
                }
                else { throw new Exception($"Couldn't load {nameof(MaterialGroup)} {this.SectionId}: Section {itemid} doesn't exist!"); }
            }
            itemIds = null;
        }

        public override void StreamWriteData(BinaryWriter outstream)
        {
            outstream.Write(this.Count);
            foreach (var item in this.Items)
                outstream.Write(item.SectionId);

            if (this.remaining_data != null)
                outstream.Write(this.remaining_data);
        }

        public override string ToString()
        {
            string items_string = (this.Items.Count == 0 ? "none" : "");

            items_string += string.Join(", ", this.Items.Select(i => i.SectionId));

            return base.ToString() + " size: " + this.size + " Count: " + this.Count + " Items: [ " + items_string + " ] " + (this.remaining_data != null ? " REMAINING DATA! " + this.remaining_data.Length + " bytes" : "");
        }
    }
}