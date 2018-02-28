using System.Collections.Generic;
using Xamarin.Forms.Maps;

namespace test
{
    class CustomMap : Map
    {
        public List<CustomPin> CustomPins { get; set; }
        public void Clear(){}
    }
}
