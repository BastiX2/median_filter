using Microsoft.Win32;
using System.Windows;
using System.Drawing;
using System;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Input;
using MedianFilter;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace MedianFilterProject
{   
    /// <summary>
    /// ViewModel 
    /// </summary>
    class MedianFilterViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Wert der Combobox
        /// </summary>
        private int filterSelectedValue = 1;

        /// <summary>
        /// Property des Combobox Wertes
        /// </summary>
        public int FilterSelectedValue
        {
            get { return filterSelectedValue; }
            set { filterSelectedValue = value; }
        }

        private Boolean applyEnabled = false;
        public Boolean ApplyEnabled
        {
            get { return applyEnabled; }
            set
            {
                applyEnabled = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("applyEnabled"));
            }
        }

        private Boolean saveEnabled = false;
        public Boolean SaveEnabled
        {
            get { return saveEnabled; }
            set
            {
                saveEnabled = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("saveEnabled"));
            }
        }

        private Boolean openFileEnabled = true;
        public Boolean OpenFileEnabled
        {
            get { return openFileEnabled; }
            set
            {
                openFileEnabled = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("openFileEnabled"));
            }
        }

        private Boolean openFolderEnabled = true;
        public Boolean OpenFolderEnabled
        {
            get { return openFolderEnabled; }
            set
            {
                openFolderEnabled = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("openFolderEnabled"));
            }
        }

        /// <summary>
        /// ImageSource der Orginalen Datei
        /// </summary>
        private ImageSource originalImageSource;

        /// <summary>
        /// OriginalImageSource Property
        /// </summary>
        public ImageSource OriginalImageSource
        {
            get { return originalImageSource; }
            set {
                originalImageSource = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OriginalImageSource"));
            }
        }
        /// <summary>
        /// ImageSource der gefilterten Bitmap
        /// </summary>
        private ImageSource filteredImageSource;

        /// <summary>
        /// FilteredImageSource Property
        /// </summary>
        public ImageSource FilteredImageSource
        {
            get { return filteredImageSource; }
            set {
                filteredImageSource = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FilteredImageSource"));
            }
        }

        /// <summary>
        /// originalBitmap Wert
        /// </summary>
        private Bitmap originalBitmap;

        /// <summary>
        /// OriginalBitmap Property
        /// </summary>
        public Bitmap OriginalBitmap
        {
            get { return originalBitmap; }
            set {
                originalBitmap = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OriginalBitmap"));              
            }
        }

        /// <summary>
        /// filteredBitmap Wert
        /// </summary>
        private Bitmap filteredBitmap;

        /// <summary>
        /// FilteredBitmap Property
        /// </summary>
        public Bitmap FilteredBitmap
        {
            get { return filteredBitmap; }
            set {   
                filteredBitmap = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FilteredBitmap"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Command beim Öffnen einer neuen Datei
        /// </summary>
        private ICommand openNewBitmapCommand = null;

        public ICommand OpenNewBitmapCommand
        {
            get
            {
                if (openNewBitmapCommand == null)
                {
                    openNewBitmapCommand = new RelayCommand(OpenNewBitmap, param => CanDisplayBitmap(param));
                }
                return openNewBitmapCommand;
            }

        }

        /// <summary>
        /// Command beim Anwenden des Filters
        /// </summary>
        private ICommand filterBitmapCommand;

        public ICommand FilterBitmapCommand
        {
            get
            {
                if (filterBitmapCommand == null)
                {
                    filterBitmapCommand = new RelayCommand(FilterBitmap, param => CanDisplayBitmap(param));
                }
                return filterBitmapCommand;
            }
        }

       
        private ICommand openNewFolderCommand = null;

        public ICommand OpenNewFolderCommand
        {
            get
            {
                if (openNewFolderCommand == null)
                {
                    openNewFolderCommand = new RelayCommand(OpenNewFolder, param => CanSaveBitmap(param));
                }
                return openNewFolderCommand;
            }

        }
        /// <summary>
        /// Command beim Speichern der Datei
        /// </summary>
        private ICommand saveNewBitmapCommand = null;

        public ICommand SaveNewBitmapCommand
        {
            get
            {
                if (saveNewBitmapCommand == null)
                {
                    saveNewBitmapCommand = new RelayCommand(SaveBitmap, param => CanSaveBitmap(param));
                }
                return saveNewBitmapCommand;
            }

        }

        private List<Bitmap> OriginalBitmaps = new List<Bitmap>();
        private List<Bitmap> FilteredBitmaps = new List<Bitmap>();

        /// <summary>
        /// Erstellt eine Bitmap basierend auf dem Pfad des OpenFileDialog und
        /// erstellt eine ImageSource und zeigt sie an
        /// </summary>
        /// <param name="obj"></param>
        private void OpenNewBitmap(Object obj)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files(*.jpg; *.bmp; *.gif; *.png)| *.jpg; *.bmp; *.gif; *.png";
            openFileDialog.ShowDialog();
            try
            {
                OriginalBitmap = new Bitmap(openFileDialog.FileName);
                
                
            }
            catch (ArgumentException)
            {
                // Beende die Methode, das Programm läuft weiter
                return;
            }

            OriginalImageSource = BitmapConverter.ImageSourceForBitmap(OriginalBitmap);
            // Button Aktivieren
            ApplyEnabled = true;
            SaveEnabled = true;

            // Bitmap ggf Zurücksetzen
            if (FilteredBitmap != null)
            {
                FilteredBitmap = null;
                FilteredImageSource = null;
            }
        }

        private void OpenNewFolder(Object obj)
        {   
            // Bild zurücksetzen wenn vorhanden
            if(OriginalBitmap != null)
            {
                OriginalBitmap = null;
                OriginalImageSource = null;
            }

            if (FilteredBitmap != null)
            {
                FilteredBitmap = null;
                FilteredImageSource = null;
            }
           
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.ShowDialog();

            string[] files = Directory.GetFiles(dialog.SelectedPath);
            string all = "";

         
            var ext = new List<string> { ".jpg", ".gif", ".png", ".bmp" };
            var myFiles = Directory.GetFiles(dialog.SelectedPath, "*.*", SearchOption.AllDirectories)
                 .Where(s => ext.Contains(Path.GetExtension(s)));

            foreach (var item in myFiles)
            {
                OriginalBitmaps.Add(new Bitmap(item));
                all += item;
            }

            MessageBox.Show(all);

        }

        /// <summary>
        /// Wenden den Filter auf die OrginalBitmap an und speichert die neue Bitmap als FilteredBitmap
        /// Erstellt eine ImageSource der gefilterten Bitmap und stellt sie dar
        /// </summary>
        /// <param name="obj"></param>
        private void FilterBitmap(Object obj)
        {
            // Check ob eine Datei vorhanden ist
            if(originalBitmap != null)
            {
                FilteredBitmap = MedianFilter.FilterBitmap(OriginalBitmap, filterSelectedValue);
                FilteredImageSource = BitmapConverter.ImageSourceForBitmap(FilteredBitmap);
                MessageBox.Show("Dein Bild ist fertig!");
            } else
            {
                // Sonst check ob Bilder im Ordner vorhanden sind
                if (OriginalBitmaps.Count == 0)
                {
                    MessageBox.Show("Keine Bilder im Ordner gefunden!");
                }
                else
                {
                    MessageBox.Show("Deine Bilder werden jetzt bearbeitet");
                    // Alle Bilder durchlaufen und Filtern
                    foreach (var item in OriginalBitmaps)
                    {
                        FilteredBitmaps.Add(MedianFilter.FilterBitmap(item, filterSelectedValue));
                    }
                       
                    //MessageBox.Show("Wert: " + filterSelectedValue);
                    MessageBox.Show("Deine Bilder sind fertig! Bitte speichern oder neuen Wert wählen.");
                }
            }
               

        }

        /// <summary>
        /// Speichert die gefilterte Bitmap in dem Pfad des SaveDialog 
        /// </summary>
        /// <param name="obj"></param>
        private void SaveBitmap(Object obj)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Image Files(*.jpg; *.bmp; *.gif; *.png)| *.jpg; *.bmp; *.gif; *.png";

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    FilteredBitmap.Save(saveFileDialog.FileName);
                }
                catch (Exception)
                {
                    return;
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
