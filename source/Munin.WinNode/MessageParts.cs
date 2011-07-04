using System;

namespace Munin.WinNode
{
    /// <summary>
    /// Class for parsing requests from the client.
    /// </summary>
    /// <example>
    /// Input: <c>fetch network</c>
    /// </example>
    public class MessageParts
    {
        private MessageParts()
        {
            this.Arguments = new string[0];
        }

        public string Command { get; set; }
        public string[] Arguments { get; set; }

        /// <summary>
        /// Parses the <paramref name="input"/> and returns a hydrated <see cref="MessageParts"/>
        /// object.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
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
