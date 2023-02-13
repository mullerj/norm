using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mil.Navy.Nrl.Norm.IntegrationTests
{
    public class NormFileTests
    {
        [Fact]
        public void GetsNormFileName()
        {
            var sessionAddress = "224.1.2.3";
            var sessionPort = 6003;
            var localNodeId = NormNode.NORM_NODE_ANY;

            var normInstance = new NormInstance();
            var normSession = normInstance.CreateSession(sessionAddress, sessionPort, localNodeId);
            normSession.StartSender(1024 * 1024, 1400, 64, 16);

            var fileContent = "Hello to the other norm node!!!!!!";
            var fileName = "test_file_name";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
            File.WriteAllText(filePath, fileContent);

            try
            {
                var normFile = normSession.FileEnqueue(filePath);
                Assert.Equal(fileName, normFile.name);
            }
            catch(Exception)
            {
                throw;
            }
            finally
            {
                normSession.StopSender();
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }

        [Fact]
        public void RenamesNormFile()
        {
            var sessionAddress = "224.1.2.3";
            var sessionPort = 6003;
            var localNodeId = NormNode.NORM_NODE_ANY;

            var normInstance = new NormInstance();
            var normSession = normInstance.CreateSession(sessionAddress, sessionPort, localNodeId);
            normSession.StartSender(1024 * 1024, 1400, 64, 16);

            var fileContent = "Hello to the other norm node!!!!!!";
            var fileName = "test_file_name";
            var expectedFileName = "new_test_file_name";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
            var expectedFilePath = Path.Combine(Directory.GetCurrentDirectory(), expectedFileName);
            File.WriteAllText(filePath, fileContent);

            try
            {
                var normFile = normSession.FileEnqueue(filePath);
                normFile.Rename(expectedFilePath);
                Assert.Equal(expectedFileName, normFile.name);
            }
            catch(Exception)
            {
                throw;
            }
            finally
            {
                normSession.StopSender();
                if (File.Exists(expectedFilePath))
                {
                    File.Delete(expectedFilePath);
                }
            }
        }
    }
}
