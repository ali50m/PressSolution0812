﻿using System;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using FanucCnc.Model;
using FanucCnc.Enum;
using GalaSoft.MvvmLight.Messaging;
using System.Reflection;
using System.Collections.ObjectModel;

namespace FanucCnc
{
    public class Fanuc
    {
        private static Fanuc _instance = null;

        private bool _simulate = false;
        
        #region 数据表
        private bool _slidingBlockTableLoaded = false;
        private Dictionary<double, double> _slidingBlockTable = new Dictionary<double, double>();
        private Dictionary<double, double> _slidingBlockReverseTable = new Dictionary<double, double>();
        private Dictionary<double, double> _slidingBlockSpeedTable = new Dictionary<double, double>();
        private string _slidingBlockTablePath= @"C:\ProgramData\BFM\PressHmi\postable\pos.csv";
        private string _slidingBlockReverseTablePath = @"C:\ProgramData\BFM\PressHmi\postable\pos_R.csv";
        private string _slidingBlockSpeedTablePath = @"C:\ProgramData\BFM\PressHmi\postable\speed.csv";

        private void ReLoadSlidingBlockTable()
        {
            _slidingBlockTableLoaded = false;
            _slidingBlockTable.Clear();
            _slidingBlockReverseTable.Clear();
            _slidingBlockSpeedTable.Clear();

            try
            {
                using (StreamReader sr = new StreamReader(_slidingBlockTablePath))
                {

                    string strLine = "";
                    while ((strLine = sr.ReadLine()) != null)
                    {
                        var lineDatas = strLine.Split(',');

                        if (lineDatas.Length < 2)
                        {
                            return;
                        }

                        //正向表
                        {
                            double data1 = 0, data2 = 0;
                            var ret_data1 = double.TryParse(lineDatas[0], out data1);
                            var ret_data2 = double.TryParse(lineDatas[1], out data2);

                            if (ret_data1 == false || ret_data2 == false) return;

                            _slidingBlockTable.Add(data1, data2);
                        }
                    }
                }

                //反向表
                using (StreamWriter sw = new StreamWriter(_slidingBlockReverseTablePath,false))
                {
                    if (_slidingBlockTable.Count < 100) return;


                    var last_item = _slidingBlockTable.ElementAt(0);
                    var temp_x = Math.Round(last_item.Value, 3);
                    for (int i=1; i<_slidingBlockTable.Count;i++)
                    {
                        var cur_item = _slidingBlockTable.ElementAt(i);

                        while(temp_x< cur_item.Value)
                        {
                            double temp_y = last_item.Key;
                            if (cur_item.Value != last_item.Value)
                            {
                                temp_y = (cur_item.Key - last_item.Key) / (cur_item.Value - last_item.Value) * (temp_x - last_item.Value) + last_item.Key;
                            }


                            var temp_x_3 = Math.Round(temp_x, 3);
                            var temp_y_3 = Math.Round(temp_y, 3);
                            _slidingBlockReverseTable.Add(temp_x_3, temp_y_3);
                            sw.WriteLine(temp_x_3.ToString("0.000") + "," + temp_y_3.ToString("0.000"));

                            temp_x = temp_x + 0.001;


                        }

                    }


                }

                //速度表
                using (StreamReader sr = new StreamReader(_slidingBlockSpeedTablePath))
                {

                    string strLine = "";
                    while ((strLine = sr.ReadLine()) != null)
                    {
                        var lineDatas = strLine.Split(',');

                        if (lineDatas.Length < 3)
                        {
                            return;
                        }

                        {
                            double data1 = 0, data2 = 0, data3 = 0;
                            var ret_data1 = double.TryParse(lineDatas[0], out data1);
                            var ret_data3 = double.TryParse(lineDatas[2], out data3);

                            if (ret_data1 == false || ret_data3 == false) return;

                            _slidingBlockSpeedTable.Add(data1, data2);
                        }
                    }
                }

            }
            catch
            {
                return;
            }

            _slidingBlockTableLoaded = true;
        }

        private void LoadSlidingBlockTable()
        {
            _slidingBlockTableLoaded = false;
            _slidingBlockTable.Clear();
            _slidingBlockReverseTable.Clear();

            try
            {
                //正向表
                using (StreamReader sr = new StreamReader(_slidingBlockTablePath))
                {

                    string strLine = "";
                    while ((strLine = sr.ReadLine()) != null)
                    {
                        var lineDatas = strLine.Split(',');

                        if (lineDatas.Length < 2)
                        {
                            return;
                        }

                        
                        {
                            double data1 = 0, data2 = 0;
                            var ret_data1 = double.TryParse(lineDatas[0], out data1);
                            var ret_data2 = double.TryParse(lineDatas[1], out data2);

                            if (ret_data1 == false || ret_data2 == false) return;

                            _slidingBlockTable.Add(data1, data2);
                        }
                    }
                }
                
                //反向表
                using (StreamReader sr = new StreamReader(_slidingBlockReverseTablePath))
                {

                    string strLine = "";
                    while ((strLine = sr.ReadLine()) != null)
                    {
                        var lineDatas = strLine.Split(',');

                        if (lineDatas.Length < 2)
                        {
                            return;
                        }

                        //正向表
                        {
                            double data1 = 0, data2 = 0;
                            var ret_data1 = double.TryParse(lineDatas[0], out data1);
                            var ret_data2 = double.TryParse(lineDatas[1], out data2);

                            if (ret_data1 == false || ret_data2 == false) return;

                            _slidingBlockReverseTable.Add(data1, data2);
                        }
                    }
                }

                //速度表
                using (StreamReader sr = new StreamReader(_slidingBlockSpeedTablePath))
                {

                    string strLine = "";
                    while ((strLine = sr.ReadLine()) != null)
                    {
                        var lineDatas = strLine.Split(',');

                        if (lineDatas.Length < 3)
                        {
                            return;
                        }

                        {
                            double data1 = 0, data2 = 0, data3 = 0;
                            var ret_data1 = double.TryParse(lineDatas[0], out data1);
                            var ret_data3 = double.TryParse(lineDatas[2], out data3);

                            if (ret_data1 == false || ret_data3 == false) return;

                            _slidingBlockSpeedTable.Add(data1, data2);
                        }
                    }
                }
            }
            catch
            {
                return;
            }

            _slidingBlockTableLoaded = true;
        }

        public string SearchScrewData(double sliding,ref double screw)
        {
            if(_slidingBlockTableLoaded==false)
            {
                return "数据表加载失败，请先加载数据表";
            }

            try
            {
                screw = _slidingBlockTable[Math.Round(sliding,3)];
            }
            catch
            {
                return "数据表加载失败，数据表中没有该数据";
            }

            return null;
        }

        public string SearchSlidingData(double screw, ref double sliding)
        {
            if (_slidingBlockTableLoaded == false)
            {
                return "数据表加载失败，请先加载数据表";
            }

            try
            {
                sliding = _slidingBlockReverseTable[Math.Round(screw, 3)];
            }
            catch
            {
                return "数据表加载失败，数据表中没有该数据";
            }

            return null;
        }

        public string SearchSlidingSpeedData(double pos, double feed, ref double speed)
        {
            if (_slidingBlockTableLoaded == false)
            {
                return "数据表加载失败，请先加载数据表";
            }

            try
            {
                speed = _slidingBlockSpeedTable[Math.Round(pos, 1)]* feed;
            }
            catch
            {
                return "数据表加载失败，数据表中没有该数据";
            }

            return null;
        }
        #endregion

        #region 配置
        private BaseInfo _baseInfo = new BaseInfo();
        public BaseInfo BaseInfo { get { return _baseInfo; } }

        private PmcBom _pmcBom = new PmcBom();
        public PmcBom CurPmcBom
        {
            get { return _pmcBom; }
        }

        private MacroBom _macroBom = new MacroBom();
        public MacroBom CurMacroBom
        {
            get { return _macroBom; }
        }

        private LimitBom _limitBom = new LimitBom();
        public LimitBom CurLimitBom
        {
            get
            {
                return _limitBom;
            }
        }

        private RecipesInfo _recipes = new RecipesInfo();
        public RecipesInfo Recipes
        {
            get
            {
                return _recipes;
            }
        }

        #endregion

        #region 静态刷新的连接句柄
        private ushort m_static_flib = 0;
        private BackgroundWorker m_static_BackgroundWorker = new BackgroundWorker();
        int m_static_freq = 10;
        private CncStaticInfo m_static_info = new CncStaticInfo();

        #endregion

        #region 页的连接句柄
        private ushort m_page_flib = 0;
        private BackgroundWorker m_page_BackgroundWorker = new BackgroundWorker();
        int m_page_freq = 10;

        private bool m_statemonitor = false;
        private StateMonitorInfo m_statemonitor_info = new StateMonitorInfo();

        private bool m_paradiechange = false;
        private ParaDieChangeInfo m_diechange_info = new ParaDieChangeInfo();

        private bool m_paradieclamp = false;
        private ParaDieClampInfo m_dieclamp_info = new ParaDieClampInfo();

        private bool m_paradieclosing = false;
        private ParaDieClosingInfo m_dieclosing_info = new ParaDieClosingInfo();
        private ParaDieClosingProcInfo m_dieclosingproc_info = new ParaDieClosingProcInfo();

        private bool m_paradieparting = false;
        private ParaDiePartingInfo m_dieparting_info = new ParaDiePartingInfo();
        private ParaDiePartingProcInfo m_diepartingproc_info = new ParaDiePartingProcInfo();

        private bool m_parapressuremaint = false;
        private ParaPressureMaintInfo m_pressuremaint_info = new ParaPressureMaintInfo();

        private bool m_paraautoairsource = false;
        private ParaAutoAirSourceInfo m_autoairsource_info = new ParaAutoAirSourceInfo();

        private bool m_parahydrdiecushion = false;
        private ParaHydrDieCushionInfo m_hydrdiecushion_info = new ParaHydrDieCushionInfo();

        private bool m_paracam = false;
        private ParaCamInfo m_cam_info = new ParaCamInfo();

        private bool m_paradiehydr = false;
        private ParaDieHydrInfo m_m_diehydr_info = new ParaDieHydrInfo();

        private bool m_paraworkcount = false;
        private ParaWorkCountInfo m_workcount_info = new ParaWorkCountInfo();

        private bool m_pararecipes = false;
        private RecipesInfo m_recipes_info = new RecipesInfo();

        private bool m_sparamachine = false;
        private SParaMachineInfo m_sparamachine_info = new SParaMachineInfo();

        private bool m_sparalubricate = false;
        private SParaLubricateInfo m_sparalubricat_info = new SParaLubricateInfo();

        private bool m_sparaanalog = false;
        private SParaAnalogInfo m_sparaanalog_info = new SParaAnalogInfo();

        private bool m_sparaencode = false;
        private SParaEncodeInfo m_sparaencode_info = new SParaEncodeInfo();

        private bool m_mnmovetable = false;
        private MaintainMoveTableInfo m_mnmovetable_info = new MaintainMoveTableInfo();

        private bool m_mntravel = false;
        private MaintainTravelInfo m_mntravel_info = new MaintainTravelInfo();

        private bool m_mndiehigh = false;
        private MaintainDieHighInfo m_mndiehigh_info = new MaintainDieHighInfo();

        private bool m_mnservo = false;
        private MaintainServoInfo m_mnservo_info = new MaintainServoInfo();

        private bool m_mnio = false;
        private MaintainIoInfo m_mnio_info = new MaintainIoInfo();

        public void ChangePageEvent(bool statemonitor = false, bool paradiechange = false, bool paradieclosing = false,
            bool paradieparting = false, bool parapressuremaint = false, bool paraautoairsource = false, bool parahydrdiecushion = false, bool paracam = false,
            bool paraworkcount = false,  bool pararecipes =false, bool sparamachine = false, bool sparalubricate = false, bool sparaanalog = false,
            bool sparaencode = false, bool paradieclamp = false, bool paradiehydr=false,
            bool mnmovetable =false, bool mntravel =false, bool mndiehigh=false , bool mnservo=false,bool mnio=false)
        {
            m_statemonitor = statemonitor;
            m_paradiechange = paradiechange;
            m_paradieclamp = paradieclamp;
            m_paradieclosing = paradieclosing;
            m_paradieparting = paradieparting;
            m_parapressuremaint = parapressuremaint;
            m_paraautoairsource = paraautoairsource;
            m_parahydrdiecushion = parahydrdiecushion;
            m_paracam = paracam;
            m_paradiehydr = paradiehydr;
            m_paraworkcount = paraworkcount;
            m_pararecipes = pararecipes;
            m_sparamachine = sparamachine;
            m_sparalubricate = sparalubricate;
            m_sparaanalog = sparaanalog;
            m_sparaencode = sparaencode;
            m_mndiehigh = mndiehigh;
            m_mnmovetable = mnmovetable;
            m_mntravel = mntravel;
            m_mnservo = mnservo;
            m_mnio = mnio;
        }

        #endregion

        #region 动态曲线的连接句柄
        private ushort m_monitorline_flib = 0;
        private BackgroundWorker m_monitorline_BackgroundWorker = new BackgroundWorker();
        int m_monitorline_freq = 100;
        private StateMonitorLineChartData m_monitorline_info = new StateMonitorLineChartData();
        bool m_monitorline_indo = false;

        private ushort m_simulatemonitorline_flib = 0;
        private BackgroundWorker m_simulatemonitorline_BackgroundWorker = new BackgroundWorker();
        int m_simulatemonitorline_freq = 50;
        private StateMonitorLineChartData m_simulatemonitorline_info = new StateMonitorLineChartData();
        bool m_simulatemonitorline_indo = false;

        #endregion

        #region Ctor
        public static Fanuc CreateInstance()
        {
            if (_instance == null)

            {
                _instance = new Fanuc();
            }
            return _instance;
        }

        public Fanuc()
        {
            ReInitialBom();

            //静态扫描线程
            m_static_BackgroundWorker.WorkerReportsProgress = false;
            m_static_BackgroundWorker.WorkerSupportsCancellation = true;
            m_static_BackgroundWorker.DoWork += ScannerStaticFunc;
            m_static_BackgroundWorker.RunWorkerCompleted += ScannerStaticCompleted;

            //页扫描线程
            m_page_BackgroundWorker.WorkerReportsProgress = false;
            m_page_BackgroundWorker.WorkerSupportsCancellation = true;
            m_page_BackgroundWorker.DoWork += ScannerPageFunc;
            m_page_BackgroundWorker.RunWorkerCompleted += ScannerPageCompleted;

            //动态曲线扫描线程
            m_monitorline_BackgroundWorker.WorkerReportsProgress = false;
            m_monitorline_BackgroundWorker.WorkerSupportsCancellation = true;
            m_monitorline_BackgroundWorker.DoWork += MonitorLineFunc;
            m_monitorline_BackgroundWorker.RunWorkerCompleted += MonitorLineCompleted;

            //仿真曲线扫描线程
            m_simulatemonitorline_BackgroundWorker.WorkerReportsProgress = false;
            m_simulatemonitorline_BackgroundWorker.WorkerSupportsCancellation = true;
            m_simulatemonitorline_BackgroundWorker.DoWork += SimulateMonitorLineFunc;
            m_simulatemonitorline_BackgroundWorker.RunWorkerCompleted += SimulateMonitorLineCompleted;

            //启动扫描
            ScannerStatic_Start();
            ScannerPage_Start();

            //加载丝杠滑块对应表
            Task.Factory.StartNew(() =>
            {
                LoadSlidingBlockTable();
            });

            
        }

        private short InitialBom()
        {
            string jsonIO;
            using (StreamReader sr = new StreamReader(@"io.cfg", true))
            {
                jsonIO = sr.ReadToEnd();
            }
            m_mnio_info = JsonConvert.DeserializeObject<MaintainIoInfo>(jsonIO);

            string jsonLimit;
            using (StreamReader sr = new StreamReader(@"limitbom.cfg", true))
            {
                jsonLimit = sr.ReadToEnd();
            }
            _limitBom = JsonConvert.DeserializeObject<LimitBom>(jsonLimit);

            string jsonMacroBom;
            using (StreamReader sr = new StreamReader(@"macrobom.cfg", true))
            {
                jsonMacroBom = sr.ReadToEnd();
            }

            _macroBom = JsonConvert.DeserializeObject<MacroBom>(jsonMacroBom);

            string jsonPmcBom;
            using (StreamReader sr = new StreamReader(@"pmcbom.cfg", true))
            {
                jsonPmcBom = sr.ReadToEnd();
            }

            _pmcBom = JsonConvert.DeserializeObject<PmcBom>(jsonPmcBom);

            string jsonBaseInfo;
            using (StreamReader sr = new StreamReader(@"baseinfo.cfg", true))
            {
                jsonBaseInfo = sr.ReadToEnd();
            }

            _baseInfo = JsonConvert.DeserializeObject<BaseInfo>(jsonBaseInfo);

            #region 配方

            var obj = CurMacroBom.GetType();
            foreach (PropertyInfo item in obj.GetProperties())
            {
                var bomItem = item.GetValue(CurMacroBom) as MacroBomItem;
                if (bomItem?.IsRecipes ?? false)
                {
                    _recipes.MacroBoms.Add(new MacroBomItemRecipes()
                    {
                        Id = bomItem.Id,
                        Name = item.GetCustomAttribute<DisplayAttribute>().Name,
                        Adr = bomItem.Adr,
                        Value = "0",
                        IsRecipes = bomItem.IsRecipes
                    });
                }
            }

            var obj_pmc = CurPmcBom.GetType();
            foreach (PropertyInfo item in obj_pmc.GetProperties())
            {
                var bomItem = item.GetValue(CurPmcBom) as PmcBomItem;
                if (bomItem?.IsRecipes ?? false)
                {
                    _recipes.PmcBoms.Add(new PmcBomItemRecipes()
                    {
                        Id = bomItem.Id,
                        Name = item.GetCustomAttribute<DisplayAttribute>().Name,
                        AdrType = bomItem.AdrType,
                        DataType = bomItem.DataType,
                        Adr = bomItem.Adr,
                        Bit = bomItem.Bit,
                        ConversionFactor = bomItem.ConversionFactor,
                        IsRecipes = bomItem.IsRecipes,
                        Value = "0"
                    });
                }
            }

            #endregion


            return 0;
        }

        private short ReInitialBom()
        {
            #region io
            for(ushort i =0; i<10;i++)
            {
                for(ushort j=0;j<8; j++)
                {
                    m_mnio_info.InputBom.Add(new IoBomItem
                    {
                        AdrType = PmcAdrTypeEnum.X,
                        Adr = i,
                        Bit = j,
                        Descrip = "输入信号X" + i + "." + j,
                        Value = false
                    });
                }
            }

            for (ushort i = 0; i < 10; i++)
            {
                for (ushort j = 0; j < 5; j++)
                {
                    m_mnio_info.OutputBom.Add(new IoBomItem
                    {
                        AdrType = PmcAdrTypeEnum.Y,
                        Adr = i,
                        Bit = j,
                        Descrip = "输入信号Y" + i + "." + j,
                        Value = false
                    });
                }
            }

            var jsonIoBom = JsonConvert.SerializeObject(m_mnio_info, Formatting.Indented);
            using (StreamWriter sw = new StreamWriter(@"io.cfg", false))
            {
                sw.Write(jsonIoBom);
            }

            #endregion

            #region limit
            #region 换模设定
            _limitBom.DCP_RapidFeed = new LimitBomItem();
            _limitBom.DCP_RapidFeed.LimitDown = 10;
            _limitBom.DCP_RapidFeed.LimitUp = 100;

            _limitBom.DCP_JogFeed = new LimitBomItem();
            _limitBom.DCP_JogFeed.LimitDown = 0;
            _limitBom.DCP_JogFeed.LimitUp = 20;

            _limitBom.DCP_InstallDieHighSet = new LimitBomItem();
            _limitBom.DCP_InstallDieHighSet.LimitDown = 900;
            _limitBom.DCP_InstallDieHighSet.LimitUp = 950;

            _limitBom.DCP_CylinderpRressureSet = new LimitBomItem();
            _limitBom.DCP_CylinderpRressureSet.LimitDown = 10;
            _limitBom.DCP_CylinderpRressureSet.LimitUp = 20;

            _limitBom.DCP_DieWeight = new LimitBomItem();
            _limitBom.DCP_DieWeight.LimitDown = 0;
            _limitBom.DCP_DieWeight.LimitUp = 10.5;

            _limitBom.DCP_LoaderSafePosition = new LimitBomItem();
            _limitBom.DCP_LoaderSafePosition.LimitDown = 0;
            _limitBom.DCP_LoaderSafePosition.LimitUp = 1000.0;

            #endregion

            #region 夹模器设定
            _limitBom.CLS_ClampRelaxPosition = new LimitBomItem();
            _limitBom.CLS_ClampRelaxPosition.LimitDown = 10;
            _limitBom.CLS_ClampRelaxPosition.LimitUp = 1000;


            #endregion

            #region 合模设定
            _limitBom.DJP_SectionNum = new LimitBomItem();
            _limitBom.DJP_SectionNum.LimitDown = 2;
            _limitBom.DJP_SectionNum.LimitUp = 8;

            _limitBom.DJP_PreDelayTime = new LimitBomItem();
            _limitBom.DJP_PreDelayTime.LimitDown = 0.1;
            _limitBom.DJP_PreDelayTime.LimitUp = 1;

            _limitBom.DJP_SafeTime = new LimitBomItem();
            _limitBom.DJP_SafeTime.LimitDown = 0;
            _limitBom.DJP_SafeTime.LimitUp = 1;

            _limitBom.DJP_SectionNum = new LimitBomItem();
            _limitBom.DJP_SectionNum.LimitDown = 2;
            _limitBom.DJP_SectionNum.LimitUp = 8;

            _limitBom.DJP_TopDeadCentre = new LimitBomItem();
            _limitBom.DJP_TopDeadCentre.LimitDown = 0;
            _limitBom.DJP_TopDeadCentre.LimitUp = 1280;

            _limitBom.DJP_Speed_BottomDeadCentre = new LimitBomItem();
            _limitBom.DJP_Speed_BottomDeadCentre.LimitDown = 0;
            _limitBom.DJP_Speed_BottomDeadCentre.LimitUp = 100;

            _limitBom.DJP_Pos_1 = new LimitBomItem();
            _limitBom.DJP_Pos_1.LimitDown = 0;
            _limitBom.DJP_Pos_1.LimitUp = 1280;
            _limitBom.DJP_Speed_1 = new LimitBomItem();
            _limitBom.DJP_Speed_1.LimitDown = 0;
            _limitBom.DJP_Speed_1.LimitUp = 100;
            _limitBom.DJP_StopTime_1 = new LimitBomItem();
            _limitBom.DJP_StopTime_1.LimitDown = 0.1;
            _limitBom.DJP_StopTime_1.LimitUp = 1;

            _limitBom.DJP_Pos_2 = new LimitBomItem();
            _limitBom.DJP_Pos_2.LimitDown = 0;
            _limitBom.DJP_Pos_2.LimitUp = 1280;
            _limitBom.DJP_Speed_2 = new LimitBomItem();
            _limitBom.DJP_Speed_2.LimitDown = 0;
            _limitBom.DJP_Speed_2.LimitUp = 100;
            _limitBom.DJP_StopTime_2 = new LimitBomItem();
            _limitBom.DJP_StopTime_2.LimitDown = 0.1;
            _limitBom.DJP_StopTime_2.LimitUp = 1;

            _limitBom.DJP_Pos_3 = new LimitBomItem();
            _limitBom.DJP_Pos_3.LimitDown = 0;
            _limitBom.DJP_Pos_3.LimitUp = 1280;
            _limitBom.DJP_Speed_3 = new LimitBomItem();
            _limitBom.DJP_Speed_3.LimitDown = 0;
            _limitBom.DJP_Speed_3.LimitUp = 100;
            _limitBom.DJP_StopTime_3 = new LimitBomItem();
            _limitBom.DJP_StopTime_3.LimitDown = 0.1;
            _limitBom.DJP_StopTime_3.LimitUp = 1;

            _limitBom.DJP_Pos_4 = new LimitBomItem();
            _limitBom.DJP_Pos_4.LimitDown = 0;
            _limitBom.DJP_Pos_4.LimitUp = 1280;
            _limitBom.DJP_Speed_4 = new LimitBomItem();
            _limitBom.DJP_Speed_4.LimitDown = 0;
            _limitBom.DJP_Speed_4.LimitUp = 100;
            _limitBom.DJP_StopTime_4 = new LimitBomItem();
            _limitBom.DJP_StopTime_4.LimitDown = 0.1;
            _limitBom.DJP_StopTime_4.LimitUp = 1;

            _limitBom.DJP_Pos_5 = new LimitBomItem();
            _limitBom.DJP_Pos_5.LimitDown = 0;
            _limitBom.DJP_Pos_5.LimitUp = 1280;
            _limitBom.DJP_Speed_5 = new LimitBomItem();
            _limitBom.DJP_Speed_5.LimitDown = 0;
            _limitBom.DJP_Speed_5.LimitUp = 100;
            _limitBom.DJP_StopTime_5 = new LimitBomItem();
            _limitBom.DJP_StopTime_5.LimitDown = 0.1;
            _limitBom.DJP_StopTime_5.LimitUp = 1;

            _limitBom.DJP_Pos_6 = new LimitBomItem();
            _limitBom.DJP_Pos_6.LimitDown = 0;
            _limitBom.DJP_Pos_6.LimitUp = 1280;
            _limitBom.DJP_Speed_6 = new LimitBomItem();
            _limitBom.DJP_Speed_6.LimitDown = 0;
            _limitBom.DJP_Speed_6.LimitUp = 100;
            _limitBom.DJP_StopTime_6 = new LimitBomItem();
            _limitBom.DJP_StopTime_6.LimitDown = 0.1;
            _limitBom.DJP_StopTime_6.LimitUp = 1;

            _limitBom.DJP_Pos_7 = new LimitBomItem();
            _limitBom.DJP_Pos_7.LimitDown = 0;
            _limitBom.DJP_Pos_7.LimitUp = 1280;
            _limitBom.DJP_Speed_7 = new LimitBomItem();
            _limitBom.DJP_Speed_7.LimitDown = 0;
            _limitBom.DJP_Speed_7.LimitUp = 100;
            _limitBom.DJP_StopTime_7 = new LimitBomItem();
            _limitBom.DJP_StopTime_7.LimitDown = 0.1;
            _limitBom.DJP_StopTime_7.LimitUp = 1;

            _limitBom.DJP_Pos_8 = new LimitBomItem();
            _limitBom.DJP_Pos_8.LimitDown = 0;
            _limitBom.DJP_Pos_8.LimitUp = 1280;
            _limitBom.DJP_Speed_8 = new LimitBomItem();
            _limitBom.DJP_Speed_8.LimitDown = 0;
            _limitBom.DJP_Speed_8.LimitUp = 100;
            _limitBom.DJP_StopTime_8 = new LimitBomItem();
            _limitBom.DJP_StopTime_8.LimitDown = 0.1;
            _limitBom.DJP_StopTime_8.LimitUp = 1;

            _limitBom.DJP_BottomDeadCentre = new LimitBomItem();
            _limitBom.DJP_BottomDeadCentre.LimitDown = 0;
            _limitBom.DJP_BottomDeadCentre.LimitUp = 20;

            _limitBom.DJP_BottomDeadCentre_StopTime = new LimitBomItem();
            _limitBom.DJP_BottomDeadCentre_StopTime.LimitDown = 0;
            _limitBom.DJP_BottomDeadCentre_StopTime.LimitUp = 20000;
            

            #endregion

            #region 分模设定
            _limitBom.DPP_SectionNum = new LimitBomItem();
            _limitBom.DPP_SectionNum.LimitDown = 1;
            _limitBom.DPP_SectionNum.LimitUp = 1;
            
            _limitBom.DPP_Pos_1 = new LimitBomItem();
            _limitBom.DPP_Pos_1.LimitDown = 0;
            _limitBom.DPP_Pos_1.LimitUp = 1280;
            _limitBom.DPP_Speed_1 = new LimitBomItem();
            _limitBom.DPP_Speed_1.LimitDown = 0;
            _limitBom.DPP_Speed_1.LimitUp = 100;
            _limitBom.DPP_Pos_2 = new LimitBomItem();
            _limitBom.DPP_Pos_2.LimitDown = 0;
            _limitBom.DPP_Pos_2.LimitUp = 1280;
            _limitBom.DPP_Speed_2 = new LimitBomItem();
            _limitBom.DPP_Speed_2.LimitDown = 0;
            _limitBom.DPP_Speed_2.LimitUp = 100;
            _limitBom.DPP_Pos_3 = new LimitBomItem();
            _limitBom.DPP_Pos_3.LimitDown = 0;
            _limitBom.DPP_Pos_3.LimitUp = 1280;
            _limitBom.DPP_Speed_3 = new LimitBomItem();
            _limitBom.DPP_Speed_3.LimitDown = 0;
            _limitBom.DPP_Speed_3.LimitUp = 100;
            _limitBom.DPP_Pos_4 = new LimitBomItem();
            _limitBom.DPP_Pos_4.LimitDown = 0;
            _limitBom.DPP_Pos_4.LimitUp = 1280;
            _limitBom.DPP_Speed_4 = new LimitBomItem();
            _limitBom.DPP_Speed_4.LimitDown = 0;
            _limitBom.DPP_Speed_4.LimitUp = 100;
            _limitBom.DPP_Pos_5 = new LimitBomItem();
            _limitBom.DPP_Pos_5.LimitDown = 0;
            _limitBom.DPP_Pos_5.LimitUp = 1280;
            _limitBom.DPP_Speed_5 = new LimitBomItem();
            _limitBom.DPP_Speed_5.LimitDown = 0;
            _limitBom.DPP_Speed_5.LimitUp = 100;
            _limitBom.DPP_Pos_6 = new LimitBomItem();
            _limitBom.DPP_Pos_6.LimitDown = 0;
            _limitBom.DPP_Pos_6.LimitUp = 1280;
            _limitBom.DPP_Speed_6 = new LimitBomItem();
            _limitBom.DPP_Speed_6.LimitDown = 0;
            _limitBom.DPP_Speed_6.LimitUp = 100;
            _limitBom.DPP_Pos_7 = new LimitBomItem();
            _limitBom.DPP_Pos_7.LimitDown = 0;
            _limitBom.DPP_Pos_7.LimitUp = 1280;
            _limitBom.DPP_Speed_7 = new LimitBomItem();
            _limitBom.DPP_Speed_7.LimitDown = 0;
            _limitBom.DPP_Speed_7.LimitUp = 100;
            _limitBom.DPP_Pos_8 = new LimitBomItem();
            _limitBom.DPP_Pos_8.LimitDown = 0;
            _limitBom.DPP_Pos_8.LimitUp = 1280;
            _limitBom.DPP_Speed_8 = new LimitBomItem();
            _limitBom.DPP_Speed_8.LimitDown = 0;
            _limitBom.DPP_Speed_8.LimitUp = 100;

            _limitBom.DPP_BottomDeadCentre = new LimitBomItem();
            _limitBom.DPP_BottomDeadCentre.LimitDown = 0;
            _limitBom.DPP_BottomDeadCentre.LimitUp = 20;

            _limitBom.DPP_Speed_TopDeadCentre = new LimitBomItem();
            _limitBom.DPP_Speed_TopDeadCentre.LimitDown = 0;
            _limitBom.DPP_Speed_TopDeadCentre.LimitUp = 20;

            _limitBom.DPP_TopDeadCentre = new LimitBomItem();
            _limitBom.DPP_TopDeadCentre.LimitDown = 0;
            _limitBom.DPP_TopDeadCentre.LimitUp = 20;

            #endregion

            #region 保压设定
            _limitBom.SPP_Pressure = new LimitBomItem();
            _limitBom.SPP_Pressure.LimitDown = 50;
            _limitBom.SPP_Pressure.LimitUp = 100;

            _limitBom.SPP_Time = new LimitBomItem();
            _limitBom.SPP_Time.LimitDown = 10;
            _limitBom.SPP_Time.LimitUp = 20;

            _limitBom.SPP_PreDelayTime = new LimitBomItem();
            _limitBom.SPP_PreDelayTime.LimitDown = 10;
            _limitBom.SPP_PreDelayTime.LimitUp = 20;

            _limitBom.SPP_ChangeMode = new LimitBomItem();
            _limitBom.SPP_ChangeMode.LimitDown = 0;
            _limitBom.SPP_ChangeMode.LimitUp = 2;

            _limitBom.SPP_ChangePressure = new LimitBomItem();
            _limitBom.SPP_ChangePressure.LimitDown = 200;
            _limitBom.SPP_ChangePressure.LimitUp = 400;

            #endregion

            #region 自动化气源
            _limitBom.AAS_StartPos_1 = new LimitBomItem();
            _limitBom.AAS_StartPos_1.LimitDown = 50;
            _limitBom.AAS_StartPos_1.LimitUp = 100;
            _limitBom.AAS_EndPos_1 = new LimitBomItem();
            _limitBom.AAS_EndPos_1.LimitDown = 750;
            _limitBom.AAS_EndPos_1.LimitUp = 900;

            _limitBom.AAS_StartPos_2 = new LimitBomItem();
            _limitBom.AAS_StartPos_2.LimitDown = 50;
            _limitBom.AAS_StartPos_2.LimitUp = 100;
            _limitBom.AAS_EndPos_2 = new LimitBomItem();
            _limitBom.AAS_EndPos_2.LimitDown = 750;
            _limitBom.AAS_EndPos_2.LimitUp = 900;

            _limitBom.AAS_StartPos_3 = new LimitBomItem();
            _limitBom.AAS_StartPos_3.LimitDown = 50;
            _limitBom.AAS_StartPos_3.LimitUp = 100;
            _limitBom.AAS_EndPos_3 = new LimitBomItem();
            _limitBom.AAS_EndPos_3.LimitDown = 750;
            _limitBom.AAS_EndPos_3.LimitUp = 900;

            _limitBom.AAS_StartPos_4 = new LimitBomItem();
            _limitBom.AAS_StartPos_4.LimitDown = 50;
            _limitBom.AAS_StartPos_4.LimitUp = 100;
            _limitBom.AAS_EndPos_4 = new LimitBomItem();
            _limitBom.AAS_EndPos_4.LimitDown = 750;
            _limitBom.AAS_EndPos_4.LimitUp = 900;

            #endregion

            #region 工件计数

            #endregion

            #region 模具液压设定
            _limitBom.DH_Mode = new LimitBomItem();
            _limitBom.DH_Mode.LimitDown = 50;
            _limitBom.DH_Mode.LimitUp = 100;
            _limitBom.DH_Pressure = new LimitBomItem();
            _limitBom.DH_Pressure.LimitDown = 750;
            _limitBom.DH_Pressure.LimitUp = 900;
            _limitBom.DH_PushPos = new LimitBomItem();
            _limitBom.DH_PushPos.LimitDown = 750;
            _limitBom.DH_PushPos.LimitUp = 900;
            _limitBom.DH_PushDelayTime = new LimitBomItem();
            _limitBom.DH_PushDelayTime.LimitDown = 750;
            _limitBom.DH_PushDelayTime.LimitUp = 900;


            #endregion

            #region 系统参数 压机设定
            _limitBom.SPM_MaxWeight = new LimitBomItem();
            _limitBom.SPM_MaxWeight.LimitDown = 0;
            _limitBom.SPM_MaxWeight.LimitUp = 999;

            _limitBom.SPM_MaxTemperature = new LimitBomItem();
            _limitBom.SPM_MaxTemperature.LimitDown = 0;
            _limitBom.SPM_MaxTemperature.LimitUp = 999;

            _limitBom.SPM_MaxError = new LimitBomItem();
            _limitBom.SPM_MaxError.LimitDown = 0;
            _limitBom.SPM_MaxError.LimitUp = 999;


            _limitBom.SPM_PressureSensorPara = new LimitBomItem();
            _limitBom.SPM_PressureSensorPara.LimitDown = 0;
            _limitBom.SPM_PressureSensorPara.LimitUp = 999;


            _limitBom.SPM_BalanceCylinderAnalog = new LimitBomItem();
            _limitBom.SPM_BalanceCylinderAnalog.LimitDown = 0;
            _limitBom.SPM_BalanceCylinderAnalog.LimitUp = 999;

            _limitBom.SPM_BalanceCylinderPressure = new LimitBomItem();
            _limitBom.SPM_BalanceCylinderPressure.LimitDown = 0;
            _limitBom.SPM_BalanceCylinderPressure.LimitUp = 999;


            _limitBom.SPM_OverflowDelay = new LimitBomItem();
            _limitBom.SPM_OverflowDelay.LimitDown = 0;
            _limitBom.SPM_OverflowDelay.LimitUp = 999;


            _limitBom.SPM_PressureDiffPara = new LimitBomItem();
            _limitBom.SPM_PressureDiffPara.LimitDown = 0;
            _limitBom.SPM_PressureDiffPara.LimitUp = 999;

            _limitBom.SPM_PressureLowAlarm = new LimitBomItem();
            _limitBom.SPM_PressureLowAlarm.LimitDown = 0;
            _limitBom.SPM_PressureLowAlarm.LimitUp = 999;

            _limitBom.SPM_LubricateDetect = new LimitBomItem();
            _limitBom.SPM_LubricateDetect.LimitDown = 0;
            _limitBom.SPM_LubricateDetect.LimitUp = 999;


            #endregion

            #region 系统参数 润滑设定
            _limitBom.SPL_CR_LubricateTime = new LimitBomItem();
            _limitBom.SPL_CR_LubricateTime.LimitDown = 0;
            _limitBom.SPL_CR_LubricateTime.LimitUp = 999;

            _limitBom.SPL_CR_SetLubricateInterval = new LimitBomItem();
            _limitBom.SPL_CR_SetLubricateInterval.LimitDown = 0;
            _limitBom.SPL_CR_SetLubricateInterval.LimitUp = 999;

            _limitBom.SPL_CR_ActualLubricateInterval = new LimitBomItem();
            _limitBom.SPL_CR_ActualLubricateInterval.LimitDown = 0;
            _limitBom.SPL_CR_ActualLubricateInterval.LimitUp = 999;

            _limitBom.SPL_CR_LubricateDetectTime = new LimitBomItem();
            _limitBom.SPL_CR_LubricateDetectTime.LimitDown = 0;
            _limitBom.SPL_CR_LubricateDetectTime.LimitUp = 999;

            _limitBom.SPL_CR_LubricateTotalNum = new LimitBomItem();
            _limitBom.SPL_CR_LubricateTotalNum.LimitDown = 0;
            _limitBom.SPL_CR_LubricateTotalNum.LimitUp = 999;

            _limitBom.SPL_CR_PowerOnLubricateTime = new LimitBomItem();
            _limitBom.SPL_CR_PowerOnLubricateTime.LimitDown = 0;
            _limitBom.SPL_CR_PowerOnLubricateTime.LimitUp = 999;

            _limitBom.SPL_CR_LubricateDetecte = new LimitBomItem();
            _limitBom.SPL_CR_LubricateDetecte.LimitDown = 0;
            _limitBom.SPL_CR_LubricateDetecte.LimitUp = 999;

            _limitBom.SPL_AC_LubricateTime = new LimitBomItem();
            _limitBom.SPL_AC_LubricateTime.LimitDown = 0;
            _limitBom.SPL_AC_LubricateTime.LimitUp = 999;

            _limitBom.SPL_AC_SetLubricateInterval = new LimitBomItem();
            _limitBom.SPL_AC_SetLubricateInterval.LimitDown = 0;
            _limitBom.SPL_AC_SetLubricateInterval.LimitUp = 999;

            _limitBom.SPL_AC_ActualLubricateInterval = new LimitBomItem();
            _limitBom.SPL_AC_ActualLubricateInterval.LimitDown = 0;
            _limitBom.SPL_AC_ActualLubricateInterval.LimitUp = 999;

            _limitBom.SPL_AC_LubricateDetectTime = new LimitBomItem();
            _limitBom.SPL_AC_LubricateDetectTime.LimitDown = 0;
            _limitBom.SPL_AC_LubricateDetectTime.LimitUp = 999;

            _limitBom.SPL_AC_LubricateTotalNum = new LimitBomItem();
            _limitBom.SPL_AC_LubricateTotalNum.LimitDown = 0;
            _limitBom.SPL_AC_LubricateTotalNum.LimitUp = 999;

            _limitBom.SPL_AC_PowerOnLubricateTime = new LimitBomItem();
            _limitBom.SPL_AC_PowerOnLubricateTime.LimitDown = 0;
            _limitBom.SPL_AC_PowerOnLubricateTime.LimitUp = 999;

            _limitBom.SPL_AC_LubricateDetecte = new LimitBomItem();
            _limitBom.SPL_AC_LubricateDetecte.LimitDown = 0;
            _limitBom.SPL_AC_LubricateDetecte.LimitUp = 999;

            _limitBom.SPL_AC2_LubricateTime = new LimitBomItem();
            _limitBom.SPL_AC2_LubricateTime.LimitDown = 0;
            _limitBom.SPL_AC2_LubricateTime.LimitUp = 999;

            _limitBom.SPL_AC2_SetLubricateInterval = new LimitBomItem();
            _limitBom.SPL_AC2_SetLubricateInterval.LimitDown = 0;
            _limitBom.SPL_AC2_SetLubricateInterval.LimitUp = 999;

            _limitBom.SPL_AC2_ActualLubricateInterval = new LimitBomItem();
            _limitBom.SPL_AC2_ActualLubricateInterval.LimitDown = 0;
            _limitBom.SPL_AC2_ActualLubricateInterval.LimitUp = 999;

            _limitBom.SPL_AC2_LubricateDetectTime = new LimitBomItem();
            _limitBom.SPL_AC2_LubricateDetectTime.LimitDown = 0;
            _limitBom.SPL_AC2_LubricateDetectTime.LimitUp = 999;

            _limitBom.SPL_AC2_LubricateTotalNum = new LimitBomItem();
            _limitBom.SPL_AC2_LubricateTotalNum.LimitDown = 0;
            _limitBom.SPL_AC2_LubricateTotalNum.LimitUp = 999;

            _limitBom.SPL_AC2_PowerOnLubricateTime = new LimitBomItem();
            _limitBom.SPL_AC2_PowerOnLubricateTime.LimitDown = 0;
            _limitBom.SPL_AC2_PowerOnLubricateTime.LimitUp = 999;

            _limitBom.SPL_AC2_LubricateDetecte = new LimitBomItem();
            _limitBom.SPL_AC2_LubricateDetecte.LimitDown = 0;
            _limitBom.SPL_AC2_LubricateDetecte.LimitUp = 999;

            _limitBom.SPL_GR_LubricateReversing = new LimitBomItem();
            _limitBom.SPL_GR_LubricateReversing.LimitDown = 0;
            _limitBom.SPL_GR_LubricateReversing.LimitUp = 999;

            _limitBom.SPL_GR_LubricateDetectTime = new LimitBomItem();
            _limitBom.SPL_GR_LubricateDetectTime.LimitDown = 0;
            _limitBom.SPL_GR_LubricateDetectTime.LimitUp = 999;

            _limitBom.SPL_SC_LubricateReversing = new LimitBomItem();
            _limitBom.SPL_SC_LubricateReversing.LimitDown = 0;
            _limitBom.SPL_SC_LubricateReversing.LimitUp = 999;

            _limitBom.SPL_OS_Time = new LimitBomItem();
            _limitBom.SPL_OS_Time.LimitDown = 0;
            _limitBom.SPL_OS_Time.LimitUp = 999;

            _limitBom.SPL_OS_IntervalTime = new LimitBomItem();
            _limitBom.SPL_OS_IntervalTime.LimitDown = 0;
            _limitBom.SPL_OS_IntervalTime.LimitUp = 999;

            _limitBom.SPL_OS_DelayTime = new LimitBomItem();
            _limitBom.SPL_OS_DelayTime.LimitDown = 0;
            _limitBom.SPL_OS_DelayTime.LimitUp = 999;

            _limitBom.SPL_TS_DelayTime = new LimitBomItem();
            _limitBom.SPL_TS_DelayTime.LimitDown = 0;
            _limitBom.SPL_TS_DelayTime.LimitUp = 999;

            _limitBom.SPL_TS_StopTime = new LimitBomItem();
            _limitBom.SPL_TS_StopTime.LimitDown = 0;
            _limitBom.SPL_TS_StopTime.LimitUp = 999;

            _limitBom.SPL_TS_RunTime = new LimitBomItem();
            _limitBom.SPL_TS_RunTime.LimitDown = 0;
            _limitBom.SPL_TS_RunTime.LimitUp = 999;

            #endregion

            #region 系统参数 模拟量设定
            _limitBom.SPA_A1_Value = new LimitBomItem();
            _limitBom.SPA_A1_Value.LimitDown = 0;
            _limitBom.SPA_A1_Value.LimitUp = 999;
            _limitBom.SPA_A1_WeightPara1 = new LimitBomItem();
            _limitBom.SPA_A1_WeightPara1.LimitDown = 0;
            _limitBom.SPA_A1_WeightPara1.LimitUp = 999;
            _limitBom.SPA_A1_WeightPara2 = new LimitBomItem();
            _limitBom.SPA_A1_WeightPara2.LimitDown = 0;
            _limitBom.SPA_A1_WeightPara2.LimitUp = 999;
            _limitBom.SPA_A1_Weight = new LimitBomItem();
            _limitBom.SPA_A1_Weight.LimitDown = 0;
            _limitBom.SPA_A1_Weight.LimitUp = 999;

            _limitBom.SPA_A2_Value = new LimitBomItem();
            _limitBom.SPA_A2_Value.LimitDown = 0;
            _limitBom.SPA_A2_Value.LimitUp = 999;
            _limitBom.SPA_A2_WeightPara1 = new LimitBomItem();
            _limitBom.SPA_A2_WeightPara1.LimitDown = 0;
            _limitBom.SPA_A2_WeightPara1.LimitUp = 999;
            _limitBom.SPA_A2_WeightPara2 = new LimitBomItem();
            _limitBom.SPA_A2_WeightPara2.LimitDown = 0;
            _limitBom.SPA_A2_WeightPara2.LimitUp = 999;
            _limitBom.SPA_A2_Weight = new LimitBomItem();
            _limitBom.SPA_A2_Weight.LimitDown = 0;
            _limitBom.SPA_A2_Weight.LimitUp = 999;

            _limitBom.SPA_A3_Value = new LimitBomItem();
            _limitBom.SPA_A3_Value.LimitDown = 0;
            _limitBom.SPA_A3_Value.LimitUp = 999;
            _limitBom.SPA_A3_WeightPara1 = new LimitBomItem();
            _limitBom.SPA_A3_WeightPara1.LimitDown = 0;
            _limitBom.SPA_A3_WeightPara1.LimitUp = 999;
            _limitBom.SPA_A3_WeightPara2 = new LimitBomItem();
            _limitBom.SPA_A3_WeightPara2.LimitDown = 0;
            _limitBom.SPA_A3_WeightPara2.LimitUp = 999;
            _limitBom.SPA_A3_Weight = new LimitBomItem();
            _limitBom.SPA_A3_Weight.LimitDown = 0;
            _limitBom.SPA_A3_Weight.LimitUp = 999;

            _limitBom.SPA_A4_Value = new LimitBomItem();
            _limitBom.SPA_A4_Value.LimitDown = 0;
            _limitBom.SPA_A4_Value.LimitUp = 999;
            _limitBom.SPA_A4_WeightPara1 = new LimitBomItem();
            _limitBom.SPA_A4_WeightPara1.LimitDown = 0;
            _limitBom.SPA_A4_WeightPara1.LimitUp = 999;
            _limitBom.SPA_A4_WeightPara2 = new LimitBomItem();
            _limitBom.SPA_A4_WeightPara2.LimitDown = 0;
            _limitBom.SPA_A4_WeightPara2.LimitUp = 999;
            _limitBom.SPA_A4_Weight = new LimitBomItem();
            _limitBom.SPA_A4_Weight.LimitDown = 0;
            _limitBom.SPA_A4_Weight.LimitUp = 999;

            _limitBom.SPA_TotalWeight = new LimitBomItem();
            _limitBom.SPA_TotalWeight.LimitDown = 0;
            _limitBom.SPA_TotalWeight.LimitUp = 999;
            _limitBom.SPA_DetectPosition = new LimitBomItem();
            _limitBom.SPA_DetectPosition.LimitDown = 0;
            _limitBom.SPA_DetectPosition.LimitUp = 999;
            _limitBom.SPA_DetectInPosition = new LimitBomItem();
            _limitBom.SPA_DetectInPosition.LimitDown = 0;
            _limitBom.SPA_DetectInPosition.LimitUp = 999;
            _limitBom.SPA_DetectSensor = new LimitBomItem();
            _limitBom.SPA_DetectSensor.LimitDown = 0;
            _limitBom.SPA_DetectSensor.LimitUp = 999;

            _limitBom.SPA_Pressure = new LimitBomItem();
            _limitBom.SPA_Pressure.LimitDown = 0;
            _limitBom.SPA_Pressure.LimitUp = 999;
            _limitBom.SPA_PressureUp = new LimitBomItem();
            _limitBom.SPA_PressureUp.LimitDown = 0;
            _limitBom.SPA_PressureUp.LimitUp = 999;
            _limitBom.SPA_PressureDown = new LimitBomItem();
            _limitBom.SPA_PressureDown.LimitDown = 0;
            _limitBom.SPA_PressureDown.LimitUp = 999;

            #endregion‘

            #region 系统参数 编码器设定
            _limitBom.SPA_IM_RESOLUTION = new LimitBomItem();
            _limitBom.SPA_IM_RESOLUTION.LimitDown = 0;
            _limitBom.SPA_IM_RESOLUTION.LimitUp = 999;
            _limitBom.SPA_IM_MOVEPITCH = new LimitBomItem();
            _limitBom.SPA_IM_MOVEPITCH.LimitDown = 0;
            _limitBom.SPA_IM_MOVEPITCH.LimitUp = 999;
            _limitBom.SPA_IM_UPPOSITION = new LimitBomItem();
            _limitBom.SPA_IM_UPPOSITION.LimitDown = 0;
            _limitBom.SPA_IM_UPPOSITION.LimitUp = 999;
            _limitBom.SPA_IM_DOWNPOSITION = new LimitBomItem();
            _limitBom.SPA_IM_DOWNPOSITION.LimitDown = 0;
            _limitBom.SPA_IM_DOWNPOSITION.LimitUp = 999;
            _limitBom.SPA_IM_SPEEDCHANGEPOSITION = new LimitBomItem();
            _limitBom.SPA_IM_SPEEDCHANGEPOSITION.LimitDown = 0;
            _limitBom.SPA_IM_SPEEDCHANGEPOSITION.LimitUp = 999;
            _limitBom.SPA_IM_LIMITUP = new LimitBomItem();
            _limitBom.SPA_IM_LIMITUP.LimitDown = 0;
            _limitBom.SPA_IM_LIMITUP.LimitUp = 999;
            _limitBom.SPA_IM_LIMITDOWN = new LimitBomItem();
            _limitBom.SPA_IM_LIMITDOWN.LimitDown = 0;
            _limitBom.SPA_IM_LIMITDOWN.LimitUp = 999;
            _limitBom.SPA_IM_ERROR = new LimitBomItem();
            _limitBom.SPA_IM_ERROR.LimitDown = 0;
            _limitBom.SPA_IM_ERROR.LimitUp = 999;
            _limitBom.SPA_IM_DIRECTION = new LimitBomItem();
            _limitBom.SPA_IM_DIRECTION.LimitDown = 0;
            _limitBom.SPA_IM_DIRECTION.LimitUp = 999;
            _limitBom.SPA_AC_RESOLUTION = new LimitBomItem();
            _limitBom.SPA_AC_RESOLUTION.LimitDown = 0;
            _limitBom.SPA_AC_RESOLUTION.LimitUp = 999;
            _limitBom.SPA_AC_MOVEPITCH = new LimitBomItem();
            _limitBom.SPA_AC_MOVEPITCH.LimitDown = 0;
            _limitBom.SPA_AC_MOVEPITCH.LimitUp = 999;
            _limitBom.SPA_AC_UPPOSITION = new LimitBomItem();
            _limitBom.SPA_AC_UPPOSITION.LimitDown = 0;
            _limitBom.SPA_AC_UPPOSITION.LimitUp = 999;
            _limitBom.SPA_AC_DOWNPOSITION = new LimitBomItem();
            _limitBom.SPA_AC_DOWNPOSITION.LimitDown = 0;
            _limitBom.SPA_AC_DOWNPOSITION.LimitUp = 999;
            _limitBom.SPA_AC_LIMITUP = new LimitBomItem();
            _limitBom.SPA_AC_LIMITUP.LimitDown = 0;
            _limitBom.SPA_AC_LIMITUP.LimitUp = 999;
            _limitBom.SPA_AC_LIMITDOWN = new LimitBomItem();
            _limitBom.SPA_AC_LIMITDOWN.LimitDown = 0;
            _limitBom.SPA_AC_LIMITDOWN.LimitUp = 999;
            _limitBom.SPA_AC_ERROR = new LimitBomItem();
            _limitBom.SPA_AC_ERROR.LimitDown = 0;
            _limitBom.SPA_AC_ERROR.LimitUp = 999;
            _limitBom.SPA_AC_DIRECTION = new LimitBomItem();
            _limitBom.SPA_AC_DIRECTION.LimitDown = 0;
            _limitBom.SPA_AC_DIRECTION.LimitUp = 999;


            #endregion

            var jsonLimitBom = JsonConvert.SerializeObject(_limitBom, Formatting.Indented);
            using (StreamWriter sw = new StreamWriter(@"limitbom.cfg", false))
            {
                sw.Write(jsonLimitBom);
            }
            #endregion

            #region macro
            _macroBom.DownDelayTime = new MacroBomItem();
            _macroBom.DownDelayTime.Id = "M1001";
            _macroBom.DownDelayTime.Adr = 100;
            _macroBom.DownTime = new MacroBomItem();
            _macroBom.DownTime.Adr = 101;
            _macroBom.SavePressureCount = new MacroBomItem();
            _macroBom.SavePressureCount.Adr = 102;
            _macroBom.UpDelayTime = new MacroBomItem();
            _macroBom.UpDelayTime.Adr = 103;
            _macroBom.UpTime = new MacroBomItem();
            _macroBom.UpTime.Adr = 104;
            
            _macroBom.SPP_Pressure = new MacroBomItem();
            _macroBom.SPP_Pressure.Id = "M1058";
            _macroBom.SPP_Pressure.Adr = 111;
            _macroBom.SPP_Pressure.IsRecipes = true;
            _macroBom.SPP_Time = new MacroBomItem();
            _macroBom.SPP_Time.Id = "M1059";
            _macroBom.SPP_Time.Adr = 112;
            _macroBom.SPP_Time.IsRecipes = true;
            _macroBom.SPP_PreDelayTime = new MacroBomItem();
            _macroBom.SPP_PreDelayTime.Id = "M1060";
            _macroBom.SPP_PreDelayTime.Adr = 113;
            _macroBom.SPP_PreDelayTime.IsRecipes = true;
            _macroBom.SPP_ChangeMode = new MacroBomItem();
            _macroBom.SPP_ChangeMode.Id = "M1061";
            _macroBom.SPP_ChangeMode.Adr = 114;
            _macroBom.SPP_ChangeMode.IsRecipes = true;
            _macroBom.SPP_ChangePressure = new MacroBomItem();
            _macroBom.SPP_ChangePressure.Id = "M1062";
            _macroBom.SPP_ChangePressure.Adr = 115;
            _macroBom.SPP_ChangePressure.IsRecipes = true;

            var jsonMacroBom = JsonConvert.SerializeObject(_macroBom, Formatting.Indented);
            using (StreamWriter sw = new StreamWriter(@"macrobom.cfg", false))
            {
                sw.Write(jsonMacroBom);
            }
            #endregion

            #region PMC

            #region 状态栏
            _pmcBom.Mode = new PmcBomItem();
            _pmcBom.Mode.Id = "PMC1001";
            _pmcBom.Mode.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.Mode.DataType = PmcDataTypeEnum.WORD;
            _pmcBom.Mode.Adr = 0;

            _pmcBom.MainStatus = new PmcBomItem();
            _pmcBom.MainStatus.Id = "PMC1002";
            _pmcBom.MainStatus.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.MainStatus.DataType = PmcDataTypeEnum.BYTE;
            _pmcBom.MainStatus.Adr = 2;

            _pmcBom.SliderPressure = new PmcBomItem();
            _pmcBom.SliderPressure.Id = "PMC1003";
            _pmcBom.SliderPressure.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SliderPressure.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SliderPressure.Adr = 4;
            _pmcBom.SliderPressure.ConversionFactor = 10;

            _pmcBom.BalanceCylinderPressure = new PmcBomItem();
            _pmcBom.BalanceCylinderPressure.Id = "PMC1004";
            _pmcBom.BalanceCylinderPressure.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.BalanceCylinderPressure.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.BalanceCylinderPressure.Adr = 8;
            _pmcBom.BalanceCylinderPressure.ConversionFactor = 10;

            _pmcBom.InstallDieHigh = new PmcBomItem();
            _pmcBom.InstallDieHigh.Id = "PMC1005";
            _pmcBom.InstallDieHigh.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.InstallDieHigh.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.InstallDieHigh.Adr = 12;
            _pmcBom.InstallDieHigh.ConversionFactor = 100;

            _pmcBom.NutTemperature = new PmcBomItem();
            _pmcBom.NutTemperature.Id = "PMC1010";
            _pmcBom.NutTemperature.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.NutTemperature.DataType = PmcDataTypeEnum.WORD;
            _pmcBom.NutTemperature.Adr = 36;
            _pmcBom.NutTemperature.ConversionFactor = 10;

            _pmcBom.TotalPiece = new PmcBomItem();
            _pmcBom.TotalPiece.Id = "PMC1006";
            _pmcBom.TotalPiece.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.TotalPiece.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.TotalPiece.Adr = 16;

            _pmcBom.TotalWork = new PmcBomItem();
            _pmcBom.TotalWork.Id = "PMC1007";
            _pmcBom.TotalWork.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.TotalWork.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.TotalWork.Adr = 20;

            _pmcBom.DayPiece = new PmcBomItem();
            _pmcBom.DayPiece.Id = "PMC1008";
            _pmcBom.DayPiece.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.DayPiece.DataType = PmcDataTypeEnum.WORD;
            _pmcBom.DayPiece.Adr = 24;

            _pmcBom.DayWork = new PmcBomItem();
            _pmcBom.DayWork.Id = "PMC1009";
            _pmcBom.DayWork.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.DayWork.DataType = PmcDataTypeEnum.WORD;
            _pmcBom.DayWork.Adr = 28;

            #endregion

            #region 状态监控
            _pmcBom.SMP_CylinderMode = new PmcBomItem();
            _pmcBom.SMP_CylinderMode.Id = "PMC1100";
            _pmcBom.SMP_CylinderMode.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SMP_CylinderMode.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SMP_CylinderMode.Adr = 100;

            _pmcBom.SMP_LoaderState = new PmcBomItem();
            _pmcBom.SMP_LoaderState.Id = "PMC1101";
            _pmcBom.SMP_LoaderState.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SMP_LoaderState.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SMP_LoaderState.Adr = 104;

            _pmcBom.SMP_WorkStep = new PmcBomItem();
            _pmcBom.SMP_WorkStep.Id = "PMC1102";
            _pmcBom.SMP_WorkStep.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SMP_WorkStep.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SMP_WorkStep.Adr = 108;

            _pmcBom.SMP_WorkTime = new PmcBomItem();
            _pmcBom.SMP_WorkTime.Id = "PMC1105";
            _pmcBom.SMP_WorkTime.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SMP_WorkTime.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SMP_WorkTime.Adr = 112;

            _pmcBom.SMP_SliderPressure = new PmcBomItem();
            _pmcBom.SMP_SliderPressure.Id = "PMC1103";
            _pmcBom.SMP_SliderPressure.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SMP_SliderPressure.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SMP_SliderPressure.Adr = 116;
            _pmcBom.SMP_SliderPressure.ConversionFactor = 10;

            _pmcBom.SMP_SliderTemperature = new PmcBomItem();
            _pmcBom.SMP_SliderTemperature.Id = "PMC1104";
            _pmcBom.SMP_SliderTemperature.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SMP_SliderTemperature.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SMP_SliderTemperature.Adr = 120;
            _pmcBom.SMP_SliderTemperature.ConversionFactor = 10;


            #endregion

            #region 换模设定
            //_pmcBom.DCP_RapidFeed = new PmcBomItem();
            //_pmcBom.DCP_RapidFeed.Id = "PMC1200";
            //_pmcBom.DCP_RapidFeed.AdrType = PmcAdrTypeEnum.E;
            //_pmcBom.DCP_RapidFeed.DataType = PmcDataTypeEnum.WORD;
            //_pmcBom.DCP_RapidFeed.Adr = 200;

            _pmcBom.DCP_JogFeed = new PmcBomItem();
            _pmcBom.DCP_JogFeed.Id = "PMC1201";
            _pmcBom.DCP_JogFeed.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.DCP_JogFeed.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.DCP_JogFeed.Adr = 200;

            _pmcBom.DCP_InstallDieHighSet = new PmcBomItem();
            _pmcBom.DCP_InstallDieHighSet.Id = "PMC1202";
            _pmcBom.DCP_InstallDieHighSet.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.DCP_InstallDieHighSet.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.DCP_InstallDieHighSet.Adr = 204;
            _pmcBom.DCP_InstallDieHighSet.ConversionFactor = 100;
            _pmcBom.DCP_InstallDieHighSet.IsRecipes = true;

            _pmcBom.DCP_InstallDieHighActual = new PmcBomItem();
            _pmcBom.DCP_InstallDieHighActual.Id = "PMC1203";
            _pmcBom.DCP_InstallDieHighActual.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.DCP_InstallDieHighActual.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.DCP_InstallDieHighActual.Adr = 220;
            _pmcBom.DCP_InstallDieHighActual.ConversionFactor = 100;

            _pmcBom.DCP_CylinderpRressureSet = new PmcBomItem();
            _pmcBom.DCP_CylinderpRressureSet.Id = "PMC1204";
            _pmcBom.DCP_CylinderpRressureSet.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.DCP_CylinderpRressureSet.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.DCP_CylinderpRressureSet.Adr = 208;
            _pmcBom.DCP_CylinderpRressureSet.ConversionFactor = 10;
            _pmcBom.DCP_CylinderpRressureSet.IsRecipes = true;

            _pmcBom.DCP_CylinderpRressureActual = new PmcBomItem();
            _pmcBom.DCP_CylinderpRressureActual.Id = "PMC1205";
            _pmcBom.DCP_CylinderpRressureActual.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.DCP_CylinderpRressureActual.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.DCP_CylinderpRressureActual.Adr = 212;
            _pmcBom.DCP_CylinderpRressureActual.ConversionFactor = 10;

            _pmcBom.DCP_DieWeight = new PmcBomItem();
            _pmcBom.DCP_DieWeight.Id = "PMC1206";
            _pmcBom.DCP_DieWeight.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.DCP_DieWeight.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.DCP_DieWeight.Adr = 216;
            _pmcBom.DCP_DieWeight.ConversionFactor = 10;
            _pmcBom.DCP_DieWeight.IsRecipes = true;

            _pmcBom.DCP_ChangeMode = new PmcBomItem();
            _pmcBom.DCP_ChangeMode.Id = "PMC1208";
            _pmcBom.DCP_ChangeMode.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.DCP_ChangeMode.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.DCP_ChangeMode.Adr = 250;
            _pmcBom.DCP_ChangeMode.Bit = 0;
            _pmcBom.DCP_ChangeMode.IsRecipes = true;


            _pmcBom.DCP_MaxWeight = new PmcBomItem();
            _pmcBom.DCP_MaxWeight.Id = "PMC1209";
            _pmcBom.DCP_MaxWeight.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.DCP_MaxWeight.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.DCP_MaxWeight.Adr = 228;
            _pmcBom.DCP_MaxWeight.ConversionFactor = 10;
            _pmcBom.DCP_MaxWeight.IsRecipes = true;

            _pmcBom.DCP_MaxNugTemperature = new PmcBomItem();
            _pmcBom.DCP_MaxNugTemperature.Id = "PMC1210";
            _pmcBom.DCP_MaxNugTemperature.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.DCP_MaxNugTemperature.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.DCP_MaxNugTemperature.Adr = 224;
            _pmcBom.DCP_MaxNugTemperature.ConversionFactor = 10;
            _pmcBom.DCP_MaxNugTemperature.IsRecipes = true;

            _pmcBom.DCP_MinCylinderpRressure = new PmcBomItem();
            _pmcBom.DCP_MinCylinderpRressure.Id = "PMC1211";
            _pmcBom.DCP_MinCylinderpRressure.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.DCP_MinCylinderpRressure.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.DCP_MinCylinderpRressure.Adr = 232;
            _pmcBom.DCP_MinCylinderpRressure.ConversionFactor = 10;
            _pmcBom.DCP_MinCylinderpRressure.IsRecipes = true;

            _pmcBom.DCP_InstallDieHighReset = new PmcBomItem();
            _pmcBom.DCP_InstallDieHighReset.Id = "PMC1212";
            _pmcBom.DCP_InstallDieHighReset.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.DCP_InstallDieHighReset.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.DCP_InstallDieHighReset.Adr = 250;
            _pmcBom.DCP_InstallDieHighReset.Bit = 0;
            _pmcBom.DCP_InstallDieHighReset.IsRecipes = true;

            _pmcBom.DCP_PressureSet = new PmcBomItem();
            _pmcBom.DCP_PressureSet.Id = "PMC1213";
            _pmcBom.DCP_PressureSet.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.DCP_PressureSet.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.DCP_PressureSet.Adr = 250;
            _pmcBom.DCP_PressureSet.Bit = 7;
            _pmcBom.DCP_PressureSet.IsRecipes = true;

            

            #endregion

            #region 合模设定
            _pmcBom.DJP_SectionNum = new PmcBomItem();
            _pmcBom.DJP_SectionNum.Id = "PMC1300";
            _pmcBom.DJP_SectionNum.AdrType = PmcAdrTypeEnum.D;
            _pmcBom.DJP_SectionNum.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.DJP_SectionNum.Adr = 1540;

            _pmcBom.DJP_TopDeadCentre = new PmcBomItem();
            _pmcBom.DJP_TopDeadCentre.Id = "PMC1301";
            _pmcBom.DJP_TopDeadCentre.AdrType = PmcAdrTypeEnum.D;
            _pmcBom.DJP_TopDeadCentre.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.DJP_TopDeadCentre.Adr = 1020;
            _pmcBom.DJP_TopDeadCentre.ConversionFactor = 1000;

            _pmcBom.DJP_Pos_1 = new PmcBomItem();
            _pmcBom.DJP_Pos_1.Id = "PMC1302";
            _pmcBom.DJP_Pos_1.AdrType = PmcAdrTypeEnum.D;
            _pmcBom.DJP_Pos_1.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.DJP_Pos_1.Adr = 1024;
            _pmcBom.DJP_Pos_1.ConversionFactor = 1000;

            _pmcBom.DJP_Speed_1 = new PmcBomItem();
            _pmcBom.DJP_Speed_1.Id = "PMC1303";
            _pmcBom.DJP_Speed_1.AdrType = PmcAdrTypeEnum.D;
            _pmcBom.DJP_Speed_1.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.DJP_Speed_1.Adr = 1120;

            _pmcBom.DJP_Pos_2 = new PmcBomItem();
            _pmcBom.DJP_Pos_2.Id = "PMC1304";
            _pmcBom.DJP_Pos_2.AdrType = PmcAdrTypeEnum.D;
            _pmcBom.DJP_Pos_2.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.DJP_Pos_2.Adr = 1028;
            _pmcBom.DJP_Pos_2.ConversionFactor = 1000;

            _pmcBom.DJP_Speed_2 = new PmcBomItem();
            _pmcBom.DJP_Speed_2.Id = "PMC1305";
            _pmcBom.DJP_Speed_2.AdrType = PmcAdrTypeEnum.D;
            _pmcBom.DJP_Speed_2.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.DJP_Speed_2.Adr = 1124;

            _pmcBom.DJP_Pos_3 = new PmcBomItem();
            _pmcBom.DJP_Pos_3.Id = "PMC1306";
            _pmcBom.DJP_Pos_3.AdrType = PmcAdrTypeEnum.D;
            _pmcBom.DJP_Pos_3.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.DJP_Pos_3.Adr = 1032;
            _pmcBom.DJP_Pos_3.ConversionFactor = 1000;

            _pmcBom.DJP_Speed_3 = new PmcBomItem();
            _pmcBom.DJP_Speed_3.Id = "PMC1307";
            _pmcBom.DJP_Speed_3.AdrType = PmcAdrTypeEnum.D;
            _pmcBom.DJP_Speed_3.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.DJP_Speed_3.Adr = 1128;

            _pmcBom.DJP_Pos_4 = new PmcBomItem();
            _pmcBom.DJP_Pos_4.Id = "PMC1308";
            _pmcBom.DJP_Pos_4.AdrType = PmcAdrTypeEnum.D;
            _pmcBom.DJP_Pos_4.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.DJP_Pos_4.Adr = 1036;
            _pmcBom.DJP_Pos_4.ConversionFactor = 1000;

            _pmcBom.DJP_Speed_4 = new PmcBomItem();
            _pmcBom.DJP_Speed_4.Id = "PMC1309";
            _pmcBom.DJP_Speed_4.AdrType = PmcAdrTypeEnum.D;
            _pmcBom.DJP_Speed_4.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.DJP_Speed_4.Adr = 1132;

            _pmcBom.DJP_Pos_5 = new PmcBomItem();
            _pmcBom.DJP_Pos_5.Id = "PMC1310";
            _pmcBom.DJP_Pos_5.AdrType = PmcAdrTypeEnum.D;
            _pmcBom.DJP_Pos_5.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.DJP_Pos_5.Adr = 1040;
            _pmcBom.DJP_Pos_5.ConversionFactor = 1000;

            _pmcBom.DJP_Speed_5 = new PmcBomItem();
            _pmcBom.DJP_Speed_5.Id = "PMC1311";
            _pmcBom.DJP_Speed_5.AdrType = PmcAdrTypeEnum.D;
            _pmcBom.DJP_Speed_5.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.DJP_Speed_5.Adr = 1136;

            _pmcBom.DJP_Pos_6 = new PmcBomItem();
            _pmcBom.DJP_Pos_6.Id = "PMC1312";
            _pmcBom.DJP_Pos_6.AdrType = PmcAdrTypeEnum.D;
            _pmcBom.DJP_Pos_6.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.DJP_Pos_6.Adr = 1044;
            _pmcBom.DJP_Pos_6.ConversionFactor = 1000;

            _pmcBom.DJP_Speed_6 = new PmcBomItem();
            _pmcBom.DJP_Speed_6.Id = "PMC1313";
            _pmcBom.DJP_Speed_6.AdrType = PmcAdrTypeEnum.D;
            _pmcBom.DJP_Speed_6.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.DJP_Speed_6.Adr = 1140;

            _pmcBom.DJP_Pos_7 = new PmcBomItem();
            _pmcBom.DJP_Pos_7.Id = "PMC1314";
            _pmcBom.DJP_Pos_7.AdrType = PmcAdrTypeEnum.D;
            _pmcBom.DJP_Pos_7.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.DJP_Pos_7.Adr = 1048;
            _pmcBom.DJP_Pos_7.ConversionFactor = 1000;

            _pmcBom.DJP_Speed_7 = new PmcBomItem();
            _pmcBom.DJP_Speed_7.Id = "PMC1315";
            _pmcBom.DJP_Speed_7.AdrType = PmcAdrTypeEnum.D;
            _pmcBom.DJP_Speed_7.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.DJP_Speed_7.Adr = 1144;


            _pmcBom.DJP_Pos_8 = new PmcBomItem();
            _pmcBom.DJP_Pos_8.Id = "PMC1316";
            _pmcBom.DJP_Pos_8.AdrType = PmcAdrTypeEnum.D;
            _pmcBom.DJP_Pos_8.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.DJP_Pos_8.Adr = 1052;
            _pmcBom.DJP_Pos_8.ConversionFactor = 1000;

            _pmcBom.DJP_Speed_8 = new PmcBomItem();
            _pmcBom.DJP_Speed_8.Id = "PMC1317";
            _pmcBom.DJP_Speed_8.AdrType = PmcAdrTypeEnum.D;
            _pmcBom.DJP_Speed_8.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.DJP_Speed_8.Adr = 1148;

            _pmcBom.DJP_BottomDeadCentre = new PmcBomItem();
            _pmcBom.DJP_BottomDeadCentre.Id = "PMC1318";
            _pmcBom.DJP_BottomDeadCentre.AdrType = PmcAdrTypeEnum.D;
            _pmcBom.DJP_BottomDeadCentre.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.DJP_BottomDeadCentre.Adr = 1064;
            _pmcBom.DJP_BottomDeadCentre.ConversionFactor = 1000;

            _pmcBom.DJP_Speed_BottomDeadCentre = new PmcBomItem();
            _pmcBom.DJP_Speed_BottomDeadCentre.Id = "PMC1319";
            _pmcBom.DJP_Speed_BottomDeadCentre.AdrType = PmcAdrTypeEnum.D;
            _pmcBom.DJP_Speed_BottomDeadCentre.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.DJP_Speed_BottomDeadCentre.Adr = 1160;

            _pmcBom.DJP_BottomDeadCentre_StopTime = new PmcBomItem();
            _pmcBom.DJP_BottomDeadCentre_StopTime.Id = "PMC1320";
            _pmcBom.DJP_BottomDeadCentre_StopTime.AdrType = PmcAdrTypeEnum.D;
            _pmcBom.DJP_BottomDeadCentre_StopTime.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.DJP_BottomDeadCentre_StopTime.Adr = 1000;

            #endregion

            #region 开模设定
            _pmcBom.DPP_SectionNum = new PmcBomItem();
            _pmcBom.DPP_SectionNum.Id = "PMC1400";
            _pmcBom.DPP_SectionNum.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.DPP_SectionNum.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.DPP_SectionNum.Adr = 2020;
            _pmcBom.DPP_BottomDeadCentre = new PmcBomItem();
            _pmcBom.DPP_BottomDeadCentre.Id = "PMC1401";
            _pmcBom.DPP_BottomDeadCentre.AdrType = PmcAdrTypeEnum.D;
            _pmcBom.DPP_BottomDeadCentre.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.DPP_BottomDeadCentre.Adr = 1064;
            _pmcBom.DPP_BottomDeadCentre.ConversionFactor = 1000;
            _pmcBom.DPP_Speed_TopDeadCentre = new PmcBomItem();
            _pmcBom.DPP_Speed_TopDeadCentre.Id = "PMC1402";
            _pmcBom.DPP_Speed_TopDeadCentre.AdrType = PmcAdrTypeEnum.D;
            _pmcBom.DPP_Speed_TopDeadCentre.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.DPP_Speed_TopDeadCentre.Adr = 1204;
            _pmcBom.DPP_Pos_1 = new PmcBomItem();
            _pmcBom.DPP_Pos_1.Id = "PMC1403";
            _pmcBom.DPP_Pos_1.AdrType = PmcAdrTypeEnum.D;
            _pmcBom.DPP_Pos_1.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.DPP_Pos_1.Adr = 1068;
            _pmcBom.DPP_Pos_1.ConversionFactor = 1000;
            _pmcBom.DPP_Speed_1 = new PmcBomItem();
            _pmcBom.DPP_Speed_1.Id = "PMC1404";
            _pmcBom.DPP_Speed_1.Adr = 1164;
            _pmcBom.DPP_Speed_1.AdrType = PmcAdrTypeEnum.D;
            _pmcBom.DPP_Speed_1.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.DPP_Pos_2 = new PmcBomItem();
            _pmcBom.DPP_Pos_2.Id = "PMC1405";
            _pmcBom.DPP_Pos_2.AdrType = PmcAdrTypeEnum.D;
            _pmcBom.DPP_Pos_2.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.DPP_Pos_2.Adr = 1072;
            _pmcBom.DPP_Pos_2.ConversionFactor = 1000;
            _pmcBom.DPP_Speed_2 = new PmcBomItem();
            _pmcBom.DPP_Speed_2.Id = "PMC1406";
            _pmcBom.DPP_Speed_2.Adr = 1168;
            _pmcBom.DPP_Speed_2.AdrType = PmcAdrTypeEnum.D;
            _pmcBom.DPP_Speed_2.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.DPP_Pos_3 = new PmcBomItem();
            _pmcBom.DPP_Pos_3.Id = "PMC1407";
            _pmcBom.DPP_Pos_3.AdrType = PmcAdrTypeEnum.D;
            _pmcBom.DPP_Pos_3.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.DPP_Pos_3.Adr = 1076;
            _pmcBom.DPP_Pos_3.ConversionFactor = 1000;
            _pmcBom.DPP_Speed_3 = new PmcBomItem();
            _pmcBom.DPP_Speed_3.Id = "PMC1408";
            _pmcBom.DPP_Speed_3.Adr = 1172;
            _pmcBom.DPP_Speed_3.AdrType = PmcAdrTypeEnum.D;
            _pmcBom.DPP_Speed_3.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.DPP_Pos_4 = new PmcBomItem();
            _pmcBom.DPP_Pos_4.Id = "PMC1409";
            _pmcBom.DPP_Pos_4.AdrType = PmcAdrTypeEnum.D;
            _pmcBom.DPP_Pos_4.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.DPP_Pos_4.Adr = 1080;
            _pmcBom.DPP_Pos_4.ConversionFactor = 1000;
            _pmcBom.DPP_Speed_4 = new PmcBomItem();
            _pmcBom.DPP_Speed_4.Id = "PMC1410";
            _pmcBom.DPP_Speed_4.Adr = 1176;
            _pmcBom.DPP_Speed_4.AdrType = PmcAdrTypeEnum.D;
            _pmcBom.DPP_Speed_4.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.DPP_Pos_5 = new PmcBomItem();
            _pmcBom.DPP_Pos_5.Id = "PMC1411";
            _pmcBom.DPP_Pos_5.AdrType = PmcAdrTypeEnum.D;
            _pmcBom.DPP_Pos_5.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.DPP_Pos_5.Adr = 1084;
            _pmcBom.DPP_Pos_5.ConversionFactor = 1000;
            _pmcBom.DPP_Speed_5 = new PmcBomItem();
            _pmcBom.DPP_Speed_5.Id = "PMC1412";
            _pmcBom.DPP_Speed_5.Adr = 1180;
            _pmcBom.DPP_Speed_5.AdrType = PmcAdrTypeEnum.D;
            _pmcBom.DPP_Speed_5.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.DPP_Pos_6 = new PmcBomItem();
            _pmcBom.DPP_Pos_6.Id = "PMC1413";
            _pmcBom.DPP_Pos_6.AdrType = PmcAdrTypeEnum.D;
            _pmcBom.DPP_Pos_6.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.DPP_Pos_6.Adr = 1088;
            _pmcBom.DPP_Pos_6.ConversionFactor = 1000;
            _pmcBom.DPP_Speed_6 = new PmcBomItem();
            _pmcBom.DPP_Speed_6.Id = "PMC1414";
            _pmcBom.DPP_Speed_6.Adr = 1184;
            _pmcBom.DPP_Speed_6.AdrType = PmcAdrTypeEnum.D;
            _pmcBom.DPP_Speed_6.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.DPP_Pos_7 = new PmcBomItem();
            _pmcBom.DPP_Pos_7.Id = "PMC1415";
            _pmcBom.DPP_Pos_7.AdrType = PmcAdrTypeEnum.D;
            _pmcBom.DPP_Pos_7.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.DPP_Pos_7.Adr = 1092;
            _pmcBom.DPP_Pos_7.ConversionFactor = 1000;
            _pmcBom.DPP_Speed_7 = new PmcBomItem();
            _pmcBom.DPP_Speed_7.Id = "PMC1416";
            _pmcBom.DPP_Speed_7.Adr = 1188;
            _pmcBom.DPP_Speed_7.AdrType = PmcAdrTypeEnum.D;
            _pmcBom.DPP_Speed_7.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.DPP_Pos_8 = new PmcBomItem();
            _pmcBom.DPP_Pos_8.Id = "PMC1417";
            _pmcBom.DPP_Pos_8.AdrType = PmcAdrTypeEnum.D;
            _pmcBom.DPP_Pos_8.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.DPP_Pos_8.Adr = 1096;
            _pmcBom.DPP_Pos_8.ConversionFactor = 1000;
            _pmcBom.DPP_Speed_8 = new PmcBomItem();
            _pmcBom.DPP_Speed_8.Id = "PMC1418";
            _pmcBom.DPP_Speed_8.Adr = 1192;
            _pmcBom.DPP_Speed_8.AdrType = PmcAdrTypeEnum.D;
            _pmcBom.DPP_Speed_8.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.DPP_TopDeadCentre = new PmcBomItem();
            _pmcBom.DPP_TopDeadCentre.Id = "PMC1419";
            _pmcBom.DPP_TopDeadCentre.Adr = 1020;
            _pmcBom.DPP_TopDeadCentre.AdrType = PmcAdrTypeEnum.D;
            _pmcBom.DPP_TopDeadCentre.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.DPP_TopDeadCentre.ConversionFactor = 1000;

            #endregion

            #region 夹模器设定
            _pmcBom.CLS_ClampStatus1 = new PmcBomItem();
            _pmcBom.CLS_ClampStatus1.Id = "PMC1500";
            _pmcBom.CLS_ClampStatus1.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_ClampStatus1.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_ClampStatus1.Adr = 500;
            _pmcBom.CLS_ClampStatus1.Bit = 0;

            _pmcBom.CLS_ClampStatus2 = new PmcBomItem();
            _pmcBom.CLS_ClampStatus2.Id = "PMC1501";
            _pmcBom.CLS_ClampStatus2.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_ClampStatus2.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_ClampStatus2.Adr = 500;
            _pmcBom.CLS_ClampStatus2.Bit = 1;

            _pmcBom.CLS_ClampRelaxPosition = new PmcBomItem();
            _pmcBom.CLS_ClampRelaxPosition.Id = "PMC1502";
            _pmcBom.CLS_ClampRelaxPosition.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_ClampRelaxPosition.DataType = PmcDataTypeEnum.WORD;
            _pmcBom.CLS_ClampRelaxPosition.ConversionFactor = 100;
            _pmcBom.CLS_ClampRelaxPosition.Adr = 528;
            _pmcBom.CLS_ClampRelaxPosition.IsRecipes = true;

            _pmcBom.CLS_ClampRelaxInPosition = new PmcBomItem();
            _pmcBom.CLS_ClampRelaxInPosition.Id = "PMC1503";
            _pmcBom.CLS_ClampRelaxInPosition.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_ClampRelaxInPosition.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_ClampRelaxInPosition.Adr = 500;
            _pmcBom.CLS_ClampRelaxInPosition.Bit = 2;
            _pmcBom.CLS_ClampRelaxInPosition.IsRecipes = true;

            _pmcBom.CLS_Clamp_Front_1_Ebable = new PmcBomItem();
            _pmcBom.CLS_Clamp_Front_1_Ebable.Id = "PMC1504";
            _pmcBom.CLS_Clamp_Front_1_Ebable.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Front_1_Ebable.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Front_1_Ebable.Adr = 501;
            _pmcBom.CLS_Clamp_Front_1_Ebable.Bit = 0;

            _pmcBom.CLS_Clamp_Front_1_MoveOutStatus = new PmcBomItem();
            _pmcBom.CLS_Clamp_Front_1_MoveOutStatus.Id = "PMC1505";
            _pmcBom.CLS_Clamp_Front_1_MoveOutStatus.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Front_1_MoveOutStatus.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Front_1_MoveOutStatus.Adr = 501;
            _pmcBom.CLS_Clamp_Front_1_MoveOutStatus.Bit = 1;

            _pmcBom.CLS_Clamp_Front_1_MoveInStatus = new PmcBomItem();
            _pmcBom.CLS_Clamp_Front_1_MoveInStatus.Id = "PMC1506";
            _pmcBom.CLS_Clamp_Front_1_MoveInStatus.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Front_1_MoveInStatus.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Front_1_MoveInStatus.Adr = 501;
            _pmcBom.CLS_Clamp_Front_1_MoveInStatus.Bit = 2;

            _pmcBom.CLS_Clamp_Front_2_Ebable = new PmcBomItem();
            _pmcBom.CLS_Clamp_Front_2_Ebable.Id = "PMC1507";
            _pmcBom.CLS_Clamp_Front_2_Ebable.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Front_2_Ebable.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Front_2_Ebable.Adr = 502;
            _pmcBom.CLS_Clamp_Front_2_Ebable.Bit = 0;

            _pmcBom.CLS_Clamp_Front_2_MoveOutStatus = new PmcBomItem();
            _pmcBom.CLS_Clamp_Front_2_MoveOutStatus.Id = "PMC1508";
            _pmcBom.CLS_Clamp_Front_2_MoveOutStatus.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Front_2_MoveOutStatus.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Front_2_MoveOutStatus.Adr = 502;
            _pmcBom.CLS_Clamp_Front_2_MoveOutStatus.Bit = 1;

            _pmcBom.CLS_Clamp_Front_2_MoveInStatus = new PmcBomItem();
            _pmcBom.CLS_Clamp_Front_2_MoveInStatus.Id = "PMC1509";
            _pmcBom.CLS_Clamp_Front_2_MoveInStatus.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Front_2_MoveInStatus.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Front_2_MoveInStatus.Adr = 502;
            _pmcBom.CLS_Clamp_Front_2_MoveInStatus.Bit = 2;

            _pmcBom.CLS_Clamp_Front_3_Ebable = new PmcBomItem();
            _pmcBom.CLS_Clamp_Front_3_Ebable.Id = "PMC1510";
            _pmcBom.CLS_Clamp_Front_3_Ebable.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Front_3_Ebable.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Front_3_Ebable.Adr = 503;
            _pmcBom.CLS_Clamp_Front_3_Ebable.Bit = 0;

            _pmcBom.CLS_Clamp_Front_3_MoveOutStatus = new PmcBomItem();
            _pmcBom.CLS_Clamp_Front_3_MoveOutStatus.Id = "PMC1511";
            _pmcBom.CLS_Clamp_Front_3_MoveOutStatus.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Front_3_MoveOutStatus.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Front_3_MoveOutStatus.Adr = 503;
            _pmcBom.CLS_Clamp_Front_3_MoveOutStatus.Bit = 1;

            _pmcBom.CLS_Clamp_Front_3_MoveInStatus = new PmcBomItem();
            _pmcBom.CLS_Clamp_Front_3_MoveInStatus.Id = "PMC1512";
            _pmcBom.CLS_Clamp_Front_3_MoveInStatus.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Front_3_MoveInStatus.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Front_3_MoveInStatus.Adr = 503;
            _pmcBom.CLS_Clamp_Front_3_MoveInStatus.Bit = 2;

            _pmcBom.CLS_Clamp_Front_4_Ebable = new PmcBomItem();
            _pmcBom.CLS_Clamp_Front_4_Ebable.Id = "PMC1513";
            _pmcBom.CLS_Clamp_Front_4_Ebable.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Front_4_Ebable.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Front_4_Ebable.Adr = 504;
            _pmcBom.CLS_Clamp_Front_4_Ebable.Bit = 0;

            _pmcBom.CLS_Clamp_Front_4_MoveOutStatus = new PmcBomItem();
            _pmcBom.CLS_Clamp_Front_4_MoveOutStatus.Id = "PMC1514";
            _pmcBom.CLS_Clamp_Front_4_MoveOutStatus.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Front_4_MoveOutStatus.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Front_4_MoveOutStatus.Adr = 504;
            _pmcBom.CLS_Clamp_Front_4_MoveOutStatus.Bit = 1;

            _pmcBom.CLS_Clamp_Front_4_MoveInStatus = new PmcBomItem();
            _pmcBom.CLS_Clamp_Front_4_MoveInStatus.Id = "PMC1515";
            _pmcBom.CLS_Clamp_Front_4_MoveInStatus.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Front_4_MoveInStatus.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Front_4_MoveInStatus.Adr = 504;
            _pmcBom.CLS_Clamp_Front_4_MoveInStatus.Bit = 2;

            _pmcBom.CLS_Clamp_Front_5_Ebable = new PmcBomItem();
            _pmcBom.CLS_Clamp_Front_5_Ebable.Id = "PMC1516";
            _pmcBom.CLS_Clamp_Front_5_Ebable.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Front_5_Ebable.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Front_5_Ebable.Adr = 505;
            _pmcBom.CLS_Clamp_Front_5_Ebable.Bit = 0;

            _pmcBom.CLS_Clamp_Front_5_MoveOutStatus = new PmcBomItem();
            _pmcBom.CLS_Clamp_Front_5_MoveOutStatus.Id = "PMC1517";
            _pmcBom.CLS_Clamp_Front_5_MoveOutStatus.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Front_5_MoveOutStatus.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Front_5_MoveOutStatus.Adr = 505;
            _pmcBom.CLS_Clamp_Front_5_MoveOutStatus.Bit = 1;

            _pmcBom.CLS_Clamp_Front_5_MoveInStatus = new PmcBomItem();
            _pmcBom.CLS_Clamp_Front_5_MoveInStatus.Id = "PMC1518";
            _pmcBom.CLS_Clamp_Front_5_MoveInStatus.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Front_5_MoveInStatus.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Front_5_MoveInStatus.Adr = 505;
            _pmcBom.CLS_Clamp_Front_5_MoveInStatus.Bit = 2;

            _pmcBom.CLS_Clamp_Front_6_Ebable = new PmcBomItem();
            _pmcBom.CLS_Clamp_Front_6_Ebable.Id = "PMC1519";
            _pmcBom.CLS_Clamp_Front_6_Ebable.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Front_6_Ebable.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Front_6_Ebable.Adr = 506;
            _pmcBom.CLS_Clamp_Front_6_Ebable.Bit = 0;

            _pmcBom.CLS_Clamp_Front_6_MoveOutStatus = new PmcBomItem();
            _pmcBom.CLS_Clamp_Front_6_MoveOutStatus.Id = "PMC1520";
            _pmcBom.CLS_Clamp_Front_6_MoveOutStatus.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Front_6_MoveOutStatus.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Front_6_MoveOutStatus.Adr = 506;
            _pmcBom.CLS_Clamp_Front_6_MoveOutStatus.Bit = 1;

            _pmcBom.CLS_Clamp_Front_6_MoveInStatus = new PmcBomItem();
            _pmcBom.CLS_Clamp_Front_6_MoveInStatus.Id = "PMC1521";
            _pmcBom.CLS_Clamp_Front_6_MoveInStatus.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Front_6_MoveInStatus.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Front_6_MoveInStatus.Adr = 506;
            _pmcBom.CLS_Clamp_Front_6_MoveInStatus.Bit = 2;

            _pmcBom.CLS_Clamp_Front_7_Ebable = new PmcBomItem();
            _pmcBom.CLS_Clamp_Front_7_Ebable.Id = "PMC1522";
            _pmcBom.CLS_Clamp_Front_7_Ebable.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Front_7_Ebable.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Front_7_Ebable.Adr = 507;
            _pmcBom.CLS_Clamp_Front_7_Ebable.Bit = 0;

            _pmcBom.CLS_Clamp_Front_7_MoveOutStatus = new PmcBomItem();
            _pmcBom.CLS_Clamp_Front_7_MoveOutStatus.Id = "PMC1523";
            _pmcBom.CLS_Clamp_Front_7_MoveOutStatus.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Front_7_MoveOutStatus.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Front_7_MoveOutStatus.Adr = 507;
            _pmcBom.CLS_Clamp_Front_7_MoveOutStatus.Bit = 1;

            _pmcBom.CLS_Clamp_Front_7_MoveInStatus = new PmcBomItem();
            _pmcBom.CLS_Clamp_Front_7_MoveInStatus.Id = "PMC1524";
            _pmcBom.CLS_Clamp_Front_7_MoveInStatus.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Front_7_MoveInStatus.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Front_7_MoveInStatus.Adr = 507;
            _pmcBom.CLS_Clamp_Front_7_MoveInStatus.Bit = 2;

            _pmcBom.CLS_Clamp_Front_8_Ebable = new PmcBomItem();
            _pmcBom.CLS_Clamp_Front_8_Ebable.Id = "PMC1525";
            _pmcBom.CLS_Clamp_Front_8_Ebable.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Front_8_Ebable.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Front_8_Ebable.Adr = 508;
            _pmcBom.CLS_Clamp_Front_8_Ebable.Bit = 0;

            _pmcBom.CLS_Clamp_Front_8_MoveOutStatus = new PmcBomItem();
            _pmcBom.CLS_Clamp_Front_8_MoveOutStatus.Id = "PMC1526";
            _pmcBom.CLS_Clamp_Front_8_MoveOutStatus.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Front_8_MoveOutStatus.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Front_8_MoveOutStatus.Adr = 508;
            _pmcBom.CLS_Clamp_Front_8_MoveOutStatus.Bit = 1;

            _pmcBom.CLS_Clamp_Front_8_MoveInStatus = new PmcBomItem();
            _pmcBom.CLS_Clamp_Front_8_MoveInStatus.Id = "PMC1527";
            _pmcBom.CLS_Clamp_Front_8_MoveInStatus.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Front_8_MoveInStatus.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Front_8_MoveInStatus.Adr = 508;
            _pmcBom.CLS_Clamp_Front_8_MoveInStatus.Bit = 2;

            _pmcBom.CLS_Clamp_Front_9_Ebable = new PmcBomItem();
            _pmcBom.CLS_Clamp_Front_9_Ebable.Id = "PMC1528";
            _pmcBom.CLS_Clamp_Front_9_Ebable.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Front_9_Ebable.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Front_9_Ebable.Adr = 509;
            _pmcBom.CLS_Clamp_Front_9_Ebable.Bit = 0;

            _pmcBom.CLS_Clamp_Front_9_MoveOutStatus = new PmcBomItem();
            _pmcBom.CLS_Clamp_Front_9_MoveOutStatus.Id = "PMC1529";
            _pmcBom.CLS_Clamp_Front_9_MoveOutStatus.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Front_9_MoveOutStatus.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Front_9_MoveOutStatus.Adr = 509;
            _pmcBom.CLS_Clamp_Front_9_MoveOutStatus.Bit = 1;

            _pmcBom.CLS_Clamp_Front_9_MoveInStatus = new PmcBomItem();
            _pmcBom.CLS_Clamp_Front_9_MoveInStatus.Id = "PMC1530";
            _pmcBom.CLS_Clamp_Front_9_MoveInStatus.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Front_9_MoveInStatus.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Front_9_MoveInStatus.Adr = 509;
            _pmcBom.CLS_Clamp_Front_9_MoveInStatus.Bit = 2;

            _pmcBom.CLS_Clamp_Front_10_Ebable = new PmcBomItem();
            _pmcBom.CLS_Clamp_Front_10_Ebable.Id = "PMC1531";
            _pmcBom.CLS_Clamp_Front_10_Ebable.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Front_10_Ebable.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Front_10_Ebable.Adr = 510;
            _pmcBom.CLS_Clamp_Front_10_Ebable.Bit = 0;

            _pmcBom.CLS_Clamp_Front_10_MoveOutStatus = new PmcBomItem();
            _pmcBom.CLS_Clamp_Front_10_MoveOutStatus.Id = "PMC1532";
            _pmcBom.CLS_Clamp_Front_10_MoveOutStatus.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Front_10_MoveOutStatus.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Front_10_MoveOutStatus.Adr = 510;
            _pmcBom.CLS_Clamp_Front_10_MoveOutStatus.Bit = 1;

            _pmcBom.CLS_Clamp_Front_10_MoveInStatus = new PmcBomItem();
            _pmcBom.CLS_Clamp_Front_10_MoveInStatus.Id = "PMC1533";
            _pmcBom.CLS_Clamp_Front_10_MoveInStatus.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Front_10_MoveInStatus.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Front_10_MoveInStatus.Adr = 510;
            _pmcBom.CLS_Clamp_Front_10_MoveInStatus.Bit = 2;

            _pmcBom.CLS_Clamp_Front_11_Ebable = new PmcBomItem();
            _pmcBom.CLS_Clamp_Front_11_Ebable.Id = "PMC1534";
            _pmcBom.CLS_Clamp_Front_11_Ebable.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Front_11_Ebable.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Front_11_Ebable.Adr = 511;
            _pmcBom.CLS_Clamp_Front_11_Ebable.Bit = 0;

            _pmcBom.CLS_Clamp_Front_11_MoveOutStatus = new PmcBomItem();
            _pmcBom.CLS_Clamp_Front_11_MoveOutStatus.Id = "PMC1535";
            _pmcBom.CLS_Clamp_Front_11_MoveOutStatus.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Front_11_MoveOutStatus.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Front_11_MoveOutStatus.Adr = 511;
            _pmcBom.CLS_Clamp_Front_11_MoveOutStatus.Bit = 1;

            _pmcBom.CLS_Clamp_Front_11_MoveInStatus = new PmcBomItem();
            _pmcBom.CLS_Clamp_Front_11_MoveInStatus.Id = "PMC1536";
            _pmcBom.CLS_Clamp_Front_11_MoveInStatus.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Front_11_MoveInStatus.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Front_11_MoveInStatus.Adr = 511;
            _pmcBom.CLS_Clamp_Front_11_MoveInStatus.Bit = 2;

            _pmcBom.CLS_Clamp_Front_12_Ebable = new PmcBomItem();
            _pmcBom.CLS_Clamp_Front_12_Ebable.Id = "PMC1537";
            _pmcBom.CLS_Clamp_Front_12_Ebable.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Front_12_Ebable.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Front_12_Ebable.Adr = 512;
            _pmcBom.CLS_Clamp_Front_12_Ebable.Bit = 0;

            _pmcBom.CLS_Clamp_Front_12_MoveOutStatus = new PmcBomItem();
            _pmcBom.CLS_Clamp_Front_12_MoveOutStatus.Id = "PMC1538";
            _pmcBom.CLS_Clamp_Front_12_MoveOutStatus.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Front_12_MoveOutStatus.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Front_12_MoveOutStatus.Adr = 512;
            _pmcBom.CLS_Clamp_Front_12_MoveOutStatus.Bit = 1;

            _pmcBom.CLS_Clamp_Front_12_MoveInStatus = new PmcBomItem();
            _pmcBom.CLS_Clamp_Front_12_MoveInStatus.Id = "PMC1539";
            _pmcBom.CLS_Clamp_Front_12_MoveInStatus.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Front_12_MoveInStatus.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Front_12_MoveInStatus.Adr = 512;
            _pmcBom.CLS_Clamp_Front_12_MoveInStatus.Bit = 2;

            _pmcBom.CLS_Clamp_Front_13_Ebable = new PmcBomItem();
            _pmcBom.CLS_Clamp_Front_13_Ebable.Id = "PMC1540";
            _pmcBom.CLS_Clamp_Front_13_Ebable.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Front_13_Ebable.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Front_13_Ebable.Adr = 513;
            _pmcBom.CLS_Clamp_Front_13_Ebable.Bit = 0;

            _pmcBom.CLS_Clamp_Front_13_MoveOutStatus = new PmcBomItem();
            _pmcBom.CLS_Clamp_Front_13_MoveOutStatus.Id = "PMC1541";
            _pmcBom.CLS_Clamp_Front_13_MoveOutStatus.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Front_13_MoveOutStatus.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Front_13_MoveOutStatus.Adr = 513;
            _pmcBom.CLS_Clamp_Front_13_MoveOutStatus.Bit = 1;

            _pmcBom.CLS_Clamp_Front_13_MoveInStatus = new PmcBomItem();
            _pmcBom.CLS_Clamp_Front_13_MoveInStatus.Id = "PMC1542";
            _pmcBom.CLS_Clamp_Front_13_MoveInStatus.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Front_13_MoveInStatus.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Front_13_MoveInStatus.Adr = 513;
            _pmcBom.CLS_Clamp_Front_13_MoveInStatus.Bit = 2;

            _pmcBom.CLS_Clamp_Back_1_Ebable = new PmcBomItem();
            _pmcBom.CLS_Clamp_Back_1_Ebable.Id = "PMC1543";
            _pmcBom.CLS_Clamp_Back_1_Ebable.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Back_1_Ebable.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Back_1_Ebable.Adr = 514;
            _pmcBom.CLS_Clamp_Back_1_Ebable.Bit = 0;

            _pmcBom.CLS_Clamp_Back_1_MoveOutStatus = new PmcBomItem();
            _pmcBom.CLS_Clamp_Back_1_MoveOutStatus.Id = "PMC1544";
            _pmcBom.CLS_Clamp_Back_1_MoveOutStatus.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Back_1_MoveOutStatus.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Back_1_MoveOutStatus.Adr = 514;
            _pmcBom.CLS_Clamp_Back_1_MoveOutStatus.Bit = 1;

            _pmcBom.CLS_Clamp_Back_1_MoveInStatus = new PmcBomItem();
            _pmcBom.CLS_Clamp_Back_1_MoveInStatus.Id = "PMC1545";
            _pmcBom.CLS_Clamp_Back_1_MoveInStatus.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Back_1_MoveInStatus.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Back_1_MoveInStatus.Adr = 514;
            _pmcBom.CLS_Clamp_Back_1_MoveInStatus.Bit = 2;

            _pmcBom.CLS_Clamp_Back_2_Ebable = new PmcBomItem();
            _pmcBom.CLS_Clamp_Back_2_Ebable.Id = "PMC1546";
            _pmcBom.CLS_Clamp_Back_2_Ebable.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Back_2_Ebable.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Back_2_Ebable.Adr = 515;
            _pmcBom.CLS_Clamp_Back_2_Ebable.Bit = 0;

            _pmcBom.CLS_Clamp_Back_2_MoveOutStatus = new PmcBomItem();
            _pmcBom.CLS_Clamp_Back_2_MoveOutStatus.Id = "PMC1547";
            _pmcBom.CLS_Clamp_Back_2_MoveOutStatus.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Back_2_MoveOutStatus.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Back_2_MoveOutStatus.Adr = 515;
            _pmcBom.CLS_Clamp_Back_2_MoveOutStatus.Bit = 1;

            _pmcBom.CLS_Clamp_Back_2_MoveInStatus = new PmcBomItem();
            _pmcBom.CLS_Clamp_Back_2_MoveInStatus.Id = "PMC1548";
            _pmcBom.CLS_Clamp_Back_2_MoveInStatus.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Back_2_MoveInStatus.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Back_2_MoveInStatus.Adr = 515;
            _pmcBom.CLS_Clamp_Back_2_MoveInStatus.Bit = 2;

            _pmcBom.CLS_Clamp_Back_3_Ebable = new PmcBomItem();
            _pmcBom.CLS_Clamp_Back_3_Ebable.Id = "PMC1549";
            _pmcBom.CLS_Clamp_Back_3_Ebable.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Back_3_Ebable.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Back_3_Ebable.Adr = 516;
            _pmcBom.CLS_Clamp_Back_3_Ebable.Bit = 0;

            _pmcBom.CLS_Clamp_Back_3_MoveOutStatus = new PmcBomItem();
            _pmcBom.CLS_Clamp_Back_3_MoveOutStatus.Id = "PMC1550";
            _pmcBom.CLS_Clamp_Back_3_MoveOutStatus.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Back_3_MoveOutStatus.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Back_3_MoveOutStatus.Adr = 516;
            _pmcBom.CLS_Clamp_Back_3_MoveOutStatus.Bit = 1;

            _pmcBom.CLS_Clamp_Back_3_MoveInStatus = new PmcBomItem();
            _pmcBom.CLS_Clamp_Back_3_MoveInStatus.Id = "PMC1551";
            _pmcBom.CLS_Clamp_Back_3_MoveInStatus.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Back_3_MoveInStatus.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Back_3_MoveInStatus.Adr = 516;
            _pmcBom.CLS_Clamp_Back_3_MoveInStatus.Bit = 2;

            _pmcBom.CLS_Clamp_Back_4_Ebable = new PmcBomItem();
            _pmcBom.CLS_Clamp_Back_4_Ebable.Id = "PMC1552";
            _pmcBom.CLS_Clamp_Back_4_Ebable.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Back_4_Ebable.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Back_4_Ebable.Adr = 517;
            _pmcBom.CLS_Clamp_Back_4_Ebable.Bit = 0;

            _pmcBom.CLS_Clamp_Back_4_MoveOutStatus = new PmcBomItem();
            _pmcBom.CLS_Clamp_Back_4_MoveOutStatus.Id = "PMC1553";
            _pmcBom.CLS_Clamp_Back_4_MoveOutStatus.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Back_4_MoveOutStatus.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Back_4_MoveOutStatus.Adr = 517;
            _pmcBom.CLS_Clamp_Back_4_MoveOutStatus.Bit = 1;

            _pmcBom.CLS_Clamp_Back_4_MoveInStatus = new PmcBomItem();
            _pmcBom.CLS_Clamp_Back_4_MoveInStatus.Id = "PMC1554";
            _pmcBom.CLS_Clamp_Back_4_MoveInStatus.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Back_4_MoveInStatus.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Back_4_MoveInStatus.Adr = 517;
            _pmcBom.CLS_Clamp_Back_4_MoveInStatus.Bit = 2;

            _pmcBom.CLS_Clamp_Back_5_Ebable = new PmcBomItem();
            _pmcBom.CLS_Clamp_Back_5_Ebable.Id = "PMC1555";
            _pmcBom.CLS_Clamp_Back_5_Ebable.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Back_5_Ebable.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Back_5_Ebable.Adr = 518;
            _pmcBom.CLS_Clamp_Back_5_Ebable.Bit = 0;

            _pmcBom.CLS_Clamp_Back_5_MoveOutStatus = new PmcBomItem();
            _pmcBom.CLS_Clamp_Back_5_MoveOutStatus.Id = "PMC1556";
            _pmcBom.CLS_Clamp_Back_5_MoveOutStatus.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Back_5_MoveOutStatus.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Back_5_MoveOutStatus.Adr = 518;
            _pmcBom.CLS_Clamp_Back_5_MoveOutStatus.Bit = 1;

            _pmcBom.CLS_Clamp_Back_5_MoveInStatus = new PmcBomItem();
            _pmcBom.CLS_Clamp_Back_5_MoveInStatus.Id = "PMC1557";
            _pmcBom.CLS_Clamp_Back_5_MoveInStatus.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Back_5_MoveInStatus.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Back_5_MoveInStatus.Adr = 518;
            _pmcBom.CLS_Clamp_Back_5_MoveInStatus.Bit = 2;

            _pmcBom.CLS_Clamp_Back_6_Ebable = new PmcBomItem();
            _pmcBom.CLS_Clamp_Back_6_Ebable.Id = "PMC1558";
            _pmcBom.CLS_Clamp_Back_6_Ebable.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Back_6_Ebable.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Back_6_Ebable.Adr = 519;
            _pmcBom.CLS_Clamp_Back_6_Ebable.Bit = 0;

            _pmcBom.CLS_Clamp_Back_6_MoveOutStatus = new PmcBomItem();
            _pmcBom.CLS_Clamp_Back_6_MoveOutStatus.Id = "PMC1559";
            _pmcBom.CLS_Clamp_Back_6_MoveOutStatus.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Back_6_MoveOutStatus.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Back_6_MoveOutStatus.Adr = 519;
            _pmcBom.CLS_Clamp_Back_6_MoveOutStatus.Bit = 1;

            _pmcBom.CLS_Clamp_Back_6_MoveInStatus = new PmcBomItem();
            _pmcBom.CLS_Clamp_Back_6_MoveInStatus.Id = "PMC1560";
            _pmcBom.CLS_Clamp_Back_6_MoveInStatus.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Back_6_MoveInStatus.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Back_6_MoveInStatus.Adr = 519;
            _pmcBom.CLS_Clamp_Back_6_MoveInStatus.Bit = 2;

            _pmcBom.CLS_Clamp_Back_7_Ebable = new PmcBomItem();
            _pmcBom.CLS_Clamp_Back_7_Ebable.Id = "PMC1561";
            _pmcBom.CLS_Clamp_Back_7_Ebable.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Back_7_Ebable.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Back_7_Ebable.Adr = 520;
            _pmcBom.CLS_Clamp_Back_7_Ebable.Bit = 0;

            _pmcBom.CLS_Clamp_Back_7_MoveOutStatus = new PmcBomItem();
            _pmcBom.CLS_Clamp_Back_7_MoveOutStatus.Id = "PMC1562";
            _pmcBom.CLS_Clamp_Back_7_MoveOutStatus.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Back_7_MoveOutStatus.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Back_7_MoveOutStatus.Adr = 520;
            _pmcBom.CLS_Clamp_Back_7_MoveOutStatus.Bit = 1;

            _pmcBom.CLS_Clamp_Back_7_MoveInStatus = new PmcBomItem();
            _pmcBom.CLS_Clamp_Back_7_MoveInStatus.Id = "PMC1563";
            _pmcBom.CLS_Clamp_Back_7_MoveInStatus.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Back_7_MoveInStatus.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Back_7_MoveInStatus.Adr = 520;
            _pmcBom.CLS_Clamp_Back_7_MoveInStatus.Bit = 2;

            _pmcBom.CLS_Clamp_Back_8_Ebable = new PmcBomItem();
            _pmcBom.CLS_Clamp_Back_8_Ebable.Id = "PMC1564";
            _pmcBom.CLS_Clamp_Back_8_Ebable.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Back_8_Ebable.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Back_8_Ebable.Adr = 521;
            _pmcBom.CLS_Clamp_Back_8_Ebable.Bit = 0;

            _pmcBom.CLS_Clamp_Back_8_MoveOutStatus = new PmcBomItem();
            _pmcBom.CLS_Clamp_Back_8_MoveOutStatus.Id = "PMC1565";
            _pmcBom.CLS_Clamp_Back_8_MoveOutStatus.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Back_8_MoveOutStatus.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Back_8_MoveOutStatus.Adr = 521;
            _pmcBom.CLS_Clamp_Back_8_MoveOutStatus.Bit = 1;

            _pmcBom.CLS_Clamp_Back_8_MoveInStatus = new PmcBomItem();
            _pmcBom.CLS_Clamp_Back_8_MoveInStatus.Id = "PMC1566";
            _pmcBom.CLS_Clamp_Back_8_MoveInStatus.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Back_8_MoveInStatus.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Back_8_MoveInStatus.Adr = 521;
            _pmcBom.CLS_Clamp_Back_8_MoveInStatus.Bit = 2;

            _pmcBom.CLS_Clamp_Back_9_Ebable = new PmcBomItem();
            _pmcBom.CLS_Clamp_Back_9_Ebable.Id = "PMC1567";
            _pmcBom.CLS_Clamp_Back_9_Ebable.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Back_9_Ebable.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Back_9_Ebable.Adr = 522;
            _pmcBom.CLS_Clamp_Back_9_Ebable.Bit = 0;

            _pmcBom.CLS_Clamp_Back_9_MoveOutStatus = new PmcBomItem();
            _pmcBom.CLS_Clamp_Back_9_MoveOutStatus.Id = "PMC1568";
            _pmcBom.CLS_Clamp_Back_9_MoveOutStatus.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Back_9_MoveOutStatus.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Back_9_MoveOutStatus.Adr = 522;
            _pmcBom.CLS_Clamp_Back_9_MoveOutStatus.Bit = 1;

            _pmcBom.CLS_Clamp_Back_9_MoveInStatus = new PmcBomItem();
            _pmcBom.CLS_Clamp_Back_9_MoveInStatus.Id = "PMC1569";
            _pmcBom.CLS_Clamp_Back_9_MoveInStatus.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Back_9_MoveInStatus.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Back_9_MoveInStatus.Adr = 522;
            _pmcBom.CLS_Clamp_Back_9_MoveInStatus.Bit = 2;

            _pmcBom.CLS_Clamp_Back_10_Ebable = new PmcBomItem();
            _pmcBom.CLS_Clamp_Back_10_Ebable.Id = "PMC1570";
            _pmcBom.CLS_Clamp_Back_10_Ebable.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Back_10_Ebable.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Back_10_Ebable.Adr = 523;
            _pmcBom.CLS_Clamp_Back_10_Ebable.Bit = 0;

            _pmcBom.CLS_Clamp_Back_10_MoveOutStatus = new PmcBomItem();
            _pmcBom.CLS_Clamp_Back_10_MoveOutStatus.Id = "PMC1571";
            _pmcBom.CLS_Clamp_Back_10_MoveOutStatus.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Back_10_MoveOutStatus.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Back_10_MoveOutStatus.Adr = 523;
            _pmcBom.CLS_Clamp_Back_10_MoveOutStatus.Bit = 1;

            _pmcBom.CLS_Clamp_Back_10_MoveInStatus = new PmcBomItem();
            _pmcBom.CLS_Clamp_Back_10_MoveInStatus.Id = "PMC1572";
            _pmcBom.CLS_Clamp_Back_10_MoveInStatus.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Back_10_MoveInStatus.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Back_10_MoveInStatus.Adr = 523;
            _pmcBom.CLS_Clamp_Back_10_MoveInStatus.Bit = 2;

            _pmcBom.CLS_Clamp_Back_11_Ebable = new PmcBomItem();
            _pmcBom.CLS_Clamp_Back_11_Ebable.Id = "PMC1573";
            _pmcBom.CLS_Clamp_Back_11_Ebable.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Back_11_Ebable.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Back_11_Ebable.Adr = 524;
            _pmcBom.CLS_Clamp_Back_11_Ebable.Bit = 0;

            _pmcBom.CLS_Clamp_Back_11_MoveOutStatus = new PmcBomItem();
            _pmcBom.CLS_Clamp_Back_11_MoveOutStatus.Id = "PMC1574";
            _pmcBom.CLS_Clamp_Back_11_MoveOutStatus.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Back_11_MoveOutStatus.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Back_11_MoveOutStatus.Adr = 524;
            _pmcBom.CLS_Clamp_Back_11_MoveOutStatus.Bit = 1;

            _pmcBom.CLS_Clamp_Back_11_MoveInStatus = new PmcBomItem();
            _pmcBom.CLS_Clamp_Back_11_MoveInStatus.Id = "PMC1575";
            _pmcBom.CLS_Clamp_Back_11_MoveInStatus.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Back_11_MoveInStatus.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Back_11_MoveInStatus.Adr = 524;
            _pmcBom.CLS_Clamp_Back_11_MoveInStatus.Bit = 2;

            _pmcBom.CLS_Clamp_Back_12_Ebable = new PmcBomItem();
            _pmcBom.CLS_Clamp_Back_12_Ebable.Id = "PMC1576";
            _pmcBom.CLS_Clamp_Back_12_Ebable.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Back_12_Ebable.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Back_12_Ebable.Adr = 525;
            _pmcBom.CLS_Clamp_Back_12_Ebable.Bit = 0;

            _pmcBom.CLS_Clamp_Back_12_MoveOutStatus = new PmcBomItem();
            _pmcBom.CLS_Clamp_Back_12_MoveOutStatus.Id = "PMC1577";
            _pmcBom.CLS_Clamp_Back_12_MoveOutStatus.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Back_12_MoveOutStatus.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Back_12_MoveOutStatus.Adr = 525;
            _pmcBom.CLS_Clamp_Back_12_MoveOutStatus.Bit = 1;

            _pmcBom.CLS_Clamp_Back_12_MoveInStatus = new PmcBomItem();
            _pmcBom.CLS_Clamp_Back_12_MoveInStatus.Id = "PMC1578";
            _pmcBom.CLS_Clamp_Back_12_MoveInStatus.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Back_12_MoveInStatus.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Back_12_MoveInStatus.Adr = 525;
            _pmcBom.CLS_Clamp_Back_12_MoveInStatus.Bit = 2;

            _pmcBom.CLS_Clamp_Back_13_Ebable = new PmcBomItem();
            _pmcBom.CLS_Clamp_Back_13_Ebable.Id = "PMC1579";
            _pmcBom.CLS_Clamp_Back_13_Ebable.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Back_13_Ebable.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Back_13_Ebable.Adr = 526;
            _pmcBom.CLS_Clamp_Back_13_Ebable.Bit = 0;

            _pmcBom.CLS_Clamp_Back_13_MoveOutStatus = new PmcBomItem();
            _pmcBom.CLS_Clamp_Back_13_MoveOutStatus.Id = "PMC1580";
            _pmcBom.CLS_Clamp_Back_13_MoveOutStatus.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Back_13_MoveOutStatus.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Back_13_MoveOutStatus.Adr = 526;
            _pmcBom.CLS_Clamp_Back_13_MoveOutStatus.Bit = 1;

            _pmcBom.CLS_Clamp_Back_13_MoveInStatus = new PmcBomItem();
            _pmcBom.CLS_Clamp_Back_13_MoveInStatus.Id = "PMC1581";
            _pmcBom.CLS_Clamp_Back_13_MoveInStatus.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CLS_Clamp_Back_13_MoveInStatus.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CLS_Clamp_Back_13_MoveInStatus.Adr = 526;
            _pmcBom.CLS_Clamp_Back_13_MoveInStatus.Bit = 2;
            #endregion

            #region 自动化气源
            _pmcBom.AAS_StartPos_1 = new PmcBomItem();
            _pmcBom.AAS_StartPos_1.Id = "PMC1600";
            _pmcBom.AAS_StartPos_1.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.AAS_StartPos_1.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.AAS_StartPos_1.Adr = 300;
            _pmcBom.AAS_StartPos_1.IsRecipes = true;

            _pmcBom.AAS_StartArr_1 = new PmcBomItem();
            _pmcBom.AAS_StartArr_1.Id = "PMC1601";
            _pmcBom.AAS_StartArr_1.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.AAS_StartArr_1.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.AAS_StartArr_1.Adr = 308;
            _pmcBom.AAS_StartArr_1.Bit = 1;
            _pmcBom.AAS_StartArr_1.IsRecipes = true;

            _pmcBom.AAS_EndPos_1 = new PmcBomItem();
            _pmcBom.AAS_EndPos_1.Id = "PMC1602";
            _pmcBom.AAS_EndPos_1.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.AAS_EndPos_1.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.AAS_EndPos_1.Adr = 304;
            _pmcBom.AAS_EndPos_1.IsRecipes = true;

            _pmcBom.AAS_EndArr_1 = new PmcBomItem();
            _pmcBom.AAS_EndArr_1.Id = "PMC1603";
            _pmcBom.AAS_EndArr_1.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.AAS_EndArr_1.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.AAS_EndArr_1.Adr = 308;
            _pmcBom.AAS_EndArr_1.Bit = 2;
            _pmcBom.AAS_EndArr_1.IsRecipes = true;

            _pmcBom.AAS_ActionFlag_1 = new PmcBomItem();
            _pmcBom.AAS_ActionFlag_1.Id = "PMC1604";
            _pmcBom.AAS_ActionFlag_1.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.AAS_ActionFlag_1.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.AAS_ActionFlag_1.Adr = 308;
            _pmcBom.AAS_ActionFlag_1.Bit = 3;
            _pmcBom.AAS_ActionFlag_1.IsRecipes = true;

            _pmcBom.AAS_Enable_1 = new PmcBomItem();
            _pmcBom.AAS_Enable_1.Id = "PMC1630";
            _pmcBom.AAS_Enable_1.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.AAS_Enable_1.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.AAS_Enable_1.Adr = 308;
            _pmcBom.AAS_Enable_1.Bit = 4;
            _pmcBom.AAS_Enable_1.IsRecipes = true;

            _pmcBom.AAS_StartPos_2 = new PmcBomItem();
            _pmcBom.AAS_StartPos_2.Id = "PMC1605";
            _pmcBom.AAS_StartPos_2.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.AAS_StartPos_2.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.AAS_StartPos_2.Adr = 310;
            _pmcBom.AAS_StartPos_2.IsRecipes = true;

            _pmcBom.AAS_StartArr_2 = new PmcBomItem();
            _pmcBom.AAS_StartArr_2.Id = "PMC1606";
            _pmcBom.AAS_StartArr_2.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.AAS_StartArr_2.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.AAS_StartArr_2.Adr = 318;
            _pmcBom.AAS_StartArr_2.Bit = 1;
            _pmcBom.AAS_StartArr_2.IsRecipes = true;

            _pmcBom.AAS_EndPos_2 = new PmcBomItem();
            _pmcBom.AAS_EndPos_2.Id = "PMC1607";
            _pmcBom.AAS_EndPos_2.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.AAS_EndPos_2.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.AAS_EndPos_2.Adr = 314;
            _pmcBom.AAS_EndPos_2.IsRecipes = true;

            _pmcBom.AAS_EndArr_2 = new PmcBomItem();
            _pmcBom.AAS_EndArr_2.Id = "PMC1608";
            _pmcBom.AAS_EndArr_2.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.AAS_EndArr_2.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.AAS_EndArr_2.Adr = 318;
            _pmcBom.AAS_EndArr_2.Bit = 2;
            _pmcBom.AAS_EndArr_2.IsRecipes = true;

            _pmcBom.AAS_ActionFlag_2 = new PmcBomItem();
            _pmcBom.AAS_ActionFlag_2.Id = "PMC1609";
            _pmcBom.AAS_ActionFlag_2.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.AAS_ActionFlag_2.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.AAS_ActionFlag_2.Adr = 318;
            _pmcBom.AAS_ActionFlag_2.Bit = 3;
            _pmcBom.AAS_ActionFlag_2.IsRecipes = true;

            _pmcBom.AAS_Enable_2 = new PmcBomItem();
            _pmcBom.AAS_Enable_2.Id = "PMC1631";
            _pmcBom.AAS_Enable_2.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.AAS_Enable_2.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.AAS_Enable_2.Adr = 318;
            _pmcBom.AAS_Enable_2.Bit = 4;
            _pmcBom.AAS_Enable_2.IsRecipes = true;

            _pmcBom.AAS_StartPos_3 = new PmcBomItem();
            _pmcBom.AAS_StartPos_3.Id = "PMC1610";
            _pmcBom.AAS_StartPos_3.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.AAS_StartPos_3.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.AAS_StartPos_3.Adr = 320;
            _pmcBom.AAS_StartPos_3.IsRecipes = true;

            _pmcBom.AAS_StartArr_3 = new PmcBomItem();
            _pmcBom.AAS_StartArr_3.Id = "PMC1611";
            _pmcBom.AAS_StartArr_3.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.AAS_StartArr_3.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.AAS_StartArr_3.Adr = 328;
            _pmcBom.AAS_StartArr_3.Bit = 1;
            _pmcBom.AAS_StartArr_3.IsRecipes = true;

            _pmcBom.AAS_EndPos_3 = new PmcBomItem();
            _pmcBom.AAS_EndPos_3.Id = "PMC1612";
            _pmcBom.AAS_EndPos_3.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.AAS_EndPos_3.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.AAS_EndPos_3.Adr = 324;
            _pmcBom.AAS_EndPos_3.IsRecipes = true;

            _pmcBom.AAS_EndArr_3 = new PmcBomItem();
            _pmcBom.AAS_EndArr_3.Id = "PMC1613";
            _pmcBom.AAS_EndArr_3.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.AAS_EndArr_3.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.AAS_EndArr_3.Adr = 328;
            _pmcBom.AAS_EndArr_3.Bit = 2;
            _pmcBom.AAS_EndArr_3.IsRecipes = true;

            _pmcBom.AAS_ActionFlag_3 = new PmcBomItem();
            _pmcBom.AAS_ActionFlag_3.Id = "PMC1614";
            _pmcBom.AAS_ActionFlag_3.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.AAS_ActionFlag_3.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.AAS_ActionFlag_3.Adr = 328;
            _pmcBom.AAS_ActionFlag_3.Bit = 3;
            _pmcBom.AAS_ActionFlag_3.IsRecipes = true;

            _pmcBom.AAS_Enable_3 = new PmcBomItem();
            _pmcBom.AAS_Enable_3.Id = "PMC1632";
            _pmcBom.AAS_Enable_3.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.AAS_Enable_3.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.AAS_Enable_3.Adr = 328;
            _pmcBom.AAS_Enable_3.Bit = 4;
            _pmcBom.AAS_Enable_3.IsRecipes = true;

            _pmcBom.AAS_StartPos_4 = new PmcBomItem();
            _pmcBom.AAS_StartPos_4.Id = "PMC1615";
            _pmcBom.AAS_StartPos_4.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.AAS_StartPos_4.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.AAS_StartPos_4.Adr = 330;
            _pmcBom.AAS_StartPos_4.IsRecipes = true;

            _pmcBom.AAS_StartArr_4 = new PmcBomItem();
            _pmcBom.AAS_StartArr_4.Id = "PMC1616";
            _pmcBom.AAS_StartArr_4.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.AAS_StartArr_4.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.AAS_StartArr_4.Adr = 338;
            _pmcBom.AAS_StartArr_4.Bit = 1;
            _pmcBom.AAS_StartArr_4.IsRecipes = true;

            _pmcBom.AAS_EndPos_4 = new PmcBomItem();
            _pmcBom.AAS_EndPos_4.Id = "PMC1617";
            _pmcBom.AAS_EndPos_4.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.AAS_EndPos_4.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.AAS_EndPos_4.Adr = 334;
            _pmcBom.AAS_EndPos_4.IsRecipes = true;

            _pmcBom.AAS_EndArr_4 = new PmcBomItem();
            _pmcBom.AAS_EndArr_4.Id = "PMC1618";
            _pmcBom.AAS_EndArr_4.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.AAS_EndArr_4.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.AAS_EndArr_4.Adr = 338;
            _pmcBom.AAS_EndArr_4.Bit = 2;
            _pmcBom.AAS_EndArr_4.IsRecipes = true;

            _pmcBom.AAS_ActionFlag_4 = new PmcBomItem();
            _pmcBom.AAS_ActionFlag_4.Id = "PMC1619";
            _pmcBom.AAS_ActionFlag_4.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.AAS_ActionFlag_4.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.AAS_ActionFlag_4.Adr = 338;
            _pmcBom.AAS_ActionFlag_4.Bit = 3;
            _pmcBom.AAS_ActionFlag_4.IsRecipes = true;

            _pmcBom.AAS_Enable_4 = new PmcBomItem();
            _pmcBom.AAS_Enable_4.Id = "PMC1633";
            _pmcBom.AAS_Enable_4.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.AAS_Enable_4.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.AAS_Enable_4.Adr = 338;
            _pmcBom.AAS_Enable_4.Bit = 4;
            _pmcBom.AAS_Enable_4.IsRecipes = true;

            #endregion

            #region 液压垫控制
            _pmcBom.HDC_StartPos_1 = new PmcBomItem();
            _pmcBom.HDC_StartPos_1.Id = "PMC1700";
            _pmcBom.HDC_StartPos_1.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.HDC_StartPos_1.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.HDC_StartPos_1.Adr = 340;
            _pmcBom.HDC_StartPos_1.ConversionFactor = 100;
            _pmcBom.HDC_StartPos_1.IsRecipes = true;

            _pmcBom.HDC_StartArr_1 = new PmcBomItem();
            _pmcBom.HDC_StartArr_1.Id = "PMC1701";
            _pmcBom.HDC_StartArr_1.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.HDC_StartArr_1.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.HDC_StartArr_1.Adr = 348;
            _pmcBom.HDC_StartArr_1.Bit = 1;
            _pmcBom.HDC_StartArr_1.IsRecipes = true;

            _pmcBom.HDC_EndPos_1 = new PmcBomItem();
            _pmcBom.HDC_EndPos_1.Id = "PMC1702";
            _pmcBom.HDC_EndPos_1.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.HDC_EndPos_1.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.HDC_EndPos_1.Adr = 344;
            _pmcBom.HDC_EndPos_1.ConversionFactor = 100;
            _pmcBom.HDC_EndPos_1.IsRecipes = true;

            _pmcBom.HDC_EndArr_1 = new PmcBomItem();
            _pmcBom.HDC_EndArr_1.Id = "PMC1703";
            _pmcBom.HDC_EndArr_1.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.HDC_EndArr_1.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.HDC_EndArr_1.Adr = 348;
            _pmcBom.HDC_EndArr_1.Bit = 2;
            _pmcBom.HDC_EndArr_1.IsRecipes = true;

            _pmcBom.HDC_ActionFlag_1 = new PmcBomItem();
            _pmcBom.HDC_ActionFlag_1.Id = "PMC1704";
            _pmcBom.HDC_ActionFlag_1.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.HDC_ActionFlag_1.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.HDC_ActionFlag_1.Adr = 348;
            _pmcBom.HDC_ActionFlag_1.Bit = 3;
            _pmcBom.HDC_ActionFlag_1.IsRecipes = true;

            _pmcBom.HDC_Enable_1 = new PmcBomItem();
            _pmcBom.HDC_Enable_1.Id = "PMC1730";
            _pmcBom.HDC_Enable_1.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.HDC_Enable_1.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.HDC_Enable_1.Adr = 348;
            _pmcBom.HDC_Enable_1.Bit = 4;
            _pmcBom.HDC_Enable_1.IsRecipes = true;

            _pmcBom.HDC_StartPos_2 = new PmcBomItem();
            _pmcBom.HDC_StartPos_2.Id = "PMC1705";
            _pmcBom.HDC_StartPos_2.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.HDC_StartPos_2.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.HDC_StartPos_2.Adr = 350;
            _pmcBom.HDC_StartPos_2.ConversionFactor = 100;
            _pmcBom.HDC_StartPos_2.IsRecipes = true;

            _pmcBom.HDC_StartArr_2 = new PmcBomItem();
            _pmcBom.HDC_StartArr_2.Id = "PMC1706";
            _pmcBom.HDC_StartArr_2.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.HDC_StartArr_2.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.HDC_StartArr_2.Adr = 358;
            _pmcBom.HDC_StartArr_2.Bit = 1;
            _pmcBom.HDC_StartArr_2.IsRecipes = true;

            _pmcBom.HDC_EndPos_2 = new PmcBomItem();
            _pmcBom.HDC_EndPos_2.Id = "PMC1707";
            _pmcBom.HDC_EndPos_2.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.HDC_EndPos_2.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.HDC_EndPos_2.Adr = 354;
            _pmcBom.HDC_EndPos_2.ConversionFactor = 100;
            _pmcBom.HDC_EndPos_2.IsRecipes = true;

            _pmcBom.HDC_EndArr_2 = new PmcBomItem();
            _pmcBom.HDC_EndArr_2.Id = "PMC1708";
            _pmcBom.HDC_EndArr_2.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.HDC_EndArr_2.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.HDC_EndArr_2.Adr = 358;
            _pmcBom.HDC_EndArr_2.Bit = 2;
            _pmcBom.HDC_EndArr_2.IsRecipes = true;

            _pmcBom.HDC_ActionFlag_2 = new PmcBomItem();
            _pmcBom.HDC_ActionFlag_2.Id = "PMC1709";
            _pmcBom.HDC_ActionFlag_2.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.HDC_ActionFlag_2.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.HDC_ActionFlag_2.Adr = 358;
            _pmcBom.HDC_ActionFlag_2.Bit = 3;
            _pmcBom.HDC_ActionFlag_2.IsRecipes = true;

            _pmcBom.HDC_Enable_2 = new PmcBomItem();
            _pmcBom.HDC_Enable_2.Id = "PMC1731";
            _pmcBom.HDC_Enable_2.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.HDC_Enable_2.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.HDC_Enable_2.Adr = 358;
            _pmcBom.HDC_Enable_2.Bit = 4;
            _pmcBom.HDC_Enable_2.IsRecipes = true;

            #endregion

            #region 备用凸轮
            _pmcBom.CAM_StartPos_1 = new PmcBomItem();
            _pmcBom.CAM_StartPos_1.Id = "PMC1800";
            _pmcBom.CAM_StartPos_1.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CAM_StartPos_1.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.CAM_StartPos_1.Adr = 360;
            _pmcBom.CAM_StartPos_1.ConversionFactor = 100;
            _pmcBom.CAM_StartPos_1.IsRecipes = true;

            _pmcBom.CAM_StartArr_1 = new PmcBomItem();
            _pmcBom.CAM_StartArr_1.Id = "PMC1801";
            _pmcBom.CAM_StartArr_1.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CAM_StartArr_1.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CAM_StartArr_1.Adr = 368;
            _pmcBom.CAM_StartArr_1.Bit = 1;
            _pmcBom.CAM_StartArr_1.IsRecipes = true;

            _pmcBom.CAM_EndPos_1 = new PmcBomItem();
            _pmcBom.CAM_EndPos_1.Id = "PMC1802";
            _pmcBom.CAM_EndPos_1.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CAM_EndPos_1.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.CAM_EndPos_1.Adr = 364;
            _pmcBom.CAM_EndPos_1.ConversionFactor = 100;
            _pmcBom.CAM_EndPos_1.IsRecipes = true;

            _pmcBom.CAM_EndArr_1 = new PmcBomItem();
            _pmcBom.CAM_EndArr_1.Id = "PMC1803";
            _pmcBom.CAM_EndArr_1.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CAM_EndArr_1.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CAM_EndArr_1.Adr = 368;
            _pmcBom.CAM_EndArr_1.Bit = 2;
            _pmcBom.CAM_EndArr_1.IsRecipes = true;

            _pmcBom.CAM_ActionFlag_1 = new PmcBomItem();
            _pmcBom.CAM_ActionFlag_1.Id = "PMC1804";
            _pmcBom.CAM_ActionFlag_1.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CAM_ActionFlag_1.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CAM_ActionFlag_1.Adr = 368;
            _pmcBom.CAM_ActionFlag_1.Bit = 3;
            _pmcBom.CAM_ActionFlag_1.IsRecipes = true;

            _pmcBom.CAM_Enable_1 = new PmcBomItem();
            _pmcBom.CAM_Enable_1.Id = "PMC1830";
            _pmcBom.CAM_Enable_1.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CAM_Enable_1.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CAM_Enable_1.Adr = 368;
            _pmcBom.CAM_Enable_1.Bit = 4;
            _pmcBom.CAM_Enable_1.IsRecipes = true;

            _pmcBom.CAM_StartPos_2 = new PmcBomItem();
            _pmcBom.CAM_StartPos_2.Id = "PMC1805";
            _pmcBom.CAM_StartPos_2.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CAM_StartPos_2.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.CAM_StartPos_2.Adr = 370;
            _pmcBom.CAM_StartPos_2.ConversionFactor = 100;
            _pmcBom.CAM_StartPos_2.IsRecipes = true;

            _pmcBom.CAM_StartArr_2 = new PmcBomItem();
            _pmcBom.CAM_StartArr_2.Id = "PMC1806";
            _pmcBom.CAM_StartArr_2.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CAM_StartArr_2.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CAM_StartArr_2.Adr = 378;
            _pmcBom.CAM_StartArr_2.Bit = 1;
            _pmcBom.CAM_StartArr_2.IsRecipes = true;

            _pmcBom.CAM_EndPos_2 = new PmcBomItem();
            _pmcBom.CAM_EndPos_2.Id = "PMC1807";
            _pmcBom.CAM_EndPos_2.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CAM_EndPos_2.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.CAM_EndPos_2.Adr = 374;
            _pmcBom.CAM_EndPos_2.ConversionFactor = 100;
            _pmcBom.CAM_EndPos_2.IsRecipes = true;

            _pmcBom.CAM_EndArr_2 = new PmcBomItem();
            _pmcBom.CAM_EndArr_2.Id = "PMC1808";
            _pmcBom.CAM_EndArr_2.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CAM_EndArr_2.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CAM_EndArr_2.Adr = 378;
            _pmcBom.CAM_EndArr_2.Bit = 2;
            _pmcBom.CAM_EndArr_2.IsRecipes = true;

            _pmcBom.CAM_ActionFlag_2 = new PmcBomItem();
            _pmcBom.CAM_ActionFlag_2.Id = "PMC1809";
            _pmcBom.CAM_ActionFlag_2.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CAM_ActionFlag_2.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CAM_ActionFlag_2.Adr = 378;
            _pmcBom.CAM_ActionFlag_2.Bit = 3;
            _pmcBom.CAM_ActionFlag_2.IsRecipes = true;

            _pmcBom.CAM_Enable_2 = new PmcBomItem();
            _pmcBom.CAM_Enable_2.Id = "PMC1831";
            _pmcBom.CAM_Enable_2.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CAM_Enable_2.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CAM_Enable_2.Adr = 378;
            _pmcBom.CAM_Enable_2.Bit = 4;
            _pmcBom.CAM_Enable_2.IsRecipes = true;

            _pmcBom.CAM_StartPos_3 = new PmcBomItem();
            _pmcBom.CAM_StartPos_3.Id = "PMC1810";
            _pmcBom.CAM_StartPos_3.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CAM_StartPos_3.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.CAM_StartPos_3.Adr = 380;
            _pmcBom.CAM_StartPos_3.ConversionFactor = 100;
            _pmcBom.CAM_StartPos_3.IsRecipes = true;

            _pmcBom.CAM_StartArr_3 = new PmcBomItem();
            _pmcBom.CAM_StartArr_3.Id = "PMC1811";
            _pmcBom.CAM_StartArr_3.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CAM_StartArr_3.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CAM_StartArr_3.Adr = 388;
            _pmcBom.CAM_StartArr_3.Bit = 1;
            _pmcBom.CAM_StartArr_3.IsRecipes = true;

            _pmcBom.CAM_EndPos_3 = new PmcBomItem();
            _pmcBom.CAM_EndPos_3.Id = "PMC1812";
            _pmcBom.CAM_EndPos_3.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CAM_EndPos_3.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.CAM_EndPos_3.Adr = 384;
            _pmcBom.CAM_EndPos_3.ConversionFactor = 100;
            _pmcBom.CAM_EndPos_3.IsRecipes = true;

            _pmcBom.CAM_EndArr_3 = new PmcBomItem();
            _pmcBom.CAM_EndArr_3.Id = "PMC1813";
            _pmcBom.CAM_EndArr_3.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CAM_EndArr_3.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CAM_EndArr_3.Adr = 388;
            _pmcBom.CAM_EndArr_3.Bit = 2;
            _pmcBom.CAM_EndArr_3.IsRecipes = true;

            _pmcBom.CAM_ActionFlag_3 = new PmcBomItem();
            _pmcBom.CAM_ActionFlag_3.Id = "PMC1814";
            _pmcBom.CAM_ActionFlag_3.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CAM_ActionFlag_3.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CAM_ActionFlag_3.Adr = 388;
            _pmcBom.CAM_ActionFlag_3.Bit = 3;
            _pmcBom.CAM_ActionFlag_3.IsRecipes = true;

            _pmcBom.CAM_Enable_3 = new PmcBomItem();
            _pmcBom.CAM_Enable_3.Id = "PMC1832";
            _pmcBom.CAM_Enable_3.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CAM_Enable_3.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CAM_Enable_3.Adr = 388;
            _pmcBom.CAM_Enable_3.Bit = 4;
            _pmcBom.CAM_Enable_3.IsRecipes = true;

            _pmcBom.CAM_StartPos_4 = new PmcBomItem();
            _pmcBom.CAM_StartPos_4.Id = "PMC1815";
            _pmcBom.CAM_StartPos_4.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CAM_StartPos_4.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.CAM_StartPos_4.Adr = 390;
            _pmcBom.CAM_StartPos_4.ConversionFactor = 100;
            _pmcBom.CAM_StartPos_4.IsRecipes = true;

            _pmcBom.CAM_StartArr_4 = new PmcBomItem();
            _pmcBom.CAM_StartArr_4.Id = "PMC1816";
            _pmcBom.CAM_StartArr_4.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CAM_StartArr_4.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CAM_StartArr_4.Adr = 398;
            _pmcBom.CAM_StartArr_4.Bit = 1;
            _pmcBom.CAM_StartArr_4.IsRecipes = true;

            _pmcBom.CAM_EndPos_4 = new PmcBomItem();
            _pmcBom.CAM_EndPos_4.Id = "PMC1817";
            _pmcBom.CAM_EndPos_4.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CAM_EndPos_4.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.CAM_EndPos_4.Adr = 394;
            _pmcBom.CAM_EndPos_4.ConversionFactor = 100;
            _pmcBom.CAM_EndPos_4.IsRecipes = true;

            _pmcBom.CAM_EndArr_4 = new PmcBomItem();
            _pmcBom.CAM_EndArr_4.Id = "PMC1818";
            _pmcBom.CAM_EndArr_4.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CAM_EndArr_4.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CAM_EndArr_4.Adr = 398;
            _pmcBom.CAM_EndArr_4.Bit = 2;
            _pmcBom.CAM_EndArr_4.IsRecipes = true;

            _pmcBom.CAM_ActionFlag_4 = new PmcBomItem();
            _pmcBom.CAM_ActionFlag_4.Id = "PMC1819";
            _pmcBom.CAM_ActionFlag_4.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CAM_ActionFlag_4.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CAM_ActionFlag_4.Adr = 398;
            _pmcBom.CAM_ActionFlag_4.Bit = 3;
            _pmcBom.CAM_ActionFlag_4.IsRecipes = true;

            _pmcBom.CAM_Enable_4 = new PmcBomItem();
            _pmcBom.CAM_Enable_4.Id = "PMC1833";
            _pmcBom.CAM_Enable_4.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.CAM_Enable_4.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.CAM_Enable_4.Adr = 398;
            _pmcBom.CAM_Enable_4.Bit = 4;
            _pmcBom.CAM_Enable_4.IsRecipes = true;

            #endregion

            #region 模具压力设定
            _pmcBom.DH_Mode = new PmcBomItem();
            _pmcBom.DH_Mode.Id = "PMC1900";
            _pmcBom.DH_Mode.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.DH_Mode.DataType = PmcDataTypeEnum.WORD;
            _pmcBom.DH_Mode.Adr = 400;
            _pmcBom.DH_Mode.IsRecipes = true;

            _pmcBom.DH_Pressure = new PmcBomItem();
            _pmcBom.DH_Pressure.Id = "PMC1901";
            _pmcBom.DH_Pressure.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.DH_Pressure.DataType = PmcDataTypeEnum.WORD;
            _pmcBom.DH_Pressure.Adr = 402;
            _pmcBom.DH_Pressure.IsRecipes = true;

            _pmcBom.DH_PushPos = new PmcBomItem();
            _pmcBom.DH_PushPos.Id = "PMC1902";
            _pmcBom.DH_PushPos.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.DH_PushPos.DataType = PmcDataTypeEnum.WORD;
            _pmcBom.DH_PushPos.Adr = 404;
            _pmcBom.DH_PushPos.IsRecipes = true;

            _pmcBom.DH_PushDelayTime = new PmcBomItem();
            _pmcBom.DH_PushDelayTime.Id = "PMC1903";
            _pmcBom.DH_PushDelayTime.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.DH_PushDelayTime.DataType = PmcDataTypeEnum.WORD;
            _pmcBom.DH_PushDelayTime.Adr = 406;
            _pmcBom.DH_PushDelayTime.IsRecipes = true;

            _pmcBom.DH_ActualPressure = new PmcBomItem();
            _pmcBom.DH_ActualPressure.Id = "PMC1904";
            _pmcBom.DH_ActualPressure.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.DH_ActualPressure.DataType = PmcDataTypeEnum.WORD;
            _pmcBom.DH_ActualPressure.Adr = 408;
            _pmcBom.DH_ActualPressure.IsRecipes = true;

            _pmcBom.DH_ActualPos = new PmcBomItem();
            _pmcBom.DH_ActualPos.Id = "PMC1905";
            _pmcBom.DH_ActualPos.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.DH_ActualPos.DataType = PmcDataTypeEnum.WORD;
            _pmcBom.DH_ActualPos.Adr = 410;
            _pmcBom.DH_ActualPos.IsRecipes = true;

            _pmcBom.DH_Run = new PmcBomItem();
            _pmcBom.DH_Run.Id = "PMC1906";
            _pmcBom.DH_Run.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.DH_Run.DataType = PmcDataTypeEnum.WORD;
            _pmcBom.DH_Run.Adr = 420;
            _pmcBom.DH_Run.Bit = 1;
            _pmcBom.DH_Run.IsRecipes = true;

            _pmcBom.DH_State = new PmcBomItem();
            _pmcBom.DH_State.Id = "PMC1907";
            _pmcBom.DH_State.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.DH_State.DataType = PmcDataTypeEnum.WORD;
            _pmcBom.DH_State.Adr = 420;
            _pmcBom.DH_State.Bit = 2;
            _pmcBom.DH_State.IsRecipes = true;

            #endregion

            #region 工件计数
            _pmcBom.WPP_DayPiece = new PmcBomItem();
            _pmcBom.WPP_DayPiece.Id = "PMC2000";
            _pmcBom.WPP_DayPiece.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.WPP_DayPiece.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.WPP_DayPiece.Adr = 500;

            _pmcBom.WPP_DayWork = new PmcBomItem();
            _pmcBom.WPP_DayWork.Id = "PMC2001";
            _pmcBom.WPP_DayWork.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.WPP_DayWork.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.WPP_DayWork.Adr = 504;

            _pmcBom.WPP_CurPiece = new PmcBomItem();
            _pmcBom.WPP_CurPiece.Id = "PMC2002";
            _pmcBom.WPP_CurPiece.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.WPP_CurPiece.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.WPP_CurPiece.Adr = 510;

            _pmcBom.WPP_SetPiece = new PmcBomItem();
            _pmcBom.WPP_SetPiece.Id = "PMC2003";
            _pmcBom.WPP_SetPiece.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.WPP_SetPiece.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.WPP_SetPiece.Adr = 514;

            _pmcBom.WPP_CurPiece2 = new PmcBomItem();
            _pmcBom.WPP_CurPiece2.Id = "PMC2004";
            _pmcBom.WPP_CurPiece2.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.WPP_CurPiece2.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.WPP_CurPiece2.Adr = 520;

            _pmcBom.WPP_TotalPiece = new PmcBomItem();
            _pmcBom.WPP_TotalPiece.Id = "PMC2005";
            _pmcBom.WPP_TotalPiece.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.WPP_TotalPiece.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.WPP_TotalPiece.Adr = 530;

            _pmcBom.WPP_TotalWork = new PmcBomItem();
            _pmcBom.WPP_TotalWork.Id = "PMC2006";
            _pmcBom.WPP_TotalWork.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.WPP_TotalWork.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.WPP_TotalWork.Adr = 534;

            #endregion

            #region 系统参数 压机设定
            _pmcBom.SPM_MaxWeight = new PmcBomItem();
            _pmcBom.SPM_MaxWeight.Id = "PMC2100";
            _pmcBom.SPM_MaxWeight.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPM_MaxWeight.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPM_MaxWeight.Adr = 600;

            _pmcBom.SPM_MaxTemperature = new PmcBomItem();
            _pmcBom.SPM_MaxTemperature.Id = "PMC2101";
            _pmcBom.SPM_MaxTemperature.AdrType = PmcAdrTypeEnum.E;

            _pmcBom.SPM_MaxTemperature.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPM_MaxTemperature.Adr = 604;

            _pmcBom.SPM_MaxError = new PmcBomItem();
            _pmcBom.SPM_MaxError.Id = "PMC2102";
            _pmcBom.SPM_MaxError.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPM_MaxError.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPM_MaxError.Adr = 608;

            _pmcBom.SPM_PressureSensorPara = new PmcBomItem();
            _pmcBom.SPM_PressureSensorPara.Id = "PMC2103";
            _pmcBom.SPM_PressureSensorPara.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPM_PressureSensorPara.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPM_PressureSensorPara.Adr = 612;

            _pmcBom.SPM_BalanceCylinderAnalog = new PmcBomItem();
            _pmcBom.SPM_BalanceCylinderAnalog.Id = "PMC2104";
            _pmcBom.SPM_BalanceCylinderAnalog.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPM_BalanceCylinderAnalog.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPM_BalanceCylinderAnalog.Adr = 616;

            _pmcBom.SPM_BalanceCylinderPressure = new PmcBomItem();
            _pmcBom.SPM_BalanceCylinderPressure.Id = "PMC2105";
            _pmcBom.SPM_BalanceCylinderPressure.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPM_BalanceCylinderPressure.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPM_BalanceCylinderPressure.Adr = 620;

            _pmcBom.SPM_OverflowDelay = new PmcBomItem();
            _pmcBom.SPM_OverflowDelay.Id = "PMC2106";
            _pmcBom.SPM_OverflowDelay.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPM_OverflowDelay.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPM_OverflowDelay.Adr = 624;

            _pmcBom.SPM_PressureDiffPara = new PmcBomItem();
            _pmcBom.SPM_PressureDiffPara.Id = "PMC2107";
            _pmcBom.SPM_PressureDiffPara.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPM_PressureDiffPara.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPM_PressureDiffPara.Adr = 628;

            _pmcBom.SPM_PressureLowAlarm = new PmcBomItem();
            _pmcBom.SPM_PressureLowAlarm.Id = "PMC2108";
            _pmcBom.SPM_PressureLowAlarm.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPM_PressureLowAlarm.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPM_PressureLowAlarm.Adr = 632;

            _pmcBom.SPM_LubricateDetect = new PmcBomItem();
            _pmcBom.SPM_LubricateDetect.Id = "PMC2109";
            _pmcBom.SPM_LubricateDetect.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPM_LubricateDetect.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPM_LubricateDetect.Adr = 636;

            #endregion

            #region 系统参数 润滑设定
            _pmcBom.SPL_AC_LubricateTime = new PmcBomItem();
            _pmcBom.SPL_AC_LubricateTime.Id = "PMC2200";
            _pmcBom.SPL_AC_LubricateTime.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPL_AC_LubricateTime.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPL_AC_LubricateTime.Adr = 700;

            _pmcBom.SPL_CR_SetLubricateInterval = new PmcBomItem();
            _pmcBom.SPL_CR_SetLubricateInterval.Id = "PMC2201";
            _pmcBom.SPL_CR_SetLubricateInterval.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPL_CR_SetLubricateInterval.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPL_CR_SetLubricateInterval.Adr = 704;

            _pmcBom.SPL_CR_ActualLubricateInterval = new PmcBomItem();
            _pmcBom.SPL_CR_ActualLubricateInterval.Id = "PMC2202";
            _pmcBom.SPL_CR_ActualLubricateInterval.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPL_CR_ActualLubricateInterval.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPL_CR_ActualLubricateInterval.Adr = 708;

            _pmcBom.SPL_CR_LubricateDetectTime = new PmcBomItem();
            _pmcBom.SPL_CR_LubricateDetectTime.Id = "PMC2203";
            _pmcBom.SPL_CR_LubricateDetectTime.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPL_CR_LubricateDetectTime.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPL_CR_LubricateDetectTime.Adr = 712;

            _pmcBom.SPL_CR_LubricateTotalNum = new PmcBomItem();
            _pmcBom.SPL_CR_LubricateTotalNum.Id = "PMC2204";
            _pmcBom.SPL_CR_LubricateTotalNum.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPL_CR_LubricateTotalNum.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPL_CR_LubricateTotalNum.Adr = 716;

            _pmcBom.SPL_CR_PowerOnLubricateTime = new PmcBomItem();
            _pmcBom.SPL_CR_PowerOnLubricateTime.Id = "PMC2205";
            _pmcBom.SPL_CR_PowerOnLubricateTime.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPL_CR_PowerOnLubricateTime.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPL_CR_PowerOnLubricateTime.Adr = 720;

            _pmcBom.SPL_CR_LubricateDetecte = new PmcBomItem();
            _pmcBom.SPL_CR_LubricateDetecte.Id = "PMC2206";
            _pmcBom.SPL_CR_LubricateDetecte.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPL_CR_LubricateDetecte.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPL_CR_LubricateDetecte.Adr = 724;

            _pmcBom.SPL_AC_LubricateTime = new PmcBomItem();
            _pmcBom.SPL_AC_LubricateTime.Id = "PMC2207";
            _pmcBom.SPL_AC_LubricateTime.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPL_AC_LubricateTime.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPL_AC_LubricateTime.Adr = 728;

            _pmcBom.SPL_AC_SetLubricateInterval = new PmcBomItem();
            _pmcBom.SPL_AC_SetLubricateInterval.Id = "PMC2208";
            _pmcBom.SPL_AC_SetLubricateInterval.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPL_AC_SetLubricateInterval.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPL_AC_SetLubricateInterval.Adr = 732;

            _pmcBom.SPL_AC_ActualLubricateInterval = new PmcBomItem();
            _pmcBom.SPL_AC_ActualLubricateInterval.Id = "PMC2209";
            _pmcBom.SPL_AC_ActualLubricateInterval.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPL_AC_ActualLubricateInterval.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPL_AC_ActualLubricateInterval.Adr = 736;

            _pmcBom.SPL_AC_LubricateDetectTime = new PmcBomItem();
            _pmcBom.SPL_AC_LubricateDetectTime.Id = "PMC2210";
            _pmcBom.SPL_AC_LubricateDetectTime.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPL_AC_LubricateDetectTime.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPL_AC_LubricateDetectTime.Adr = 740;

            _pmcBom.SPL_AC_LubricateTotalNum = new PmcBomItem();
            _pmcBom.SPL_AC_LubricateTotalNum.Id = "PMC2211";
            _pmcBom.SPL_AC_LubricateTotalNum.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPL_AC_LubricateTotalNum.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPL_AC_LubricateTotalNum.Adr = 744;

            _pmcBom.SPL_AC_PowerOnLubricateTime = new PmcBomItem();
            _pmcBom.SPL_AC_PowerOnLubricateTime.Id = "PMC2212";
            _pmcBom.SPL_AC_PowerOnLubricateTime.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPL_AC_PowerOnLubricateTime.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPL_AC_PowerOnLubricateTime.Adr = 748;

            _pmcBom.SPL_AC2_LubricateTime = new PmcBomItem();
            _pmcBom.SPL_AC2_LubricateTime.Id = "PMC2213";
            _pmcBom.SPL_AC2_LubricateTime.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPL_AC2_LubricateTime.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPL_AC2_LubricateTime.Adr = 752;

            _pmcBom.SPL_AC2_SetLubricateInterval = new PmcBomItem();
            _pmcBom.SPL_AC2_SetLubricateInterval.Id = "PMC2214";
            _pmcBom.SPL_AC2_SetLubricateInterval.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPL_AC2_SetLubricateInterval.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPL_AC2_SetLubricateInterval.Adr = 756;

            _pmcBom.SPL_AC2_ActualLubricateInterval = new PmcBomItem();
            _pmcBom.SPL_AC2_ActualLubricateInterval.Id = "PMC2215";
            _pmcBom.SPL_AC2_ActualLubricateInterval.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPL_AC2_ActualLubricateInterval.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPL_AC2_ActualLubricateInterval.Adr = 760;

            _pmcBom.SPL_AC2_LubricateDetectTime = new PmcBomItem();
            _pmcBom.SPL_AC2_LubricateDetectTime.Id = "PMC2216";
            _pmcBom.SPL_AC2_LubricateDetectTime.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPL_AC2_LubricateDetectTime.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPL_AC2_LubricateDetectTime.Adr = 764;

            _pmcBom.SPL_AC2_LubricateTotalNum = new PmcBomItem();
            _pmcBom.SPL_AC2_LubricateTotalNum.Id = "PMC2217";
            _pmcBom.SPL_AC2_LubricateTotalNum.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPL_AC2_LubricateTotalNum.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPL_AC2_LubricateTotalNum.Adr = 768;

            _pmcBom.SPL_AC2_PowerOnLubricateTime = new PmcBomItem();
            _pmcBom.SPL_AC2_PowerOnLubricateTime.Id = "PMC2218";
            _pmcBom.SPL_AC2_PowerOnLubricateTime.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPL_AC2_PowerOnLubricateTime.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPL_AC2_PowerOnLubricateTime.Adr = 772;

            _pmcBom.SPL_GR_LubricateReversing = new PmcBomItem();
            _pmcBom.SPL_GR_LubricateReversing.Id = "PMC2219";
            _pmcBom.SPL_GR_LubricateReversing.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPL_GR_LubricateReversing.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPL_GR_LubricateReversing.Adr = 776;

            _pmcBom.SPL_GR_LubricateDetectTime = new PmcBomItem();
            _pmcBom.SPL_GR_LubricateDetectTime.Id = "PMC2220";
            _pmcBom.SPL_GR_LubricateDetectTime.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPL_GR_LubricateDetectTime.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPL_GR_LubricateDetectTime.Adr = 780;

            _pmcBom.SPL_SC_LubricateReversing = new PmcBomItem();
            _pmcBom.SPL_SC_LubricateReversing.Id = "PMC2221";
            _pmcBom.SPL_SC_LubricateReversing.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPL_SC_LubricateReversing.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPL_SC_LubricateReversing.Adr = 784;

            _pmcBom.SPL_OS_Time = new PmcBomItem();
            _pmcBom.SPL_OS_Time.Id = "PMC2222";
            _pmcBom.SPL_OS_Time.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPL_OS_Time.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPL_OS_Time.Adr = 788;

            _pmcBom.SPL_OS_IntervalTime = new PmcBomItem();
            _pmcBom.SPL_OS_IntervalTime.Id = "PMC2223";
            _pmcBom.SPL_OS_IntervalTime.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPL_OS_IntervalTime.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPL_OS_IntervalTime.Adr = 792;

            _pmcBom.SPL_OS_DelayTime = new PmcBomItem();
            _pmcBom.SPL_OS_DelayTime.Id = "PMC2224";
            _pmcBom.SPL_OS_DelayTime.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPL_OS_DelayTime.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPL_OS_DelayTime.Adr = 796;

            _pmcBom.SPL_TS_DelayTime = new PmcBomItem();
            _pmcBom.SPL_TS_DelayTime.Id = "PMC2225";
            _pmcBom.SPL_TS_DelayTime.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPL_TS_DelayTime.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPL_TS_DelayTime.Adr = 800;

            _pmcBom.SPL_TS_StopTime = new PmcBomItem();
            _pmcBom.SPL_TS_StopTime.Id = "PMC2226";
            _pmcBom.SPL_TS_StopTime.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPL_TS_StopTime.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPL_TS_StopTime.Adr = 804;

            _pmcBom.SPL_TS_RunTime = new PmcBomItem();
            _pmcBom.SPL_TS_RunTime.Id = "PMC2227";
            _pmcBom.SPL_TS_RunTime.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPL_TS_RunTime.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPL_TS_RunTime.Adr = 808;

            #endregion

            #region 系统参数 模拟量设定
            _pmcBom.SPA_A1_Value = new PmcBomItem();
            _pmcBom.SPA_A1_Value.Id = "PMC2300";
            _pmcBom.SPA_A1_Value.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPA_A1_Value.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPA_A1_Value.Adr = 900;
            _pmcBom.SPA_A1_WeightPara1 = new PmcBomItem();
            _pmcBom.SPA_A1_WeightPara1.Id = "PMC2301";
            _pmcBom.SPA_A1_WeightPara1.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPA_A1_WeightPara1.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPA_A1_WeightPara1.Adr = 904;
            _pmcBom.SPA_A1_WeightPara2 = new PmcBomItem();
            _pmcBom.SPA_A1_WeightPara2.Id = "PMC2302";
            _pmcBom.SPA_A1_WeightPara2.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPA_A1_WeightPara2.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPA_A1_WeightPara2.Adr = 908;
            _pmcBom.SPA_A1_Weight = new PmcBomItem();
            _pmcBom.SPA_A1_Weight.Id = "PMC2303";
            _pmcBom.SPA_A1_Weight.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPA_A1_Weight.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPA_A1_Weight.Adr = 912;
            _pmcBom.SPA_A2_Value = new PmcBomItem();
            _pmcBom.SPA_A2_Value.Id = "PMC2304";
            _pmcBom.SPA_A2_Value.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPA_A2_Value.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPA_A2_Value.Adr = 916;
            _pmcBom.SPA_A2_WeightPara1 = new PmcBomItem();
            _pmcBom.SPA_A2_WeightPara1.Id = "PMC2305";
            _pmcBom.SPA_A2_WeightPara1.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPA_A2_WeightPara1.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPA_A2_WeightPara1.Adr = 920;
            _pmcBom.SPA_A2_WeightPara2 = new PmcBomItem();
            _pmcBom.SPA_A2_WeightPara2.Id = "PMC2306";
            _pmcBom.SPA_A2_WeightPara2.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPA_A2_WeightPara2.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPA_A2_WeightPara2.Adr = 924;
            _pmcBom.SPA_A2_Weight = new PmcBomItem();
            _pmcBom.SPA_A2_Weight.Id = "PMC2307";
            _pmcBom.SPA_A2_Weight.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPA_A2_Weight.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPA_A2_Weight.Adr = 928;
            _pmcBom.SPA_A3_Value = new PmcBomItem();
            _pmcBom.SPA_A3_Value.Id = "PMC2308";
            _pmcBom.SPA_A3_Value.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPA_A3_Value.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPA_A3_Value.Adr = 932;
            _pmcBom.SPA_A3_WeightPara1 = new PmcBomItem();
            _pmcBom.SPA_A3_WeightPara1.Id = "PMC2309";
            _pmcBom.SPA_A3_WeightPara1.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPA_A3_WeightPara1.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPA_A3_WeightPara1.Adr = 936;
            _pmcBom.SPA_A3_WeightPara2 = new PmcBomItem();
            _pmcBom.SPA_A3_WeightPara2.Id = "PMC2310";
            _pmcBom.SPA_A3_WeightPara2.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPA_A3_WeightPara2.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPA_A3_WeightPara2.Adr = 940;
            _pmcBom.SPA_A3_Weight = new PmcBomItem();
            _pmcBom.SPA_A3_Weight.Id = "PMC2311";
            _pmcBom.SPA_A3_Weight.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPA_A3_Weight.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPA_A3_Weight.Adr = 944;
            _pmcBom.SPA_A4_Value = new PmcBomItem();
            _pmcBom.SPA_A4_Value.Id = "PMC2312";
            _pmcBom.SPA_A4_Value.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPA_A4_Value.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPA_A4_Value.Adr = 948;
            _pmcBom.SPA_A4_WeightPara1 = new PmcBomItem();
            _pmcBom.SPA_A4_WeightPara1.Id = "PMC2313";
            _pmcBom.SPA_A4_WeightPara1.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPA_A4_WeightPara1.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPA_A4_WeightPara1.Adr = 952;
            _pmcBom.SPA_A4_WeightPara2 = new PmcBomItem();
            _pmcBom.SPA_A4_WeightPara2.Id = "PMC2314";
            _pmcBom.SPA_A4_WeightPara2.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPA_A4_WeightPara2.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPA_A4_WeightPara2.Adr = 956;
            _pmcBom.SPA_A4_Weight = new PmcBomItem();
            _pmcBom.SPA_A4_Weight.Id = "PMC2315";
            _pmcBom.SPA_A4_Weight.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPA_A4_Weight.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPA_A4_Weight.Adr = 960;

            _pmcBom.SPA_TotalWeight = new PmcBomItem();
            _pmcBom.SPA_TotalWeight.Id = "PMC2316";
            _pmcBom.SPA_TotalWeight.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPA_TotalWeight.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPA_TotalWeight.Adr = 964;
            _pmcBom.SPA_DetectPosition = new PmcBomItem();
            _pmcBom.SPA_DetectPosition.Id = "PMC2317";
            _pmcBom.SPA_DetectPosition.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPA_DetectPosition.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPA_DetectPosition.Adr = 968;
            _pmcBom.SPA_DetectInPosition = new PmcBomItem();
            _pmcBom.SPA_DetectInPosition.Id = "PMC2318";
            _pmcBom.SPA_DetectInPosition.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPA_DetectInPosition.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPA_DetectInPosition.Adr = 972;
            _pmcBom.SPA_DetectSensor = new PmcBomItem();
            _pmcBom.SPA_DetectSensor.Id = "PMC2319";
            _pmcBom.SPA_DetectSensor.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPA_DetectSensor.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPA_DetectSensor.Adr = 976;

            _pmcBom.SPA_Pressure = new PmcBomItem();
            _pmcBom.SPA_Pressure.Id = "PMC2320";
            _pmcBom.SPA_Pressure.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPA_Pressure.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPA_Pressure.Adr = 980;
            _pmcBom.SPA_PressureUp = new PmcBomItem();
            _pmcBom.SPA_PressureUp.Id = "PMC2321";
            _pmcBom.SPA_PressureUp.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPA_PressureUp.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPA_PressureUp.Adr = 984;
            _pmcBom.SPA_PressureDown = new PmcBomItem();
            _pmcBom.SPA_PressureDown.Id = "PMC2322";
            _pmcBom.SPA_PressureDown.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPA_PressureDown.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPA_PressureDown.Adr = 988;


            #endregion

            #region 系统参数 编码器设定
            _pmcBom.SPA_IM_RESOLUTION = new PmcBomItem();
            _pmcBom.SPA_IM_RESOLUTION.Id = "PMC2400";
            _pmcBom.SPA_IM_RESOLUTION.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPA_IM_RESOLUTION.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPA_IM_RESOLUTION.Adr = 980;
            _pmcBom.SPA_IM_MOVEPITCH = new PmcBomItem();
            _pmcBom.SPA_IM_MOVEPITCH.Id = "PMC2401";
            _pmcBom.SPA_IM_MOVEPITCH.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPA_IM_MOVEPITCH.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPA_IM_MOVEPITCH.Adr = 980;
            _pmcBom.SPA_IM_UPPOSITION = new PmcBomItem();
            _pmcBom.SPA_IM_UPPOSITION.Id = "PMC2402";
            _pmcBom.SPA_IM_UPPOSITION.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPA_IM_UPPOSITION.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPA_IM_UPPOSITION.Adr = 980;
            _pmcBom.SPA_IM_DOWNPOSITION = new PmcBomItem();
            _pmcBom.SPA_IM_DOWNPOSITION.Id = "PMC2403";
            _pmcBom.SPA_IM_DOWNPOSITION.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPA_IM_DOWNPOSITION.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPA_IM_DOWNPOSITION.Adr = 980;
            _pmcBom.SPA_IM_SPEEDCHANGEPOSITION = new PmcBomItem();
            _pmcBom.SPA_IM_SPEEDCHANGEPOSITION.Id = "PMC2404";
            _pmcBom.SPA_IM_SPEEDCHANGEPOSITION.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPA_IM_SPEEDCHANGEPOSITION.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPA_IM_SPEEDCHANGEPOSITION.Adr = 980;
            _pmcBom.SPA_IM_LIMITUP = new PmcBomItem();
            _pmcBom.SPA_IM_LIMITUP.Id = "PMC2405";
            _pmcBom.SPA_IM_LIMITUP.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPA_IM_LIMITUP.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPA_IM_LIMITUP.Adr = 980;
            _pmcBom.SPA_IM_LIMITDOWN = new PmcBomItem();
            _pmcBom.SPA_IM_LIMITDOWN.Id = "PMC2406";
            _pmcBom.SPA_IM_LIMITDOWN.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPA_IM_LIMITDOWN.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPA_IM_LIMITDOWN.Adr = 980;
            _pmcBom.SPA_IM_ERROR = new PmcBomItem();
            _pmcBom.SPA_IM_ERROR.Id = "PMC2407";
            _pmcBom.SPA_IM_ERROR.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPA_IM_ERROR.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPA_IM_ERROR.Adr = 980;
            _pmcBom.SPA_IM_DIRECTION = new PmcBomItem();
            _pmcBom.SPA_IM_DIRECTION.Id = "PMC2408";
            _pmcBom.SPA_IM_DIRECTION.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPA_IM_DIRECTION.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPA_IM_DIRECTION.Adr = 980;
            _pmcBom.SPA_AC_RESOLUTION = new PmcBomItem();
            _pmcBom.SPA_AC_RESOLUTION.Id = "PMC2409";
            _pmcBom.SPA_AC_RESOLUTION.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPA_AC_RESOLUTION.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPA_AC_RESOLUTION.Adr = 980;
            _pmcBom.SPA_AC_MOVEPITCH = new PmcBomItem();
            _pmcBom.SPA_AC_MOVEPITCH.Id = "PMC2410";
            _pmcBom.SPA_AC_MOVEPITCH.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPA_AC_MOVEPITCH.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPA_AC_MOVEPITCH.Adr = 980;
            _pmcBom.SPA_AC_UPPOSITION = new PmcBomItem();
            _pmcBom.SPA_AC_UPPOSITION.Id = "PMC2411";
            _pmcBom.SPA_AC_UPPOSITION.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPA_AC_UPPOSITION.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPA_AC_UPPOSITION.Adr = 980;
            _pmcBom.SPA_AC_DOWNPOSITION = new PmcBomItem();
            _pmcBom.SPA_AC_DOWNPOSITION.Id = "PMC2412";
            _pmcBom.SPA_AC_DOWNPOSITION.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPA_AC_DOWNPOSITION.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPA_AC_DOWNPOSITION.Adr = 980;
            _pmcBom.SPA_AC_LIMITUP = new PmcBomItem();
            _pmcBom.SPA_AC_LIMITUP.Id = "PMC2413";
            _pmcBom.SPA_AC_LIMITUP.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPA_AC_LIMITUP.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPA_AC_LIMITUP.Adr = 980;
            _pmcBom.SPA_AC_LIMITDOWN = new PmcBomItem();
            _pmcBom.SPA_AC_LIMITDOWN.Id = "PMC2414";
            _pmcBom.SPA_AC_LIMITDOWN.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPA_AC_LIMITDOWN.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPA_AC_LIMITDOWN.Adr = 980;
            _pmcBom.SPA_AC_ERROR = new PmcBomItem();
            _pmcBom.SPA_AC_ERROR.Id = "PMC2415";
            _pmcBom.SPA_AC_ERROR.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPA_AC_ERROR.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPA_AC_ERROR.Adr = 980;
            _pmcBom.SPA_AC_DIRECTION = new PmcBomItem();
            _pmcBom.SPA_AC_DIRECTION.Id = "PMC2416";
            _pmcBom.SPA_AC_DIRECTION.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.SPA_AC_DIRECTION.DataType = PmcDataTypeEnum.LONG;
            _pmcBom.SPA_AC_DIRECTION.Adr = 980;


            #endregion

            #region 模高调整操作提示
            _pmcBom.MDH_SEL = new PmcBomItem();
            _pmcBom.MDH_SEL.Id = "PMC2500";
            _pmcBom.MDH_SEL.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.MDH_SEL.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.MDH_SEL.Adr = 610;
            _pmcBom.MDH_SEL.Bit = 0;

            _pmcBom.MDH_BL = new PmcBomItem();
            _pmcBom.MDH_BL.Id = "PMC2501";
            _pmcBom.MDH_BL.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.MDH_BL.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.MDH_BL.Adr = 600;
            _pmcBom.MDH_BL.Bit = 1;

            _pmcBom.MDH_SafeCock = new PmcBomItem();
            _pmcBom.MDH_SafeCock.Id = "PMC2502";
            _pmcBom.MDH_SafeCock.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.MDH_SafeCock.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.MDH_SafeCock.Adr = 600;
            _pmcBom.MDH_SafeCock.Bit = 6;

            _pmcBom.MDH_TableClamped = new PmcBomItem();
            _pmcBom.MDH_TableClamped.Id = "PMC2503";
            _pmcBom.MDH_TableClamped.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.MDH_TableClamped.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.MDH_TableClamped.Adr = 600;
            _pmcBom.MDH_TableClamped.Bit = 7;

            _pmcBom.MDH_Emg = new PmcBomItem();
            _pmcBom.MDH_Emg.Id = "PMC2504";
            _pmcBom.MDH_Emg.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.MDH_Emg.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.MDH_Emg.Adr = 600;
            _pmcBom.MDH_Emg.Bit = 5;

            _pmcBom.MDH_MoveTableAmp = new PmcBomItem();
            _pmcBom.MDH_MoveTableAmp.Id = "PMC2505";
            _pmcBom.MDH_MoveTableAmp.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.MDH_MoveTableAmp.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.MDH_MoveTableAmp.Adr = 610;
            _pmcBom.MDH_MoveTableAmp.Bit = 1;

            _pmcBom.MDH_Manual = new PmcBomItem();
            _pmcBom.MDH_Manual.Id = "PMC2506";
            _pmcBom.MDH_Manual.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.MDH_Manual.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.MDH_Manual.Adr = 610;
            _pmcBom.MDH_Manual.Bit = 2;

            _pmcBom.MDH_UnLimitDown = new PmcBomItem();
            _pmcBom.MDH_UnLimitDown.Id = "PMC2507";
            _pmcBom.MDH_UnLimitDown.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.MDH_UnLimitDown.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.MDH_UnLimitDown.Adr = 610;
            _pmcBom.MDH_UnLimitDown.Bit = 5;

            _pmcBom.MDH_UnLimitUp = new PmcBomItem();
            _pmcBom.MDH_UnLimitUp.Id = "PMC2508";
            _pmcBom.MDH_UnLimitUp.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.MDH_UnLimitUp.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.MDH_UnLimitUp.Adr = 610;
            _pmcBom.MDH_UnLimitUp.Bit = 4;

            _pmcBom.MDH_ManualDown = new PmcBomItem();
            _pmcBom.MDH_ManualDown.Id = "PMC2509";
            _pmcBom.MDH_ManualDown.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.MDH_ManualDown.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.MDH_ManualDown.Adr = 610;
            _pmcBom.MDH_ManualDown.Bit = 7;

            _pmcBom.MDH_ManualUp = new PmcBomItem();
            _pmcBom.MDH_ManualUp.Id = "PMC2510";
            _pmcBom.MDH_ManualUp.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.MDH_ManualUp.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.MDH_ManualUp.Adr = 610;
            _pmcBom.MDH_ManualUp.Bit = 6;

            _pmcBom.MDH_Auto = new PmcBomItem();
            _pmcBom.MDH_Auto.Id = "PMC2511";
            _pmcBom.MDH_Auto.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.MDH_Auto.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.MDH_Auto.Adr = 610;
            _pmcBom.MDH_Auto.Bit = 3;

            _pmcBom.MDH_UpDieCenter = new PmcBomItem();
            _pmcBom.MDH_UpDieCenter.Id = "PMC2512";
            _pmcBom.MDH_UpDieCenter.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.MDH_UpDieCenter.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.MDH_UpDieCenter.Adr = 601;
            _pmcBom.MDH_UpDieCenter.Bit = 5;

            _pmcBom.MDH_AutoAllowUp = new PmcBomItem();
            _pmcBom.MDH_AutoAllowUp.Id = "PMC2513";
            _pmcBom.MDH_AutoAllowUp.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.MDH_AutoAllowUp.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.MDH_AutoAllowUp.Adr = 611;
            _pmcBom.MDH_AutoAllowUp.Bit = 0;

            _pmcBom.MDH_AutoAllowDown = new PmcBomItem();
            _pmcBom.MDH_AutoAllowDown.Id = "PMC2514";
            _pmcBom.MDH_AutoAllowDown.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.MDH_AutoAllowDown.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.MDH_AutoAllowDown.Adr = 611;
            _pmcBom.MDH_AutoAllowDown.Bit = 1;

            _pmcBom.MDH_AutoUp = new PmcBomItem();
            _pmcBom.MDH_AutoUp.Id = "PMC2515";
            _pmcBom.MDH_AutoUp.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.MDH_AutoUp.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.MDH_AutoUp.Adr = 611;
            _pmcBom.MDH_AutoUp.Bit = 2;

            _pmcBom.MDH_AutoDown = new PmcBomItem();
            _pmcBom.MDH_AutoDown.Id = "PMC2516";
            _pmcBom.MDH_AutoDown.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.MDH_AutoDown.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.MDH_AutoDown.Adr = 611;
            _pmcBom.MDH_AutoDown.Bit = 3;

            #endregion

            #region 移台控制提示
            _pmcBom.MMT_MovePump = new PmcBomItem();
            _pmcBom.MMT_MovePump.Id = "PMC3100";
            _pmcBom.MMT_MovePump.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.MMT_MovePump.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.MMT_MovePump.Adr = 620;
            _pmcBom.MMT_MovePump.Bit = 0;

            _pmcBom.MMT_EMG = new PmcBomItem();
            _pmcBom.MMT_EMG.Id = "PMC3101";
            _pmcBom.MMT_EMG.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.MMT_EMG.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.MMT_EMG.Adr = 600;
            _pmcBom.MMT_EMG.Bit = 5;

            _pmcBom.MMT_ChangeMode = new PmcBomItem();
            _pmcBom.MMT_ChangeMode.Id = "PMC3102";
            _pmcBom.MMT_ChangeMode.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.MMT_ChangeMode.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.MMT_ChangeMode.Adr = 620;
            _pmcBom.MMT_ChangeMode.Bit = 1;

            _pmcBom.MMT_UpDieCenter = new PmcBomItem();
            _pmcBom.MMT_UpDieCenter.Id = "PMC3103";
            _pmcBom.MMT_UpDieCenter.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.MMT_UpDieCenter.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.MMT_UpDieCenter.Adr = 601;
            _pmcBom.MMT_UpDieCenter.Bit = 5;

            _pmcBom.MMT_UpBTN = new PmcBomItem();
            _pmcBom.MMT_UpBTN.Id = "PMC3104";
            _pmcBom.MMT_UpBTN.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.MMT_UpBTN.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.MMT_UpBTN.Adr = 620;
            _pmcBom.MMT_UpBTN.Bit = 2;

            _pmcBom.MMT_PumpStation = new PmcBomItem();
            _pmcBom.MMT_PumpStation.Id = "PMC3105";
            _pmcBom.MMT_PumpStation.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.MMT_PumpStation.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.MMT_PumpStation.Adr = 620;
            _pmcBom.MMT_PumpStation.Bit = 3;

            _pmcBom.MMT_OUT_Mag = new PmcBomItem();
            _pmcBom.MMT_OUT_Mag.Id = "PMC3106";
            _pmcBom.MMT_OUT_Mag.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.MMT_OUT_Mag.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.MMT_OUT_Mag.Adr = 620;
            _pmcBom.MMT_OUT_Mag.Bit = 4;

            _pmcBom.MMT_UpTimeOut = new PmcBomItem();
            _pmcBom.MMT_UpTimeOut.Id = "PMC3107";
            _pmcBom.MMT_UpTimeOut.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.MMT_UpTimeOut.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.MMT_UpTimeOut.Adr = 620;
            _pmcBom.MMT_UpTimeOut.Bit = 6;

            _pmcBom.MMT_UpFin = new PmcBomItem();
            _pmcBom.MMT_UpFin.Id = "PMC3108";
            _pmcBom.MMT_UpFin.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.MMT_UpFin.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.MMT_UpFin.Adr = 620;
            _pmcBom.MMT_UpFin.Bit = 5;

            _pmcBom.MMT_MoveAmp = new PmcBomItem();
            _pmcBom.MMT_MoveAmp.Id = "PMC3109";
            _pmcBom.MMT_MoveAmp.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.MMT_MoveAmp.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.MMT_MoveAmp.Adr = 620;
            _pmcBom.MMT_MoveAmp.Bit = 7;

            _pmcBom.MMT_SafeDoorOpen = new PmcBomItem();
            _pmcBom.MMT_SafeDoorOpen.Id = "PMC3110";
            _pmcBom.MMT_SafeDoorOpen.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.MMT_SafeDoorOpen.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.MMT_SafeDoorOpen.Adr = 621;
            _pmcBom.MMT_SafeDoorOpen.Bit = 0;

            _pmcBom.MMT_MoveOutBTN = new PmcBomItem();
            _pmcBom.MMT_MoveOutBTN.Id = "PMC3111";
            _pmcBom.MMT_MoveOutBTN.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.MMT_MoveOutBTN.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.MMT_MoveOutBTN.Adr = 621;
            _pmcBom.MMT_MoveOutBTN.Bit = 1;

            _pmcBom.MMT_MoveOutFin = new PmcBomItem();
            _pmcBom.MMT_MoveOutFin.Id = "PMC3112";
            _pmcBom.MMT_MoveOutFin.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.MMT_MoveOutFin.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.MMT_MoveOutFin.Adr = 621;
            _pmcBom.MMT_MoveOutFin.Bit = 2;

            _pmcBom.MMT_TwinTable = new PmcBomItem();
            _pmcBom.MMT_TwinTable.Id = "PMC3113";
            _pmcBom.MMT_TwinTable.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.MMT_TwinTable.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.MMT_TwinTable.Adr = 621;
            _pmcBom.MMT_TwinTable.Bit = 3;

            _pmcBom.MMT_MoveInBTN = new PmcBomItem();
            _pmcBom.MMT_MoveInBTN.Id = "PMC3114";
            _pmcBom.MMT_MoveInBTN.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.MMT_MoveInBTN.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.MMT_MoveInBTN.Adr = 621;
            _pmcBom.MMT_MoveInBTN.Bit = 4;

            _pmcBom.MMT_MoveInFin = new PmcBomItem();
            _pmcBom.MMT_MoveInFin.Id = "PMC3115";
            _pmcBom.MMT_MoveInFin.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.MMT_MoveInFin.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.MMT_MoveInFin.Adr = 621;
            _pmcBom.MMT_MoveInFin.Bit = 5;

            _pmcBom.MMT_ClampBTN = new PmcBomItem();
            _pmcBom.MMT_ClampBTN.Id = "PMC3116";
            _pmcBom.MMT_ClampBTN.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.MMT_ClampBTN.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.MMT_ClampBTN.Adr = 621;
            _pmcBom.MMT_ClampBTN.Bit = 6;

            _pmcBom.MMT_In_Mag = new PmcBomItem();
            _pmcBom.MMT_In_Mag.Id = "PMC3117";
            _pmcBom.MMT_In_Mag.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.MMT_In_Mag.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.MMT_In_Mag.Adr = 621;
            _pmcBom.MMT_In_Mag.Bit = 7;

            _pmcBom.MMT_ClampTimeOut = new PmcBomItem();
            _pmcBom.MMT_ClampTimeOut.Id = "PMC3118";
            _pmcBom.MMT_ClampTimeOut.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.MMT_ClampTimeOut.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.MMT_ClampTimeOut.Adr = 622;
            _pmcBom.MMT_ClampTimeOut.Bit = 1;

            _pmcBom.MMT_ClampFin = new PmcBomItem();
            _pmcBom.MMT_ClampFin.Id = "PMC3118";
            _pmcBom.MMT_ClampFin.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.MMT_ClampFin.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.MMT_ClampFin.Adr = 622;
            _pmcBom.MMT_ClampFin.Bit = 0;

            #endregion

            #region 行程操作提示
            _pmcBom.MTO_ServoReady = new PmcBomItem();
            _pmcBom.MTO_ServoReady.Id = "PMC3200";
            _pmcBom.MTO_ServoReady.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.MTO_ServoReady.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.MTO_ServoReady.Adr = 600;
            _pmcBom.MTO_ServoReady.Bit = 0;

            _pmcBom.MTO_BL = new PmcBomItem();
            _pmcBom.MTO_BL.Id = "PMC3201";
            _pmcBom.MTO_BL.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.MTO_BL.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.MTO_BL.Adr = 600;
            _pmcBom.MTO_BL.Bit = 1;

            _pmcBom.MTO_Lubrication = new PmcBomItem();
            _pmcBom.MTO_Lubrication.Id = "PMC3202";
            _pmcBom.MTO_Lubrication.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.MTO_Lubrication.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.MTO_Lubrication.Adr = 600;
            _pmcBom.MTO_Lubrication.Bit = 2;

            _pmcBom.MTO_AirPress = new PmcBomItem();
            _pmcBom.MTO_AirPress.Id = "PMC3203";
            _pmcBom.MTO_AirPress.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.MTO_AirPress.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.MTO_AirPress.Adr = 600;
            _pmcBom.MTO_AirPress.Bit = 3;

            _pmcBom.MTO_PE = new PmcBomItem();
            _pmcBom.MTO_PE.Id = "PMC3204";
            _pmcBom.MTO_PE.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.MTO_PE.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.MTO_PE.Adr = 600;
            _pmcBom.MTO_PE.Bit = 4;

            _pmcBom.MTO_EMG = new PmcBomItem();
            _pmcBom.MTO_EMG.Id = "PMC3205";
            _pmcBom.MTO_EMG.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.MTO_EMG.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.MTO_EMG.Adr = 600;
            _pmcBom.MTO_EMG.Bit = 5;

            _pmcBom.MTO_SafeCock = new PmcBomItem();
            _pmcBom.MTO_SafeCock.Id = "PMC3206";
            _pmcBom.MTO_SafeCock.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.MTO_SafeCock.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.MTO_SafeCock.Adr = 600;
            _pmcBom.MTO_SafeCock.Bit = 6;

            _pmcBom.MTO_TableClamp = new PmcBomItem();
            _pmcBom.MTO_TableClamp.Id = "PMC3207";
            _pmcBom.MTO_TableClamp.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.MTO_TableClamp.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.MTO_TableClamp.Adr = 600;
            _pmcBom.MTO_TableClamp.Bit = 7;

            _pmcBom.MTO_DieClamp = new PmcBomItem();
            _pmcBom.MTO_DieClamp.Id = "PMC3208";
            _pmcBom.MTO_DieClamp.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.MTO_DieClamp.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.MTO_DieClamp.Adr = 601;
            _pmcBom.MTO_DieClamp.Bit = 0;

            _pmcBom.MTO_SafeDoorClose = new PmcBomItem();
            _pmcBom.MTO_SafeDoorClose.Id = "PMC3209";
            _pmcBom.MTO_SafeDoorClose.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.MTO_SafeDoorClose.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.MTO_SafeDoorClose.Adr = 601;
            _pmcBom.MTO_SafeDoorClose.Bit = 1;

            _pmcBom.MTO_Counter = new PmcBomItem();
            _pmcBom.MTO_Counter.Id = "PMC3210";
            _pmcBom.MTO_Counter.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.MTO_Counter.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.MTO_Counter.Adr = 601;
            _pmcBom.MTO_Counter.Bit = 2;

            _pmcBom.MTO_JOG = new PmcBomItem();
            _pmcBom.MTO_JOG.Id = "PMC3211";
            _pmcBom.MTO_JOG.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.MTO_JOG.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.MTO_JOG.Adr = 601;
            _pmcBom.MTO_JOG.Bit = 3;

            _pmcBom.MTO_JOGOK = new PmcBomItem();
            _pmcBom.MTO_JOGOK.Id = "PMC3211";
            _pmcBom.MTO_JOGOK.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.MTO_JOGOK.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.MTO_JOGOK.Adr = 601;
            _pmcBom.MTO_JOGOK.Bit = 4;

            _pmcBom.MTO_UpDieCenter = new PmcBomItem();
            _pmcBom.MTO_UpDieCenter.Id = "PMC3212";
            _pmcBom.MTO_UpDieCenter.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.MTO_UpDieCenter.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.MTO_UpDieCenter.Adr = 601;
            _pmcBom.MTO_UpDieCenter.Bit = 5;

            _pmcBom.MTO_Single = new PmcBomItem();
            _pmcBom.MTO_Single.Id = "PMC3213";
            _pmcBom.MTO_Single.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.MTO_Single.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.MTO_Single.Adr = 601;
            _pmcBom.MTO_Single.Bit = 6;

            _pmcBom.MTO_SingleOK = new PmcBomItem();
            _pmcBom.MTO_SingleOK.Id = "PMC3214";
            _pmcBom.MTO_SingleOK.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.MTO_SingleOK.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.MTO_SingleOK.Adr = 602;
            _pmcBom.MTO_SingleOK.Bit = 0;

            _pmcBom.MTO_Continue = new PmcBomItem();
            _pmcBom.MTO_Continue.Id = "PMC3215";
            _pmcBom.MTO_Continue.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.MTO_Continue.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.MTO_Continue.Adr = 601;
            _pmcBom.MTO_Continue.Bit = 7;

            _pmcBom.MTO_ContinueOK = new PmcBomItem();
            _pmcBom.MTO_ContinueOK.Id = "PMC3215";
            _pmcBom.MTO_ContinueOK.AdrType = PmcAdrTypeEnum.E;
            _pmcBom.MTO_ContinueOK.DataType = PmcDataTypeEnum.BIT;
            _pmcBom.MTO_ContinueOK.Adr = 602;
            _pmcBom.MTO_ContinueOK.Bit = 1;
            #endregion


            var jsonPmcBom = JsonConvert.SerializeObject(_pmcBom, Formatting.Indented);
            using (StreamWriter sw = new StreamWriter(@"pmcbom.cfg", false))
            {
                sw.Write(jsonPmcBom);
            }

            #endregion

            #region 配方
            
            var obj = CurMacroBom.GetType();
            foreach (PropertyInfo item in obj.GetProperties())
            {
                var bomItem = item.GetValue(CurMacroBom) as MacroBomItem;
                if (bomItem?.IsRecipes ?? false)
                {
                    _recipes.MacroBoms.Add(new MacroBomItemRecipes()
                    {
                        Id = bomItem.Id,
                        Name= item.GetCustomAttribute<DisplayAttribute>().Name,
                        Adr = bomItem.Adr,
                        Value = "0",
                        IsRecipes = bomItem.IsRecipes
                    });
                }
            }

            var obj_pmc = CurPmcBom.GetType();
            foreach (PropertyInfo item in obj_pmc.GetProperties())
            {
                var bomItem = item.GetValue(CurPmcBom) as PmcBomItem;
                if (bomItem?.IsRecipes ?? false)
                {
                    _recipes.PmcBoms.Add(new PmcBomItemRecipes()
                    {
                        Id = bomItem.Id,
                        Name = item.GetCustomAttribute<DisplayAttribute>().Name,
                        AdrType = bomItem.AdrType,
                        DataType = bomItem.DataType,
                        Adr = bomItem.Adr,
                        Bit = bomItem.Bit,
                        ConversionFactor = bomItem.ConversionFactor,
                        IsRecipes = bomItem.IsRecipes,
                        Value = "0"
                    });
                }
            }

            #endregion

            #region 基础信息
            _baseInfo.Ip = "10.10.10.10";
            _baseInfo.Port = 8193;
            _baseInfo.Timeout = 1000;
            _baseInfo.Increment = 1000.0;
            _baseInfo.CsdFolder = @"C:\Program Files (x86)\CNCScreenE";
            _baseInfo.SciChartXTimeMax = 10;
            _baseInfo.RealTimeSciChartInflgAdrType = 12;
            _baseInfo.RealTimeSciChartInflgAdr = 444;
            _baseInfo.RealTimeSciChartInflgBit = 0;

            _baseInfo.SimulateSciChartInflgAdrType = 12;
            _baseInfo.SimulateSciChartInflgAdr = 444;
            _baseInfo.SimulateSciChartInflgBit = 1;

            var jsonBaseInfo = JsonConvert.SerializeObject(_baseInfo, Formatting.Indented);
            using (StreamWriter sw = new StreamWriter(@"baseinfo.cfg", false))
            {
                sw.Write(jsonBaseInfo);
            }
            #endregion

            return 0;
        }

        #endregion

        #region 即时连接
        public string GetPmc<T>(PmcBomItem pmc, ref T data)
        {
            if (pmc == null) return "设定数据失败,PMC配置信息不完整";

            ushort flib = 0;
            var ret = BuildConnect(ref flib);
            if (ret != 0) return "设定数据失败,连接失败";

            ret = GetPmc(_pmcBom.Mode, ref data, flib);

            FreeConnect(flib);

            if (ret != 0) return "获取数据失败,CNC数据读取失败";
            return null;
        }

        public string SetPmc(PmcBomItem pmc, LimitBomItem limit, string data)
        {
            if (pmc == null) return "设定数据失败,PMC配置信息不完整";

            ushort flib = 0;
            var ret = BuildConnect(ref flib);
            if (ret != 0) return "设定数据失败,连接失败";

            var res = SetPmc_InTask(flib, pmc, limit, data);

            FreeConnect(flib);

            return res;

        }

        public string SetSlidingTableDataPmc(PmcBomItem pmc, LimitBomItem limit, string data)
        {
            if (pmc == null) return "设定数据失败,PMC配置信息不完整";

            ushort flib = 0;
            var ret = BuildConnect(ref flib);
            if (ret != 0) return "设定数据失败,连接失败";

            var res = SetSlidingTableDataPmc_InTask(flib, pmc, limit, data);

            FreeConnect(flib);

            return res;

        }
        

        public string ChangePmcBit(PmcBomItem pmc)
        {
            if (pmc == null) return "设定数据失败,PMC配置信息不完整";
            if (pmc.DataType != PmcDataTypeEnum.BIT) return "设定数据失败,PMC类型非开关量";

            ushort flib = 0;
            var ret = BuildConnect(ref flib);
            if (ret != 0) return "设定数据失败,连接失败";

            double temp = 0;
            GetPmc(pmc, ref temp, flib);

            string set_ret;
            if (temp > 0) set_ret = SetPmc_InTask(flib, pmc, null, "false");
            else set_ret = SetPmc_InTask(flib, pmc, null, "true");

            FreeConnect(flib);

            return set_ret;

        }

        public string SetMacro(MacroBomItem macro, LimitBomItem limit, string data)
        {
            if (macro == null) return "设定数据失败,PMC配置信息不完整";

            ushort flib = 0;
            var ret = BuildConnect(ref flib);
            if (ret != 0) return "设定数据失败,连接失败";

            var res = SetMacro_InTask(flib, macro, limit, data);

            FreeConnect(flib);

            return res;
        }

        public string GetMacro(MacroBomItem macro, ref string data)
        {
            if (macro == null) return "获得数据失败,宏变量配置信息不完整";

            ushort flib = 0;
            var ret = BuildConnect(ref flib);
            if (ret != 0) return "获得数据失败,连接失败";

            double temp_data = 0;
            ret = GetMacro(macro.Adr, ref temp_data, flib);
            

            FreeConnect(flib);

            if (ret != 0) return "获得数据失败(" + ret.ToString() + ")";

            data = temp_data.ToString();
            return null;
        }

        public string SaveRecipesToPC(string path)
        {
            //RecipesInfo recipes = new RecipesInfo();
            //var obj = CurMacroBom.GetType();
            //foreach (PropertyInfo item in obj.GetProperties())
            //{
            //    var bomItem = item.GetValue(CurMacroBom) as MacroBomItem;
            //    if (bomItem?.IsRecipes??false)
            //    {
            //        string temp_data = "";
            //        var ret = GetMacro(bomItem, ref temp_data);
            //        if(ret!=null)
            //        {
            //            return "保存配方失败，写入宏变量失败，返回:" + ret;
            //        }

            //        recipes.MacroBoms.Add(new MacroBomItemRecipes()
            //        {
            //            Id = bomItem.Id,
            //            Adr = bomItem.Adr,
            //            Value= temp_data,
            //            IsRecipes = bomItem.IsRecipes
            //        });
            //    }
            //}

            //var obj_pmc = CurPmcBom.GetType();
            //foreach (PropertyInfo item in obj_pmc.GetProperties())
            //{
            //    var bomItem = item.GetValue(CurPmcBom) as PmcBomItem;
            //    if (bomItem?.IsRecipes ?? false)
            //    {
            //        string ret;
            //        string data_res="";
            //        switch (bomItem.DataType)
            //        {
            //            case PmcDataTypeEnum.BIT:
            //                bool bTemp = false;
            //                ret = GetPmc(bomItem,ref bTemp);
            //                data_res = bTemp.ToString();
            //                break;
            //            case PmcDataTypeEnum.BYTE:
            //                byte cTemp = 0;
            //                ret = GetPmc(bomItem, ref cTemp);
            //                data_res = cTemp.ToString();
            //                break;
            //            case PmcDataTypeEnum.WORD:
            //                short iTemp = 0;
            //                ret = GetPmc(bomItem, ref iTemp);
            //                data_res = iTemp.ToString();
            //                break;
            //            case PmcDataTypeEnum.LONG:
            //                int lTemp = 0;
            //                ret = GetPmc(bomItem, ref lTemp);
            //                data_res = lTemp.ToString();
            //                break;
            //            default:
            //                ret = "类型错误";
            //                break;
            //        }

            //        if (ret != null)
            //        {
            //            return "保存配方失败，写入PMC失败，返回:" + ret;
            //        }

            //        recipes.PmcBoms.Add(new PmcBomItemRecipes() {
            //            Id = bomItem.Id,
            //            AdrType = bomItem.AdrType,
            //            DataType = bomItem.DataType,
            //            Adr = bomItem.Adr,
            //            Bit = bomItem.Bit,
            //            ConversionFactor = bomItem.ConversionFactor,
            //            IsRecipes = bomItem.IsRecipes,
            //            Value = data_res
            //        });
            //    }
            //}

            var jsonData = JsonConvert.SerializeObject(_recipes, Formatting.Indented);
            using (System.IO.StreamWriter sw =new StreamWriter(path))
            {
                sw.WriteLine(jsonData);
            }
            
            return null;

        }

        public string AutoSetDieClosingPara()
        {


            ushort flib = 0;
            var ret = BuildConnect(ref flib);
            if (ret != 0) return "设定数据失败,连接失败";

            double temp_data = 0;
            ret = GetPmc(_pmcBom.DJP_SectionNum, ref temp_data, flib);
            if (ret != 0) return "自动设定数据失败,合模段数获得失败";

            double bdc = 0;
            ret = GetPmc(_pmcBom.DJP_BottomDeadCentre, ref bdc, flib);
            if (ret != 0) return "自动设定数据失败,下死点位置获取失败";

            double bdc_s = 0;
            ret = GetPmc(_pmcBom.DJP_Speed_BottomDeadCentre, ref bdc_s, flib);
            if (ret != 0) return "自动设定数据失败,下死点速度获取失败";

            if (temp_data < 8)
            {
                var ret_pmc = SetPmc_InTask(flib,_pmcBom.DJP_Pos_8, _limitBom.DJP_Pos_8, bdc.ToString());
                if (ret_pmc != null) return "自动设定数据失败";
                ret_pmc = SetPmc_InTask(flib, _pmcBom.DJP_Speed_8, _limitBom.DJP_Speed_8, bdc_s.ToString());
                if (ret_pmc != null) return "自动设定数据失败";
            }
            if (temp_data < 7)
            {
                var ret_pmc = SetPmc_InTask(flib, _pmcBom.DJP_Pos_7, _limitBom.DJP_Pos_7, bdc.ToString());
                if (ret_pmc != null) return "自动设定数据失败";
                ret_pmc = SetPmc_InTask(flib, _pmcBom.DJP_Speed_7, _limitBom.DJP_Speed_7, bdc_s.ToString());
                if (ret_pmc != null) return "自动设定数据失败";
            };
            if (temp_data < 6)
            {
                var ret_pmc = SetPmc_InTask(flib, _pmcBom.DJP_Pos_6, _limitBom.DJP_Pos_6, bdc.ToString());
                if (ret_pmc != null) return "自动设定数据失败";
                ret_pmc = SetPmc_InTask(flib, _pmcBom.DJP_Speed_6, _limitBom.DJP_Speed_6, bdc_s.ToString());
                if (ret_pmc != null) return "自动设定数据失败";
            }
            if (temp_data < 5)
            {
                var ret_pmc = SetPmc_InTask(flib, _pmcBom.DJP_Pos_5, _limitBom.DJP_Pos_5, bdc.ToString());
                if (ret_pmc != null) return "自动设定数据失败";
                ret_pmc = SetPmc_InTask(flib, _pmcBom.DJP_Speed_5, _limitBom.DJP_Speed_5, bdc_s.ToString());
                if (ret_pmc != null) return "自动设定数据失败";
            }
            if (temp_data < 4)
            {
                var ret_pmc = SetPmc_InTask(flib, _pmcBom.DJP_Pos_4, _limitBom.DJP_Pos_4, bdc.ToString());
                if (ret_pmc != null) return "自动设定数据失败";
                ret_pmc = SetPmc_InTask(flib, _pmcBom.DJP_Speed_4, _limitBom.DJP_Speed_4, bdc_s.ToString());
                if (ret_pmc != null) return "自动设定数据失败";
            }
            if (temp_data < 3)
            {
                var ret_pmc = SetPmc_InTask(flib, _pmcBom.DJP_Pos_3, _limitBom.DJP_Pos_3, bdc.ToString());
                if (ret_pmc != null) return "自动设定数据失败";
                ret_pmc = SetPmc_InTask(flib, _pmcBom.DJP_Speed_3, _limitBom.DJP_Speed_3, bdc_s.ToString());
                if (ret_pmc != null) return "自动设定数据失败";
            }
            if (temp_data < 2)
            {
                var ret_pmc = SetPmc_InTask(flib, _pmcBom.DJP_Pos_2, _limitBom.DJP_Pos_2, bdc.ToString());
                if (ret_pmc != null) return "自动设定数据失败";
                ret_pmc = SetPmc_InTask(flib, _pmcBom.DJP_Speed_2, _limitBom.DJP_Speed_2, bdc_s.ToString());
                if (ret_pmc != null) return "自动设定数据失败";
            }
            if (temp_data < 1)
            {
                var ret_pmc = SetPmc_InTask(flib, _pmcBom.DJP_Pos_1, _limitBom.DJP_Pos_1, bdc.ToString());
                if (ret_pmc != null) return "自动设定数据失败";
                ret_pmc = SetPmc_InTask(flib, _pmcBom.DJP_Speed_1, _limitBom.DJP_Speed_1, bdc_s.ToString());
                if (ret_pmc != null) return "自动设定数据失败";
            }

            FreeConnect(flib);
            return null;
        }


        public string AutoSetDiePartingPara()
        {

            ushort flib = 0;
            var ret = BuildConnect(ref flib);
            if (ret != 0) return "设定数据失败,连接失败";

            double temp_data = 0;
            ret = GetPmc(_pmcBom.DPP_SectionNum, ref temp_data, flib);
            if (ret != 0) return "自动设定数据失败,分模段数获得失败";

            double bdc = 0;
            ret = GetPmc(_pmcBom.DPP_TopDeadCentre, ref bdc, flib);
            if (ret != 0) return "自动设定数据失败,上死点位置获取失败";

            double bdc_s = 0;
            ret = GetPmc(_pmcBom.DPP_Speed_TopDeadCentre, ref bdc_s, flib);
            if (ret != 0) return "自动设定数据失败,上死点速度获取失败";

            if (temp_data < 8)
            {
                var ret_pmc = SetPmc_InTask(flib, _pmcBom.DPP_Pos_8, _limitBom.DPP_Pos_8, bdc.ToString());
                if (ret_pmc != null) return "自动设定数据失败";
                ret_pmc = SetPmc_InTask(flib, _pmcBom.DPP_Speed_8, _limitBom.DPP_Speed_8, bdc_s.ToString());
                if (ret_pmc != null) return "自动设定数据失败";
            }
            if (temp_data < 7)
            {
                var ret_pmc = SetPmc_InTask(flib, _pmcBom.DPP_Pos_7, _limitBom.DPP_Pos_7, bdc.ToString());
                if (ret_pmc != null) return "自动设定数据失败";
                ret_pmc = SetPmc_InTask(flib, _pmcBom.DPP_Speed_7, _limitBom.DPP_Speed_7, bdc_s.ToString());
                if (ret_pmc != null) return "自动设定数据失败";
            };
            if (temp_data < 6)
            {
                var ret_pmc = SetPmc_InTask(flib, _pmcBom.DPP_Pos_6, _limitBom.DPP_Pos_6, bdc.ToString());
                if (ret_pmc != null) return "自动设定数据失败";
                ret_pmc = SetPmc_InTask(flib, _pmcBom.DPP_Speed_6, _limitBom.DPP_Speed_6, bdc_s.ToString());
                if (ret_pmc != null) return "自动设定数据失败";
            }
            if (temp_data < 5)
            {
                var ret_pmc = SetPmc_InTask(flib, _pmcBom.DPP_Pos_5, _limitBom.DPP_Pos_5, bdc.ToString());
                if (ret_pmc != null) return "自动设定数据失败";
                ret_pmc = SetPmc_InTask(flib, _pmcBom.DPP_Speed_5, _limitBom.DPP_Speed_5, bdc_s.ToString());
                if (ret_pmc != null) return "自动设定数据失败";
            }
            if (temp_data < 4)
            {
                var ret_pmc = SetPmc_InTask(flib, _pmcBom.DPP_Pos_4, _limitBom.DPP_Pos_4, bdc.ToString());
                if (ret_pmc != null) return "自动设定数据失败";
                ret_pmc = SetPmc_InTask(flib, _pmcBom.DPP_Speed_4, _limitBom.DPP_Speed_4, bdc_s.ToString());
                if (ret_pmc != null) return "自动设定数据失败";
            }
            if (temp_data < 3)
            {
                var ret_pmc = SetPmc_InTask(flib, _pmcBom.DPP_Pos_3, _limitBom.DPP_Pos_3, bdc.ToString());
                if (ret_pmc != null) return "自动设定数据失败";
                ret_pmc = SetPmc_InTask(flib, _pmcBom.DPP_Speed_3, _limitBom.DPP_Speed_3, bdc_s.ToString());
                if (ret_pmc != null) return "自动设定数据失败";
            }
            if (temp_data < 2)
            {
                var ret_pmc = SetPmc_InTask(flib, _pmcBom.DPP_Pos_2, _limitBom.DPP_Pos_2, bdc.ToString());
                if (ret_pmc != null) return "自动设定数据失败";
                ret_pmc = SetPmc_InTask(flib, _pmcBom.DPP_Speed_2, _limitBom.DPP_Speed_2, bdc_s.ToString());
                if (ret_pmc != null) return "自动设定数据失败";
            }
            if (temp_data < 1)
            {
                var ret_pmc = SetPmc_InTask(flib, _pmcBom.DPP_Pos_1, _limitBom.DPP_Pos_1, bdc.ToString());
                if (ret_pmc != null) return "自动设定数据失败";
                ret_pmc = SetPmc_InTask(flib, _pmcBom.DPP_Speed_1, _limitBom.DPP_Speed_1, bdc_s.ToString());
                if (ret_pmc != null) return "自动设定数据失败";
            }

            FreeConnect(flib);
            return null;
        }

        #endregion

        #region 扫描

        #region 静态
        private void ScannerStaticFunc(object sender, DoWorkEventArgs e)
        {
            short ret = -16;
            short conn = -16;
            DateTime temp_time=DateTime.Now;
            double temp_pos=0;



            while (m_static_BackgroundWorker.CancellationPending == false)
            {
                Thread.Sleep(m_static_freq);

                if (conn != 0)
                {
                    FreeConnect(m_static_flib);

                    ret = BuildConnect(ref m_static_flib);
                    if (ret == 0) conn = 0;
                }

                m_static_info.Increment = _baseInfo.Increment;

                #region CNC状态
                Focas1.ODBST odbst = new Focas1.ODBST();
                ret = Focas1.cnc_statinfo(m_static_flib, odbst);
                if (ret == -16)
                {
                    m_static_info.LampStatus = 4;
                    conn = -16;
                }
                else if (ret == 0)
                {
                    if (odbst.alarm == 1)//故障
                    {
                        m_static_info.LampStatus = 3;
                    }
                    else if (odbst.run > 0)//运行
                    {
                        m_static_info.LampStatus = 1;
                    }
                    else if (odbst.emergency > 0)//待机(急停中)
                    {
                        m_static_info.LampStatus = 2;
                    }
                    else//待机(非急停)
                    {
                        m_static_info.LampStatus = 2;
                    }
                }
                else
                {
                    m_static_info.LampStatus = 4;
                }
                #endregion

                #region 读取报警信息
                Focas1.ODBALMMSG2 almmsg = new Focas1.ODBALMMSG2();
                short alarm_num = 10;
                m_static_info.CncAlarmFlag = false;
                var ret_alm = Focas1.cnc_rdalmmsg2(m_static_flib, -1, ref alarm_num, almmsg);
                if (ret_alm == 0)
                {

                    if (alarm_num != 0) m_static_info.CncAlarmFlag = true;

                    m_static_info.CncAlarmList.Clear();
                    for (int i = 0; i < alarm_num; i++)
                    {
                        try
                        {
                            string strdata = "msg" + (i + 1).ToString();
                            object alm = almmsg.GetType().GetField(strdata).GetValue(almmsg);

                            int alm_no = int.Parse(alm.GetType().GetField("alm_no").GetValue(alm).ToString());
                            short type = short.Parse(alm.GetType().GetField("type").GetValue(alm).ToString());
                            short axis = short.Parse(alm.GetType().GetField("axis").GetValue(alm).ToString());
                            short len = short.Parse(alm.GetType().GetField("msg_len").GetValue(alm).ToString());


                            byte[] alm_msg_array = Encoding.GetEncoding("GBK").GetBytes(alm.GetType().GetField("alm_msg").GetValue(alm).ToString());
                            var alm_msg = System.Text.Encoding.Default.GetString(alm_msg_array.Take(len).ToArray());

                            m_static_info.CncAlarmList.Add(new CncAlarm { Alm_No = alm_no, Type = type, Axis = axis, Alm_Msg = alm_msg });
                        }
                        catch { }


                        //alm_msg = alm_msg.Replace("\n", "");

                        
                    }

                }
                else
                {
                    m_static_info.CncAlarmList.Clear();
                }

                if (_slidingBlockTableLoaded==false)
                {
                    m_static_info.CncAlarmList.Add(new CncAlarm { Alm_No = 0, Type = 19, Axis = 0, Alm_Msg = "加载丝杠滑块数据表失败" });
                }

                #endregion

                #region 滑块位移与速度                
                if (_slidingBlockTableLoaded == true)
                {
                    Focas1.ODBAXIS mac13 = new Focas1.ODBAXIS();
                    Focas1.cnc_machine(m_static_flib, 13, 8, mac13);

                    Focas1.ODBAXIS mac14 = new Focas1.ODBAXIS();
                    Focas1.cnc_machine(m_static_flib, 14, 8, mac14);

                    m_static_info.SliderPosition = (double)mac13.data[0] / _baseInfo.Increment - (double)mac14.data[0] / _baseInfo.Increment;


                   var time_span = (DateTime.Now - temp_time).TotalMilliseconds;
                    m_static_info.SliderSpeed = Math.Round((m_static_info.SliderPosition - temp_pos) / time_span * 1000.0, 3);
                   temp_pos = m_static_info.SliderPosition;
                   temp_time = DateTime.Now;
                }

                #endregion

                #region 程序名称
                StringBuilder str = new StringBuilder();
                var ret_pn = Focas1.cnc_pdf_rdmain(m_static_flib, str);
                if (ret_pn == 0)
                {
                    string[] sArray = str.ToString().Split('/');
                    m_static_info.WorkPartName = sArray[sArray.Count() - 1];
                }
                else
                {
                    m_static_info.WorkPartName = "";
                }

                #endregion

                #region 其他状态信息
                int mode_temp = 0;
                GetPmc(_pmcBom.Mode, ref mode_temp, m_static_flib);
                m_static_info.Mode = mode_temp;

                int mainstatus_temp = 0;
                GetPmc(_pmcBom.MainStatus, ref mainstatus_temp, m_static_flib);
                m_static_info.MainStatus = mainstatus_temp;

                double sp_temp = 0;
                GetPmc(_pmcBom.SliderPressure, ref sp_temp, m_static_flib);
                m_static_info.SliderPressure = sp_temp;

                double bcp_temp = 0;
                GetPmc(_pmcBom.BalanceCylinderPressure, ref bcp_temp, m_static_flib);
                m_static_info.BalanceCylinderPressure = bcp_temp;

                double idh_temp = 0;
                GetPmc(_pmcBom.InstallDieHigh, ref idh_temp, m_static_flib);
                m_static_info.InstallDieHigh = idh_temp;

                double nt_temp = 0;
                GetPmc(_pmcBom.NutTemperature, ref nt_temp, m_static_flib);
                m_static_info.NutTemperature = nt_temp;

                int tp_temp = 0;
                GetPmc(_pmcBom.TotalPiece, ref tp_temp, m_static_flib);
                m_static_info.TotalPiece = tp_temp;

                int tw_temp = 0;
                GetPmc(_pmcBom.TotalWork, ref tw_temp, m_static_flib);
                m_static_info.TotalWork = tw_temp;

                int dp_temp = 0;
                GetPmc(_pmcBom.DayPiece, ref dp_temp, m_static_flib);
                m_static_info.DayPiece = dp_temp;

                int dw_temp = 0;
                GetPmc(_pmcBom.DayWork, ref dw_temp, m_static_flib);
                m_static_info.DayWork = dw_temp;

                #endregion


                Messenger.Default.Send<CncStaticInfo>(m_static_info, "CncStaticInfoMsg");

            }

            FreeConnect(m_static_flib);
        }

        public void ScannerStatic_Start()
        {
            //TODO:NO CNC
            if (_simulate == false) m_static_BackgroundWorker.RunWorkerAsync();
        }

        public void ScannerStatic_Cancel()
        {
            m_static_BackgroundWorker.CancelAsync();
        }

        private void ScannerStaticCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            FreeConnect(m_static_flib);
            m_static_flib = 0;
        }

        #endregion

        #region 页
        private void ScannerPageFunc(object sender, DoWorkEventArgs e)
        {
            short ret = 0;
            short conn = -16;

            while (m_page_BackgroundWorker.CancellationPending == false)
            {
                Thread.Sleep(m_page_freq);

                if (conn != 0)
                {
                    FreeConnect(m_page_flib);

                    ret = BuildConnect(ref m_page_flib);
                    if (ret == 0) conn = 0;
                }

                #region 状态监控
                if (m_statemonitor == true)
                {
                    m_statemonitor_info.LineChartFlag = m_monitorline_indo;
                    m_statemonitor_info.Increment = _baseInfo.Increment;

                    double downdelay_temp = 0;
                    GetMacro(_macroBom.DownDelayTime.Adr, ref downdelay_temp, m_page_flib);
                    m_statemonitor_info.DownDelayTime = downdelay_temp;

                    double downtime_temp = 0;
                    GetMacro(_macroBom.DownTime.Adr, ref downtime_temp, m_page_flib);
                    m_statemonitor_info.DownTime = downtime_temp;

                    double spc_temp = 0;
                    GetMacro(_macroBom.SavePressureCount.Adr, ref spc_temp, m_page_flib);
                    m_statemonitor_info.SavePressureCount = spc_temp;

                    double updelay_temp = 0;
                    GetMacro(_macroBom.UpDelayTime.Adr, ref updelay_temp, m_page_flib);
                    m_statemonitor_info.UpDelayTime = updelay_temp;

                    double uptime_temp = 0;
                    GetMacro(_macroBom.UpTime.Adr, ref uptime_temp, m_page_flib);
                    m_statemonitor_info.UpTime = uptime_temp;

                    int mode_temp = 0;
                    GetPmc(_pmcBom.SMP_CylinderMode, ref mode_temp, m_page_flib);
                    m_statemonitor_info.CylinderMode = mode_temp;

                    int load_temp = 0;
                    GetPmc(_pmcBom.SMP_LoaderState, ref load_temp, m_page_flib);
                    m_statemonitor_info.LoaderState = load_temp;

                    int ws_temp = 0;
                    GetPmc(_pmcBom.SMP_WorkStep, ref ws_temp, m_page_flib);
                    m_statemonitor_info.WorkStep = ws_temp;

                    int wt_temp = 0;
                    GetPmc(_pmcBom.SMP_WorkTime, ref wt_temp, m_page_flib);
                    m_statemonitor_info.WorkTime = wt_temp;

                    int sp_temp = 0;
                    GetPmc(_pmcBom.SMP_SliderPressure, ref sp_temp, m_page_flib);
                    m_statemonitor_info.SliderPressure = sp_temp;

                    int st_temp = 0;
                    GetPmc(_pmcBom.SMP_SliderTemperature, ref st_temp, m_page_flib);
                    m_statemonitor_info.SliderTemperature = st_temp;



                    Messenger.Default.Send<StateMonitorInfo>(m_statemonitor_info, "StateMonitorInfoMsg");
                }

                #endregion

                #region 换模设定
                if (m_paradiechange == true)
                {
                    m_diechange_info.Increment = _baseInfo.Increment;

                    int jf_temp = 0;
                    GetPmc(_pmcBom.DCP_JogFeed, ref jf_temp, m_page_flib);
                    m_diechange_info.JogFeed = jf_temp;

                    double idh_temp = 0;
                    GetPmc(_pmcBom.DCP_InstallDieHighSet, ref idh_temp, m_page_flib);
                    m_diechange_info.InstallDieHighSet = idh_temp;

                    double idha_temp = 0;
                    GetPmc(_pmcBom.DCP_InstallDieHighActual, ref idha_temp, m_page_flib);
                    m_diechange_info.InstallDieHighActual = idha_temp;

                    double idha_cr = 0;
                    GetPmc(_pmcBom.DCP_CylinderpRressureSet, ref idha_cr, m_page_flib);
                    m_diechange_info.CylinderpRressureSet = idha_cr;

                    double idha_cra = 0;
                    GetPmc(_pmcBom.DCP_CylinderpRressureActual, ref idha_cra, m_page_flib);
                    m_diechange_info.CylinderpRressureActual = idha_cra;

                    double idha_dw = 0;
                    GetPmc(_pmcBom.DCP_DieWeight, ref idha_dw, m_page_flib);
                    m_diechange_info.DieWeight = idha_dw;

                    double cm_lsp = 0;
                    GetPmc(_pmcBom.DCP_ChangeMode, ref cm_lsp, m_page_flib);
                    m_diechange_info.ChangeMode = cm_lsp;

                    double mw_lsp = 0;
                    GetPmc(_pmcBom.DCP_MaxWeight, ref mw_lsp, m_page_flib);
                    m_diechange_info.MaxWeight = mw_lsp;

                    double mut_lsp = 0;
                    GetPmc(_pmcBom.DCP_MaxNugTemperature, ref mut_lsp, m_page_flib);
                    m_diechange_info.MaxNugTemperature = mut_lsp;

                    double mcp_lsp = 0;
                    GetPmc(_pmcBom.DCP_MinCylinderpRressure, ref mcp_lsp, m_page_flib);
                    m_diechange_info.MinCylinderpRressure = mcp_lsp;

                    Messenger.Default.Send<ParaDieChangeInfo>(m_diechange_info, "ParaDieChangeInfoMsg");
                }

                #endregion

                #region 夹模器设定
                if (m_paradieclamp == true)
                {
                    double cs1 = 0;
                    GetPmc(_pmcBom.CLS_ClampStatus1, ref cs1, m_page_flib);
                    m_dieclamp_info.ClampStatus1 = cs1;

                    double cs2 = 0;
                    GetPmc(_pmcBom.CLS_ClampStatus2, ref cs2, m_page_flib);
                    m_dieclamp_info.ClampStatus2 = cs2;

                    double crp = 0;
                    GetPmc(_pmcBom.CLS_ClampRelaxPosition, ref crp, m_page_flib);
                    m_dieclamp_info.ClampRelaxPosition = crp;

                    double crip = 0;
                    GetPmc(_pmcBom.CLS_ClampRelaxInPosition, ref crip, m_page_flib);
                    m_dieclamp_info.ClampRelaxInPosition = crip;

                    double f1e = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Front_1_Ebable, ref f1e, m_page_flib);
                    m_dieclamp_info.Clamp_Front_1_Ebable = f1e;

                    double f1mo = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Front_1_MoveOutStatus, ref f1mo, m_page_flib);
                    m_dieclamp_info.Clamp_Front_1_MoveOutStatus = f1mo;

                    double f1mi = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Front_1_MoveInStatus, ref f1mi, m_page_flib);
                    m_dieclamp_info.Clamp_Front_1_MoveInStatus = f1mi;

                    double f2e = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Front_2_Ebable, ref f2e, m_page_flib);
                    m_dieclamp_info.Clamp_Front_2_Ebable = f2e;

                    double f2mo = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Front_2_MoveOutStatus, ref f2mo, m_page_flib);
                    m_dieclamp_info.Clamp_Front_2_MoveOutStatus = f2mo;

                    double f2mi = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Front_2_MoveInStatus, ref f2mi, m_page_flib);
                    m_dieclamp_info.Clamp_Front_2_MoveInStatus = f2mi;

                    double f3e = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Front_3_Ebable, ref f3e, m_page_flib);
                    m_dieclamp_info.Clamp_Front_3_Ebable = f3e;

                    double f3mo = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Front_3_MoveOutStatus, ref f3mo, m_page_flib);
                    m_dieclamp_info.Clamp_Front_3_MoveOutStatus = f3mo;

                    double f3mi = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Front_3_MoveInStatus, ref f3mi, m_page_flib);
                    m_dieclamp_info.Clamp_Front_3_MoveInStatus = f3mi;

                    double f4e = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Front_4_Ebable, ref f4e, m_page_flib);
                    m_dieclamp_info.Clamp_Front_4_Ebable = f4e;

                    double f4mo = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Front_4_MoveOutStatus, ref f4mo, m_page_flib);
                    m_dieclamp_info.Clamp_Front_4_MoveOutStatus = f4mo;

                    double f4mi = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Front_4_MoveInStatus, ref f4mi, m_page_flib);
                    m_dieclamp_info.Clamp_Front_4_MoveInStatus = f4mi;

                    double f5e = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Front_5_Ebable, ref f5e, m_page_flib);
                    m_dieclamp_info.Clamp_Front_5_Ebable = f5e;

                    double f5mo = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Front_5_MoveOutStatus, ref f5mo, m_page_flib);
                    m_dieclamp_info.Clamp_Front_5_MoveOutStatus = f5mo;

                    double f5mi = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Front_5_MoveInStatus, ref f5mi, m_page_flib);
                    m_dieclamp_info.Clamp_Front_5_MoveInStatus = f5mi;

                    double f6e = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Front_6_Ebable, ref f6e, m_page_flib);
                    m_dieclamp_info.Clamp_Front_6_Ebable = f6e;

                    double f6mo = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Front_6_MoveOutStatus, ref f6mo, m_page_flib);
                    m_dieclamp_info.Clamp_Front_6_MoveOutStatus = f6mo;

                    double f6mi = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Front_6_MoveInStatus, ref f6mi, m_page_flib);
                    m_dieclamp_info.Clamp_Front_6_MoveInStatus = f6mi;

                    double f7e = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Front_7_Ebable, ref f7e, m_page_flib);
                    m_dieclamp_info.Clamp_Front_7_Ebable = f7e;

                    double f7mo = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Front_7_MoveOutStatus, ref f7mo, m_page_flib);
                    m_dieclamp_info.Clamp_Front_7_MoveOutStatus = f7mo;

                    double f7mi = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Front_7_MoveInStatus, ref f7mi, m_page_flib);
                    m_dieclamp_info.Clamp_Front_7_MoveInStatus = f7mi;

                    double f8e = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Front_8_Ebable, ref f8e, m_page_flib);
                    m_dieclamp_info.Clamp_Front_8_Ebable = f8e;

                    double f8mo = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Front_8_MoveOutStatus, ref f8mo, m_page_flib);
                    m_dieclamp_info.Clamp_Front_8_MoveOutStatus = f8mo;

                    double f8mi = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Front_8_MoveInStatus, ref f8mi, m_page_flib);
                    m_dieclamp_info.Clamp_Front_8_MoveInStatus = f8mi;

                    double f9e = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Front_9_Ebable, ref f9e, m_page_flib);
                    m_dieclamp_info.Clamp_Front_9_Ebable = f9e;

                    double f9mo = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Front_9_MoveOutStatus, ref f9mo, m_page_flib);
                    m_dieclamp_info.Clamp_Front_9_MoveOutStatus = f9mo;

                    double f9mi = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Front_9_MoveInStatus, ref f9mi, m_page_flib);
                    m_dieclamp_info.Clamp_Front_9_MoveInStatus = f9mi;

                    double f10e = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Front_10_Ebable, ref f10e, m_page_flib);
                    m_dieclamp_info.Clamp_Front_10_Ebable = f10e;

                    double f10mo = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Front_10_MoveOutStatus, ref f10mo, m_page_flib);
                    m_dieclamp_info.Clamp_Front_10_MoveOutStatus = f10mo;

                    double f10mi = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Front_10_MoveInStatus, ref f10mi, m_page_flib);
                    m_dieclamp_info.Clamp_Front_10_MoveInStatus = f10mi;

                    double f11e = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Front_11_Ebable, ref f11e, m_page_flib);
                    m_dieclamp_info.Clamp_Front_11_Ebable = f11e;

                    double f11mo = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Front_11_MoveOutStatus, ref f11mo, m_page_flib);
                    m_dieclamp_info.Clamp_Front_11_MoveOutStatus = f11mo;

                    double f11mi = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Front_11_MoveInStatus, ref f11mi, m_page_flib);
                    m_dieclamp_info.Clamp_Front_11_MoveInStatus = f11mi;

                    double f12e = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Front_12_Ebable, ref f12e, m_page_flib);
                    m_dieclamp_info.Clamp_Front_12_Ebable = f12e;

                    double f12mo = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Front_12_MoveOutStatus, ref f12mo, m_page_flib);
                    m_dieclamp_info.Clamp_Front_12_MoveOutStatus = f12mo;

                    double f12mi = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Front_12_MoveInStatus, ref f12mi, m_page_flib);
                    m_dieclamp_info.Clamp_Front_12_MoveInStatus = f12mi;

                    double f13e = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Front_13_Ebable, ref f13e, m_page_flib);
                    m_dieclamp_info.Clamp_Front_13_Ebable = f13e;

                    double f13mo = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Front_13_MoveOutStatus, ref f13mo, m_page_flib);
                    m_dieclamp_info.Clamp_Front_13_MoveOutStatus = f13mo;

                    double f13mi = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Front_13_MoveInStatus, ref f13mi, m_page_flib);
                    m_dieclamp_info.Clamp_Front_13_MoveInStatus = f13mi;

                    double b1e = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Back_1_Ebable, ref b1e, m_page_flib);
                    m_dieclamp_info.Clamp_Back_1_Ebable = b1e;

                    double b1mo = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Back_1_MoveOutStatus, ref b1mo, m_page_flib);
                    m_dieclamp_info.Clamp_Back_1_MoveOutStatus = b1mo;

                    double b1mi = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Back_1_MoveInStatus, ref b1mi, m_page_flib);
                    m_dieclamp_info.Clamp_Back_1_MoveInStatus = b1mi;

                    double b2e = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Back_2_Ebable, ref b2e, m_page_flib);
                    m_dieclamp_info.Clamp_Back_2_Ebable = b2e;

                    double b2mo = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Back_2_MoveOutStatus, ref b2mo, m_page_flib);
                    m_dieclamp_info.Clamp_Back_2_MoveOutStatus = b2mo;

                    double b2mi = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Back_2_MoveInStatus, ref b2mi, m_page_flib);
                    m_dieclamp_info.Clamp_Back_2_MoveInStatus = b2mi;

                    double b3e = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Back_3_Ebable, ref b3e, m_page_flib);
                    m_dieclamp_info.Clamp_Back_3_Ebable = b3e;

                    double b3mo = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Back_3_MoveOutStatus, ref b3mo, m_page_flib);
                    m_dieclamp_info.Clamp_Back_3_MoveOutStatus = b3mo;

                    double b3mi = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Back_3_MoveInStatus, ref b3mi, m_page_flib);
                    m_dieclamp_info.Clamp_Back_3_MoveInStatus = b3mi;

                    double b4e = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Back_4_Ebable, ref b4e, m_page_flib);
                    m_dieclamp_info.Clamp_Back_4_Ebable = b4e;

                    double b4mo = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Back_4_MoveOutStatus, ref b4mo, m_page_flib);
                    m_dieclamp_info.Clamp_Back_4_MoveOutStatus = b4mo;

                    double b4mi = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Back_4_MoveInStatus, ref b4mi, m_page_flib);
                    m_dieclamp_info.Clamp_Back_4_MoveInStatus = b4mi;

                    double b5e = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Back_5_Ebable, ref b5e, m_page_flib);
                    m_dieclamp_info.Clamp_Back_5_Ebable = b5e;

                    double b5mo = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Back_5_MoveOutStatus, ref b5mo, m_page_flib);
                    m_dieclamp_info.Clamp_Back_5_MoveOutStatus = b5mo;

                    double b5mi = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Back_5_MoveInStatus, ref b5mi, m_page_flib);
                    m_dieclamp_info.Clamp_Back_5_MoveInStatus = b5mi;

                    double b6e = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Back_6_Ebable, ref b6e, m_page_flib);
                    m_dieclamp_info.Clamp_Back_6_Ebable = b6e;

                    double b6mo = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Back_6_MoveOutStatus, ref b6mo, m_page_flib);
                    m_dieclamp_info.Clamp_Back_6_MoveOutStatus = b6mo;

                    double b6mi = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Back_6_MoveInStatus, ref b6mi, m_page_flib);
                    m_dieclamp_info.Clamp_Back_6_MoveInStatus = b6mi;

                    double b7e = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Back_7_Ebable, ref b7e, m_page_flib);
                    m_dieclamp_info.Clamp_Back_7_Ebable = b7e;

                    double b7mo = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Back_7_MoveOutStatus, ref b7mo, m_page_flib);
                    m_dieclamp_info.Clamp_Back_7_MoveOutStatus = b7mo;

                    double b7mi = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Back_7_MoveInStatus, ref b7mi, m_page_flib);
                    m_dieclamp_info.Clamp_Back_7_MoveInStatus = b7mi;

                    double b8e = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Back_8_Ebable, ref b8e, m_page_flib);
                    m_dieclamp_info.Clamp_Back_8_Ebable = b8e;

                    double b8mo = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Back_8_MoveOutStatus, ref b8mo, m_page_flib);
                    m_dieclamp_info.Clamp_Back_8_MoveOutStatus = b8mo;

                    double b8mi = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Back_8_MoveInStatus, ref b8mi, m_page_flib);
                    m_dieclamp_info.Clamp_Back_8_MoveInStatus = b8mi;

                    double b9e = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Back_9_Ebable, ref b9e, m_page_flib);
                    m_dieclamp_info.Clamp_Back_9_Ebable = b9e;

                    double b9mo = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Back_9_MoveOutStatus, ref b9mo, m_page_flib);
                    m_dieclamp_info.Clamp_Back_9_MoveOutStatus = b9mo;

                    double b9mi = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Back_9_MoveInStatus, ref b9mi, m_page_flib);
                    m_dieclamp_info.Clamp_Back_9_MoveInStatus = b9mi;

                    double b10e = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Back_10_Ebable, ref b10e, m_page_flib);
                    m_dieclamp_info.Clamp_Back_10_Ebable = b10e;

                    double b10mo = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Back_10_MoveOutStatus, ref b10mo, m_page_flib);
                    m_dieclamp_info.Clamp_Back_10_MoveOutStatus = b10mo;

                    double b10mi = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Back_10_MoveInStatus, ref b10mi, m_page_flib);
                    m_dieclamp_info.Clamp_Back_10_MoveInStatus = b10mi;

                    double b11e = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Back_11_Ebable, ref b11e, m_page_flib);
                    m_dieclamp_info.Clamp_Back_11_Ebable = b11e;

                    double b11mo = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Back_11_MoveOutStatus, ref b11mo, m_page_flib);
                    m_dieclamp_info.Clamp_Back_11_MoveOutStatus = b11mo;

                    double b11mi = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Back_11_MoveInStatus, ref b11mi, m_page_flib);
                    m_dieclamp_info.Clamp_Back_11_MoveInStatus = b11mi;

                    double b12e = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Back_12_Ebable, ref b12e, m_page_flib);
                    m_dieclamp_info.Clamp_Back_12_Ebable = b12e;

                    double b12mo = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Back_12_MoveOutStatus, ref b12mo, m_page_flib);
                    m_dieclamp_info.Clamp_Back_12_MoveOutStatus = b12mo;

                    double b12mi = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Back_12_MoveInStatus, ref b12mi, m_page_flib);
                    m_dieclamp_info.Clamp_Back_12_MoveInStatus = b12mi;

                    double b13e = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Back_13_Ebable, ref b13e, m_page_flib);
                    m_dieclamp_info.Clamp_Back_13_Ebable = b13e;

                    double b13mo = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Back_13_MoveOutStatus, ref b13mo, m_page_flib);
                    m_dieclamp_info.Clamp_Back_13_MoveOutStatus = b13mo;

                    double b13mi = 0;
                    GetPmc(_pmcBom.CLS_Clamp_Back_13_MoveInStatus, ref b13mi, m_page_flib);
                    m_dieclamp_info.Clamp_Back_13_MoveInStatus = b13mi;

                    Messenger.Default.Send<ParaDieClampInfo>(m_dieclamp_info, "ParaDieClampInfoMsg");
                }
                #endregion

                #region 合模设定
                if (m_paradieclosing == true)
                {
                    m_statemonitor_info.LineChartFlag = m_simulatemonitorline_indo;

                    double sn_temp = 0;
                    GetPmc(_pmcBom.DJP_SectionNum, ref sn_temp, m_page_flib);
                    m_dieclosing_info.SectionNum = sn_temp;
                    
                    //double pdt_temp = 0;
                    //GetMacro(_pmcBom.DJP_PreDelayTime, ref pdt_temp, m_page_flib);
                    //m_dieclosing_info.PreDelayTime = pdt_temp;

                    //double st_temp = 0;
                    //GetMacro(_macroBom.DJP_SafeTime.Adr, ref st_temp, m_page_flib);
                    //m_dieclosing_info.SafeTime = st_temp;

                    Messenger.Default.Send<ParaDieClosingInfo>(m_dieclosing_info, "ParaDieClosingInfoMsg");

                    double sliding = 0;

                    double p0_temp = 0;
                    GetPmc(_pmcBom.DJP_TopDeadCentre, ref p0_temp, m_page_flib);
                    SearchSlidingData(p0_temp, ref sliding);
                    m_dieclosingproc_info.TopDeadCentre = Math.Round(sliding,2);

                    double p1_temp = 0;
                    GetPmc(_pmcBom.DJP_Pos_1, ref p1_temp, m_page_flib);
                    SearchSlidingData(p1_temp , ref sliding);
                    m_dieclosingproc_info.Pos_1 = Math.Round(sliding, 2);
                    double s1_temp = 0;
                    GetPmc(_pmcBom.DJP_Speed_1, ref s1_temp, m_page_flib);
                    m_dieclosingproc_info.Speed_1 = s1_temp;
                    //double t1_temp = 0;
                    //GetPmc(_macroBom.DJP_StopTime_1.Adr, ref t1_temp, m_page_flib);
                    //m_dieclosingproc_info.StopTime_1 = t1_temp;

                    double p2_temp = 0;
                    GetPmc(_pmcBom.DJP_Pos_2, ref p2_temp, m_page_flib);
                    SearchSlidingData(p2_temp , ref sliding);
                    m_dieclosingproc_info.Pos_2 = Math.Round(sliding, 2);
                    double s2_temp = 0;
                    GetPmc(_pmcBom.DJP_Speed_2, ref s2_temp, m_page_flib);
                    m_dieclosingproc_info.Speed_2 = s2_temp;
                    //double t2_temp = 0;
                    //GetPmc(_pmcBom.DJP_StopTime_2.Adr, ref t2_temp, m_page_flib);
                    //m_dieclosingproc_info.StopTime_2 = t2_temp;

                    double p3_temp = 0;
                    GetPmc(_pmcBom.DJP_Pos_3, ref p3_temp, m_page_flib);
                    SearchSlidingData(p3_temp , ref sliding);
                    m_dieclosingproc_info.Pos_3 = Math.Round(sliding, 2);
                    double s3_temp = 0;
                    GetPmc(_pmcBom.DJP_Speed_3, ref s3_temp, m_page_flib);
                    m_dieclosingproc_info.Speed_3 = s3_temp;
                    //double t3_temp = 0;
                    //GetPmc(_macroBom.DJP_StopTime_3.Adr, ref t3_temp, m_page_flib);
                    //m_dieclosingproc_info.StopTime_3 = t3_temp;

                    double p4_temp = 0;
                    GetPmc(_pmcBom.DJP_Pos_4, ref p4_temp, m_page_flib);
                    SearchSlidingData(p4_temp , ref sliding);
                    m_dieclosingproc_info.Pos_4 = Math.Round(sliding, 2);
                    double s4_temp = 0;
                    GetPmc(_pmcBom.DJP_Speed_4, ref s4_temp, m_page_flib);
                    m_dieclosingproc_info.Speed_4 = s4_temp;
                    //double t4_temp = 0;
                    //GetPmc(_macroBom.DJP_StopTime_4.Adr, ref t4_temp, m_page_flib);
                    //m_dieclosingproc_info.StopTime_4 = t4_temp;

                    double p5_temp = 0;
                    GetPmc(_pmcBom.DJP_Pos_5, ref p5_temp, m_page_flib);
                    SearchSlidingData(p5_temp , ref sliding);
                    m_dieclosingproc_info.Pos_5 = Math.Round(sliding, 2);
                    double s5_temp = 0;
                    GetPmc(_pmcBom.DJP_Speed_5, ref s5_temp, m_page_flib);
                    m_dieclosingproc_info.Speed_5 = s5_temp;
                    //double t5_temp = 0;
                    //GetPmc(_macroBom.DJP_StopTime_5.Adr, ref t5_temp, m_page_flib);
                    //m_dieclosingproc_info.StopTime_5 = t5_temp;

                    double p6_temp = 0;
                    GetPmc(_pmcBom.DJP_Pos_6, ref p6_temp, m_page_flib);
                    SearchSlidingData(p6_temp , ref sliding);
                    m_dieclosingproc_info.Pos_6 = Math.Round(sliding, 2);
                    double s6_temp = 0;
                    GetPmc(_pmcBom.DJP_Speed_6, ref s6_temp, m_page_flib);
                    m_dieclosingproc_info.Speed_6 = s6_temp;
                    //double t6_temp = 0;
                    //GetPmc(_macroBom.DJP_StopTime_6.Adr, ref t6_temp, m_page_flib);
                    //m_dieclosingproc_info.StopTime_6 = t6_temp;

                    double p7_temp = 0;
                    GetPmc(_pmcBom.DJP_Pos_7, ref p7_temp, m_page_flib);
                    SearchSlidingData(p7_temp , ref sliding);
                    m_dieclosingproc_info.Pos_7 = Math.Round(sliding, 2);
                    double s7_temp = 0;
                    GetPmc(_pmcBom.DJP_Speed_7, ref s7_temp, m_page_flib);
                    m_dieclosingproc_info.Speed_7 = s7_temp;
                    //double t7_temp = 0;
                    //GetPmc(_macroBom.DJP_StopTime_7.Adr, ref t7_temp, m_page_flib);
                    //m_dieclosingproc_info.StopTime_7 = t7_temp;

                    double p8_temp = 0;
                    GetPmc(_pmcBom.DJP_Pos_8, ref p8_temp, m_page_flib);
                    SearchSlidingData(p8_temp , ref sliding);
                    m_dieclosingproc_info.Pos_8 = Math.Round(sliding, 2);
                    double s8_temp = 0;
                    GetPmc(_pmcBom.DJP_Speed_8, ref s8_temp, m_page_flib);
                    m_dieclosingproc_info.Speed_8 = s8_temp;
                    //double t8_temp = 0;
                    //GetPmc(_pmcBom.DJP_StopTime_8, ref t8_temp, m_page_flib);
                    //m_dieclosingproc_info.StopTime_8 = t8_temp;

                    double bdc_temp = 0;
                    GetPmc(_pmcBom.DJP_BottomDeadCentre, ref bdc_temp, m_page_flib);
                    SearchSlidingData(bdc_temp, ref sliding);
                    m_dieclosingproc_info.BottomDeadCentre = Math.Round(sliding, 2);

                    double bdc_st_temp = 0;
                    GetPmc(_pmcBom.DJP_BottomDeadCentre_StopTime, ref bdc_st_temp, m_page_flib);
                    m_dieclosingproc_info.BottomDeadCentre_StopTime = bdc_st_temp;

                    double bdcs_temp = 0;
                    GetPmc(_pmcBom.DJP_Speed_BottomDeadCentre, ref bdcs_temp, m_page_flib);
                    m_dieclosingproc_info.Speed_BottomDeadCentre = bdcs_temp;

                    double sn2_temp = 0;
                    GetPmc(_pmcBom.DJP_SectionNum, ref sn2_temp, m_page_flib);
                    m_dieclosingproc_info.SectionNum = sn2_temp;

                    Messenger.Default.Send<ParaDieClosingProcInfo>(m_dieclosingproc_info, "ParaDieClosingProcInfoMsg");
                }
                #endregion

                #region 开模设定
                if (m_paradieparting == true)
                {
                    m_statemonitor_info.LineChartFlag = m_simulatemonitorline_indo;

                    double sliding = 0;
                    double sn_temp = 0;
                    GetPmc(_pmcBom.DPP_SectionNum, ref sn_temp, m_page_flib);
                    m_dieparting_info.SectionNum = sn_temp;


                    Messenger.Default.Send<ParaDiePartingInfo>(m_dieparting_info, "ParaDiePartingInfoMsg");

                    double p1_temp = 0;
                    GetPmc(_pmcBom.DPP_Pos_1, ref p1_temp, m_page_flib);
                    SearchSlidingData(p1_temp, ref sliding);
                    m_diepartingproc_info.Pos_1 = Math.Round(sliding, 2);
                    double s1_temp = 0;
                    GetPmc(_pmcBom.DPP_Speed_1, ref s1_temp, m_page_flib);
                    m_diepartingproc_info.Speed_1 = s1_temp;
                    double p2_temp = 0;
                    GetPmc(_pmcBom.DPP_Pos_2, ref p2_temp, m_page_flib);
                    SearchSlidingData(p2_temp, ref sliding);
                    m_diepartingproc_info.Pos_2 = Math.Round(sliding, 2);
                    double s2_temp = 0;
                    GetPmc(_pmcBom.DPP_Speed_2, ref s2_temp, m_page_flib);
                    m_diepartingproc_info.Speed_2 = s2_temp;
                    double p3_temp = 0;
                    GetPmc(_pmcBom.DPP_Pos_3, ref p3_temp, m_page_flib);
                    SearchSlidingData(p3_temp, ref sliding);
                    m_diepartingproc_info.Pos_3 = Math.Round(sliding, 2);
                    double s3_temp = 0;
                    GetPmc(_pmcBom.DPP_Speed_3, ref s3_temp, m_page_flib);
                    m_diepartingproc_info.Speed_3 = s3_temp;
                    double p4_temp = 0;
                    GetPmc(_pmcBom.DPP_Pos_4, ref p4_temp, m_page_flib);
                    SearchSlidingData(p4_temp, ref sliding);
                    m_diepartingproc_info.Pos_4 = Math.Round(sliding, 2);
                    double s4_temp = 0;
                    GetPmc(_pmcBom.DPP_Speed_4, ref s4_temp, m_page_flib);
                    m_diepartingproc_info.Speed_4 = s4_temp;
                    double p5_temp = 0;
                    GetPmc(_pmcBom.DPP_Pos_5, ref p5_temp, m_page_flib);
                    SearchSlidingData(p5_temp, ref sliding);
                    m_diepartingproc_info.Pos_5 = Math.Round(sliding, 2);
                    double s5_temp = 0;
                    GetPmc(_pmcBom.DPP_Speed_5, ref s5_temp, m_page_flib);
                    m_diepartingproc_info.Speed_5 = s5_temp;
                    double p6_temp = 0;
                    GetPmc(_pmcBom.DPP_Pos_6, ref p6_temp, m_page_flib);
                    SearchSlidingData(p6_temp, ref sliding);
                    m_diepartingproc_info.Pos_6 = Math.Round(sliding, 2);
                    double s6_temp = 0;
                    GetPmc(_pmcBom.DPP_Speed_6, ref s6_temp, m_page_flib);
                    m_diepartingproc_info.Speed_6 = s6_temp;
                    double p7_temp = 0;
                    GetPmc(_pmcBom.DPP_Pos_7, ref p7_temp, m_page_flib);
                    SearchSlidingData(p7_temp, ref sliding);
                    m_diepartingproc_info.Pos_7 = Math.Round(sliding, 2);
                    double s7_temp = 0;
                    GetPmc(_pmcBom.DPP_Speed_7, ref s7_temp, m_page_flib);
                    m_diepartingproc_info.Speed_7 = s7_temp;
                    double p8_temp = 0;
                    GetPmc(_pmcBom.DPP_Pos_8, ref p8_temp, m_page_flib);
                    SearchSlidingData(p8_temp, ref sliding);
                    m_diepartingproc_info.Pos_8 = Math.Round(sliding, 2);
                    double s8_temp = 0;
                    GetPmc(_pmcBom.DPP_Speed_8, ref s8_temp, m_page_flib);
                    m_diepartingproc_info.Speed_8 = s8_temp;

                    //double t1_temp = 0;
                    //GetMacro(_macroBom.DPP_StopTime_1.Adr, ref t1_temp, m_page_flib);
                    //m_diepartingproc_info.StopTime_1 = t1_temp;
                    double bdc_temp = 0;
                    GetPmc(_pmcBom.DPP_BottomDeadCentre, ref bdc_temp, m_page_flib);
                    SearchSlidingData(bdc_temp, ref sliding);
                    m_diepartingproc_info.BottomDeadCentre = Math.Round(sliding, 2);
                    double bdcs_temp = 0;
                    GetPmc(_pmcBom.DPP_Speed_TopDeadCentre, ref bdcs_temp, m_page_flib);
                    m_diepartingproc_info.Speed_TopDeadCentre = bdcs_temp;
                    double tdc_temp = 0;
                    GetPmc(_pmcBom.DPP_TopDeadCentre, ref tdc_temp, m_page_flib);
                    SearchSlidingData(tdc_temp, ref sliding);
                    m_diepartingproc_info.TopDeadCentre = Math.Round(sliding, 2);
                    

                    double sn2_temp = 0;
                    GetPmc(_pmcBom.DPP_SectionNum, ref sn2_temp, m_page_flib);
                    m_diepartingproc_info.SectionNum = sn2_temp;

                    Messenger.Default.Send<ParaDiePartingProcInfo>(m_diepartingproc_info, "ParaDiePartingProcInfoMsg");
                }
                #endregion

                #region 保压设定
                if (m_parapressuremaint == true)
                {
                    double p_temp = 0;
                    GetMacro(_macroBom.SPP_Pressure.Adr, ref p_temp, m_page_flib);
                    m_pressuremaint_info.Pressure = p_temp;

                    double t_temp = 0;
                    GetMacro(_macroBom.SPP_Time.Adr, ref t_temp, m_page_flib);
                    m_pressuremaint_info.Time = t_temp;

                    double pdt_temp = 0;
                    GetMacro(_macroBom.SPP_PreDelayTime.Adr, ref pdt_temp, m_page_flib);
                    m_pressuremaint_info.PreDelayTime = pdt_temp;

                    double cm_temp = 0;
                    GetMacro(_macroBom.SPP_ChangeMode.Adr, ref cm_temp, m_page_flib);
                    m_pressuremaint_info.ChangeMode = cm_temp;

                    double cp_temp = 0;
                    GetMacro(_macroBom.SPP_ChangePressure.Adr, ref cp_temp, m_page_flib);
                    m_pressuremaint_info.ChangePressure = cp_temp;

                    Messenger.Default.Send<ParaPressureMaintInfo>(m_pressuremaint_info, "ParaPressureMaintInfoMsg");
                }
                #endregion

                #region 自动化气源
                if (m_paraautoairsource == true)
                {
                    double sp1_temp = 0;
                    GetPmc(_pmcBom.AAS_StartPos_1, ref sp1_temp, m_page_flib);
                    m_autoairsource_info.StartPos_1 = sp1_temp;

                    double sa1_temp = 0;
                    GetPmc(_pmcBom.AAS_StartArr_1, ref sa1_temp, m_page_flib);
                    m_autoairsource_info.StartArr_1 = sa1_temp;

                    double ep1_temp = 0;
                    GetPmc(_pmcBom.AAS_EndPos_1, ref ep1_temp, m_page_flib);
                    m_autoairsource_info.EndPos_1 = ep1_temp;

                    double ea1_temp = 0;
                    GetPmc(_pmcBom.AAS_EndArr_1, ref ea1_temp, m_page_flib);
                    m_autoairsource_info.EndArr_1 = ea1_temp;

                    double af1_temp = 0;
                    GetPmc(_pmcBom.AAS_ActionFlag_1, ref af1_temp, m_page_flib);
                    m_autoairsource_info.ActionFlag_1 = af1_temp;

                    double en1_temp = 0;
                    GetPmc(_pmcBom.AAS_Enable_1, ref en1_temp, m_page_flib);
                    m_autoairsource_info.Enable_1 = en1_temp;

                    double sp2_temp = 0;
                    GetPmc(_pmcBom.AAS_StartPos_2, ref sp2_temp, m_page_flib);
                    m_autoairsource_info.StartPos_2 = sp2_temp;

                    double sa2_temp = 0;
                    GetPmc(_pmcBom.AAS_StartArr_2, ref sa2_temp, m_page_flib);
                    m_autoairsource_info.StartArr_2 = sa2_temp;

                    double ep2_temp = 0;
                    GetPmc(_pmcBom.AAS_EndPos_2, ref ep2_temp, m_page_flib);
                    m_autoairsource_info.EndPos_2 = ep2_temp;

                    double ea2_temp = 0;
                    GetPmc(_pmcBom.AAS_EndArr_2, ref ea2_temp, m_page_flib);
                    m_autoairsource_info.EndArr_2 = ea2_temp;

                    double af2_temp = 0;
                    GetPmc(_pmcBom.AAS_ActionFlag_2, ref af2_temp, m_page_flib);
                    m_autoairsource_info.ActionFlag_2 = af2_temp;

                    double en2_temp = 0;
                    GetPmc(_pmcBom.AAS_Enable_2, ref en2_temp, m_page_flib);
                    m_autoairsource_info.Enable_2 = en2_temp;

                    double sp3_temp = 0;
                    GetPmc(_pmcBom.AAS_StartPos_3, ref sp3_temp, m_page_flib);
                    m_autoairsource_info.StartPos_3 = sp3_temp;

                    double sa3_temp = 0;
                    GetPmc(_pmcBom.AAS_StartArr_3, ref sa3_temp, m_page_flib);
                    m_autoairsource_info.StartArr_3 = sa3_temp;

                    double ep3_temp = 0;
                    GetPmc(_pmcBom.AAS_EndPos_3, ref ep3_temp, m_page_flib);
                    m_autoairsource_info.EndPos_3 = ep3_temp;

                    double ea3_temp = 0;
                    GetPmc(_pmcBom.AAS_EndArr_3, ref ea3_temp, m_page_flib);
                    m_autoairsource_info.EndArr_3 = ea3_temp;

                    double af3_temp = 0;
                    GetPmc(_pmcBom.AAS_ActionFlag_3, ref af3_temp, m_page_flib);
                    m_autoairsource_info.ActionFlag_3 = af3_temp;

                    double en3_temp = 0;
                    GetPmc(_pmcBom.AAS_Enable_3, ref en3_temp, m_page_flib);
                    m_autoairsource_info.Enable_3 = en3_temp;

                    double sp4_temp = 0;
                    GetPmc(_pmcBom.AAS_StartPos_4, ref sp4_temp, m_page_flib);
                    m_autoairsource_info.StartPos_4 = sp4_temp;

                    double sa4_temp = 0;
                    GetPmc(_pmcBom.AAS_StartArr_4, ref sa4_temp, m_page_flib);
                    m_autoairsource_info.StartArr_4 = sa4_temp;

                    double ep4_temp = 0;
                    GetPmc(_pmcBom.AAS_EndPos_4, ref ep4_temp, m_page_flib);
                    m_autoairsource_info.EndPos_4 = ep4_temp;

                    double ea4_temp = 0;
                    GetPmc(_pmcBom.AAS_EndArr_4, ref ea4_temp, m_page_flib);
                    m_autoairsource_info.EndArr_4 = ea4_temp;

                    double af4_temp = 0;
                    GetPmc(_pmcBom.AAS_ActionFlag_4, ref af4_temp, m_page_flib);
                    m_autoairsource_info.ActionFlag_4 = af4_temp;

                    double en4_temp = 0;
                    GetPmc(_pmcBom.AAS_Enable_4, ref en4_temp, m_page_flib);
                    m_autoairsource_info.Enable_4 = en4_temp;

                    Messenger.Default.Send<ParaAutoAirSourceInfo>(m_autoairsource_info, "ParaAutoAirSourceInfoMsg");
                }
                #endregion

                #region 液压垫控制
                if (m_parahydrdiecushion == true)
                {
                    double sp1_temp = 0;
                    GetPmc(_pmcBom.HDC_StartPos_1, ref sp1_temp, m_page_flib);
                    m_hydrdiecushion_info.StartPos_1 = sp1_temp;

                    double sa1_temp = 0;
                    GetPmc(_pmcBom.HDC_StartArr_1, ref sa1_temp, m_page_flib);
                    m_hydrdiecushion_info.StartArr_1 = sa1_temp;

                    double ep1_temp = 0;
                    GetPmc(_pmcBom.HDC_EndPos_1, ref ep1_temp, m_page_flib);
                    m_hydrdiecushion_info.EndPos_1 = ep1_temp;

                    double ea1_temp = 0;
                    GetPmc(_pmcBom.HDC_EndArr_1, ref ea1_temp, m_page_flib);
                    m_hydrdiecushion_info.EndArr_1 = ea1_temp;

                    double af1_temp = 0;
                    GetPmc(_pmcBom.HDC_ActionFlag_1, ref af1_temp, m_page_flib);
                    m_hydrdiecushion_info.ActionFlag_1 = af1_temp;

                    double en1_temp = 0;
                    GetPmc(_pmcBom.HDC_Enable_1, ref en1_temp, m_page_flib);
                    m_hydrdiecushion_info.Enable_1 = en1_temp;

                    double sp2_temp = 0;
                    GetPmc(_pmcBom.HDC_StartPos_2, ref sp2_temp, m_page_flib);
                    m_hydrdiecushion_info.StartPos_2 = sp2_temp;

                    double sa2_temp = 0;
                    GetPmc(_pmcBom.HDC_StartArr_2, ref sa2_temp, m_page_flib);
                    m_hydrdiecushion_info.StartArr_2 = sa2_temp;

                    double ep2_temp = 0;
                    GetPmc(_pmcBom.HDC_EndPos_2, ref ep2_temp, m_page_flib);
                    m_hydrdiecushion_info.EndPos_2 = ep2_temp;

                    double ea2_temp = 0;
                    GetPmc(_pmcBom.HDC_EndArr_2, ref ea2_temp, m_page_flib);
                    m_hydrdiecushion_info.EndArr_2 = ea2_temp;

                    double af2_temp = 0;
                    GetPmc(_pmcBom.HDC_ActionFlag_2, ref af2_temp, m_page_flib);
                    m_hydrdiecushion_info.ActionFlag_2 = af2_temp;

                    double en2_temp = 0;
                    GetPmc(_pmcBom.HDC_Enable_2, ref en2_temp, m_page_flib);
                    m_hydrdiecushion_info.Enable_2 = en2_temp;

                    Messenger.Default.Send<ParaHydrDieCushionInfo>(m_hydrdiecushion_info, "ParaHydrDieCushionInfoMsg");
                }
                #endregion

                #region 备用凸轮
                if (m_paracam == true)
                {
                    double sp1_temp = 0;
                    GetPmc(_pmcBom.CAM_StartPos_1, ref sp1_temp, m_page_flib);
                    m_cam_info.StartPos_1 = sp1_temp;

                    double sa1_temp = 0;
                    GetPmc(_pmcBom.CAM_StartArr_1, ref sa1_temp, m_page_flib);
                    m_cam_info.StartArr_1 = sa1_temp;

                    double ep1_temp = 0;
                    GetPmc(_pmcBom.CAM_EndPos_1, ref ep1_temp, m_page_flib);
                    m_cam_info.EndPos_1 = ep1_temp;

                    double ea1_temp = 0;
                    GetPmc(_pmcBom.CAM_EndArr_1, ref ea1_temp, m_page_flib);
                    m_cam_info.EndArr_1 = ea1_temp;

                    double af1_temp = 0;
                    GetPmc(_pmcBom.CAM_ActionFlag_1, ref af1_temp, m_page_flib);
                    m_cam_info.ActionFlag_1 = af1_temp;


                    double en1_temp = 0;
                    GetPmc(_pmcBom.CAM_Enable_1, ref en1_temp, m_page_flib);
                    m_cam_info.Enable_1 = en1_temp;

                    double sp2_temp = 0;
                    GetPmc(_pmcBom.CAM_StartPos_2, ref sp2_temp, m_page_flib);
                    m_cam_info.StartPos_2 = sp2_temp;

                    double sa2_temp = 0;
                    GetPmc(_pmcBom.CAM_StartArr_2, ref sa2_temp, m_page_flib);
                    m_cam_info.StartArr_2 = sa2_temp;

                    double ep2_temp = 0;
                    GetPmc(_pmcBom.CAM_EndPos_2, ref ep2_temp, m_page_flib);
                    m_cam_info.EndPos_2 = ep2_temp;

                    double ea2_temp = 0;
                    GetPmc(_pmcBom.CAM_EndArr_2, ref ea2_temp, m_page_flib);
                    m_cam_info.EndArr_2 = ea2_temp;

                    double af2_temp = 0;
                    GetPmc(_pmcBom.CAM_ActionFlag_2, ref af2_temp, m_page_flib);
                    m_cam_info.ActionFlag_2 = af2_temp;

                    double en2_temp = 0;
                    GetPmc(_pmcBom.CAM_Enable_2, ref en2_temp, m_page_flib);
                    m_cam_info.Enable_2 = en2_temp;

                    double sp3_temp = 0;
                    GetPmc(_pmcBom.CAM_StartPos_3, ref sp3_temp, m_page_flib);
                    m_cam_info.StartPos_3 = sp3_temp;

                    double sa3_temp = 0;
                    GetPmc(_pmcBom.CAM_StartArr_3, ref sa3_temp, m_page_flib);
                    m_cam_info.StartArr_3 = sa3_temp;

                    double ep3_temp = 0;
                    GetPmc(_pmcBom.CAM_EndPos_3, ref ep3_temp, m_page_flib);
                    m_cam_info.EndPos_3 = ep3_temp;

                    double ea3_temp = 0;
                    GetPmc(_pmcBom.CAM_EndArr_3, ref ea3_temp, m_page_flib);
                    m_cam_info.EndArr_3 = ea3_temp;

                    double af3_temp = 0;
                    GetPmc(_pmcBom.CAM_ActionFlag_3, ref af3_temp, m_page_flib);
                    m_cam_info.ActionFlag_3 = af3_temp;

                    double en3_temp = 0;
                    GetPmc(_pmcBom.CAM_Enable_3, ref en3_temp, m_page_flib);
                    m_cam_info.Enable_3 = en3_temp;

                    double sp4_temp = 0;
                    GetPmc(_pmcBom.CAM_StartPos_4, ref sp4_temp, m_page_flib);
                    m_cam_info.StartPos_4 = sp4_temp;

                    double sa4_temp = 0;
                    GetPmc(_pmcBom.CAM_StartArr_4, ref sa4_temp, m_page_flib);
                    m_cam_info.StartArr_4 = sa4_temp;

                    double ep4_temp = 0;
                    GetPmc(_pmcBom.CAM_EndPos_4, ref ep4_temp, m_page_flib);
                    m_cam_info.EndPos_4 = ep4_temp;

                    double ea4_temp = 0;
                    GetPmc(_pmcBom.CAM_EndArr_4, ref ea4_temp, m_page_flib);
                    m_cam_info.EndArr_4 = ea4_temp;

                    double af4_temp = 0;
                    GetPmc(_pmcBom.CAM_ActionFlag_4, ref af4_temp, m_page_flib);
                    m_cam_info.ActionFlag_4 = af4_temp;

                    double en4_temp = 0;
                    GetPmc(_pmcBom.CAM_Enable_4, ref en4_temp, m_page_flib);
                    m_cam_info.Enable_4 = en4_temp;

                    Messenger.Default.Send<ParaCamInfo>(m_cam_info, "ParaCamInfoMsg");
                }
                #endregion

                #region 模具液压设定
                if (m_paradiehydr == true)
                {
                    double dhmo_temp = 0;
                    GetPmc(_pmcBom.DH_Mode, ref dhmo_temp, m_page_flib);
                    m_m_diehydr_info.Mode = dhmo_temp;

                    double dhps_temp = 0;
                    GetPmc(_pmcBom.DH_Pressure, ref dhps_temp, m_page_flib);
                    m_m_diehydr_info.Pressure = dhps_temp;

                    double dhpp_temp = 0;
                    GetPmc(_pmcBom.DH_PushPos, ref dhpp_temp, m_page_flib);
                    m_m_diehydr_info.PushPos = dhpp_temp;

                    double dhpdt_temp = 0;
                    GetPmc(_pmcBom.DH_PushDelayTime, ref dhpdt_temp, m_page_flib);
                    m_m_diehydr_info.PushDelayTime = dhpdt_temp;

                    double dhap_temp = 0;
                    GetPmc(_pmcBom.DH_ActualPressure, ref dhap_temp, m_page_flib);
                    m_m_diehydr_info.ActualPressure = dhap_temp;

                    double dhapos_temp = 0;
                    GetPmc(_pmcBom.DH_ActualPos, ref dhapos_temp, m_page_flib);
                    m_m_diehydr_info.ActualPos = dhapos_temp;

                    double dhrun_temp = 0;
                    GetPmc(_pmcBom.DH_Run, ref dhrun_temp, m_page_flib);
                    m_m_diehydr_info.Run = dhrun_temp;

                    double dhstate_temp = 0;
                    GetPmc(_pmcBom.DH_State, ref dhstate_temp, m_page_flib);
                    m_m_diehydr_info.State = dhstate_temp;

                    Messenger.Default.Send<ParaDieHydrInfo>(m_m_diehydr_info, "ParaDieHydrInfoMsg");
                }

                #endregion

                #region 工件计数
                if (m_paraworkcount == true)
                {
                    double dp_temp = 0;
                    GetPmc(_pmcBom.WPP_DayPiece, ref dp_temp, m_page_flib);
                    m_workcount_info.DayPiece = dp_temp;

                    double dw_temp = 0;
                    GetPmc(_pmcBom.WPP_DayWork, ref dw_temp, m_page_flib);
                    m_workcount_info.DayWork = dw_temp;

                    double cp_temp = 0;
                    GetPmc(_pmcBom.WPP_CurPiece, ref cp_temp, m_page_flib);
                    m_workcount_info.CurPiece = cp_temp;

                    double sp_temp = 0;
                    GetPmc(_pmcBom.WPP_SetPiece, ref sp_temp, m_page_flib);
                    m_workcount_info.SetPiece = sp_temp;

                    double cp2_temp = 0;
                    GetPmc(_pmcBom.WPP_SetPiece, ref cp2_temp, m_page_flib);
                    m_workcount_info.CurPiece2 = cp2_temp;

                    double tp_temp = 0;
                    GetPmc(_pmcBom.WPP_TotalPiece, ref tp_temp, m_page_flib);
                    m_workcount_info.TotalPiece = tp_temp;

                    double tw_temp = 0;
                    GetPmc(_pmcBom.WPP_TotalWork, ref tw_temp, m_page_flib);
                    m_workcount_info.TotalWork = tw_temp;

                    Messenger.Default.Send<ParaWorkCountInfo>(m_workcount_info, "ParaWorkCountInfoMsg");
                }
                #endregion

                #region 配方
                if(m_pararecipes==true)
                {
                    foreach(var macro_item in _recipes.MacroBoms)
                    {
                        string temp_data = "";
                        GetMacro(new MacroBomItem() {
                            Id=macro_item.Id,
                            Adr=macro_item.Adr,
                            IsRecipes=macro_item.IsRecipes
                        }, ref temp_data);
                        macro_item.Value = temp_data;

                    }
                    
                    foreach(var pmc_item in _recipes.PmcBoms)
                    {
                        string data_res = "";
                        var bomItem = new PmcBomItem()
                        {
                            Id= pmc_item.Id,
                            Adr= pmc_item.Adr,
                            AdrType= pmc_item.AdrType,
                            Bit= pmc_item.Bit,
                            ConversionFactor= pmc_item.ConversionFactor,
                            DataType= pmc_item.DataType,
                            IsRecipes= pmc_item.IsRecipes
                        };

                        switch (bomItem.DataType)
                        {
                            case PmcDataTypeEnum.BIT:
                                bool bTemp = false;
                                GetPmc(bomItem, ref bTemp);
                                data_res = bTemp.ToString();
                                break;
                            case PmcDataTypeEnum.BYTE:
                                byte cTemp = 0;
                                GetPmc(bomItem, ref cTemp);
                                data_res = cTemp.ToString();
                                break;
                            case PmcDataTypeEnum.WORD:
                                short iTemp = 0;
                                GetPmc(bomItem, ref iTemp);
                                data_res = iTemp.ToString();
                                break;
                            case PmcDataTypeEnum.LONG:
                                int lTemp = 0;
                                GetPmc(bomItem, ref lTemp);
                                data_res = lTemp.ToString();
                                break;
                            default:
                                break;
                        }

                        pmc_item.Value = data_res;
                    }

                    Messenger.Default.Send<RecipesInfo>(_recipes, "ParaRecipesInfoMsg");
                }


                #endregion

                #region 系统参数 压机设定
                if (m_paraworkcount == false)
                {
                    double spm_weight = 0;
                    GetPmc(_pmcBom.SPM_MaxWeight, ref spm_weight, m_page_flib);
                    m_sparamachine_info.MaxWeight = spm_weight;

                    double spm_maxt = 0;
                    GetPmc(_pmcBom.SPM_MaxTemperature, ref spm_maxt, m_page_flib);
                    m_sparamachine_info.MaxTemperature = spm_maxt;

                    double spm_maxe = 0;
                    GetPmc(_pmcBom.SPM_MaxError, ref spm_maxe, m_page_flib);
                    m_sparamachine_info.MaxError = spm_maxe;

                    double spm_psp = 0;
                    GetPmc(_pmcBom.SPM_PressureSensorPara, ref spm_psp, m_page_flib);
                    m_sparamachine_info.PressureSensorPara = spm_psp;

                    double spm_bca = 0;
                    GetPmc(_pmcBom.SPM_BalanceCylinderAnalog, ref spm_bca, m_page_flib);
                    m_sparamachine_info.BalanceCylinderAnalog = spm_bca;

                    double spm_bcp = 0;
                    GetPmc(_pmcBom.SPM_BalanceCylinderPressure, ref spm_bcp, m_page_flib);
                    m_sparamachine_info.BalanceCylinderPressure = spm_bcp;

                    double spm_ofd = 0;
                    GetPmc(_pmcBom.SPM_OverflowDelay, ref spm_ofd, m_page_flib);
                    m_sparamachine_info.OverflowDelay = spm_ofd;

                    double spm_pdp = 0;
                    GetPmc(_pmcBom.SPM_PressureDiffPara, ref spm_pdp, m_page_flib);
                    m_sparamachine_info.PressureDiffPara = spm_pdp;

                    double spm_pla = 0;
                    GetPmc(_pmcBom.SPM_PressureLowAlarm, ref spm_pla, m_page_flib);
                    m_sparamachine_info.PressureLowAlarm = spm_pla;

                    double spm_ld = 0;
                    GetPmc(_pmcBom.SPM_LubricateDetect, ref spm_ld, m_page_flib);
                    m_sparamachine_info.LubricateDetect = spm_ld;

                    Messenger.Default.Send<SParaMachineInfo>(m_sparamachine_info, "SParaMachineInfoMsg");
                }

                #endregion

                #region 系统参数 润滑设定
                if (m_sparalubricate == true)
                {
                    double spm_crlt = 0;
                    GetPmc(_pmcBom.SPL_CR_LubricateTime, ref spm_crlt, m_page_flib);
                    m_sparalubricat_info.CR_LubricateTime = spm_crlt;

                    double spm_crsli = 0;
                    GetPmc(_pmcBom.SPL_CR_SetLubricateInterval, ref spm_crsli, m_page_flib);
                    m_sparalubricat_info.CR_SetLubricateInterval = spm_crsli;

                    double spm_crali = 0;
                    GetPmc(_pmcBom.SPL_CR_ActualLubricateInterval, ref spm_crali, m_page_flib);
                    m_sparalubricat_info.CR_ActualLubricateInterval = spm_crali;

                    double spm_crldt = 0;
                    GetPmc(_pmcBom.SPL_CR_LubricateDetectTime, ref spm_crldt, m_page_flib);
                    m_sparalubricat_info.CR_LubricateDetectTime = spm_crldt;

                    double spm_crltn = 0;
                    GetPmc(_pmcBom.SPL_CR_LubricateTotalNum, ref spm_crltn, m_page_flib);
                    m_sparalubricat_info.CR_LubricateTotalNum = spm_crltn;

                    double spm_crpolt = 0;
                    GetPmc(_pmcBom.SPL_CR_PowerOnLubricateTime, ref spm_crpolt, m_page_flib);
                    m_sparalubricat_info.CR_PowerOnLubricateTime = spm_crpolt;

                    double spm_crld = 0;
                    GetPmc(_pmcBom.SPL_CR_LubricateDetecte, ref spm_crld, m_page_flib);
                    m_sparalubricat_info.CR_LubricateDetecte = spm_crld;

                    double spm_AClt = 0;
                    GetPmc(_pmcBom.SPL_AC_LubricateTime, ref spm_AClt, m_page_flib);
                    m_sparalubricat_info.AC_LubricateTime = spm_AClt;

                    double spm_ACsli = 0;
                    GetPmc(_pmcBom.SPL_AC_SetLubricateInterval, ref spm_ACsli, m_page_flib);
                    m_sparalubricat_info.AC_SetLubricateInterval = spm_ACsli;

                    double spm_ACali = 0;
                    GetPmc(_pmcBom.SPL_AC_ActualLubricateInterval, ref spm_ACali, m_page_flib);
                    m_sparalubricat_info.AC_ActualLubricateInterval = spm_ACali;

                    double spm_ACldt = 0;
                    GetPmc(_pmcBom.SPL_AC_LubricateDetectTime, ref spm_ACldt, m_page_flib);
                    m_sparalubricat_info.AC_LubricateDetectTime = spm_ACldt;

                    double spm_ACltn = 0;
                    GetPmc(_pmcBom.SPL_AC_LubricateTotalNum, ref spm_ACltn, m_page_flib);
                    m_sparalubricat_info.AC_LubricateTotalNum = spm_ACltn;

                    double spm_ACpolt = 0;
                    GetPmc(_pmcBom.SPL_AC_PowerOnLubricateTime, ref spm_ACpolt, m_page_flib);
                    m_sparalubricat_info.AC_PowerOnLubricateTime = spm_ACpolt;

                    double spm_ACld = 0;
                    GetPmc(_pmcBom.SPL_AC_LubricateDetecte, ref spm_ACld, m_page_flib);
                    m_sparalubricat_info.AC_LubricateDetecte = spm_ACld;

                    double spm_AC2lt = 0;
                    GetPmc(_pmcBom.SPL_AC2_LubricateTime, ref spm_AC2lt, m_page_flib);
                    m_sparalubricat_info.AC2_LubricateTime = spm_AC2lt;

                    double spm_AC2sli = 0;
                    GetPmc(_pmcBom.SPL_AC2_SetLubricateInterval, ref spm_AC2sli, m_page_flib);
                    m_sparalubricat_info.AC2_SetLubricateInterval = spm_AC2sli;

                    double spm_AC2ali = 0;
                    GetPmc(_pmcBom.SPL_AC2_ActualLubricateInterval, ref spm_AC2ali, m_page_flib);
                    m_sparalubricat_info.AC2_ActualLubricateInterval = spm_AC2ali;

                    double spm_AC2ldt = 0;
                    GetPmc(_pmcBom.SPL_AC2_LubricateDetectTime, ref spm_AC2ldt, m_page_flib);
                    m_sparalubricat_info.AC2_LubricateDetectTime = spm_AC2ldt;

                    double spm_AC2ltn = 0;
                    GetPmc(_pmcBom.SPL_AC2_LubricateTotalNum, ref spm_AC2ltn, m_page_flib);
                    m_sparalubricat_info.AC2_LubricateTotalNum = spm_AC2ltn;

                    double spm_AC2polt = 0;
                    GetPmc(_pmcBom.SPL_AC2_PowerOnLubricateTime, ref spm_AC2polt, m_page_flib);
                    m_sparalubricat_info.AC2_PowerOnLubricateTime = spm_AC2polt;

                    double spm_AC2ld = 0;
                    GetPmc(_pmcBom.SPL_AC2_LubricateDetecte, ref spm_AC2ld, m_page_flib);
                    m_sparalubricat_info.AC2_LubricateDetecte = spm_AC2ld;

                    double spm_GRlr = 0;
                    GetPmc(_pmcBom.SPL_GR_LubricateReversing, ref spm_GRlr, m_page_flib);
                    m_sparalubricat_info.GR_LubricateReversing = spm_GRlr;


                    double spm_GRldt = 0;
                    GetPmc(_pmcBom.SPL_GR_LubricateDetectTime, ref spm_GRldt, m_page_flib);
                    m_sparalubricat_info.GR_LubricateDetectTime = spm_GRldt;

                    double spm_SClr = 0;
                    GetPmc(_pmcBom.SPL_SC_LubricateReversing, ref spm_SClr, m_page_flib);
                    m_sparalubricat_info.SC_LubricateReversing = spm_SClr;

                    double spm_OSt = 0;
                    GetPmc(_pmcBom.SPL_OS_Time, ref spm_OSt, m_page_flib);
                    m_sparalubricat_info.OS_Time = spm_OSt;

                    double spm_OSit = 0;
                    GetPmc(_pmcBom.SPL_OS_IntervalTime, ref spm_OSit, m_page_flib);
                    m_sparalubricat_info.OS_IntervalTime = spm_OSit;

                    double spm_OSdt = 0;
                    GetPmc(_pmcBom.SPL_OS_DelayTime, ref spm_OSdt, m_page_flib);
                    m_sparalubricat_info.OS_DelayTime = spm_OSdt;


                    double spm_TSdt = 0;
                    GetPmc(_pmcBom.SPL_TS_DelayTime, ref spm_TSdt, m_page_flib);
                    m_sparalubricat_info.TS_DelayTime = spm_TSdt;

                    double spm_TSst = 0;
                    GetPmc(_pmcBom.SPL_TS_StopTime, ref spm_TSst, m_page_flib);
                    m_sparalubricat_info.TS_StopTime = spm_TSst;

                    double spm_TSrt = 0;
                    GetPmc(_pmcBom.SPL_TS_RunTime, ref spm_TSrt, m_page_flib);
                    m_sparalubricat_info.TS_RunTime = spm_TSrt;

                    Messenger.Default.Send<SParaLubricateInfo>(m_sparalubricat_info, "SParaLubricateInfoMsg");
                }
                #endregion

                #region 系统参数 模拟量设定
                if (m_sparaanalog == true)
                {
                    double spa_a1v = 0;
                    GetPmc(_pmcBom.SPA_A1_Value, ref spa_a1v, m_page_flib);
                    m_sparaanalog_info.A1_Value = spa_a1v;

                    double spa_a2v = 0;
                    GetPmc(_pmcBom.SPA_A2_Value, ref spa_a2v, m_page_flib);
                    m_sparaanalog_info.A2_Value = spa_a2v;

                    double spa_a3v = 0;
                    GetPmc(_pmcBom.SPA_A3_Value, ref spa_a3v, m_page_flib);
                    m_sparaanalog_info.A3_Value = spa_a3v;

                    double spa_a4v = 0;
                    GetPmc(_pmcBom.SPA_A4_Value, ref spa_a4v, m_page_flib);
                    m_sparaanalog_info.A4_Value = spa_a4v;

                    double spa_a1p1 = 0;
                    GetPmc(_pmcBom.SPA_A1_WeightPara1, ref spa_a1p1, m_page_flib);
                    m_sparaanalog_info.A1_WeightPara1 = spa_a1p1;

                    double spa_a2p1 = 0;
                    GetPmc(_pmcBom.SPA_A2_WeightPara1, ref spa_a2p1, m_page_flib);
                    m_sparaanalog_info.A2_WeightPara1 = spa_a2p1;

                    double spa_a3p1 = 0;
                    GetPmc(_pmcBom.SPA_A3_WeightPara1, ref spa_a3p1, m_page_flib);
                    m_sparaanalog_info.A3_WeightPara1 = spa_a3p1;

                    double spa_a4p1 = 0;
                    GetPmc(_pmcBom.SPA_A4_WeightPara1, ref spa_a4p1, m_page_flib);
                    m_sparaanalog_info.A4_WeightPara1 = spa_a4p1;

                    double spa_a1p2 = 0;
                    GetPmc(_pmcBom.SPA_A1_WeightPara2, ref spa_a1p2, m_page_flib);
                    m_sparaanalog_info.A1_WeightPara2 = spa_a1p2;

                    double spa_a2p2 = 0;
                    GetPmc(_pmcBom.SPA_A2_WeightPara2, ref spa_a2p2, m_page_flib);
                    m_sparaanalog_info.A2_WeightPara2 = spa_a2p2;

                    double spa_a3p2 = 0;
                    GetPmc(_pmcBom.SPA_A3_WeightPara2, ref spa_a3p2, m_page_flib);
                    m_sparaanalog_info.A3_WeightPara2 = spa_a3p2;

                    double spa_a4p2 = 0;
                    GetPmc(_pmcBom.SPA_A4_WeightPara2, ref spa_a4p2, m_page_flib);
                    m_sparaanalog_info.A4_WeightPara2 = spa_a4p2;

                    double spa_a1w = 0;
                    GetPmc(_pmcBom.SPA_A1_Weight, ref spa_a1w, m_page_flib);
                    m_sparaanalog_info.A1_Weight = spa_a1w;

                    double spa_a2w = 0;
                    GetPmc(_pmcBom.SPA_A2_Weight, ref spa_a2w, m_page_flib);
                    m_sparaanalog_info.A2_Weight = spa_a2w;

                    double spa_a3w = 0;
                    GetPmc(_pmcBom.SPA_A3_Weight, ref spa_a3w, m_page_flib);
                    m_sparaanalog_info.A3_Weight = spa_a3w;

                    double spa_a4w = 0;
                    GetPmc(_pmcBom.SPA_A4_Weight, ref spa_a4w, m_page_flib);
                    m_sparaanalog_info.A4_Weight = spa_a4w;

                    double spa_tw = 0;
                    GetPmc(_pmcBom.SPA_TotalWeight, ref spa_tw, m_page_flib);
                    m_sparaanalog_info.TotalWeight = spa_tw;

                    double spa_dp = 0;
                    GetPmc(_pmcBom.SPA_DetectPosition, ref spa_dp, m_page_flib);
                    m_sparaanalog_info.DetectPosition = spa_dp;

                    double spa_dip = 0;
                    GetPmc(_pmcBom.SPA_DetectInPosition, ref spa_dip, m_page_flib);
                    m_sparaanalog_info.DetectInPosition = spa_dip;

                    double spa_ds = 0;
                    GetPmc(_pmcBom.SPA_DetectSensor, ref spa_ds, m_page_flib);
                    m_sparaanalog_info.DetectSensor = spa_ds;

                    double spa_p = 0;
                    GetPmc(_pmcBom.SPA_Pressure, ref spa_p, m_page_flib);
                    m_sparaanalog_info.Pressure = spa_p;

                    double spa_pu = 0;
                    GetPmc(_pmcBom.SPA_PressureUp, ref spa_pu, m_page_flib);
                    m_sparaanalog_info.PressureUp = spa_pu;

                    double spa_pd = 0;
                    GetPmc(_pmcBom.SPA_PressureDown, ref spa_pd, m_page_flib);
                    m_sparaanalog_info.PressureDown = spa_pd;

                    Messenger.Default.Send<SParaAnalogInfo>(m_sparaanalog_info, "SParaAnalogInfoMsg");
                }

                #endregion

                #region 系统参数 编码器设定
                if (m_sparaencode == true)
                {
                    double im_rsl = 0;
                    GetPmc(_pmcBom.SPA_IM_RESOLUTION, ref im_rsl, m_page_flib);
                    m_sparaencode_info.IM_RESOLUTION = im_rsl;

                    double im_mp = 0;
                    GetPmc(_pmcBom.SPA_IM_MOVEPITCH, ref im_mp, m_page_flib);
                    m_sparaencode_info.IM_MOVEPITCH = im_mp;

                    double im_up = 0;
                    GetPmc(_pmcBom.SPA_IM_UPPOSITION, ref im_up, m_page_flib);
                    m_sparaencode_info.IM_UPPOSITION = im_up;

                    double im_dp = 0;
                    GetPmc(_pmcBom.SPA_IM_DOWNPOSITION, ref im_dp, m_page_flib);
                    m_sparaencode_info.IM_DOWNPOSITION = im_dp;

                    double im_scp = 0;
                    GetPmc(_pmcBom.SPA_IM_SPEEDCHANGEPOSITION, ref im_scp, m_page_flib);
                    m_sparaencode_info.IM_SPEEDCHANGEPOSITION = im_scp;

                    double im_lu = 0;
                    GetPmc(_pmcBom.SPA_IM_LIMITUP, ref im_lu, m_page_flib);
                    m_sparaencode_info.IM_LIMITUP = im_lu;

                    double im_ld = 0;
                    GetPmc(_pmcBom.SPA_IM_LIMITDOWN, ref im_ld, m_page_flib);
                    m_sparaencode_info.IM_LIMITDOWN = im_ld;

                    double im_drt = 0;
                    GetPmc(_pmcBom.SPA_IM_DIRECTION, ref im_drt, m_page_flib);
                    m_sparaencode_info.IM_DIRECTION = im_drt;

                    double im_acr = 0;
                    GetPmc(_pmcBom.SPA_AC_RESOLUTION, ref im_acr, m_page_flib);
                    m_sparaencode_info.AC_RESOLUTION = im_acr;

                    double im_acm = 0;
                    GetPmc(_pmcBom.SPA_AC_MOVEPITCH, ref im_acm, m_page_flib);
                    m_sparaencode_info.AC_MOVEPITCH = im_acm;

                    double im_acup = 0;
                    GetPmc(_pmcBom.SPA_AC_UPPOSITION, ref im_acup, m_page_flib);
                    m_sparaencode_info.AC_UPPOSITION = im_acup;

                    double im_acdp = 0;
                    GetPmc(_pmcBom.SPA_AC_DOWNPOSITION, ref im_acdp, m_page_flib);
                    m_sparaencode_info.AC_DOWNPOSITION = im_acdp;

                    double im_aclu = 0;
                    GetPmc(_pmcBom.SPA_AC_LIMITUP, ref im_aclu, m_page_flib);
                    m_sparaencode_info.AC_LIMITUP = im_aclu;

                    double im_acld = 0;
                    GetPmc(_pmcBom.SPA_AC_LIMITDOWN, ref im_acld, m_page_flib);
                    m_sparaencode_info.AC_LIMITDOWN = im_acld;

                    double im_ace = 0;
                    GetPmc(_pmcBom.SPA_AC_ERROR, ref im_ace, m_page_flib);
                    m_sparaencode_info.AC_ERROR = im_ace;

                    double im_acdir = 0;
                    GetPmc(_pmcBom.SPA_AC_DIRECTION, ref im_acdir, m_page_flib);
                    m_sparaencode_info.AC_DIRECTION = im_acdir;


                    Messenger.Default.Send<SParaEncodeInfo>(m_sparaencode_info, "SParaEncodeInfoMsg");


                }

                #endregion

                #region 维护信息 伺服信息
                if(m_mnservo==true)
                {

                }

                #endregion

                #region 维护信息 装模高度
                if (m_mndiehigh == true)
                {
                    double temp_sel = 0;
                    GetPmc(_pmcBom.MDH_SEL, ref temp_sel, m_page_flib);
                    m_mndiehigh_info.MDH_SEL = temp_sel;

                    double temp_bl = 0;
                    GetPmc(_pmcBom.MDH_BL, ref temp_bl, m_page_flib);
                    m_mndiehigh_info.MDH_BL = temp_bl;

                    double temp_sc = 0;
                    GetPmc(_pmcBom.MDH_SafeCock, ref temp_sc, m_page_flib);
                    m_mndiehigh_info.MDH_SafeCock = temp_sc;

                    double temp_tc = 0;
                    GetPmc(_pmcBom.MDH_TableClamped, ref temp_tc, m_page_flib);
                    m_mndiehigh_info.MDH_TableClamped = temp_tc;

                    double temp_emg = 0;
                    GetPmc(_pmcBom.MDH_Emg, ref temp_emg, m_page_flib);
                    m_mndiehigh_info.MDH_Emg = temp_emg;

                    double temp_mta = 0;
                    GetPmc(_pmcBom.MDH_MoveTableAmp, ref temp_mta, m_page_flib);
                    m_mndiehigh_info.MDH_MoveTableAmp = temp_mta;

                    double temp_m = 0;
                    GetPmc(_pmcBom.MDH_Manual, ref temp_m, m_page_flib);
                    m_mndiehigh_info.MDH_Manual = temp_m;

                    double temp_uld = 0;
                    GetPmc(_pmcBom.MDH_UnLimitDown, ref temp_uld, m_page_flib);
                    m_mndiehigh_info.MDH_UnLimitDown = temp_uld;

                    double temp_ulu = 0;
                    GetPmc(_pmcBom.MDH_UnLimitUp, ref temp_ulu, m_page_flib);
                    m_mndiehigh_info.MDH_UnLimitUp = temp_ulu;

                    double temp_md = 0;
                    GetPmc(_pmcBom.MDH_ManualDown, ref temp_md, m_page_flib);
                    m_mndiehigh_info.MDH_ManualDown = temp_md;

                    double temp_mu = 0;
                    GetPmc(_pmcBom.MDH_ManualUp, ref temp_mu, m_page_flib);
                    m_mndiehigh_info.MDH_ManualUp = temp_mu;

                    double temp_au = 0;
                    GetPmc(_pmcBom.MDH_Auto, ref temp_au, m_page_flib);
                    m_mndiehigh_info.MDH_Auto = temp_au;

                    double temp_udc = 0;
                    GetPmc(_pmcBom.MDH_UpDieCenter, ref temp_udc, m_page_flib);
                    m_mndiehigh_info.MDH_UpDieCenter = temp_udc;

                    double temp_aau = 0;
                    GetPmc(_pmcBom.MDH_AutoAllowUp, ref temp_aau, m_page_flib);
                    m_mndiehigh_info.MDH_AutoAllowUp = temp_aau;

                    double temp_aad = 0;
                    GetPmc(_pmcBom.MDH_AutoAllowDown, ref temp_aad, m_page_flib);
                    m_mndiehigh_info.MDH_AutoAllowDown = temp_aad;

                    double temp_auu = 0;
                    GetPmc(_pmcBom.MDH_AutoUp, ref temp_auu, m_page_flib);
                    m_mndiehigh_info.MDH_AutoUp = temp_auu;

                    double temp_aud = 0;
                    GetPmc(_pmcBom.MDH_AutoDown, ref temp_aud, m_page_flib);
                    m_mndiehigh_info.MDH_AutoDown = temp_aud;

                    Messenger.Default.Send<MaintainDieHighInfo>(m_mndiehigh_info, "MaintainDieHighInfoMsg");
                }

                #endregion

                #region 维护信息 行程操作
                if (m_mntravel == true)
                {
                    double temp_sr = 0;
                    GetPmc(_pmcBom.MTO_ServoReady, ref temp_sr, m_page_flib);
                    m_mntravel_info.MTO_ServoReady = temp_sr;

                    double temp_bl = 0;
                    GetPmc(_pmcBom.MTO_BL, ref temp_bl, m_page_flib);
                    m_mntravel_info.MTO_BL = temp_bl;

                    double temp_l = 0;
                    GetPmc(_pmcBom.MTO_Lubrication, ref temp_l, m_page_flib);
                    m_mntravel_info.MTO_Lubrication = temp_l;

                    double temp_ap = 0;
                    GetPmc(_pmcBom.MTO_AirPress, ref temp_ap, m_page_flib);
                    m_mntravel_info.MTO_AirPress = temp_ap;

                    double temp_pe = 0;
                    GetPmc(_pmcBom.MTO_PE, ref temp_pe, m_page_flib);
                    m_mntravel_info.MTO_PE = temp_pe;

                    double temp_emg = 0;
                    GetPmc(_pmcBom.MTO_EMG, ref temp_emg, m_page_flib);
                    m_mntravel_info.MTO_EMG = temp_emg;

                    double temp_sc = 0;
                    GetPmc(_pmcBom.MTO_SafeCock, ref temp_sc, m_page_flib);
                    m_mntravel_info.MTO_SafeCock = temp_sc;

                    double temp_tc = 0;
                    GetPmc(_pmcBom.MTO_TableClamp, ref temp_tc, m_page_flib);
                    m_mntravel_info.MTO_TableClamp = temp_tc;

                    double temp_dc = 0;
                    GetPmc(_pmcBom.MTO_DieClamp, ref temp_dc, m_page_flib);
                    m_mntravel_info.MTO_DieClamp = temp_dc;

                    double temp_sdc = 0;
                    GetPmc(_pmcBom.MTO_SafeDoorClose, ref temp_sdc, m_page_flib);
                    m_mntravel_info.MTO_SafeDoorClose = temp_sdc;

                    double temp_c = 0;
                    GetPmc(_pmcBom.MTO_Counter, ref temp_c, m_page_flib);
                    m_mntravel_info.MTO_Counter = temp_c;

                    double temp_j = 0;
                    GetPmc(_pmcBom.MTO_JOG, ref temp_j, m_page_flib);
                    m_mntravel_info.MTO_JOG = temp_j;

                    double temp_jok = 0;
                    GetPmc(_pmcBom.MTO_JOGOK, ref temp_jok, m_page_flib);
                    m_mntravel_info.MTO_JOGOK = temp_jok;

                    double temp_udc = 0;
                    GetPmc(_pmcBom.MTO_UpDieCenter, ref temp_udc, m_page_flib);
                    m_mntravel_info.MTO_UpDieCenter = temp_udc;

                    double temp_s = 0;
                    GetPmc(_pmcBom.MTO_Single, ref temp_s, m_page_flib);
                    m_mntravel_info.MTO_Single = temp_s;

                    double temp_sok = 0;
                    GetPmc(_pmcBom.MTO_SingleOK, ref temp_sok, m_page_flib);
                    m_mntravel_info.MTO_SingleOK = temp_sok;

                    double temp_cont = 0;
                    GetPmc(_pmcBom.MTO_Continue, ref temp_cont, m_page_flib);
                    m_mntravel_info.MTO_Continue = temp_cont;

                    double temp_contok = 0;
                    GetPmc(_pmcBom.MTO_ContinueOK, ref temp_contok, m_page_flib);
                    m_mntravel_info.MTO_ContinueOK = temp_contok;

                    Messenger.Default.Send<MaintainTravelInfo>(m_mntravel_info, "MaintainTravelInfoMsg");
                }

                #endregion

                #region 维护信息 移台控制
                if (m_mnmovetable == true)
                {
                    double temp_mp = 0;
                    GetPmc(_pmcBom.MMT_MovePump, ref temp_mp, m_page_flib);
                    m_mnmovetable_info.MMT_MovePump = temp_mp;

                    double temp_emg = 0;
                    GetPmc(_pmcBom.MMT_EMG, ref temp_emg, m_page_flib);
                    m_mnmovetable_info.MMT_EMG = temp_emg;

                    double temp_cm = 0;
                    GetPmc(_pmcBom.MMT_ChangeMode, ref temp_cm, m_page_flib);
                    m_mnmovetable_info.MMT_ChangeMode = temp_cm;

                    double temp_udc = 0;
                    GetPmc(_pmcBom.MMT_UpDieCenter, ref temp_udc, m_page_flib);
                    m_mnmovetable_info.MMT_UpDieCenter = temp_udc;

                    double temp_up = 0;
                    GetPmc(_pmcBom.MMT_UpBTN, ref temp_up, m_page_flib);
                    m_mnmovetable_info.MMT_UpBTN = temp_up;

                    double temp_ps = 0;
                    GetPmc(_pmcBom.MMT_PumpStation, ref temp_ps, m_page_flib);
                    m_mnmovetable_info.MMT_PumpStation = temp_ps;

                    double temp_omag = 0;
                    GetPmc(_pmcBom.MMT_OUT_Mag, ref temp_omag, m_page_flib);
                    m_mnmovetable_info.MMT_OUT_Mag = temp_omag;

                    double temp_uto = 0;
                    GetPmc(_pmcBom.MMT_UpTimeOut, ref temp_uto, m_page_flib);
                    m_mnmovetable_info.MMT_UpTimeOut = temp_uto;

                    double temp_uf = 0;
                    GetPmc(_pmcBom.MMT_UpFin, ref temp_uf, m_page_flib);
                    m_mnmovetable_info.MMT_UpFin = temp_uf;

                    double temp_ma = 0;
                    GetPmc(_pmcBom.MMT_MoveAmp, ref temp_ma, m_page_flib);
                    m_mnmovetable_info.MMT_MoveAmp = temp_ma;

                    double temp_sdo = 0;
                    GetPmc(_pmcBom.MMT_SafeDoorOpen, ref temp_sdo, m_page_flib);
                    m_mnmovetable_info.MMT_SafeDoorOpen = temp_sdo;

                    double temp_mo = 0;
                    GetPmc(_pmcBom.MMT_MoveOutBTN, ref temp_mo, m_page_flib);
                    m_mnmovetable_info.MMT_MoveOutBTN = temp_mo;

                    double temp_mof = 0;
                    GetPmc(_pmcBom.MMT_MoveOutFin, ref temp_mof, m_page_flib);
                    m_mnmovetable_info.MMT_MoveOutFin = temp_mof;

                    double temp_tt = 0;
                    GetPmc(_pmcBom.MMT_TwinTable, ref temp_tt, m_page_flib);
                    m_mnmovetable_info.MMT_TwinTable = temp_tt;

                    double temp_mi = 0;
                    GetPmc(_pmcBom.MMT_MoveInBTN, ref temp_mi, m_page_flib);
                    m_mnmovetable_info.MMT_MoveInBTN = temp_mi;

                    double temp_mif = 0;
                    GetPmc(_pmcBom.MMT_MoveInFin, ref temp_mif, m_page_flib);
                    m_mnmovetable_info.MMT_MoveInFin = temp_mif;

                    double temp_c = 0;
                    GetPmc(_pmcBom.MMT_ClampBTN, ref temp_c, m_page_flib);
                    m_mnmovetable_info.MMT_ClampBTN = temp_c;

                    double temp_imag = 0;
                    GetPmc(_pmcBom.MMT_In_Mag, ref temp_imag, m_page_flib);
                    m_mnmovetable_info.MMT_In_Mag = temp_imag;

                    double temp_cto = 0;
                    GetPmc(_pmcBom.MMT_ClampTimeOut, ref temp_cto, m_page_flib);
                    m_mnmovetable_info.MMT_ClampTimeOut = temp_cto;

                    double temp_cf = 0;
                    GetPmc(_pmcBom.MMT_ClampFin, ref temp_cf, m_page_flib);
                    m_mnmovetable_info.MMT_ClampFin = temp_cf;

                    Messenger.Default.Send<MaintainMoveTableInfo>(m_mnmovetable_info, "MaintainMoveTableInfoMsg");
                }

                #endregion

                #region 维护信息 IO
                if(m_mnio == true)
                {
                    Dictionary<string, byte> ioBuf = new Dictionary<string, byte>();

                    foreach(var item in m_mnio_info.InputBom)
                    {
                        if(ioBuf.ContainsKey(item.AdrType.ToString()+item.Adr))
                        {
                            byte bd = (byte)(0x01 << item.Bit);
                            item.Value = (ioBuf[item.AdrType.ToString() + item.Adr] & bd) > 0;
                        }
                        else
                        {
                            byte data = 0;
                            ReadPmcDataByByte_InTask(m_page_flib, (short)item.AdrType, item.Adr, ref data);
                            byte bd = (byte)(0x01 << item.Bit);
                            item.Value = (data & bd) > 0;

                            ioBuf.Add(item.AdrType.ToString() + item.Adr, data);
                        }
                    }

                    foreach (var item in m_mnio_info.OutputBom)
                    {
                        if (ioBuf.ContainsKey(item.AdrType.ToString() + item.Adr))
                        {
                            byte bd = (byte)(0x01 << item.Bit);
                            item.Value = (ioBuf[item.AdrType.ToString() + item.Adr] & bd) > 0;
                        }
                        else
                        {
                            byte data = 0;
                            ReadPmcDataByByte_InTask(m_page_flib, (short)item.AdrType, item.Adr, ref data);
                            byte bd = (byte)(0x01 << item.Bit);
                            item.Value = (data & bd) > 0;

                            ioBuf.Add(item.AdrType.ToString() + item.Adr, data);
                        }
                    }

                    Messenger.Default.Send<MaintainIoInfo>(m_mnio_info, "MaintainIoInfoMsg");
                }

                #endregion

            }

            FreeConnect(m_page_flib);
        }

        public void ScannerPage_Start()
        {
            //TODO:NO CNC
            if (_simulate == false) m_page_BackgroundWorker.RunWorkerAsync();
        }

        public void ScannerPage_Cancel()
        {
            m_page_BackgroundWorker.CancelAsync();
        }

        private void ScannerPageCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            FreeConnect(m_page_flib);
            m_page_flib = 0;
        }

        #endregion

        #region 曲线
        private void MonitorLineFunc(object sender, DoWorkEventArgs e)
        {
            short ret = 0;
            short conn = -16;
            double index = 0;
            DateTime temp_time=DateTime.Now;
            double temp_pos=0;
            bool inflag = false;
            bool first_item = true;

            while (m_monitorline_BackgroundWorker.CancellationPending == false)
            {
                Thread.Sleep(m_monitorline_freq);

                if (_slidingBlockTableLoaded == false) continue;

                if (conn != 0)
                {
                    FreeConnect(m_monitorline_flib);

                    ret = BuildConnect(ref m_monitorline_flib);
                    if (ret == 0) conn = 0;
                }

                {
                    ReadPmcDataByBit_InTask(m_monitorline_flib, _baseInfo.RealTimeSciChartInflgAdrType,
                        _baseInfo.RealTimeSciChartInflgAdr,
                        _baseInfo.RealTimeSciChartInflgBit,
                        ref inflag);
                }

                inflag = true;

                if (inflag == true)
                {

                    if (first_item == true)
                    {
                        Focas1.ODBAXIS mac = new Focas1.ODBAXIS();
                        ret = Focas1.cnc_machine(m_monitorline_flib, 13, 200, mac);
                        if (ret == -16) conn = -16;

                        if (ret == 0)
                        {
                            double sliding = (double)mac.data[0] / _baseInfo.Increment;
                            temp_pos = sliding;
                            temp_time = DateTime.Now;
                            first_item = false;
                        }
                    }
                    else
                    {

                        try
                        {
                            Focas1.ODBAXIS absolute = new Focas1.ODBAXIS();
                            var ret_1 = Focas1.cnc_machine(m_monitorline_flib, 13, 200, absolute);
                            if (ret_1 == -16) conn = -16;
                            
                            Focas1.ODBACT fbuf = new Focas1.ODBACT();
                            var ret_2 = Focas1.cnc_actf(m_monitorline_flib, fbuf);
                            if (ret_2 == -16) conn = -16;

                            if (ret_1 == 0 && ret_2==0)
                            {
                                var time_span = (DateTime.Now - temp_time).TotalMilliseconds;

                                double screw = (double)absolute.data[0] / _baseInfo.Increment;
                                double sliding = 0;
                                var ret_sliding = SearchSlidingData(screw, ref sliding);

                                if(ret_sliding==null)
                                {

                                    double speed = 0;
                                    var ret_speed = SearchSlidingSpeedData(sliding, fbuf.data, ref speed);

                                    if(ret_speed==null)
                                    {
                                        m_monitorline_info.Pos = sliding;
                                        m_monitorline_info.Speed = speed;


                                        int sp_temp = 0;
                                        GetPmc(_pmcBom.SMP_SliderPressure, ref sp_temp, m_page_flib);
                                        m_monitorline_info.Tem = sp_temp;

                                        int st_temp = 0;
                                        GetPmc(_pmcBom.SMP_SliderTemperature, ref st_temp, m_page_flib);
                                        m_monitorline_info.Press = st_temp;

                                        index += time_span;
                                        m_monitorline_info.Time = index;
                                        Messenger.Default.Send<StateMonitorLineChartData>(m_monitorline_info, "StateMonitorLineChartDataMsg");

                                    }


                                }
                            }
                        }
                        catch { }
                    }
                }
                else
                {

                    first_item = true;
                    index = 0;
                }

            }

            FreeConnect(m_monitorline_flib);
        }

        public void MonitorLine_Start()
        {
            try
            {
                //TODO:NO CNC
                if (_simulate == false) m_monitorline_BackgroundWorker.RunWorkerAsync();
                m_monitorline_indo = true;
            }
            catch
            {

            }
        }

        public void MonitorLine_Cancel()
        {
            m_monitorline_BackgroundWorker.CancelAsync();
        }

        private void MonitorLineCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            m_monitorline_indo = false;

            FreeConnect(m_monitorline_flib);
            m_monitorline_flib = 0;
        }

        #endregion

        #region 预览曲线
        private void SimulateMonitorLineFunc(object sender, DoWorkEventArgs e)
        {
            short ret = 0;
            short conn = -16;
            double index = 0;
            DateTime temp_time = DateTime.Now;
            double temp_pos = 0;
            bool inflag = false;
            bool first_item = true;

            while (m_simulatemonitorline_BackgroundWorker.CancellationPending == false)
            {
                Thread.Sleep(m_simulatemonitorline_freq);

                if (_slidingBlockTableLoaded == false) continue;

                if (conn != 0)
                {
                    FreeConnect(m_simulatemonitorline_flib);

                    ret = BuildConnect(ref m_simulatemonitorline_flib);
                    if (ret == 0) conn = 0;
                }

                {
                    ReadPmcDataByBit_InTask(m_simulatemonitorline_flib, _baseInfo.SimulateSciChartInflgAdrType,
                        _baseInfo.SimulateSciChartInflgAdr,
                        _baseInfo.SimulateSciChartInflgBit,
                        ref inflag);
                }

                if (inflag == true)
                {

                    if (first_item == true)
                    {
                        Focas1.ODBAXIS absolute = new Focas1.ODBAXIS();
                        ret = Focas1.cnc_absolute2(m_simulatemonitorline_flib, -1, 200, absolute);
                        if (ret == -16) conn = -16;

                        double sliding = 0;
                        var ret_sliding = SearchSlidingData((double)absolute.data[0] / _baseInfo.Increment, ref sliding);

                        if (ret_sliding == null)
                        {
                            temp_pos = sliding;
                            temp_time = DateTime.Now;
                            first_item = false;
                        }
                    }
                    else
                    {

                        try
                        {
                            Focas1.ODBAXIS absolute = new Focas1.ODBAXIS();
                            ret = Focas1.cnc_absolute2(m_simulatemonitorline_flib, -1, 200, absolute);
                            if (ret == -16) conn = -16;

                            short svmeter_num = 1;
                            Focas1.ODBSVLOAD loadmeter = new Focas1.ODBSVLOAD();
                            Focas1.cnc_rdsvmeter(m_simulatemonitorline_flib, ref svmeter_num, loadmeter);

                            if (ret == 0)
                            {
                                var time_span = (DateTime.Now - temp_time).TotalMilliseconds;

                                double screw = (double)absolute.data[0] / _baseInfo.Increment;
                                double sliding = 0;
                                var ret_sliding = SearchSlidingData(screw, ref sliding);

                                if (ret_sliding == null)
                                {
                                    m_simulatemonitorline_info.Pos = sliding;
                                    m_simulatemonitorline_info.Speed = (m_simulatemonitorline_info.Pos - temp_pos) / time_span * 1000.0;
                                    m_simulatemonitorline_info.Tem = screw;
                                    m_simulatemonitorline_info.Press = loadmeter.svload1.data * Math.Pow(10, -1 * loadmeter.svload1.dec);

                                    temp_pos = m_simulatemonitorline_info.Pos;
                                    temp_time = DateTime.Now;



                                    index += time_span;
                                    m_simulatemonitorline_info.Time = index;
                                    Messenger.Default.Send<StateMonitorLineChartData>(m_simulatemonitorline_info, "SimulateLineChartDataMsg");
                                }
                            }
                        }
                        catch { }
                    }
                }
                else
                {

                    first_item = true;
                    index = 0;
                }

            }

            FreeConnect(m_simulatemonitorline_flib);
        }

        public void SimulateMonitorLine_Start()
        {
            try
            {
                //TODO:NO CNC
                if (_simulate == false) m_simulatemonitorline_BackgroundWorker.RunWorkerAsync();
                m_simulatemonitorline_indo = true;
            }
            catch
            {

            }
        }

        public void SimulateMonitorLine_Cancel()
        {
            m_simulatemonitorline_BackgroundWorker.CancelAsync();
        }

        private void SimulateMonitorLineCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            m_simulatemonitorline_indo = false;

            FreeConnect(m_simulatemonitorline_flib);
            m_simulatemonitorline_flib = 0;
        }
        #endregion

        #endregion

        #region 公共函数
        private short BuildConnect(ref ushort flib)
        {
            short ret = 0;
            Focas1.cnc_freelibhndl(flib);


            //ret = Focas1.cnc_allclibhndl2(0, out flib);
            ret = Focas1.cnc_allclibhndl3(_baseInfo.Ip, _baseInfo.Port, _baseInfo.Timeout, out flib);
            return ret;
        }

        private short FreeConnect(ushort flib)
        {
            short ret = Focas1.cnc_freelibhndl(flib);
            return ret;
        }

        private short GetMacro(short mac_num, ref double data, ushort flib)
        {
            Focas1.ODBM buf = new Focas1.ODBM();

            var ret = Focas1.cnc_rdmacro(flib, mac_num, 10, buf);
            if (ret != 0) return ret;
            int mcr = buf.mcr_val;
            short dec = buf.dec_val;
            dec = (short)(dec * (-1));
            data = (double)(mcr * Math.Pow(10, dec));


            return 0;
        }

        private short GetPmc<T>(PmcBomItem item, ref T data, ushort flib)
        {
            if (item == null) return -999;

            short ret = 0;

            switch (item.DataType)
            {
                case PmcDataTypeEnum.BIT:
                    bool bTemp = false;
                    ret = ReadPmcDataByBit_InTask(flib, (short)item.AdrType, item.Adr, item.Bit, ref bTemp);
                    data = (T)Convert.ChangeType(bTemp, typeof(T));
                    break;
                case PmcDataTypeEnum.BYTE:
                    byte cTemp = 0;
                    ret = ReadPmcDataByByte_InTask(flib, (short)item.AdrType, item.Adr, ref cTemp);
                    if (item.ConversionFactor.HasValue == true)
                    {
                        if (item.ConversionFactor.Value != 0)
                        {
                            var temp = (double)cTemp / item.ConversionFactor.Value;
                            data = (T)Convert.ChangeType(temp, typeof(T));

                        }
                        else
                        {
                            data = (T)Convert.ChangeType(cTemp, typeof(T));
                        }
                    }
                    else data = (T)Convert.ChangeType(cTemp, typeof(T));
                    break;
                case PmcDataTypeEnum.WORD:
                    short iTemp = 0;
                    ret = ReadPmcDataByWord_InTask(flib, (short)item.AdrType, item.Adr, ref iTemp);
                    if (item.ConversionFactor.HasValue == true)
                    {
                        if (item.ConversionFactor.Value != 0)
                        {
                            var temp = (double)iTemp / item.ConversionFactor.Value;
                            data = (T)Convert.ChangeType(temp, typeof(T));

                        }
                        else
                        {
                            data = (T)Convert.ChangeType(iTemp, typeof(T));
                        }
                    }
                    else data = (T)Convert.ChangeType(iTemp, typeof(T));
                    break;
                case PmcDataTypeEnum.LONG:
                    int lTemp = 0;
                    ret = ReadPmcDataByLong_InTask(flib, (short)item.AdrType, item.Adr, ref lTemp);
                    if (item.ConversionFactor.HasValue == true)
                    {
                        if (item.ConversionFactor.Value != 0)
                        {
                            var temp = (double)lTemp / item.ConversionFactor.Value;
                            data = (T)Convert.ChangeType(temp, typeof(T));

                        }
                        else
                        {
                            data = (T)Convert.ChangeType(lTemp, typeof(T));
                        }
                    }
                    else data = (T)Convert.ChangeType(lTemp, typeof(T));
                    break;
            }

            return ret;
        }

        private short ReadPmcDataByBit_InTask(ushort flib, short type, ushort adr, ushort bit, ref bool data)
        {

            Focas1.IODBPMC0 buf = new Focas1.IODBPMC0();
            buf.cdata = new byte[1];
            short ret = Focas1.pmc_rdpmcrng(flib, type, 0, adr, adr, 9, buf);
            if (ret != 0) return ret;

            byte bd = (byte)(0x01 << bit);
            data = (buf.cdata[0] & bd) > 0;

            return 0;
        }

        private short ReadPmcDataByByte_InTask(ushort flib, short type, ushort adr, ref byte data)
        {
            Focas1.IODBPMC0 buf = new Focas1.IODBPMC0();
            buf.cdata = new byte[1];
            short ret = Focas1.pmc_rdpmcrng(flib, type, 0, adr, adr, 9, buf);
            if (ret != 0) return ret;

            data = buf.cdata[0];

            return 0;
        }

        private short ReadPmcDataByWord_InTask(ushort flib, short type, ushort adr, ref short data)
        {
            Focas1.IODBPMC1 buf = new Focas1.IODBPMC1();
            buf.idata = new short[1];
            ushort adr_end = (ushort)(adr + 1);
            short ret = Focas1.pmc_rdpmcrng(flib, type, 0, adr, adr_end, 10, buf);
            if (ret != 0) return ret;

            data = buf.idata[0];

            return 0;
        }

        private short ReadPmcDataByLong_InTask(ushort flib, short type, ushort adr, ref int data)
        {
            Focas1.IODBPMC2 buf = new Focas1.IODBPMC2();
            buf.ldata = new int[1];
            ushort adr_end = (ushort)(adr + 3);
            short ret = Focas1.pmc_rdpmcrng(flib, type, 0, adr, adr_end, 12, buf);
            if (ret != 0) return ret;

            data = buf.ldata[0];

            return 0;
        }

        private string SetPmc_InTask(ushort flib, PmcBomItem pmc, LimitBomItem limit, string data)
        {
            short ret = 0;
            bool ret_parse = false;

            switch (pmc.DataType)
            {
                case PmcDataTypeEnum.BIT:
                    bool bTemp = false;
                    ret_parse = bool.TryParse(data, out bTemp);
                    if (ret_parse == false) return "设定数据失败,输入数据格式不正确";

                    if (ret_parse == true)
                    {
                        ret = WritePmcDataByBit_InTask(flib, (short)pmc.AdrType, pmc.Adr, pmc.Bit, bTemp);
                        if (ret != 0) return "设定数据失败,CNC设定失败";
                    }
                    break;
                case PmcDataTypeEnum.BYTE:
                    byte cTemp = 0;
                    if (pmc.ConversionFactor == null)
                    {
                        ret_parse = byte.TryParse(data, out cTemp);

                        if (limit != null)
                        {
                            if (limit.LimitDown.HasValue)
                            {
                                if (cTemp < limit.LimitDown.Value) return "设定数据失败,数据超限";
                            }
                            if (limit.LimitUp.HasValue)
                            {
                                if (cTemp > limit.LimitUp.Value) return "设定数据失败,数据超限";
                            }
                        }
                    }
                    else
                    {
                        double db;
                        ret_parse = double.TryParse(data, out db);

                        if (limit != null)
                        {
                            if (limit.LimitDown.HasValue)
                            {
                                if (db < limit.LimitDown.Value) return "设定数据失败,数据超限";
                            }
                            if (limit.LimitUp.HasValue)
                            {
                                if (db > limit.LimitUp.Value) return "设定数据失败,数据超限";
                            }
                        }

                        if (db * pmc.ConversionFactor > 255) return "设定数据失败,输入数据超限(系统)";
                        cTemp = (byte)(db * pmc.ConversionFactor);
                    }

                    if (ret_parse == false) return "设定数据失败,输入数据格式不正确";

                    if (ret_parse == true)
                    {
                        ret = WritePmcDataByByte_InTask(flib, (short)pmc.AdrType, pmc.Adr, cTemp);
                        if (ret != 0) return "设定数据失败,CNC设定失败";
                    }
                    break;
                case PmcDataTypeEnum.WORD:
                    short iTemp = 0;

                    if (pmc.ConversionFactor == null)
                    {
                        ret_parse = short.TryParse(data, out iTemp);

                        if (limit != null)
                        {
                            if (limit.LimitDown.HasValue)
                            {
                                if (iTemp < limit.LimitDown.Value) return "设定数据失败,数据超限";
                            }
                            if (limit.LimitUp.HasValue)
                            {
                                if (iTemp > limit.LimitUp.Value) return "设定数据失败,数据超限";
                            }
                        }
                    }
                    else
                    {
                        double db;
                        ret_parse = double.TryParse(data, out db);

                        if (limit.LimitDown.HasValue)
                        {
                            if (db < limit.LimitDown.Value) return "设定数据失败,数据超限";
                        }
                        if (limit.LimitUp.HasValue)
                        {
                            if (db > limit.LimitUp.Value) return "设定数据失败,数据超限";
                        }

                        if (db * pmc.ConversionFactor > 65535) return "设定数据失败,输入数据超限(系统)";
                        iTemp = (short)(db * pmc.ConversionFactor);
                    }

                    if (ret_parse == false) return "设定数据失败,输入数据格式不正确";

                    if (ret_parse == true)
                    {
                        ret = WritePmcDataByWord_InTask(flib, (short)pmc.AdrType, pmc.Adr, iTemp);
                        if (ret != 0) return "设定数据失败,CNC设定失败";
                    }
                    break;
                case PmcDataTypeEnum.LONG:
                    int lTemp = 0;

                    if (pmc.ConversionFactor == null)
                    {
                        ret_parse = int.TryParse(data, out lTemp);

                        if (limit.LimitDown.HasValue)
                        {
                            if (lTemp < limit.LimitDown.Value) return "设定数据失败,数据超限";
                        }
                        if (limit.LimitUp.HasValue)
                        {
                            if (lTemp > limit.LimitUp.Value) return "设定数据失败,数据超限";
                        }

                    }
                    else
                    {
                        double db;
                        ret_parse = double.TryParse(data, out db);

                        if (limit.LimitDown.HasValue)
                        {
                            if (db < limit.LimitDown.Value) return "设定数据失败,数据超限";
                        }
                        if (limit.LimitUp.HasValue)
                        {
                            if (db > limit.LimitUp.Value) return "设定数据失败,数据超限";
                        }

                        if (db * pmc.ConversionFactor > 4294967295) return "设定数据失败,输入数据超限(系统)";
                        lTemp = (int)(db * pmc.ConversionFactor);
                    }
                    if (ret_parse == false) return "设定数据失败,输入数据格式不正确";


                    if (ret_parse == true)
                    {
                        ret = WritePmcDataByLong_InTask(flib, (short)pmc.AdrType, pmc.Adr, lTemp);
                        if (ret != 0) return "设定数据失败,CNC设定失败";
                    }
                    break;
            }



            return null;
        }

        private string SetSlidingTableDataPmc_InTask(ushort flib, PmcBomItem pmc, LimitBomItem limit, string data)
        {
            short ret = 0;
            bool ret_parse = false;

            switch (pmc.DataType)
            {
                case PmcDataTypeEnum.LONG:
                    int lTemp = 0;
                    double db;

                    if (pmc.ConversionFactor == null)
                    {
                        return "设定数据失败,PMC数据配置错误,没有定义转换因子";

                    }
                    else
                    {
                        ret_parse = double.TryParse(data, out db);

                        if (limit.LimitDown.HasValue)
                        {
                            if (db < limit.LimitDown.Value) return "设定数据失败,数据超限";
                        }
                        if (limit.LimitUp.HasValue)
                        {
                            if (db > limit.LimitUp.Value) return "设定数据失败,数据超限";
                        }

                        if (db * pmc.ConversionFactor > 4294967295) return "设定数据失败,输入数据超限(系统)";
                        //lTemp = (int)(db * pmc.ConversionFactor);
                    }
                    if (ret_parse == false) return "设定数据失败,输入数据格式不正确";


                    if (ret_parse == true)
                    {
                        if(_slidingBlockTableLoaded==false) return "设定数据失败,工艺数据表中未成功加载";

                        
                        double screw = 0;
                        var ret_screw = SearchScrewData(db, ref screw);
                        if (ret_screw != null) return ret_screw;
                        lTemp = (int)(screw * pmc.ConversionFactor);

                        ret = WritePmcDataByLong_InTask(flib, (short)pmc.AdrType, pmc.Adr, lTemp);
                        if (ret != 0) return "设定数据失败,CNC设定失败";

                        //设定对应的E地址
                        ret = WritePmcDataByLong_InTask(flib, (short)PmcAdrTypeEnum.E, pmc.Adr, (int)(db * pmc.ConversionFactor));
                        if (ret != 0) return "设定数据失败,CNC设定失败";

                    }
                    break;
                default:
                    return "设定数据失败,数据类型错误";
            }



            return null;
        }

        private short WritePmcDataByBit_InTask(ushort flib, short type, ushort adr, ushort bit, bool data)
        {

            Focas1.IODBPMC0 buf = new Focas1.IODBPMC0();
            buf.cdata = new byte[1];
            var ret = Focas1.pmc_rdpmcrng(flib, type, 0, adr, adr, 9, buf);
            if (ret != 0) return ret;

            byte bd = (byte)(0x01 << bit);
            if (data == true)
            {
                buf.cdata[0] = (byte)(buf.cdata[0] | bd);
            }
            else
            {
                buf.cdata[0] = (byte)(buf.cdata[0] & (~bd));
            }

            ret = Focas1.pmc_wrpmcrng(flib, 9, buf);
            if (ret != 0) return ret;

            return 0;
        }

        private short WritePmcDataByByte_InTask(ushort flib, short type, ushort adr, byte data)
        {
            Focas1.IODBPMC0 buf = new Focas1.IODBPMC0();
            buf.cdata = new byte[12];
            buf.cdata[0] = data;
            buf.datano_s = (short)adr;
            buf.datano_e = (short)adr;
            buf.type_a = type;
            buf.type_d = 0;
            var ret = Focas1.pmc_wrpmcrng(flib, 9, buf);

            return ret;
        }

        private short WritePmcDataByWord_InTask(ushort flib, short type, ushort adr, short data)
        {

            Focas1.IODBPMC1 buf = new Focas1.IODBPMC1();
            buf.idata = new short[5];
            buf.idata[0] = data;
            buf.datano_s = (short)adr;
            buf.datano_e = (short)(adr + 1);
            buf.type_a = type;
            buf.type_d = 1;
            var ret = Focas1.pmc_wrpmcrng(flib, 10, buf);

            return ret;
        }

        private short WritePmcDataByLong_InTask(ushort flib, short type, ushort adr, int data)
        {

            Focas1.IODBPMC2 buf = new Focas1.IODBPMC2();
            buf.ldata = new int[5];
            buf.ldata[0] = data;
            buf.datano_s = (short)adr;
            buf.datano_e = (short)(adr + 3);
            buf.type_a = type;
            buf.type_d = 2;
            var ret = Focas1.pmc_wrpmcrng(flib, 12, buf);

            return ret;
        }

        public string SetMacro_InTask(ushort flib, MacroBomItem macro, LimitBomItem limit, string data)
        {

            bool ret_parse = false;

            double dtemp;
            ret_parse = double.TryParse(data, out dtemp);

            if (limit != null)
            {
                if (limit.LimitDown.HasValue)
                {
                    if (dtemp < limit.LimitDown.Value) return "设定数据失败,数据超限";
                }
                if (limit.LimitUp.HasValue)
                {
                    if (dtemp > limit.LimitUp.Value) return "设定数据失败,数据超限";
                }
            }

            var ret = WriteMacroData_InTask(flib, macro.Adr, dtemp);

            if (ret != 0) return "设定数据失败,CNC设定失败";

            return null;
        }

        private short WriteMacroData_InTask(ushort flib, int mac_num, double data)
        {
            int num = 1;
            var ret = Focas1.cnc_wrmacror2(flib, mac_num, ref num, data);

            return 0;
        }

        private short ReadCycleTime_InTask(ushort flib, ref int time)
        {
            time = 0;

            Focas1.IODBPSD param1 = new Focas1.IODBPSD();
            var ret = Focas1.cnc_rdparam(flib, 6757, -1, 8, param1);
            if (ret != 0) return ret;
            Focas1.IODBPSD param2 = new Focas1.IODBPSD();
            ret = Focas1.cnc_rdparam(flib, 6758, -1, 8, param2);
            if (ret != 0) return ret;

            time = param1.u.ldata / 1000 + param2.u.ldata * 60;

            return 0;
        }

        #endregion


    }
}
