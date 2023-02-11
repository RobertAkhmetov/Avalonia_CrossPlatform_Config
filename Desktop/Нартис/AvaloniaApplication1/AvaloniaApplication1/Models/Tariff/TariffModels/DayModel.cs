using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AvaloniaApplication1.Models.Tariff.TariffModels
{

    public class SwitchMarkModel:ObservableObject,ICloneable //класс - заглушка (TODO снять,когда класс будет реализован)
    {
        private bool _fullDay;
        private bool _isActive = true;
        [JsonIgnore] public RelayCommand<SwitchMarkModel> IsFullDayCommand;

        private int _tariff = 1;
        private DateTime time;

        public SwitchMarkModel()
        {
            Time = new DateTime(DateTime.Now.Year, 1, 1, 0, 0, 0);
        }

        public DateTime Time
        {
            get => time;
            //set => Set("Time", ref time, value);  //"SetProperty" вместо "set" в этом ObservableObject
            set => SetProperty(ref time, value, "Time");
        }

        public int Tariff
        {
            get => _tariff;
            //set => Set("Tariff", ref _tariff, value);
            set => SetProperty(ref _tariff, value, "Tariff");
        }

        public bool IsFullDay
        {
            get => _fullDay;
            set
            {
                //Set("IsFullDay", ref _fullDay, value);
                SetProperty(ref _fullDay, value, "IsFullDay");
                IsFullDayCommand?.Execute(this);
            }
        }

        public bool IsActive
        {
            get => _isActive;
            //set => Set("IsActive", ref _isActive, value);
            set => SetProperty(ref _isActive, value, "IsActive");
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

    }

    public class DayModel : ObservableObject, ICloneable
    {
        private ObservableCollection<SwitchMarkModel> _switchMarks = new ObservableCollection<SwitchMarkModel>() { new SwitchMarkModel() { } };
        private string _description;

        [JsonIgnore]
        public string Description { get => _description; set => SetProperty(ref _description, value, "Description"); }
        public ObservableCollection<SwitchMarkModel> SwitchMarks { get => _switchMarks; set => SetProperty(ref _switchMarks, value, "SwitchMarks"); }
        public int Index { get; set; }
        [JsonIgnore]
        public SolidColorBrush DayColor => new SolidColorBrush((Avalonia.Media.Color)Avalonia.Media.Color.Parse(_colors[Index - 1]));//переопределил тут парс с конверта 

        private string[] _colors = new[] { "#ffc0cb", "#008080", "#ff0000", "#ffd700", "#40e0d0",
                                            "#0000ff", "#ffa500", "#800080", "#800000", "#00ff00",
                                            "#20b2aa", "#ff00ff", "#008000", "#088da5", "#ff7f50",
                                            "#daa520", "#794044", "#ff4040", "#999999", "#6897bb", "#ff1493",
                                            "#6dc066", "#ccff00", "#00ff00", "#808080" };

        public DayModel(int dayIndex)
        {
            Index = dayIndex;
            //TODO: закончить инициализацию
        }


        public void UpdateDescription()
        {
            var descrption = string.Empty;
            foreach (var sm in SwitchMarks)
            {
                if (sm.IsFullDay)
                {
                    descrption = "T" + sm.Tariff;
                    break;
                }

                if (!descrption.Equals(string.Empty)) descrption += "\n";
                descrption += $"{sm.Time.ToString("HH:mm:ss")} T{sm.Tariff}";
            }

            Description = descrption;
        }

        internal void ReorderSMM()
        {
            SwitchMarks = new ObservableCollection<SwitchMarkModel>(SwitchMarks.OrderBy(value => value.Time));
            UpdateDescription();
        }

        [JsonIgnore]
        public bool IsFullDayActive
        {
            get
            {
                return SwitchMarks.Count(value => value.IsFullDay) > 0;
            }
        }

        public object Clone()
        {
            var clone = MemberwiseClone() as DayModel;
            clone.SwitchMarks = new ObservableCollection<SwitchMarkModel>();
            foreach (var model in SwitchMarks)
            {
                clone.SwitchMarks.Add((SwitchMarkModel)model.Clone());
            }
            return clone;
        }
    }
}
