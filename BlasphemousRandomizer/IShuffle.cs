namespace BlasphemousRandomizer
{
    public interface IShuffle
    {
        // Create filler and any extra data this shuffler needs
        void Init();

        // Reset randomized dictionary
        void Reset();

        // Load dictionary with randomized data
        void Shuffle(int seed);

        // Calculate spoiler text based on shuffle
        //string GetSpoiler();
    }
}
