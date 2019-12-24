using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monocle;
using MonoMod;

namespace TowerFall
{
    class patch_MapButton : MapButton
    {
        [MonoModIgnore]
        public override void OnConfirm()
        {
            throw new NotImplementedException();
        }
        [MonoModIgnore]
        protected override List<Image> InitImages()
        {
            throw new NotImplementedException();
        }

        public patch_MapButton() : base("")
        {
            // no-op
        }

        
        public void SetAuthor(string author)
        {
            Author = author;
        }

        [MonoModIgnore]
        public string Author { get; private set; }
    }
}
