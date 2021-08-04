using System;
using System.Collections.Generic;
using System.Text;

namespace MonsterConverterInterface.MonsterTypes
{
    public class Voice
    {
        public Voice(string sound, SoundLevel volume = SoundLevel.Say)
        {
            Sound = sound;
            SoundLevel = volume;
        }

        public string Sound { get; set; }
        public SoundLevel SoundLevel { get; set; }
    }
}
