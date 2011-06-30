using NUnit.Framework;

namespace Munin.WinNode.Tests
{
    [TestFixture]
    class MessagePartsTests
    {
        [Test]
        public void MessageParts_Created_From_Command_Only_Input()
        {
            string message = "list";
            
            var messageParts = MessageParts.FromString(message);
            
            Assert.AreEqual("list", messageParts.Command);
            Assert.AreEqual(0, messageParts.Arguments.Length);
        }

        [Test]
        public void MessageParts_Created_From_Command_And_Single_Argument_Input()
        {
            string message = "list arg1";
            
            var messageParts = MessageParts.FromString(message);
            
            Assert.AreEqual("list", messageParts.Command);
            Assert.AreEqual(1, messageParts.Arguments.Length);
            Assert.AreEqual("arg1", messageParts.Arguments[0]);
        }

        [Test]
        public void MessageParts_Created_From_Command_And_Multiple_Arguments_Input()
        {
            string message = "list arg1 arg2 arg3";
            
            var messageParts = MessageParts.FromString(message);
            
            Assert.AreEqual("list", messageParts.Command);
            Assert.AreEqual(3, messageParts.Arguments.Length);
            Assert.AreEqual("arg1", messageParts.Arguments[0]);
            Assert.AreEqual("arg2", messageParts.Arguments[1]);
            Assert.AreEqual("arg3", messageParts.Arguments[2]);
        }

        [Test]
        public void MessageParts_Trims_Input()
        {
            string message = " list     ";

            var messageParts = MessageParts.FromString(message);
            Assert.AreEqual("list", messageParts.Command);
        }
    }
}
