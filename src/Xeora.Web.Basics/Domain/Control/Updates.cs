using System;

namespace Xeora.Web.Basics.Domain.Control
{
    public class Updates
    {
        public Updates()
        {
            this.Local = true;
            this.Blocks = new string[] { };
        }

        public Updates(bool local, string[] blocks)
        {
            this.Local = local;
            this.Blocks = blocks ?? new string[] { };
        }

        public bool Local { get; }
        public string[] Blocks { get; private set; }

        public void Setup(string parentBlockId)
        {
            if (string.IsNullOrEmpty(parentBlockId))
                return;

            if (!this.Local)
                return;

            if (string.IsNullOrEmpty(parentBlockId) || Array.IndexOf(this.Blocks, parentBlockId) != -1) return;
            
            string[] blocks = 
                new string[this.Blocks.Length + 1];
            Array.Copy(this.Blocks, blocks, this.Blocks.Length);
            blocks[^1] = parentBlockId;

            this.Blocks = blocks;
        }

        public Updates Clone() =>
            new Updates(this.Local, this.Blocks);
    }
}