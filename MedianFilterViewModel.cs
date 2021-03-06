﻿using Microsoft.Win32;
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
using System.Collections.ObjectModel;

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
        /// Property für den Content des Speichern buttons
        /// </summary>
        private string saveContent = "Save File";

        /// <summary>
        /// saveContent Feld
        /// </summary>
        public string SaveContent
        {
            get { return saveContent; }
            set
            {
                saveContent = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("saveContent"));
            }
        }

        /// <summary>
        /// Property des boolean um button zu aktivieren/deaktivieren
        /// </summary>
        private bool applyEnabled = false;

        /// <summary>
        /// applyEnabled Feld
        /// </summary>
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
        /// Property des boolean um button zu aktivieren/deaktivieren
        /// </summary>
        private bool saveEnabled = false;

        /// <summary>
        /// saveEnabled Feld
        /// </summary>
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
        /// Property des boolean um button zu aktivieren/deaktivieren
        /// </summary>
        private bool openFileEnabled = true;

        /// <summary>
        /// openFileEnabled Feld
        /// </summary>
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
        /// Property des boolean um button zu aktivieren/deaktivieren
        /// </summary>
        private bool openFolderEnabled = true;

        /// <summary>
        /// openFolderEnabled Feld
        /// </summary>
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
        /// ImageSource der Orginal Bitmap
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
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("originalImageSource"));
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
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("filteredImageSource"));
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
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("originalBitmap"));
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
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("filteredBitmap"));
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
                    createBitmapCommand = new RelayCommand(CreateBitmap, null);
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
                    filterBitmapCommand = new RelayCommand(FilterBitmap, null);
                }
                return filterBitmapCommand;
            }
        }

        /// <summary>
        /// Command zum Öffnen eines Ordners
        /// </summary>
        private ICommand openNewFolderCommand = null;

        public ICommand OpenNewFolderCommand
        {
            get
            {
                if (openNewFolderCommand == null)
                {
                    openNewFolderCommand = new RelayCommand(OpenNewFolder, null);
                }
                return openNewFolderCommand;
            }

        }
        /// <summary>
        /// Command beim Speichern der Datei(en)
        /// </summary>
        private ICommand saveNewBitmapCommand = null;

        public ICommand SaveNewBitmapCommand
        {
            get
            {
                if (saveNewBitmapCommand == null)
                {
                    saveNewBitmapCommand = new RelayCommand(SaveBitmap, null);
                }
                return saveNewBitmapCommand;
            }

        }

        /// <summary>
        /// Liste aller eingelesenen Dateinamen
        /// </summary>
        private List<string> fileNames;

        public List<string> FileNames
        {
            get { return fileNames; }
            set
            {
                fileNames = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FileNames"));
            }
        }
        

        /// <summary>
        /// Liste aller Bitmaps im Ordner
        /// </summary>
        private List<Bitmap> originalBitmapList = new List<Bitmap>();

        /// <summary>
        /// Liste der Orginal Filenamen mit extension
        /// </summary>
        //private ObservableCollection<string> originalFileNameList = new ObservableCollection<string>();

        public ObservableCollection<string> OriginalFileNameList { get; set; }

        /// <summary>
        /// Liste aller gefilterten Bitmaps
        /// </summary>
        private List<Bitmap> filteredBitmapList = new List<Bitmap>();

        /// <summary>
        /// Öffnet ein Datei
        /// Wird beendet wenn keine Datei ausgewählt wurde, sonst
        /// erstellt eine Bitmap basierend auf dem Pfad des OpenFileDialog und
        /// erstellt eine ImageSource basierend auf der Bitmap
        /// </summary>
        /// <param name="obj"></param>
        private void CreateBitmap(object obj)
        {
            // Listen leeren
            originalBitmapList.Clear();
            filteredBitmapList.Clear();
            OriginalFileNameList = new ObservableCollection<string>();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OriginalFileNameList"));

            // gefiltertebitmap zurücksetzen wenn eine alte vorhanden ist
            if (FilteredBitmap != null)
            {
                FilteredBitmap = null;
                FilteredImageSource = null;
            }

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
                MessageBox.Show("Bilddatei konnte nicht erstellt werden");

                return;
            }
            // ImageSource basierend auf Bitmap erstellen
            OriginalImageSource = BitmapConverter.CreateImageSourceFromBitmap(OriginalBitmap);

            // Button Aktivieren und benennen
            ApplyEnabled = true;
            SaveContent = "Save File";

        }

        /// <summary>
        /// Öffnen eines Ordners
        /// wird beendet wenn kein Ordner ausgewählt wurde
        /// Fügt alle Bilddatei im ausgewählten Ordner der liste
        /// originalBitmapList hinzu
        /// </summary>
        /// <param name="obj"></param>
        private void OpenNewFolder(object obj)
        {
            // Button deaktivieren
            SaveEnabled = false;
            ApplyEnabled = false;

            // Listen leeren
            originalBitmapList.Clear();
            filteredBitmapList.Clear();
            OriginalFileNameList = new ObservableCollection<string>();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OriginalFileNameList"));

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

            // Bitmaps erstellen und hinzufügen
            foreach (var image in imageList)
            {
                originalBitmapList.Add(new Bitmap(image));
                OriginalFileNameList.Add(Path.GetFileName(image).ToString());
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OriginalFileNameList"));
            // MessageBox.Show(String.Format("Es wurden {0} Bilder hinzugefügt", originalBitmapList.Count()));
            // Button aktivieren und umbenennen
            SaveContent = "Save Files";
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
                FilteredBitmap = BitmapFilter.MedianFilterBitmap(OriginalBitmap, FilterSelectedValue);
                FilteredImageSource = BitmapConverter.CreateImageSourceFromBitmap(FilteredBitmap);
                MessageBox.Show("Dein Bild ist fertig!");
            }
            else
            {
                MessageBox.Show("Deine Bilder werden jetzt bearbeitet");
                // Alle Bilder durchlaufen und Filtern
                foreach (var bitmap in originalBitmapList)
                {
                    filteredBitmapList.Add(BitmapFilter.MedianFilterBitmap(bitmap, FilterSelectedValue));
                }

                MessageBox.Show("Deine Bilder sind fertig! Bitte speichern oder neuen Wert wählen.");

            }

            SaveEnabled = true;
            applyEnabled = true;
        }

        /// <summary>
        /// Speichert die gefilterte Bitmap in dem Pfad des SaveDialog
        /// Kann nur aufgerufen werden wenn ein Bild bzw Ordner vorhanden sind
        /// </summary>
        /// <param name="obj"></param>
        private void SaveBitmap(object obj)
        {
            // Check ob eine Datei oder ein Ordner geöffnet wurde
            if (FilteredBitmap != null)
            {
                // Datei Speichern
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Image Files(*.jpg; *.bmp; *.gif; *.png)| *.jpg; *.bmp; *.gif; *.png";
                // Speichern wenn ein Name eingegeben wurde
                if (saveFileDialog.ShowDialog() == true)
                {
                    FilteredBitmap.Save(saveFileDialog.FileName);
                }
            }
            else
            {
                // Ordner speichern
                System.Windows.Forms.FolderBrowserDialog openFolderDialog = new System.Windows.Forms.FolderBrowserDialog();
                openFolderDialog.ShowDialog();

                // Kein Ordner ausgewählt
                if (string.IsNullOrEmpty(openFolderDialog.SelectedPath))
                {
                    // Beende die Methode, das Programm läuft weiter
                    return;
                }


                string folder = openFolderDialog.SelectedPath;
                string suffix = "_filtered_";
                for (int i = 0; i < filteredBitmapList.Count(); i++)
                {
                    // Aufteiles des Dateinames in Name und Extension
                    var fileNameFull = OriginalFileNameList[i].Split('.');
                    var fileName = fileNameFull[0];
                    var fileExtension = fileNameFull[1];
                    // Dateiname setzt sich aus Orginalname + Suffix + Filterstärke + Orginalextension zusammen
                    filteredBitmapList[i].Save(folder + "\\" + fileName + suffix + FilterSelectedValue + "." + fileExtension);
                }

            }

        }
      
    }

}
