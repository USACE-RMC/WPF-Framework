using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FrameworkInterfaces;

namespace Demo_ProjectUI.Project.Response_Elements
{
    class ResponseElement : ElementBase
    {
        public ResponseElement(string name, IElementCollection parentCollection) : base(name, parentCollection)
        {
            Name = name;
            SetIsDirty(false);
        }

        public override string Name { get; set; }
        public override string Description { get => "Response Element"; set => throw new NotImplementedException(); }

        public override DateTime CreationDate => DateTime.Now;

        public override DateTime LastModified => DateTime.Now;

        public override string NameOnDisk => "Response Element";

        public override Bitmap ElementImage => Properties.Resources.Delete;

        public override bool CanCopyFromExternal => true;

        public override bool IsValid { get => true; }

        public override IElement Copy(string newName = null)
        {
            //throw new NotImplementedException();
            return null;
        }

        public override IElement CopyFromExternal(string itemName, string fullFileName)
        {
            //throw new NotImplementedException();
            return null;
        }

        public override void Delete()
        {
            //throw new NotImplementedException();
        }


        public override void Open()
        {
            //throw new NotImplementedException();
        }

        public override void Save()
        {
            //throw new NotImplementedException();
        }
    }
}
