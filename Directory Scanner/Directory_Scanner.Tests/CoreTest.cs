using Directory_Scanner.Model;
using Directory_Scanner.Model.Threads;
using Directory_Scanner.Model.Tree;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System.Collections.Concurrent;

namespace Directory_Scanner.Tests
{
    [TestClass]
    public class CoreTest
    {
        [TestMethod]
        public void TaskQueue_Check_Multiple_Thread_Work()
        {
            // Arrange
            int threads = 10;
            var taskQueue = new TaskQueue(threads);
            List<string> names = new List<string>() { "A", "B", "C", "D", "E", "F" };
            var result = new ConcurrentBag<string>();

            // Act
            foreach (string name in names)
            {
                var testObject = new TestClassA(name);
                taskQueue.EnqueueTask(() => result.Add(testObject.WaitAndPrint()));
            }

            while (taskQueue.WaitingCount != threads)
            {

            }

            // Assert
            Assert.IsTrue(taskQueue.WaitingCount == threads);
            taskQueue.Close();
            Assert.IsTrue(result.All(names.Contains));
        }

        [TestMethod]
        public void DirectoryScanner_Check_Max_and_Current_File_Fraction()
        {
            // Arrange
            int threads = 16;
            string path = "C:/Dima";

            // Act
            DirectoryScanner directoryScanner = new DirectoryScanner(threads);
            FileSystemTree result = directoryScanner.StartScanning(path);

            // Assert
            double expected = (double)directoryScanner.currentFiles/ directoryScanner.maxFiles;
            Assert.IsTrue(expected > 0.9);
        }

        [TestMethod]
        public void DirectoryScanner_Check_Negative_ThreadCount()
        {
            // Arrange
            int threads = -20;
            DirectoryScanner directoryScanner;
            
            // Act
            directoryScanner = new DirectoryScanner(threads);

            // Assert
            Assert.AreEqual(directoryScanner._defaultThreads, directoryScanner.MaxThreads);
        }

        [TestMethod]
        public void DirectoryScanner_False_Directory()
        {
            // Arrange
            int threads = 16;
            string path = "C:/X-files";
            Exception expected = null;

            // Act
            try
            {
                DirectoryScanner directoryScanner = new DirectoryScanner(threads);
                FileSystemTree result = directoryScanner.StartScanning(path);
                
            }
            catch(Exception ex) 
            {
                expected = ex;
            }

            // Assert
            Assert.AreNotEqual(null, expected);
        }

        [TestMethod]
        public void DirectoryScanner_Check_Root_Size_From_Children()
        {
            // Arrange
            int threads = 16;
            string path = "C:/Dima";

            // Act
            DirectoryScanner directoryScanner = new DirectoryScanner(threads);
            FileSystemTree result = directoryScanner.StartScanning(path);
            long expected = result.Root.Size;
            long actual = result.Root.Children.Select(c => c.Size).Sum();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void DirectoryScanner_AreNotEqual_Suspended_and_Real_Root_Size()
        {
            // Arrange
            int threads = 16;
            string path = "C:/Program Files";
            FileSystemTree result;

            // Act
            DirectoryScanner directoryScanner = new DirectoryScanner(threads);
            result = directoryScanner.StartScanning(path);
            long expected = result.Root.Size;
            Task.Run(() =>
            {
                Thread.Sleep(3000);
                directoryScanner.SuspendWorkers();
            });
            result = directoryScanner.StartScanning(path);
            long actual = result.Root.Size;

            // Assert
            Assert.AreNotEqual(expected, actual);
        }
    }
}