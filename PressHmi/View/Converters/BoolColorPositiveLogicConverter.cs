﻿using System;
using System.Windows.Data;
using System.Globalization;

namespace PressHmi.View.Converters
{

    [ValueConversion(typeof(bool), typeof(string))]
    public class BoolColorPositiveLogicConverter : IValueConverter
    {
        public object Convert(object value,
            Type targetType, object parameter, CultureInfo culture)
        {
            bool temp = (bool)value;
            if (temp == true) return "Green";
            else return "#FF212121";
        }
        //反转换方法，将字符串转换为日期类型
        public object ConvertBack(object value,
            Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}