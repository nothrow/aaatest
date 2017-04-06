using JetBrains.Annotations;

namespace aaatest.framework
{
    /// <summary>
    /// Information about the method creating the test
    /// </summary>
    public sealed class CallerInformation
    {
        public CallerInformation([NotNull] string memberName, [NotNull] string sourceFilePath, int sourceLineNumber)
        {
            Check.NotNull(memberName, nameof(memberName));
            Check.NotNull(sourceFilePath, nameof(sourceFilePath));

            MemberName = memberName;
            SourceFilePath = sourceFilePath;
            SourceLineNumber = sourceLineNumber;
        }

        [NotNull]
        public string MemberName { get; }
        [NotNull]
        public string SourceFilePath { get; }
        public int SourceLineNumber { get; }
    }
}