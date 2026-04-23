namespace GabrielBertasso.Helpers
{
    public static class MathHelper
    {
        public static int WrapIndex(int length, int index)
        {
            if (index < 0)
            {
                index = length - 1;
            }
            else if (index >= length)
            {
                index = 0;
            }

            return index;
        }
    }
}