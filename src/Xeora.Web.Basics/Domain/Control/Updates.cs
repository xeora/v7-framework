using System;

namespace Xeora.Web.Basics.Domain.Control
{
    public class Updates
    {
        public Updates(bool local, string[] blocks)
        {
            this.Local = local;
            this.Blocks = blocks;
            if (this.Blocks == null)
                this.Blocks = new string[] { };
        }

        public bool Local { get; private set; }
        public string[] Blocks { get; private set; }

        public void Setup(string parentBlockID)
        {
            if (string.IsNullOrEmpty(parentBlockID))
                return;

            if (!this.Local)
                return;

            if (!string.IsNullOrEmpty(parentBlockID) &&
                Array.IndexOf<string>(this.Blocks, parentBlockID) == -1)
            {
                string[] blocks = new string[this.Blocks.Length + 1];
                Array.Copy(this.Blocks, blocks, this.Blocks.Length);
                blocks[blocks.Length - 1] = parentBlockID;

                this.Blocks = blocks;
            }
        }
    }
}