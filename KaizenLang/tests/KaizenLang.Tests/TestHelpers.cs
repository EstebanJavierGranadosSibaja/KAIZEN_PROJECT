using System;
using KaizenLang.UI;

namespace KaizenLang.Tests
{
    public static class TestHelpers
    {
        // Set a constant response for any prompt
        public static void UseConstantInput(ExecutionService executor, string response)
        {
            executor.InputProvider = (prompt) => response;
        }

        // Set a custom provider
        public static void UseProvider(ExecutionService executor, Func<string?, string?>? provider)
        {
            executor.InputProvider = provider;
        }
    }
}
