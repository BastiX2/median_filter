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
        /// Property des Combobox Wertes, default 1
        /// </summary>
        public int FilterSelectedValue { get; set; } = 1;

        /// <summary>
        /// bool um button zu aktivieren/deaktivieren
        /// </summary>
        private bool applyEnabled = false;
        public bool ApplyEnabled
        {
            get { return applyEnabled; }
            set
            {
                applyEnabled = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("applyEnabled"));
            }
        }
        /// <summary>
        /// bool um button zu aktivieren/deaktivieren
        /// </summary>
        private bool saveEnabled = false;
        public bool SaveEnabled
        {
            get { return saveEnabled; }
            set
            {
                saveEnabled = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("saveEnabled"));
            }
        }

        /// <summary>
        /// bool um button zu aktivieren/deaktivieren
        /// </summary>
        private bool openFileEnabled = true;
        public bool OpenFileEnabled
        {
            get { return openFileEnabled; }
            set
            {
                openFileEnabled = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("openFileEnabled"));
            }
        }

        /// <summary>
        /// bool um button zu aktivieren/deaktivieren
        /// </summary>
        private bool openFolderEnabled = true;
        public bool OpenFolderEnabled
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
        /// originalImageSource Feld
        /// </summary>
        public ImageSource OriginalImageSource
        {
            get { return originalImageSource; }
            set
            {
                originalImageSource = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OriginalImageSource"));
            }
        }
        /// <summary>
        /// ImageSource der gefilterten Bitmap
        /// </summary>
        private ImageSource filteredImageSource;

        /// <summary>
        /// filteredImageSource Feld
        /// </summary>
        public ImageSource FilteredImageSource
        {
            get { return filteredImageSource; }
            set
            {
                filteredImageSource = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FilteredImageSource"));
            }
        }

        /// <summary>
        /// originale Bitmap
        /// </summary>
        private Bitmap originalBitmap;

        /// <summary>
        /// originalBitmap Feld
        /// </summary>
        public Bitmap OriginalBitmap
        {
            get { return originalBitmap; }
            set
            {
                originalBitmap = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OriginalBitmap"));
            }
        }

        /// <summary>
        /// gefilterte Bitmap
        /// </summary>
        private Bitmap filteredBitmap;

        /// <summary>
        /// filteredBitmap Feld
        /// </summary>
        public Bitmap FilteredBitmap
        {
            get { return filteredBitmap; }
            set
            {
                filteredBitmap = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FilteredBitmap"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Command zum erstellen einer Bitmap
        /// </summary>
        private ICommand createBitmapCommand = null;

        public ICommand CreateBitmapCommand
        {
            get
            {
                if (createBitmapCommand == null)
                {
                    createBitmapCommand = new RelayCommand(CreateBitmap, param => CanDisplayBitmap(param));
                }
                return createBitmapCommand;
            }

        }

        /// <summary>
        /// Command zum Anwenden des Filters auf eine Bitmap
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
        /// <summary>
        /// Liste aller Bitmaps im Ordner
        /// </summary>
        private List<Bitmap> OriginalBitmapList = new List<Bitmap>();
        /// <summary>
        /// Liste der Orginal Filenamen mit extenstion
        /// </summary>
        private List<string> OrignalFileNameList = new List<string>();
        /// <summary>
        /// Liste aller gefilterten Bitmaps
        /// </summary>
        private List<Bitmap> FilteredBitmapList = new List<Bitmap>();

        /// <summary>
        /// Öffnet ein Datei
        /// Wird beendet wenn keine Datei ausgewählt wurde, sonst
        /// erstellt eine Bitmap basierend auf dem Pfad des OpenFileDialog und
        /// erstellt eine ImageSource basierend auf der Bitmap
        /// </summary>
        /// <param name="obj"></param>
        private void CreateBitmap(object obj)
        {
            // Speichern button deaktivieren
            SaveEnabled = false;
            ApplyEnabled = false;

            // Filedialog öffnen
            OpenFileDialog openFileDialog = new OpenFileDialog();
            // Filter damit nur Bilddateien ausgewählt werden können
            openFileDialog.Filter = "Image Files(*.jpg; *.bmp; *.gif; *.png)| *.jpg; *.bmp; *.gif; *.png";
            openFileDialog.ShowDialog();
            // Versuche eine Bitmap basierend auf dem Pfad zu erstellen
            try
            {
                OriginalBitmap = new Bitmap(openFileDialog.FileName);
            }
            catch (ArgumentException)
            {
                // Beende die Methode, das Programm läuft weiter
                return;
            }
            // ImageSource basierend auf Bitmap erstellen
            OriginalImageSource = BitmapConverter.ImageSourceForBitmap(OriginalBitmap);

            // Button Aktivieren
            ApplyEnabled = true;

            // gefiltertebitmap zurücksetzen wenn eine alte vorhanden ist
            if (FilteredBitmap != null)
            {
                FilteredBitmap = null;
                FilteredImageSource = null;
            }
        }

        /// <summary>
        /// Öffnen eines Ordners
        /// wird beendet wenn kein Ordner ausgewählt wurde
        /// Fügt alle Bilddatei im ausgewählten Ordner der liste
        /// OriginalBitmapList hinzu
        /// </summary>
        /// <param name="obj"></param>
        private void OpenNewFolder(object obj)
        {
            SaveEnabled = false;
            ApplyEnabled = false;

            // Bild zurücksetzen wenn vorhanden
            if (OriginalBitmap != null)
            {
                OriginalBitmap = null;
                OriginalImageSource = null;
            }

            if (FilteredBitmap != null)
            {
                FilteredBitmap = null;
                FilteredImageSource = null;
            }

            // Ordner dialog
            System.Windows.Forms.FolderBrowserDialog openFolderDialog = new System.Windows.Forms.FolderBrowserDialog();
            openFolderDialog.ShowDialog();

            // Kein Ornder ausgewählt
            if (string.IsNullOrEmpty(openFolderDialog.SelectedPath))
            {
                // Beende die Methode, das Programm läuft weiter
                return;
            }

            var allowedExtensionList = new List<string> { ".jpg", ".gif", ".png", ".bmp" };
            var imageList = Directory.GetFiles(openFolderDialog.SelectedPath, "*.*")
                 .Where(s => allowedExtensionList.Contains(Path.GetExtension(s)));
            
            // Ordner enthählt keine Bilder
            if (imageList.Count() == 0)
            {
                MessageBox.Show("Der Ordner enthält keine Bilder");
                return;
            }

            // Liste leeren
            OriginalBitmapList.Clear();

            // Bitmaps erstellen und hinzufügen
            foreach (var image in imageList)
            {
                OriginalBitmapList.Add(new Bitmap(image));
                OrignalFileNameList.Add(Path.GetFileName(image));
            }

            MessageBox.Show(String.Format("Es wurden {0} Bilder hinzugefügt", OriginalBitmapList.Count()));     

            ApplyEnabled = true;

        }

        /// <summary>
        /// Wenden den Filter auf die OrginalBitmap an und speichert die neue Bitmap als FilteredBitmap
        /// Erstellt eine ImageSource der gefilterten Bitmap und stellt sie dar
        /// </summary>
        /// <param name="obj"></param>
        private void FilterBitmap(object obj)
        {
            // Check ob eine Datei vorhanden ist
            if (originalBitmap != null)
            {
                FilteredBitmap = MedianFilter.FilterBitmap(OriginalBitmap, FilterSelectedValue);
                FilteredImageSource = BitmapConverter.ImageSourceForBitmap(FilteredBitmap);
                MessageBox.Show("Dein Bild ist fertig!");
                
            }
            else
            {
                MessageBox.Show("Deine Bilder werden jetzt bearbeitet");
                // Alle Bilder durchlaufen und Filtern
                foreach (var bitmap in OriginalBitmapList)
                {
                    FilteredBitmapList.Add(MedianFilter.FilterBitmap(bitmap, FilterSelectedValue));
                }

                MessageBox.Show("Deine Bilder sind fertig! Bitte speichern oder neuen Wert wählen.");

            }

            SaveEnabled = true;
            applyEnabled = true;
        }

        /// <summary>
        /// Speichert die gefilterte Bitmap in dem Pfad des SaveDialog 
        /// </summary>
        /// <param name="obj"></param>
        private void SaveBitmap(object obj)
        {
            if(FilteredBitmap != null)
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
            } else
            {
                System.Windows.Forms.FolderBrowserDialog openFolderDialog = new System.Windows.Forms.FolderBrowserDialog();
                openFolderDialog.ShowDialog();

                // Kein Ornder ausgewählt
                if (string.IsNullOrEmpty(openFolderDialog.SelectedPath))
                {
                    // Beende die Methode, das Programm läuft weiter
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
