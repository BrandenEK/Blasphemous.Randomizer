namespace BlasphemousRandomizer
{
    public interface IShuffle
    {
        // Create filler and any extra data this shuffler needs
        void Init();

        // Load dictionary with randomized data
        void Shuffle(int seed);
    }
}
