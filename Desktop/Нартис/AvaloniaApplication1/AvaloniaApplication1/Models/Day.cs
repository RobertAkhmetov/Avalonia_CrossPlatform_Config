//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

namespace AvaloniaApplication1.Models
{
    using System;
    using Avalonia.Media;//using System.Windows.Media;
    using CommunityToolkit.Mvvm; //    using GalaSoft.MvvmLight;
    using CommunityToolkit.Mvvm.Messaging; //    using GalaSoft.MvvmLight.Messaging;
    using CommunityToolkit.Mvvm.ComponentModel; //для включения observableobject
    using System.Drawing;
    using AvaloniaApplication1.Models.Tariff.TariffModels;

    //разблокировать позже using AvaloniaApplication1.Models.Tariff.TariffModels;


    public class Day: CommunityToolkit.Mvvm.ComponentModel.ObservableObject
    {
        public DateTime Date;

        private DayUIModel currentTemplate;

        private Avalonia.Media.Brush dayColor;

        public Day(DateTime date)
        {
            this.Date = date;
            this.Title = date.Day.ToString();
            if (date.DayOfWeek == DayOfWeek.Sunday || date.DayOfWeek == DayOfWeek.Saturday) this.IsHoliday = true;
        }
        public DayUIModel CurrentTemplate
        {
            get => this.currentTemplate;
            set
            {
                this.SetProperty(ref this.currentTemplate, value, "CurrentTemplate");
                this.DayColor = this.currentTemplate?.DayModel?.DayColor ?? new SolidColorBrush((Avalonia.Media.Color)Avalonia.Media.Color.Parse("#FFFFFF"));
            }
        }

        public Avalonia.Media.Brush DayColor
        {
            get => this.dayColor;
            set => this.SetProperty(ref this.dayColor, value, "DayColor");
        }

        public bool IsHoliday { get; set; }

        public string Title { get; set; }

    }
}
