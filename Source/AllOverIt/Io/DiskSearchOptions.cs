using System;

namespace AllOverIt.Io
{
    /// <summary>Contains options that can be applied when performing a directory or file search.</summary>
    [Flags]
    public enum DiskSearchOptions
    {
        /// <summary>Indicates no special handling is to be applied.</summary>
        None = 0,

        /// <summary>Indicates if the search will include sub-folders.</summary>
        IncludeSubDirectories = 1,

        /// <summary>Indicates if unauthorized exceptions are to be suppressed.</summary>
        IgnoreUnauthorizedException = 2,

        /// <summary>Indicates if general IO Exceptions (such as file in use) are to be suppressed.</summary>
        IgnoreIoException = 4
    }
}
