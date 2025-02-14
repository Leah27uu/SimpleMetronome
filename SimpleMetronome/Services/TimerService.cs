using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace SimpleMetronome.Services
{
    public class TimerService
    {
        private System.Timers.Timer _timer;
        private int _bpm;
        private int _currentBeat = 1; // Track current beat
        private int _beatsPerMeasure = 4; //Default time signature
        private bool _isRunning;
        private bool _accentFirstBeat = false; // Default: No accent

        public event Action<bool> Tick; // true = accented beat, false = normal beat

        public TimerService()
        {
            _timer = new System.Timers.Timer();
            _timer.Elapsed += OnTimerElapsed;
            _timer.AutoReset = true;
        }

        public void Start(int bpm, int beatsPerMeasure, bool acentFirstBeat)
        {
            _bpm = bpm;
            _beatsPerMeasure = beatsPerMeasure;
            _accentFirstBeat = acentFirstBeat;
            _timer.Interval = 60000.0 / _bpm; // Calculate beat interval
            _timer.Start();
            _isRunning = true;
            _currentBeat = 1; // Reset beat count when starting
        }

        public void Stop()
        {
            _timer.Stop();
            _isRunning = false;
            _currentBeat = 1; // Reset beat count when stopped
        }

        public void Restart(int bpm, int beatsPerMeasure, bool accentFirstBeat)
        {
            Stop();
            Start(bpm, beatsPerMeasure, accentFirstBeat);
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            bool isAccent = _accentFirstBeat && _currentBeat == 1; // First beat is accented
            Tick?.Invoke(isAccent); // Notify subscribers

            _currentBeat++;
            if (_currentBeat > _beatsPerMeasure)
            {
                _currentBeat = 1; // Reset beat count when measure completes
            }
        }

        public bool IsRunning => _isRunning;
    }
}
