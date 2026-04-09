# FORGE — Research Log
> R&D journal for CIR audit trail. Each entry documents a hypothesis, result and architectural decision.

---

## 2026-04-04 — Project initialization and architecture design
### Hypothesis
A layered modular architecture (Core / Compilation / Agents / Validation) can effectively isolate the four core research problems: LLM code generation, Roslyn compilation sandboxing, multi-agent orchestration, and structured output validation.
### Result
### Architectural Decision
### Next Step
Implement the Roslyn compilation pipeline in FORGE.Compilation: ICodeGenerator (LLM → C# string), RoslynCompiler (AST validation + emit), and StrategyLoader (AssemblyLoadContext sandbox instantiation).

---

## 2026-04-08 — LLM code generation abstraction layer
### Hypothesis
Defining ICodeGenerator as the single abstraction point for LLM-to-code generation allows multiple provider implementations (Claude, Gemini) to coexist without modifying any upstream pipeline code, satisfying the Open/Closed principle.
### Result
ICodeGenerator interface implemented. ClaudeCodeGenerator and GeminiCodeGenerator both implemented against the Anthropic and Google Gemini REST APIs respectively. Configuration externalized to appsettings.json via IOptions<T> pattern — no magic strings or hardcoded keys in source.
### Architectural Decision
One concrete class per LLM provider, each bound to its own strongly-typed options class (ClaudeOptions, GeminiOptions) via the CodeGenerators section of appsettings.json. Switching providers at runtime requires only a DI registration change, not a code change.
### Next Step
Implement RoslynCompiler: AST validation (dangerous namespace blocking), CSharpCompilation emit to MemoryStream, and AssemblyLoadContext sandbox instantiation of IStrategy.