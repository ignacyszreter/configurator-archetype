namespace ArchetypeConfigurator;

public interface IVariablesRepository
{
    HashSet<Variable> GetVariables();
    void Save(HashSet<Variable> variables);
}