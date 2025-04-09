public interface IJutsu
{
    string Name { get; }
    int JutsuId { get; }

    // TODO: Add Chakra and BaseDamage
    void CastJutsu();

}