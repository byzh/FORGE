# FORGE — Financial Orchestration & Runtime Generation Engine

## Project Context
R&D system for dynamic generation and execution of financial trading strategies
via Roslyn runtime compilation and multi-role LLM agent orchestration.
Registered R&D project (CIR/JEI), fiscal year 2025-2026.

## Tech Stack
- Language      : C# 13 / .NET 10
- Dynamic build : Microsoft.CodeAnalysis (Roslyn)
- LLM           : Anthropic Claude API (claude-sonnet-4-5)
- Market data   : Tastytrade API (REST + WebSocket)
- Testing       : xUnit + FluentAssertions
- Logging       : Serilog (structured JSON)

## Module Architecture
- FORGE.Core        : OOP abstractions (IStrategy, IAgent, ISignal, IValidator)
- FORGE.Compilation : Roslyn pipeline (parse → AST validation → compile → sandbox)
- FORGE.Agents      : specialized LLM agents + arbitration layer
- FORGE.Validation  : anti-hallucination layer, Greeks coherence checks
- FORGE.Tests       : unit and integration tests

## System Data Flow
Natural Language Input
("Sell put on TSLA when IV Rank > 60")
        ↓
LLM Code Generation
(Claude API → generates C# strategy code as string)
        ↓
Roslyn Compilation Pipeline          ← Verrou 1
├── AST validation (detect dangerous code)
├── CSharpCompilation → Emit
├── AssemblyLoadContext (sandbox isolation)
└── Error feedback loop → LLM self-correction
        ↓
Compiled Strategy (IStrategy instance)
        ↓
Input Layer
├── Tastytrade API   (positions, Greeks, IV)
├── Market data      (spot price, VIX)
└── Text data        (earnings, news, Fed)
        ↓
Agent Layer                          ← Verrou 2
├── MacroAgent
├── TechnicalAgent
├── RiskAgent
└── ArbitrationLayer
        ↓
Validation Layer                     ← Verrou 3
├── Structured output parsing
├── Greeks coherence check
└── Confidence scoring
        ↓
Decision Output (auditable log)

## Module Mapping
| Layer                              | C# Module          |
|------------------------------------|--------------------|
| Domain types & abstractions        | FORGE.Core         |
| LLM code generation                | FORGE.Compilation  |
| Roslyn pipeline + sandbox          | FORGE.Compilation  |
| Data input (Tastytrade, market, text) | FORGE.Core      |
| Agent layer                        | FORGE.Agents       |
| Validation layer                   | FORGE.Validation   |
| Decision output / audit log        | FORGE.Core         |

## Coding Conventions
- Naming        : PascalCase for classes/methods, camelCase for local variables
- Interfaces    : always prefixed with I (IStrategy, IAgent)
- Async         : async/await everywhere IO is involved
- No magic strings : use constants or enums
- Every public class must have a corresponding unit test
- XML doc comments on all public interfaces and methods

## R&D Technical Obstacles (research context)
1. Secure injection of LLM-generated code via Roslyn + AssemblyLoadContext
2. Deterministic arbitration of contradictory signals across LLM agents
3. Coherence validation of LLM outputs against structured financial data

## Financial Domain — Key Vocabulary
- Greeks   : Delta, Theta, Vega, Gamma (option sensitivities)
- IV Rank  : relative implied volatility over 52 weeks (0-100%)
- Short Put: sold put, income strategy
- PMCC     : Poor Man's Covered Call (long LEAPS + short call)
- Main underlyings: QQQ, TSLA, GOOGL, MU

## What You Must NOT Do
- Never write inside /bin or /obj
- Never hardcode API keys in source code
- Never use dynamic in C# except inside FORGE.Compilation
- No static classes except for pure helper utilities

## SOLID Principles — Non-Negotiable
- **S** Single Responsibility : one class = one reason to change
- **O** Open/Closed           : extend via interfaces, never modify stable classes
- **L** Liskov Substitution   : subtypes must be fully substitutable for base types
- **I** Interface Segregation : prefer small focused interfaces over large ones
- **D** Dependency Inversion  : depend on abstractions (IAgent, IValidator), never on concrete classes

## Unit Testing Rules — Non-Negotiable
- Every public method must have at least one unit test
- Tests must cover: happy path, edge cases, and failure cases
- Test naming convention: MethodName_Scenario_ExpectedResult
- Never create a class without creating its corresponding test class in FORGE.Tests
- Mocks via NSubstitute, assertions via FluentAssertions