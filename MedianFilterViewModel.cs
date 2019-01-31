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

        private string originalFilePath;

        public string OriginalFilePath
        {
            get { return originalFilePath; }
            set { originalFilePath = value; }
        }

        private ImageSource originalImageSource;

        public ImageSource OriginalImageSource
        {
            get { return originalImageSource; }
            set {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OriginalImageSource"));
                originalImageSource = value;
            }
        }

        private ImageSource filteredImageSource;

        public ImageSource FilteredImageSource
        {
            get { return filteredImageSource; }
            set { filteredImageSource = value; }
        }

        private string newFilePath;

        public string NewFilePath
        {
            get { return newFilePath; }
            set { newFilePath = value; }
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
            set { filteredBitmap = value; }
        }

        private ICommand displayNewBitmap = null;

        public ICommand DisplayNewBitmapCommand
        {
            get
            {
                if (displayNewBitmap == null)
                {
                    displayNewBitmap = new RelayCommand(DisplayNewBitmap, param => CanExecuteCalculation(param));
                }
                return displayNewBitmap;
            }

        }

        private void DisplayNewBitmap(Object obj)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files(*.BMP; *.JPG; *.GIF)| *.BMP; *.JPG; *.GIF";
            openFileDialog.ShowDialog();

            Bitmap bitmap = new Bitmap(openFileDialog.FileName);

            OriginalImageSource = BitmapConverter.ImageSourceForBitmap(bitmap);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OriginalImageSource"));
        }

        private bool CanExecuteCalculation(object param)
        {
            return true;
        }
    }
}
