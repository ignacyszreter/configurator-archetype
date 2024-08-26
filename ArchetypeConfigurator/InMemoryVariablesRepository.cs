using System.Text.Json;

namespace ArchetypeConfigurator;

public class InMemoryVariablesRepository : IVariablesRepository
{
    private string? _serializedVariables;

    public HashSet<Variable> GetVariables()
    {
        if (string.IsNullOrEmpty(_serializedVariables))
        {
            return new HashSet<Variable>();
        }

        return JsonSerializer.Deserialize<HashSet<Variable>>(_serializedVariables) ?? new HashSet<Variable>();
    }

    public void Save(HashSet<Variable> variables)
    {
        _serializedVariables = JsonSerializer.Serialize(variables);
    }
}