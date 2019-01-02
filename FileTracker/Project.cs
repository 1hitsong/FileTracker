using LiteDB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileTracker
{
    public class Project
    {
        [BsonId]
        public Guid Id { get; set; }
        public string ProjectName { get; set; }
        public ObservableCollection<File> Files { get; set; }

        public Project(string name)
        {
            ProjectName = name;
            Files = GetProjectFiles();
        }

        public Project()
        {
        }

        public void AddToDatabase()
        {
            using (var db = new LiteDatabase(@"ProjectData.db"))
            {
                var col = db.GetCollection<Project>("projects");
                col.Insert(this);
                col.EnsureIndex(x => x.ProjectName);
            }
        }

        public static ObservableCollection<Project> GetAllProjects()
        {
            using (var db = new LiteDatabase(@"ProjectData.db"))
            {
                var col = db.GetCollection<Project>("projects");
                return new ObservableCollection<Project>(col.FindAll().OrderBy(x => x.ProjectName));
            }
        }

        public ObservableCollection<File> GetProjectFiles()
        {
            using (var db = new LiteDatabase(@"ProjectData.db"))
            {
                var col = db.GetCollection<Project>("Projects");
                var project = col.FindOne(Query.EQ("ProjectName", ProjectName));        
                
                if (project == null) return new ObservableCollection<File>();

                return project.Files.Count == 0 ? new ObservableCollection<File>() : project.Files;
            }
        }

        public void AddFile(File fileToAdd)
        {
            Files.Add(fileToAdd);
            UpdateDatabase();
        }

        public void RemoveFile(int fileIndex)
        {
            if (fileIndex < 0) return;

            Files.RemoveAt(fileIndex);

            UpdateDatabase();
        }

        public void UpdateDatabase()
        {
            using (var db = new LiteDatabase(@"ProjectData.db"))
            {
                var col = db.GetCollection<Project>("Projects");
                col.Update(this);
            }
        }

        public void Export()
        {
            string fileName = Path.GetFullPath($@"File List\{ProjectName}.txt");
            Directory.CreateDirectory(Path.GetDirectoryName(fileName));

            using (TextWriter tw = new StreamWriter(fileName))
            {
                foreach (File file in Files)
                    tw.WriteLine(file.Name);
            }
        }

        public void Archive()
        {
            using (var db = new LiteDatabase(@"ProjectData.db"))
            {
                var col = db.GetCollection<Project>("Projects");
                col.Delete(this.Id);
            }
        }
    }
}
