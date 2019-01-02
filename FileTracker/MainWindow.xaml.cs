using LiteDB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FileTracker
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private ObservableCollection<Project> _projectList { get; set; }
        public ObservableCollection<Project> ProjectList
        {
            get { return _projectList; }
            set
            {
                if (_projectList != value)
                {
                    _projectList = value;
                    OnPropertyChanged("ProjectList");
                }
            }
        }


        private Project _activeProject;

        public Project ActiveProject
        {
            get { return _activeProject; }
            set
            {
                if (_activeProject != value)
                {
                    _activeProject = value;
                    OnPropertyChanged("ActiveProject");
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            files.DataContext = this;
            ProjectListDropdown.DataContext = this;

            ProjectList = Project.GetAllProjects();

            if (!ProjectList.Any())
            {
                ShowProjectCanvas();
                return;
            }

            ShowFileCanvas();
        }

        public void ChangeProject(object sender, SelectionChangedEventArgs e)
        {
            ComboBox projectDropdown = e.OriginalSource as ComboBox;

            if (projectDropdown.SelectedIndex >= 0)
                ActiveProject = ProjectList[projectDropdown.SelectedIndex];
        }

        public void AddNewFileToProject(object sender, DragEventArgs e)
        {
            if (DropFileCanvas.Visibility == Visibility.Hidden) return;

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] droppedFile = (string[])e.Data.GetData(DataFormats.FileDrop);

                ActiveProject.AddFile(new File(@droppedFile[0]));
                OpenFileIfRequested(@droppedFile[0]);
                
                ShowFileCanvas();
            }
        }

        private void OpenFileIfRequested(string newFile)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl)) System.Diagnostics.Process.Start(newFile);
        }

        private void ShowDropCanvas(object sender, EventArgs e)
        {
            if (AddFileCanvas.Visibility == Visibility.Hidden) return;

            AddProjectCanvas.Visibility = Visibility.Hidden;
            AddFileCanvas.Visibility = Visibility.Hidden;
            DropFileCanvas.Visibility = Visibility.Visible;
        }

        private void ShowProjectCanvas(object sender, MouseButtonEventArgs e)
        {
            ShowProjectCanvas();
        }

        private void ShowProjectCanvas()
        {
            newProjectName.Text = "";
            newProjectName.Focus();

            AddProjectCanvas.Visibility = Visibility.Visible;
            AddFileCanvas.Visibility = Visibility.Hidden;
            DropFileCanvas.Visibility = Visibility.Hidden;
        }

        private void ShowFileCanvas()
        {
            AddProjectCanvas.Visibility = Visibility.Hidden;
            AddFileCanvas.Visibility = Visibility.Visible;
            DropFileCanvas.Visibility = Visibility.Hidden;
        }

        private void ShowFileCanvas(object sender, EventArgs e)
        {
            if (DropFileCanvas.Visibility == Visibility.Visible) ShowFileCanvas();
        }

        private void ExportFileList(object sender, RoutedEventArgs e)
        {
            ActiveProject.Export();
        }

        private void CreateNewProject(object sender, RoutedEventArgs e)
        {
            Project newProject = new Project(newProjectName.Text);
            ProjectList.Add(newProject);
            newProject.AddToDatabase();

            ActiveProject = newProject;

            ProjectListDropdown.SelectedValue = newProject.ProjectName;

            ShowFileCanvas();
        }

        private void DeleteSelectedFile(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete || e.Key == Key.Back) ActiveProject.RemoveFile(files.SelectedIndex);
        }

        private void ArchiveProject(object sender, RoutedEventArgs e)
        {
            ActiveProject.Archive();
            ProjectList.RemoveAt(ProjectListDropdown.SelectedIndex);
            ProjectListDropdown.SelectedIndex = 0;

            if (!ProjectList.Any()) ShowProjectCanvas();
        }

        protected virtual void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
