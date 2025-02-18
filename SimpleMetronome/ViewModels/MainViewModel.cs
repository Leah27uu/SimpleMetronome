using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using SimpleMetronome.Helpers; // Imports RelayCommand
using SimpleMetronome.Services; // Imports RelayCommand
using System.Timers;
using System.Reflection;
using System.Diagnostics;

namespace SimpleMetronome.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private int _bpm = 120;
        private bool _isRunning;
        private string _startStopText = "Start";
        private string _selectedTimeSignature = "4/4";
        private string _selectedSound = "Click"; // Default selected sound 
        private int _beatsPerMeasure = 4;
        private bool _accentFirstBeat = false; // Default: No accent
        private double _volume = 1.0; // Default volume at 100%
        private string _version;

        private List<DateTime> _tapTimes = new();
        private const int MaxTapCount = 5; // Number of taps to average


        private readonly AudioService _audioService;
        private readonly TimerService _timerService;


        


        public ObservableCollection<string> TimeSignatures { get; set; }
        public ObservableCollection<string> SoundOptions { get; set; }

        public int BPM
        {
            get => _bpm;
            set 
            {
                _bpm = value;
                OnPropertyChanged();
                if (_isRunning) _timerService.Restart(BPM, _beatsPerMeasure, AccentFirstBeat); // Update timer
            }
        }

        private string GetVersion()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            return version?.ToString() ?? "1.0.0";
        }

        public string Version { get; set; }
        
        

        public double Volume
        {
            get => _volume;
            set
            {
                _volume = Math.Clamp(value, 0.0, 1.0); 
                OnPropertyChanged();
                _audioService.SetVolume(_volume);
            }
        }

        public bool AccentFirstBeat
        {
            get => _accentFirstBeat;
            set
            {
                _accentFirstBeat = value;
                OnPropertyChanged();
                if (_isRunning) _timerService.Restart(BPM, _beatsPerMeasure, AccentFirstBeat);
            }
        }

        public string StartStopText
        {
            get => _startStopText;
            set { _startStopText = value; OnPropertyChanged(); }
        }

        public string SelectedTimeSignature
        {
            get => _selectedTimeSignature;
            set
            {
                _selectedTimeSignature = value;
                _beatsPerMeasure = int.Parse(value.Split('/')[0]); // Extract beats from "4/4"
                OnPropertyChanged();
                if (_isRunning) _timerService.Restart(BPM, _beatsPerMeasure, AccentFirstBeat);
            }
        }

        public string SelectedSound
        {
            get => _selectedSound;
            set
            {
                _selectedSound = value;
                OnPropertyChanged();
            }
        }

        public ICommand IncreaseBPMCommand { get; }
        public ICommand DecreaseBPMCommand { get; }
        public ICommand ToggleMetronomeCommand { get; }
        public ICommand TapTempoCommand { get; }

        public MainViewModel()
        {
            Version = GetVersion();
            _timerService = new TimerService();
            _audioService = new AudioService();
            _timerService.Tick += (isAccent) => _audioService.PlayTick(isAccent); // Play correct sound

            TimeSignatures = new ObservableCollection<string> { "2/4", "3/4", "4/4", "6/8" };
            SoundOptions = new ObservableCollection<string> { "Click", "Beep", "Drumstick" };

            IncreaseBPMCommand = new RelayCommand(_ => IncreaseBPM());
            DecreaseBPMCommand = new RelayCommand(_ => DecreaseBPM());
            ToggleMetronomeCommand = new RelayCommand(_ => ToggleMetronome());
            TapTempoCommand = new RelayCommand(_ => TapTempo());
        }

        private void IncreaseBPM() => BPM = Math.Min(BPM + 1, 240);
        private void DecreaseBPM() => BPM = Math.Max(BPM - 1, 40);

        private void TapTempo()
        {
            DateTime now = DateTime.Now;

            if (_tapTimes.Count >= MaxTapCount)
                _tapTimes.RemoveAt(0);

            _tapTimes.Add(now);

            if (_tapTimes.Count > 1)
            {
                double avgInterval = _tapTimes.Zip(_tapTimes.Skip(1), (prev, next) => (next - prev).TotalSeconds).Average();

                int newBPM = (int)Math.Round(60.0 / avgInterval);

                // Ensure BPM is within the valid range (40 - 240 BPM)
                newBPM = Math.Clamp(newBPM, 40, 240);

                // 🔹 Adjust BPM using existing logic instead of setting it directly
                while (BPM < newBPM) IncreaseBPM();
                while (BPM > newBPM) DecreaseBPM();

                Debug.WriteLine($"🎵 Tap Tempo BPM Set To: {BPM}");
            }
        }


        private void ToggleMetronome()
        {
            if (_isRunning)
            {
                _timerService.Stop();
                StartStopText = "Start";
            }
            else
            {
                _timerService.Start(BPM, _beatsPerMeasure, AccentFirstBeat);
                StartStopText = "Stop";
            }
            _isRunning = !_isRunning;        
            
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            _timerService.Tick += (isAccent) => _audioService.PlayTick(isAccent);
        }
    }
}
