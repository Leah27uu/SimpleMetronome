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

namespace SimpleMetronome.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private int _bpm = 120;
        private bool _isRunning;
        private string _startStopText = "Start";
        private string _selectedTimeSignature = "4/4";
        private string _selectedSound = "Click";
        private int _beatsPerMeasure = 4;
        private bool _accentFirstBeat = false; // Default: No accent

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

        public MainViewModel()
        {
            _timerService = new TimerService();
            _audioService = new AudioService();
            _timerService.Tick += (isAccent) => _audioService.PlayTick(isAccent); // Play correct sound

            TimeSignatures = new ObservableCollection<string> { "2/4", "3/4", "4/4", "6/8" };
            SoundOptions = new ObservableCollection<string> { "Click", "Beep", "Drumstick" };

            IncreaseBPMCommand = new RelayCommand(_ => IncreaseBPM());
            DecreaseBPMCommand = new RelayCommand(_ => DecreaseBPM());
            ToggleMetronomeCommand = new RelayCommand(_ => ToggleMetronome());
        }

        private void IncreaseBPM() => BPM = Math.Min(BPM + 1, 240);
        private void DecreaseBPM() => BPM = Math.Max(BPM - 1, 40);

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
