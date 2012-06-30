using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using System.IO;
using EpubEditor.Model;
using System.Collections.Generic;
using GalaSoft.MvvmLight.Command;
using System.Windows.Forms;
using System;

namespace EpubEditor.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm/getstarted
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        #region Fields
        private ObservableCollection<FileInfo> _files;

       
        private FileInfo _selectedFile;
        private bool isDirty;

        private string _selectedZipFileName;
        private ZipReader zipReader;
        private EpubFile _epubFile;

        #endregion  Fields

        #region Properties

        public List<string> ZipFileNames { get; set; }
        public string Folder { get; private set; }
        
        public RelayCommand BrowseCommand { get; private set; }
        public RelayCommand ViewFileCommand { get; private set; }
        public RelayCommand SaveZipCommand { get; private set; }
              
        public string Content
        {
            get
            {
                return _epubFile.Content;
            }
            set
            {
                if (value == _epubFile.Content)
                    return;

                _epubFile.Content = value;
                RaisePropertyChanged("Content");
                isDirty = true;
            }
        }

        public ObservableCollection<FileInfo> Files
        {
            get
            {
                return _files;
            }
            set
            {
                _files = value;
                RaisePropertyChanged("Filepaths");
            }
        }



        public string SelectedZipFileName
        {
            get
            {
                return _selectedZipFileName;
            }
            set
            {
                _selectedZipFileName = value;
                if (zipReader != null)
                {
                    try
                    {
                        if (_selectedZipFileName.EndsWith(new string[] {
                                        ".jpg", ".jpeg", ".png", 
                                        ".gif", ".ttf", ".otf" }))
                        {
                            Content = "File format is not supported";
                        }
                        else
                        {
                            Content = zipReader.ReadZipEntry(_selectedZipFileName);
                            isDirty = false;
                        }
                        RaisePropertyChanged("Content");
                        RaisePropertyChanged("SelectedZipFileName");
                    }
                    catch (Exception)
                    {
                        //ignore an exception 
                    }
                }
            }
        }


        public FileInfo SelectedFile
        {
            get { return _selectedFile; }
            set
            {
                _selectedFile = value;

                if (_selectedFile != null)
                {

                    zipReader = new ZipReader(_selectedFile.FullName);
                    zipReader.ReadMemoryFile();

                    ZipFileNames = zipReader.FileNameList;

                    Content = String.Empty;
                    SelectedZipFileName = String.Empty;
                    RaisePropertyChanged("SelectedFile");

                    RaisePropertyChanged("Content");
                    RaisePropertyChanged("ZipFileNames");
                    RaisePropertyChanged("SelectedZipFileName");

                    isDirty = false;
                }

            }
        }


        #endregion


        #region Constructor
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            _epubFile = new EpubFile();

            BrowseCommand = new RelayCommand(Browse);
            ViewFileCommand = new RelayCommand(ViewFile);
            SaveZipCommand = new RelayCommand(SaveZip, CanSaveZip);

            if (!Storage.StorageExists())
            {
                Storage.CreateFile();
            }         

            string folder = Storage.GetLastFolder();

            if (!string.IsNullOrEmpty(folder))
            {
                if (Directory.Exists(folder))
                {
                    _files = GetFileInfo(folder);
                    Folder = folder;
                    RaisePropertyChanged("Folder");
                    RaisePropertyChanged("Files");
                }
            }
        }
        #endregion

        #region Commands
        private void Browse()
        {
            var dialog = new FolderBrowserDialog();

            DialogResult result = dialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                _files = GetFileInfo(dialog.SelectedPath);
                Storage.AddData(dialog.SelectedPath);
                Folder = dialog.SelectedPath;
                isDirty = false;
                RaisePropertyChanged("Folder");
                RaisePropertyChanged("Files");
                RaisePropertyChanged("SelectedFile");
                RaisePropertyChanged("Content");
            }
        }

        internal void ViewFile()
        {
            if (SelectedFile != null)
            {
                System.Diagnostics.Process.Start(SelectedFile.FullName);
            }
        }

        private void SaveZip()
        {
          
            try
            {


                if (SelectedFile != null)
                {
                    EpubFile epub = new EpubFile();
                    epub.Name = _selectedZipFileName;
                    epub.Content = Content;
                    ZipReader.UpdateZip(SelectedFile.FullName, epub);
                    isDirty = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private bool CanSaveZip()
        {
            if (_epubFile == null)
                return false;
            else
                return !(string.IsNullOrEmpty(Content)) && isDirty;
        }
        #endregion

        

        private ObservableCollection<FileInfo> GetFileInfo(string dir)
        {
            List<FileInfo> list = new List<FileInfo>();

            string[] filenames = GetFilenames(dir);
            foreach (var s in filenames)
            {
                list.Add(new FileInfo(s));
            }

            list.Sort((a, b) => a.Name.CompareTo(b.Name));
            return new ObservableCollection<FileInfo>(list);
        }


        public string[] GetFilenames(string dir)
        {
            string[] filenames = Directory.GetFiles(dir,
                                   "*.epub", SearchOption.AllDirectories);

            return filenames;
        }

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}