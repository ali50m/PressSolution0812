﻿using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using GalaSoft.MvvmLight.Ioc;
using AutoMapper;
using PressHmi.Model;
using PressHmi.View;
using FanucCnc;
using FanucCnc.Model;
using Newtonsoft.Json;
using Abt.Controls.SciChart.Model.DataSeries;

namespace PressHmi.ViewModel
{
    public class StateMonitorPageViewModel : ViewModelBase
    {
        
        private Fanuc _fanuc;
        private IMapper _mapper;

        public ICommand LoadedCommand { get; set; }
        public ICommand UnloadedCommand { get; set; }

        public ICommand StartSciChartCmd { get; set; }
        public ICommand StopSciChartCmd { get; set; }


        #region chart button
        public ICommand ChartMoveCmd { get; set; }

        public ICommand ChartZoomCmd { get; set; }

        public ICommand ChartAutoCmd { get; set; }

        private bool m_ChartMoveChecked;
        public bool ChartMoveChecked
        {
            get { return m_ChartMoveChecked; }
            set
            {
                if (m_ChartMoveChecked != value)
                {
                    m_ChartMoveChecked = value;
                    RaisePropertyChanged(() => ChartMoveChecked);
                }
            }
        }

        private bool m_ChartZoomChecked;
        public bool ChartZoomChecked
        {
            get { return m_ChartZoomChecked; }
            set
            {
                if (m_ChartZoomChecked != value)
                {
                    m_ChartZoomChecked = value;
                    RaisePropertyChanged(() => ChartZoomChecked);
                }
            }
        }

        private bool m_ChartAutoChecked;
        public bool ChartAutoChecked
        {
            get { return m_ChartAutoChecked; }
            set
            {
                if (m_ChartAutoChecked != value)
                {
                    m_ChartAutoChecked = value;
                    RaisePropertyChanged(() => ChartAutoChecked);
                }
            }
        }

        public void ChangeChartButton(bool move = false, bool zoom = false,bool auto=false)
        {
            ChartMoveChecked = move;
            ChartZoomChecked = zoom;
            ChartAutoChecked = auto;
        }

        private void OnChartMoveCmd()
        {
            ChangeChartButton(move: true);
        }

        private void OnChartZoomCmd()
        {
            ChangeChartButton(zoom: true);
        }

        private void OnChartAutoCmd()
        {
            ChangeChartButton(auto: true);
        }


        #endregion

        private StateMonitorInfoDto m_StateMonitorInfo = new StateMonitorInfoDto();
        public StateMonitorInfoDto StateMonitorInfo
        {
            get { return m_StateMonitorInfo; }
            set
            {
                if (m_StateMonitorInfo != value)
                {
                    m_StateMonitorInfo = value;
                    RaisePropertyChanged(() => StateMonitorInfo);
                }
            }
        }

        private bool m_Series1Visible;
        public bool Series1Visible
        {
            get { return m_Series1Visible; }
            set
            {
                if (m_Series1Visible != value)
                {
                    m_Series1Visible = value;
                    RaisePropertyChanged(() => Series1Visible);
                }
            }
        }

        private bool m_Series2Visible;
        public bool Series2Visible
        {
            get { return m_Series2Visible; }
            set
            {
                if (m_Series2Visible != value)
                {
                    m_Series2Visible = value;
                    RaisePropertyChanged(() => Series2Visible);
                }
            }
        }

        private bool m_Series3Visible;
        public bool Series3Visible
        {
            get { return m_Series3Visible; }
            set
            {
                if (m_Series3Visible != value)
                {
                    m_Series3Visible = value;
                    RaisePropertyChanged(() => Series3Visible);
                }
            }
        }

        private bool m_Series4Visible;
        public bool Series4Visible
        {
            get { return m_Series4Visible; }
            set
            {
                if (m_Series4Visible != value)
                {
                    m_Series4Visible = value;
                    RaisePropertyChanged(() => Series4Visible);
                }
            }
        }

        public StateMonitorPageViewModel(IMapper mapper, Fanuc fanuc)
        {
            _fanuc = fanuc;
            _mapper = mapper;

            string jsonBaseInfo;
            using (StreamReader sr = new StreamReader(@"baseinfo.cfg", true))
            {
                jsonBaseInfo = sr.ReadToEnd();
            }
            
            var _baseInfo = JsonConvert.DeserializeObject<BaseInfo>(jsonBaseInfo);
            var fifo = _baseInfo.SciChartXTimeMax;
            PosSeries = new XyDataSeries<double, double>();
            //PosSeries.FifoCapacity = fifo;
            SpeedSeries = new XyDataSeries<double, double>();
            //SpeedSeries.FifoCapacity = fifo;
            TemSeries = new XyDataSeries<double, double>();
            //TemSeries.FifoCapacity = fifo;
            PressSeries = new XyDataSeries<double, double>();
            //PressSeries.FifoCapacity = fifo;
            
            LoadedCommand = new RelayCommand(OnLoaded);
            UnloadedCommand = new RelayCommand(OnUnloaded);
            StartSciChartCmd = new RelayCommand(OnStartSciChart);
            StopSciChartCmd = new RelayCommand(OnStopSciChart);

            ChartMoveCmd = new RelayCommand(OnChartMoveCmd);
            ChartZoomCmd = new RelayCommand(OnChartZoomCmd);
            ChartAutoCmd = new RelayCommand(OnChartAutoCmd);
            OnChartAutoCmd();

            Messenger.Default.Register<StateMonitorInfo>(this, "StateMonitorInfoMsg", msg =>
            {
                StateMonitorInfo = _mapper.Map<StateMonitorInfo, StateMonitorInfoDto>(msg);
            });

            Messenger.Default.Register<StateMonitorLineChartData>(this, "StateMonitorLineChartDataMsg", OnData);

        }

        private void OnLoaded()
        {
            _fanuc.ChangePageEvent(statemonitor: true);
        }

        private void OnUnloaded()
        {
            _fanuc.MonitorLine_Cancel();
        }

        private void OnStartSciChart()
        {
            _fanuc.MonitorLine_Start();
        }

        private void OnStopSciChart()
        {
            _fanuc.MonitorLine_Cancel();
        }

        #region scichart 
        private double temp_index = 0;

        private IXyDataSeries<double, double> m_PosSeries;
        public IXyDataSeries<double, double> PosSeries
        {
            get { return m_PosSeries; }
            set
            {
                if (m_PosSeries != value)
                {
                    m_PosSeries = value;
                    RaisePropertyChanged(() => PosSeries);
                }
            }
        }

        private IXyDataSeries<double, double> m_SpeedSeries;
        public IXyDataSeries<double, double> SpeedSeries
        {
            get { return m_SpeedSeries; }
            set
            {
                if (m_SpeedSeries != value)
                {
                    m_SpeedSeries = value;
                    RaisePropertyChanged(() => SpeedSeries);
                }
            }
        }

        private IXyDataSeries<double, double> m_TemSeries;
        public IXyDataSeries<double, double> TemSeries
        {
            get { return m_TemSeries; }
            set
            {
                if (m_TemSeries != value)
                {
                    m_TemSeries = value;
                    RaisePropertyChanged(() => TemSeries);
                }
            }
        }

        private IXyDataSeries<double, double> m_PressSeries;
        public IXyDataSeries<double, double> PressSeries
        {
            get { return m_PressSeries; }
            set
            {
                if (m_PressSeries != value)
                {
                    m_PressSeries = value;
                    RaisePropertyChanged(() => PressSeries);
                }
            }
        }

        private void OnData(StateMonitorLineChartData info)
        {
            if (temp_index <= info.Time)
            {
                PosSeries.Append(info.Time, info.Pos);
                SpeedSeries.Append(info.Time, info.Speed);
                TemSeries.Append(info.Time, info.Tem);
                PressSeries.Append(info.Time, info.Press);
                temp_index = info.Time;
            }
            else
            {
                PosSeries.Clear();
                SpeedSeries.Clear();
                TemSeries.Clear();
                PressSeries.Clear();
                temp_index = info.Time;
            }
        }
        #endregion
    }
}
