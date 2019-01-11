﻿using System.Collections.ObjectModel;
using DT.Samples.Agora.Cross.Views;
using Xamarin.Agora.Full.Forms;
using Xamarin.Forms;

namespace DT.Samples.Agora.Cross.ViewModels
{
    public class MultiRoomViewModel : BaseViewModel
    {
        private bool _isEnded = false;
        private IAgoraService _agoraService;

        public ObservableCollection<uint> UsersOnCall { get; set; } = new ObservableCollection<uint>();

        private bool _isAudioMute;
        public bool IsAudioMute
        {
            get => _isAudioMute;
            set => SetProperty(ref _isAudioMute, value);
        }

        private bool _isVideoMute;
        public bool IsVideoMute
        {
            get => _isVideoMute;
            set => SetProperty(ref _isVideoMute, value);
        }

        private bool _isSpeakerEnabled;
        public bool IsSpeakerEnabled
        {
            get => _isSpeakerEnabled;
            set => SetProperty(ref _isSpeakerEnabled, value);
        }

        private bool _isCameraSwitched;
        public bool IsCameraSwitched
        {
            get => _isCameraSwitched;
            set => SetProperty(ref _isCameraSwitched, value);
        }

        public string Room { get; set; }

        public Command EndSessionCommand { get; }
        public Command AudioMuteCommand { get; }
        public Command VideoMuteCommand { get; }
        public Command SpeakerCommand { get; }
        public Command SwitchCameraCommand { get; }

        public MultiRoomViewModel(string roomName = null)
        {
            Title = roomName;
            Room = roomName;

            EndSessionCommand = new Command(OnEndSession);
            AudioMuteCommand = new Command(OnAudioMute);
            VideoMuteCommand = new Command(OnVideoMute);
            SpeakerCommand = new Command(OnSpeaker);
            SwitchCameraCommand = new Command(SwitchCamera);
        }

        private void SwitchCamera(object param)
        {
            IsCameraSwitched = !IsCameraSwitched;
            _agoraService.ToggleCamera();
        }

        private void OnSpeaker(object param)
        {
            IsSpeakerEnabled = !IsSpeakerEnabled;
            _agoraService.SetSpeakerEnabled(IsSpeakerEnabled);
        }

        private void OnAudioMute(object param)
        {
            IsAudioMute = !IsAudioMute;
            _agoraService.SetAudioMute(IsAudioMute);
        }

        private void OnVideoMute(object param)
        {
            IsVideoMute = !IsVideoMute;
            _agoraService.SetVideoMute(IsVideoMute);
        }

        private void OnEndSession(object param)
        {
            if (_isEnded)
            {
                return;
            }
            _isEnded = true;
            _agoraService.JoinChannelSuccess -= _agoraService_JoinChannelSuccess;;
            _agoraService.OnDisconnected -= _agoraService_OnDisconnected;
            _agoraService.OnNewStream -= _agoraService_OnNewStream;
            _agoraService.EndSession();
            if(param == null || (param is bool && (bool)param != false))
                (Application.Current.MainPage as MainPage).Detail.Navigation.PopAsync();
        }

        public void Init()
        {
            if (_agoraService == null)
            {
                _agoraService = AgoraService.Current;
                _agoraService.JoinChannelSuccess += _agoraService_JoinChannelSuccess;
                _agoraService.OnDisconnected += _agoraService_OnDisconnected;
                _agoraService.OnNewStream += _agoraService_OnNewStream;
                _agoraService.StartSession(Room, Consts.AgoraKey);
            }
        }

        void _agoraService_JoinChannelSuccess(uint obj)
        {
        }

        void _agoraService_OnNewStream(uint uid, int width, int height)
        {
            UsersOnCall.Add(uid);
        }

        void _agoraService_OnDisconnected(uint uid)
        {
            UsersOnCall.Remove(uid);
        }
    }
}
