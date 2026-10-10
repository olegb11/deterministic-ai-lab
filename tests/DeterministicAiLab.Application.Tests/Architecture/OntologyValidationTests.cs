using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using FluentAssertions;
using Xunit;

namespace DeterministicAiLab.Application.Tests.Architecture;

public class OntologyValidationTests
{
    [Fact]
    public void All_Bdd_Features_Should_Reference_Valid_Ontology_Terms()
    {
        var rootDir = GetSolutionRoot(AppContext.BaseDirectory);
        var ontologyPath = Path.Combine(rootDir, "docs", "specs", "ontology.json");
        var specsDir = Path.Combine(rootDir, "docs", "specs");

        File.Exists(ontologyPath).Should().BeTrue($"Ontology file must exist at {ontologyPath}");

        // 1. Load terms and their synonyms from ontology.json
        var ontologyJson = File.ReadAllText(ontologyPath);
        using var doc = JsonDocument.Parse(ontologyJson);
        var termsArray = doc.RootElement.GetProperty("terms");

        var allowedTermsAndPhrases = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var term in termsArray.EnumerateArray())
        {
            if (term.TryGetProperty("name", out var nameProp))
            {
                var name = nameProp.GetString();
                if (!string.IsNullOrWhiteSpace(name))
                {
                    allowedTermsAndPhrases.Add(name);
                }
            }

            if (term.TryGetProperty("synonyms", out var synonymsProp) && synonymsProp.ValueKind == JsonValueKind.Array)
            {
                foreach (var syn in synonymsProp.EnumerateArray())
                {
                    var synString = syn.GetString();
                    if (!string.IsNullOrWhiteSpace(synString))
                    {
                        allowedTermsAndPhrases.Add(synString);
                    }
                }
            }
        }

        if (!Directory.Exists(specsDir))
        {
            return;
        }

        var featureFiles = Directory.GetFiles(specsDir, "*.feature", SearchOption.AllDirectories);
        if (featureFiles.Length == 0)
        {
            return;
        }

        var allFeaturesContent = string.Join(Environment.NewLine, featureFiles.Select(File.ReadAllText));

        // 2. Validate that core ontology concepts/synonyms are represented in specs
        var missingTerms = new List<string>();
        foreach (var concept in allowedTermsAndPhrases)
        {
            // Check if either the exact concept or its synonym appears in the feature files
            bool exists = allFeaturesContent.Contains(concept, StringComparison.OrdinalIgnoreCase);
            if (!exists)
            {
                missingTerms.Add(concept);
            }
        }

        // To avoid failing on alternative synonyms (e.g., if "Cart" is used instead of "ShoppingCart"), 
        // we can ensure at least the primary name OR one of its synonyms exists per term block, 
        // or just list them. For simplicity, let's ensure primary names are present or referenced.
        // Let's refine: ensure that for each term object, at least one of its identifiers (name or synonyms) is present.

        // Better validation per term entity:
        var missingConcepts = new List<string>();
        foreach (var term in termsArray.EnumerateArray())
        {
            var identifiers = new List<string>();
            if (term.TryGetProperty("name", out var nameProp)) identifiers.Add(nameProp.GetString()!);
            if (term.TryGetProperty("synonyms", out var synsProp))
            {
                foreach (var s in synsProp.EnumerateArray()) identifiers.Add(s.GetString()!);
            }

            bool anyFound = identifiers.Any(id => !string.IsNullOrEmpty(id) && allFeaturesContent.Contains(id, StringComparison.OrdinalIgnoreCase));
            if (!anyFound && identifiers.Count > 0)
            {
                missingConcepts.Add(identifiers[0]);
            }
        }

        missingConcepts.Should().BeEmpty(
            because: "core ontology concepts should be referenced within BDD feature files. Missing concepts: " +
                     string.Join(", ", missingConcepts)
        );
    }

    private static string GetSolutionRoot(string currentDir)
    {
        var dir = new DirectoryInfo(currentDir);
        while (dir != null && !dir.GetFiles("*.sln").Any())
        {
            dir = dir.Parent;
        }
        return dir?.FullName ?? throw new InvalidOperationException("Solution root not found.");
    }
}