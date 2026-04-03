Create a new LLM financial agent named $ARGUMENTS inside FORGE.Agents.
Steps:
1. Implement IFinancialAgent interface
2. Store the system prompt in a private const string
3. Parse LLM response into a typed AgentSignal
4. Unit test with mocked HttpClient
5. Register in the DI container
