using NUnit.Framework;
using System;
using System.IO;
using System.Text;

namespace Frends.Community.Azure.Tests
{
    public class UtilsTest
    {
        private string _existingFileName;
        private FileInfo _file;
        private string _testDirectory;
        private string _testPath;

        [SetUp]
        public void Setup()
        {
            _existingFileName = "existing_file.txt";
            // create test folder
            _testDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testDirectory);
            _testPath = Path.Combine(_testDirectory, _existingFileName);
            File.WriteAllText(_testPath, "I'm walking here! I'm walking here!");
            _file = new FileInfo(_testPath);

            if (!_file.Exists) throw new Exception("File Be Not Present.");
        }

        [TearDown]
        public void Cleanup()
        {
            // remove all test files and directory
            if (Directory.Exists(_testDirectory))
                Directory.Delete(_testDirectory, true);
        }

        [Test]
        public void GetRenamedFileName_DoesNotRename_If_FileName_Is_Available()
        {
            var fileName = "new_file_name.txt";
            var result = Utils.GetRenamedFileName(fileName, _testDirectory);

            Assert.AreEqual(fileName, result);
        }

        [Test]
        public void GetRenamedFileName_AddsNumberInParenthesis()
        {
            var result = Utils.GetRenamedFileName(_existingFileName, _testDirectory);

            Assert.AreNotEqual(_existingFileName, result);
            Assert.IsTrue(result.Contains("(1)"));
        }

        [Test]
        public void GetRenamedFileName_IncrementsNumberUntillAvailableFileNameIsFound()
        {
            //create files ..(1) to ...(10)
            for (var i = 0; i < 10; i++)
            {
                var fileName = Utils.GetRenamedFileName(_existingFileName, _testDirectory);
                File.WriteAllText(Path.Combine(_testDirectory, fileName), "You can't handle the truth!");
            }

            // we should have 11 files in test directory, last being 'existing_file(10).txt'
            Assert.AreEqual(11, Directory.GetFiles(_testDirectory, "*.txt").Length);
            Assert.IsTrue(File.Exists(Path.Combine(_testDirectory, "existing_file(10).txt")));
        }

        [Test]
        public void GetStream_ReturnsReadableStream()
        {
            // UploadAsync needs readable stream.
            using (var stream = Utils.GetStream(false, false, Encoding.UTF8, _file))
            {
                Assert.IsTrue(stream.CanRead);
            }

            using (var stream = Utils.GetStream(false, true, Encoding.UTF8, _file))
            {
                Assert.IsTrue(stream.CanRead);
            }

            using (var stream = Utils.GetStream(true, false, Encoding.UTF8, _file))
            {
                Assert.IsTrue(stream.CanRead);
            }

            using (var stream = Utils.GetStream(true, true, Encoding.UTF8, _file))
            {
                Assert.IsTrue(stream.CanRead);
            }

            using (var file = _file.Open(FileMode.Open, FileAccess.Read))
            {
                Assert.IsTrue(file.CanRead); // == streams dispose and file is closed properly.
            }
        }

        [Test]
        public void GetStream_ReturnsFileUncompressed()
        {
            using (var stream = Utils.GetStream(false, true, Encoding.UTF8, _file))
            {
                Assert.AreEqual(
                    stream.Length,
                    Encoding.UTF8.GetBytes(File.ReadAllText(_file.FullName)).Length
                );
            }
        }

        [Test]
        public void GetStream_ReturnsFileCompressed()
        {
            using (var stream = Utils.GetStream(true, false, Encoding.UTF8, _file))
            {
                Assert.AreNotEqual(
                    stream.Length,
                    Encoding.UTF8.GetBytes(File.ReadAllText(_file.FullName)).Length
                );
            }
        }

        [Test]
        public void GetStream_ReturnsCompressedMemoryStream()
        {
            using (var stream = Utils.GetStream(true, false, Encoding.UTF8, _file))
            {
                Assert.IsTrue(
                    stream.Length != Encoding.UTF8.GetBytes(File.ReadAllText(_file.FullName)).Length &&
                    stream is MemoryStream
                );
            }
        }
    }
}