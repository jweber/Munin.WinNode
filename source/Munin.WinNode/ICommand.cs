namespace Munin.WinNode
{
    /// <summary>
    /// Interface for defining various commands that are accepted throug the Munin Node protocol
    /// </summary>
    interface ICommand
    {
        /// <summary>
        /// Name of the command
        /// </summary>
        string Command { get; }

        /// <summary>
        /// Indicates if the response from this command will end with a period
        /// on a line by itself.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [end response with period]; otherwise, <c>false</c>.
        /// </value>
        bool EndResponseWithPeriod { get; }

        /// <summary>
        /// Executes the command with the specified arguments. The <paramref name="response"/>
        /// will be written back to the client.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        /// <param name="response">The response.</param>
        void Execute(string[] arguments, out string response);
    }
}
