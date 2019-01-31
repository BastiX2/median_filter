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
                    displayNewBitmap = new RelayCommand(DisplayNewBitmap, param => CanDisplayBitmap(param));
                }
                return displayNewBitmap;
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
            Bitmap bitmap = new Bitmap(openFileDialog.FileName);

            OriginalImageSource = BitmapConverter.ImageSourceForBitmap(bitmap);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OriginalImageSource"));
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
