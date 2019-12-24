﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PD2ModelParser.Sections
{
    class TopologyIP : AbstractSection, ISection
    {
        public UInt32 id;
        public UInt32 size;

        public UInt32 topology_id;

        public byte[] remaining_data = null;

        public TopologyIP(uint sec_id, Topology top)
        {
            this.id = sec_id;
            this.size = 0;
            this.topology_id = top.id;
        }

        public TopologyIP(BinaryReader br, SectionHeader sh)
        {
            this.id = sh.id;
            this.size = sh.size;
            this.topology_id = br.ReadUInt32();
            this.remaining_data = null;
            if ((sh.offset + 12 + sh.size) > br.BaseStream.Position)
                this.remaining_data = br.ReadBytes((int)((sh.offset + 12 + sh.size) - br.BaseStream.Position));
        }

        public override void StreamWriteData(BinaryWriter outstream)
        {
            outstream.Write(this.topology_id);

            if (this.remaining_data != null)
                outstream.Write(this.remaining_data);
        }

        public override string ToString()
        {
            return base.ToString() + " size: " + this.size + " Topology sectionID: " + this.topology_id + (this.remaining_data != null ? " REMAINING DATA! " + this.remaining_data.Length + " bytes" : "");
        }

        public override uint SectionId
        {
            get => id;
            set => id = value;
        }

        public override uint TypeCode => Tags.topologyIP_tag;
    }
}
