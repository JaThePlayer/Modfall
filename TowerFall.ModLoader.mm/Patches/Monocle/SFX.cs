using Microsoft.Xna.Framework.Audio;
using MonoMod;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TowerFall;
using TowerFall.ModLoader.mm;

#pragma warning disable CS0626
#pragma warning disable CS0108
namespace Monocle
{
    class patch_SFX : SFX
    {
        public patch_SFX() : base(null, true)
        {
            // no-op
        }
        [MonoModIgnore]
        public SoundEffect Data { get; private set; }

        public extern void orig_ctor(string filename, bool obeysMasterPitch = true);
        [MonoModConstructor]
        public void ctor(string filename, bool obeysMasterPitch = true)
        {
            if (File.Exists(Audio.LOAD_PREFIX + filename + ".wav"))
            {
                orig_ctor(filename, obeysMasterPitch);
                return;
            }
            // Custom SFX - uses absolute path
            FileStream fileStream = new FileStream(filename, FileMode.Open);
            try
            {
                Data = SoundEffect.FromStream(fileStream);
            }
            catch (NoAudioHardwareException)
            {
                Data = null;
            }
            fileStream.Close();
        }
    }
}
