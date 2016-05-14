using Moq;
using NUnit.Framework;
using Rabbitual.Agents.DownloaderAgent;
using Rabbitual.Agents.WeatherAgent;
using Rabbitual.Core;
using Rabbitual.Core.Logging;
using Rabbitual.Logging;
namespace Rabbitual.UnitTests.Agents.DownloaderAgent
{
    [TestFixture]
    public class when_passing_message_to_download
    {
        [Test]
        public void should_download()
        {
            DownloadItem arg=null;
            var dl = new Mock<IDownloader>();
            dl
                .Setup(x => x.Download(It.IsAny<DownloadItem>()))
                .Callback((DownloadItem b)=> arg=b);

            var ds = new Mock<IDiskSize>();
            ds.Setup(x => x.GetTotalFreeSpace(It.IsAny<string>())).Returns(1000);

            var mp = new Mock<IMessagePublisher>();


            var a = new Rabbitual.Agents.DownloaderAgent.DownloaderAgent(mp.Object, dl.Object, new Logger(), ds.Object, new DownloaderOptions
            {
                DataFolder = "c:\\temp",
            });
            var m = new Message();
            m.Data["Url"] = "http://www.bjarte.com/poo/foo";
            a.DoWork(m);

            Assert.AreEqual(@"c:\temp\www.bjarte.com\poo", arg.Folder);
            Assert.AreEqual("foo", arg.File);
            Assert.AreEqual("http://www.bjarte.com/poo/foo", arg.Url);

        }
    }
}