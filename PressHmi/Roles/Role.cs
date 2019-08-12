﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using Newtonsoft.Json;
using FanucCnc.Model;
using FanucCnc.Enum;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using DataCenter.Model;
using DataCenter.Services;

namespace PressHmi.Roles
{
    public class Role
    {
        private static Role _instance = null;
        public OperatorAuthTypeEnum OperatorRole { get; set; }

        public static Role CreateInstance()
        {
            if (_instance == null)

            {
                _instance = new Role();
            }
            return _instance;
        }

        public Role()
        {
            Initial();
        }

        public void Initial()
        {
            OperatorRole = OperatorAuthTypeEnum.OPERATOR;
            Messenger.Default.Send<OperatorAuthTypeEnum>(OperatorRole, "OperatorAuthMsg");
        }

        public bool SetOperatorRole(OperatorAuthTypeEnum role,string pwd)
        {
            var srv = new RoleInfoService();
            var ret = srv.AuthR((int)role, pwd);

            if (ret == false) return ret;

            OperatorRole = role;

            Messenger.Default.Send<OperatorAuthTypeEnum>(role, "OperatorAuthMsg");

            return true;
        }
    }
}
