using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Media;
using System.IO;

namespace SimpleMetronome.Services
{

    public class AudioService
    {
        private SoundPlayer _normalBeat;
        private SoundPlayer _accentBeat;

        public AudioService()
        {
            string normalPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "tick.wav");
            string accentPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "tick(Accent).wav");

            _normalBeat = new SoundPlayer(normalPath);
            _accentBeat = new SoundPlayer(accentPath);
        }

        public void PlayTick(bool isAccent)
        {
            if (isAccent)
                _accentBeat.Play();
            else
                _normalBeat.Play();
            
        }
    }
}
