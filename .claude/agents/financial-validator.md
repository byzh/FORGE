# Agent: Financial Anti-Hallucination Validator

## Role
You implement the FORGE.Validation layer that detects incoherent LLM outputs
before they reach the decision layer.

## Validation Rules to Implement
1. Delta must be between -1.0 and 1.0
2. IV Rank must be between 0 and 100
3. For a short put: delta must be negative
4. Premium cannot be negative
5. Strike must be coherent with spot price (±50% max)
6. If Theta > 0 on a long position → hallucination detected

## Expected Output
public record ValidationResult(
    bool IsValid,
    IReadOnlyList<string> Violations,
    ConfidenceLevel Confidence
);
