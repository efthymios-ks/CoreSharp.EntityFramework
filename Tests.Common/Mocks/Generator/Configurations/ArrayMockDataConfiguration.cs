namespace Tests.Common.Mocks.Generator.Configurations;

internal sealed class ArrayMockDataConfiguration : CollectionMockDataConfigurationBase
{
    public override bool CanCreate(Type type)
        => type.IsArray;

    public override object Create(Type type, IMockDataGenerator mockDataGenerator)
    {
        var elementType = type.GetElementType()!;
        var rank = type.GetArrayRank();
        var lengths = new int[rank];
        var indices = new int[rank];

        // Generate random sizes for each dimension
        for (var i = 0; i < rank; i++)
        {
            lengths[i] = mockDataGenerator.Random.Next(CollectionMin, CollectionMax);
        }

        var array = Array.CreateInstance(elementType, lengths);

        // Loop through each possible index in the multi-dimensional array
        while (true)
        {
            array.SetValue(mockDataGenerator.Create(elementType), indices);

            // Move to the next element in the array (simulate multi-dimensional looping)
            var dimension = rank - 1;
            while (dimension >= 0)
            {
                indices[dimension]++;
                if (indices[dimension] < lengths[dimension])
                {
                    break;
                }

                indices[dimension] = 0;
                dimension--;
            }

            if (dimension < 0)
            {
                break;
            }
        }

        return array;
    }
}
