/// This file is part of astyle-extension by lukamicoder
/// https://github.com/lukamicoder/astyle-extension
/// Licensed under Apache License Version 2.0
///
/// Modifications:
///   - renamed namespace to BN_Astyle

using System;

namespace BN_Astyle
{
    public class AStyleErrorArgs : EventArgs {
        public string Message { get; set; }

        public AStyleErrorArgs(string message) {
            Message = message;
        }
    }
}
