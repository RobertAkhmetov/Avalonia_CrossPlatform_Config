using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaApplication1.Models.Tariff.TariffModels
{
    public class DayUIModel : ObservableObject, ICloneable
    {
        private DayModel _dayModel;

        public DayModel DayModel { get => _dayModel; set => SetProperty(ref _dayModel, value, "DayModel"); }
        public DayUIModel(int index)
        {
            DayModel = new DayModel(index);
            DayModel.UpdateDescription();
        }

        public DayUIModel(int index, ObservableCollection<SwitchMarkModel> models)
        {
            DayModel = new DayModel(index) { SwitchMarks = models };
            DayModel.UpdateDescription();
        }

        public DayUIModel()
        {

        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
