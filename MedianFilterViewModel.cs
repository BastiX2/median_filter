using Microsoft.Win32;
using System.Windows;
using System.Drawing;
using System.Runtime.InteropServices;
using System;
using System.Windows.Media;
using System.Windows.Interop;
using System.ComponentModel;
using System.Windows.Input;
using MedianFilter;

namespace MedianFilterProject
{
    class MedianFilterViewModel : INotifyPropertyChanged
    {

        private int filterSelectedValue;

        public int FilterSelectedValue
        {
            get { return filterSelectedValue; }
            set { filterSelectedValue = value; }
        }


        private ImageSource originalImageSource;

        public ImageSource OriginalImageSource
        {
            get { return originalImageSource; }
            set {
                originalImageSource = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OriginalImageSource"));
            }
        }

        private ImageSource filteredImageSource;

        public ImageSource FilteredImageSource
        {
            get { return filteredImageSource; }
            set {
                filteredImageSource = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FilteredImageSource"));
            }
        }

        private Bitmap originalBitmap;

        public Bitmap OriginalBitmap
        {
            get { return originalBitmap; }
            set {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OriginalBitmap"));
                originalBitmap = value;
            }
        }

        private Bitmap filteredBitmap;

        public event PropertyChangedEventHandler PropertyChanged;

        public Bitmap FilteredBitmap
        {
            get { return filteredBitmap; }
            set {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FilteredBitmap"));
                filteredBitmap = value;
            }
        }

        private ICommand displayNewBitmap = null;

        public ICommand DisplayNewBitmapCommand
        {
            get
            {
                if (displayNewBitmap == null)
                {
                    displayNewBitmap = new RelayCommand(DisplayNewBitmap, param => CanDisplayBitmap(param));
                }
                return displayNewBitmap;
            }

        }

        private ICommand filterBitmapCommand;

        public ICommand FilterBitmapCommand
        {
            get { return filterBitmapCommand; }
            set {
                filterBitmapCommand = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FilteredBitmap"));              
            }
        }

        private ICommand saveNewBitmap = null;

        public ICommand SaveNewBitmapCommand
        {
            get
            {
                if (saveNewBitmap == null)
                {
                    saveNewBitmap = new RelayCommand(SaveBitmap, param => CanSaveBitmap(param));
                }
                return saveNewBitmap;
            }

        }


        private void DisplayNewBitmap(Object obj)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files(*.BMP; *.JPG; *.GIF; *.PNG)| *.BMP; *.JPG; *.GIF; *.PNG";
            openFileDialog.ShowDialog();
            OriginalBitmap = new Bitmap(openFileDialog.FileName);
            OriginalImageSource = BitmapConverter.ImageSourceForBitmap(OriginalBitmap);

            FilteredBitmap = MedianFilter.FilterBitmap(OriginalBitmap);
            FilteredImageSource = BitmapConverter.ImageSourceForBitmap(FilteredBitmap);

            MessageBox.Show("Wert: "+filterSelectedValue);
        }

        private void SaveBitmap(Object obj)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Image Files(*.BMP; *.JPG; *.GIF; *.PNG)| *.BMP; *.JPG; *.GIF; *.PNG";

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    OriginalBitmap.Save(@"H:\Desktop\new.bmp");
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }


        private bool CanDisplayBitmap(object param)
        {
            return true;
        }

        private bool CanSaveBitmap(object param)
        {
            return true;
        }
    }
}
