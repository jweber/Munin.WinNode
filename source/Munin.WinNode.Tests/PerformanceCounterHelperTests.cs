using NUnit.Framework;

namespace Munin.WinNode.Tests
{
    [TestFixture]
    class PerformanceCounterHelperTests
    {
        [Test]
        public void CleanName_Keeps_Adapter_Number()
        {
            string input = "Broadcom BCM5709C NetXtreme II GigE (NDIS VBD Client) #4";
            string expected = "Broadcom BCM5709C NetXtreme II GigE [NDIS VBD Client] _4";

            string result = PerformanceCounterHelper.CleanName(input);

            Assert.AreEqual(expected, result);
        }
    }
}
