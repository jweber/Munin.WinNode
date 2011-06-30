using System;

namespace Munin.WinNode
{
    class MessageParts
    {
        private MessageParts()
        {
            this.Arguments = new string[0];
        }

        public string Command { get; set; }
        public string[] Arguments { get; set; }

        public static MessageParts FromString(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return new MessageParts();

            var inputParts = input.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var output = new MessageParts
                         {
                             Command = inputParts[0]
                         };

            if (inputParts.Length > 1)
            {
                output.Arguments = new string[inputParts.Length - 1];
                for (int i = 1; i < inputParts.Length; i++)
                    output.Arguments[i - 1] = inputParts[i];
            }

            return output;
        }
    }
}
