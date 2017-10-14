using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows.Controls;
using System.Windows.Input;
using GraphicFilters.ViewModels.Commands;

namespace GraphicFilters.ViewModels
{
    public class GaussianBlurDialogViewModel : INotifyPropertyChanged
    {
        private DataTable kernel;
        private string text;


        public GaussianBlurDialogViewModel()
        {
            kernel = new DataTable();
            kernel.Columns.Add("");
            kernel.Columns.Add("");
            kernel.Columns.Add("");
            kernel.Columns.Add("");
            kernel.Rows.Add(1, 2, 3, 4);
            kernel.Rows.Add(4, 3, 2, 1);
            kernel.Rows.Add(1, 2, 3, 4);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public DataTable Kernel
        {
            get { return kernel; }
            set
            {
                kernel = value;
                OnPropertyChanged(nameof(Kernel));
            }
        }
        public string Text
        {
            get { return text; }
            set
            {
                text = value;
                OnPropertyChanged("Text");
            }
        }

        public ICommand Add { get { return new RelayCommand(addRow, () => true); } }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void addRow()
        {
            kernel.Rows.Add(1, 2, 3, 4);
            OnPropertyChanged("Kernel");
        }
    }
}
