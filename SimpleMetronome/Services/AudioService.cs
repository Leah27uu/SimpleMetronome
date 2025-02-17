using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Media;
using System.IO;
using NAudio.Wave;

namespace SimpleMetronome.Services
{

    public class AudioService
    {
        private WaveOutEvent _outputDevice;
        private AudioFileReader _normalBeat;
        private AudioFileReader _accentBeat;
        private string selectedSound = "Beep.wav";
        private string selectedAccentSound = "Beep(Accent).wav";
        private float _volume = 1.0f; // Default full volume

        public AudioService()
        {
            string normalPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", selectedSound);
            string accentPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", selectedAccentSound);

           if (File.Exists(normalPath) && File.Exists(accentPath))
            {
                _outputDevice = new WaveOutEvent();
                _normalBeat = new AudioFileReader(normalPath) { Volume = _volume };
                _accentBeat = new AudioFileReader(accentPath) { Volume = _volume };
            }
           else
            {
                throw new FileNotFoundException("Audio files not found in Assets folder.");
            }

        }

        public void ChangeSound(string newSound)
        {
            if (newSound == "Beep")
            {
                selectedSound = "Beep.wav";
                selectedAccentSound = "Beep(Accent).wav";

            }
            else if (newSound == "Click")
            {
                selectedSound = "Click.wav";
                selectedAccentSound = "Click(Accent).wav";

            }
            else if (newSound == "Drumstick")
            {
                selectedSound = "Drumstick.wav";
                selectedAccentSound = "Drumstick(Accent).wav";
            }

            string normalPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", selectedSound);
            string accentPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", selectedAccentSound);

            if (File.Exists(normalPath) && File.Exists(accentPath))
            {
                _normalBeat?.Dispose();
                _accentBeat?.Dispose();

                _normalBeat = new AudioFileReader(normalPath) { Volume = _volume };
                _accentBeat = new AudioFileReader(accentPath) { Volume = _volume };

                Console.WriteLine($" Sound changed to: {newSound} (Normal: {selectedSound}, Accent: {selectedAccentSound})");
            }
        }


        public void PlayTick(bool isAccent, string SelectedSound)
        {
            _outputDevice?.Stop();
            _outputDevice?.Dispose();
            _outputDevice = new WaveOutEvent();

            var sound = isAccent ? _accentBeat : _normalBeat;
            sound.Position = 0;

            _outputDevice.Init(sound);
            _outputDevice.Play();
            
        }

        public void SetVolume(double volume)
        {
            _volume = (float)Math.Clamp(volume, 0.0, 1.0);
            if (_normalBeat != null) _normalBeat.Volume = _volume;
            if (_accentBeat != null) _accentBeat.Volume = _volume;
        }

        public void Dispose()
        {
            _outputDevice?.Dispose();
            _normalBeat?.Dispose();
            _accentBeat?.Dispose();
        }
    }
}
