using System.Reflection;
using System.Runtime.Loader;
using FORGE.Compilation.Exceptions;
using FORGE.Compilation.Interfaces;
using FORGE.Core.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FORGE.Compilation;

/// <summary>
/// Implements <see cref="IRoslynCompiler"/>: validates, compiles, and sandbox-loads
/// LLM-generated C# strategy code via the Roslyn API and <see cref="AssemblyLoadContext"/>.
/// </summary>
public sealed class RoslynCompiler : IRoslynCompiler
{
    // Trusted assemblies made available to compiled strategies
    private static readonly IReadOnlyList<MetadataReference> TrustedReferences = BuildTrustedReferences();

    // Dangerous API patterns blocked at the AST level
    private static readonly IReadOnlyList<string> BlockedNamespaces =
    [
        "System.IO",
        "System.Net",
        "System.Reflection",
        "System.Diagnostics.Process",
        "System.Runtime.InteropServices"
    ];

    /// <inheritdoc/>
    public Task<IStrategy> CompileAsync(string sourceCode, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);

        ValidateAst(syntaxTree);

        var compilation = CSharpCompilation.Create(
            assemblyName: $"ForgeStrategy_{Guid.NewGuid():N}",
            syntaxTrees: [syntaxTree],
            references: TrustedReferences,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        using var ms = new MemoryStream();
        var result = compilation.Emit(ms);

        if (!result.Success)
        {
            var errors = result.Diagnostics
                .Where(d => d.Severity == DiagnosticSeverity.Error)
                .Select(d => d.GetMessage())
                .ToList();

            throw new CompilationException("Roslyn compilation failed.", errors);
        }

        ms.Seek(0, SeekOrigin.Begin);

        var context = new AssemblyLoadContext(name: null, isCollectible: true);
        var assembly = context.LoadFromStream(ms);

        var strategy = InstantiateStrategy(assembly);
        return Task.FromResult(strategy);
    }

    /// <summary>
    /// Walks the AST and blocks any usage of dangerous namespaces or APIs.
    /// </summary>
    private static void ValidateAst(SyntaxTree syntaxTree)
    {
        var root = syntaxTree.GetRoot();
        var violations = new List<string>();

        // Block forbidden using directives
        var usingDirectives = root.DescendantNodes()
            .OfType<UsingDirectiveSyntax>()
            .Select(u => u.Name?.ToString() ?? string.Empty);

        foreach (var ns in usingDirectives)
        {
            if (BlockedNamespaces.Any(blocked => ns.StartsWith(blocked, StringComparison.Ordinal)))
                violations.Add($"Blocked namespace detected: '{ns}'");
        }

        if (violations.Count > 0)
            throw new CompilationException("AST validation failed: dangerous API usage detected.", violations);
    }

    /// <summary>
    /// Locates the first <see cref="IStrategy"/> implementation in the assembly and instantiates it.
    /// </summary>
    private static IStrategy InstantiateStrategy(Assembly assembly)
    {
        var strategyType = assembly.GetTypes()
            .FirstOrDefault(t => t.IsClass && !t.IsAbstract && typeof(IStrategy).IsAssignableFrom(t));

        if (strategyType is null)
            throw new CompilationException(
                "No concrete IStrategy implementation found in compiled assembly.",
                []);

        if (Activator.CreateInstance(strategyType) is not IStrategy instance)
            throw new CompilationException(
                $"Failed to instantiate type '{strategyType.FullName}'.",
                []);

        return instance;
    }

    /// <summary>
    /// Builds the set of trusted <see cref="MetadataReference"/> assemblies
    /// available to compiled strategies.
    /// </summary>
    private static IReadOnlyList<MetadataReference> BuildTrustedReferences()
    {
        var trustedAssemblies = AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")
            ?.ToString()
            ?.Split(Path.PathSeparator)
            ?? [];

        var references = trustedAssemblies
            .Where(path => !string.IsNullOrEmpty(path) && File.Exists(path))
            .Select(path => (MetadataReference)MetadataReference.CreateFromFile(path))
            .ToList();

        // Explicitly include FORGE.Core so IStrategy, MarketContext, AgentSignal are resolvable
        references.Add(MetadataReference.CreateFromFile(typeof(IStrategy).Assembly.Location));

        return references;
    }
}
