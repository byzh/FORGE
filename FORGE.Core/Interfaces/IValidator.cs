using FORGE.Core.Enums;

namespace FORGE.Core.Interfaces;

/// <summary>
/// Result produced by any <see cref="IValidator{T}"/> implementation.
/// </summary>
/// <param name="IsValid">Indicates whether the validated input passed all rules.</param>
/// <param name="Violations">
/// Ordered list of human-readable violation messages; empty when <paramref name="IsValid"/> is <see langword="true"/>.
/// </param>
/// <param name="Confidence">
/// Validator's confidence in the result — may be lower when input data is sparse or ambiguous.
/// </param>
public record ValidationResult(
    bool IsValid,
    IReadOnlyList<string> Violations,
    ConfidenceLevel Confidence);

/// <summary>
/// Contract for any component that validates an input of type <typeparamref name="T"/>
/// against a set of domain rules (e.g. Greeks coherence, LLM output sanity checks).
/// </summary>
/// <typeparam name="T">The type of object being validated.</typeparam>
public interface IValidator<T>
{
    /// <summary>
    /// Validates the provided <paramref name="input"/> asynchronously.
    /// </summary>
    /// <param name="input">The object to validate.</param>
    /// <param name="ct">Cancellation token for cooperative cancellation.</param>
    /// <returns>
    /// A <see cref="ValidationResult"/> indicating whether the input is valid
    /// and listing any violations detected.
    /// </returns>
    Task<ValidationResult> ValidateAsync(T input, CancellationToken ct = default);
}
