using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ReversiDebugUI
{
    /// <summary>
    /// StateControl.xaml 的交互逻辑
    /// </summary>
    public partial class StateControl : UserControl
    {
        public StateControl()
        {
            InitializeComponent();
        }

        private bool isVeryBusy = false;
        /// <summary>
        /// 设置当前状态为非常忙
        /// </summary>
        public bool IsVeryBusy
        {
            get
            {
                return isVeryBusy;
            }
            set
            {
                if (value) busyBar.Visibility = Visibility.Visible;
                else busyBar.Visibility = Visibility.Collapsed;
                isVeryBusy = value;
            }
        }

        public string CurrentStateText
        {
            set
            {
                stateText.Text = value;
                IsVeryBusy = false;
            }
        }
    }
}
