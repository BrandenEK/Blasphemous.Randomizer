namespace Blasphemous.Randomizer
{
    public interface IShuffle
    {
        // Load dictionary with randomized data
        bool Shuffle(int seed, Config config);
    }
}
