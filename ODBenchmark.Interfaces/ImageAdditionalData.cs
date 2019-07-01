using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODBenchmark.Interfaces
{
    public class ImageAdditionalData
    {
        public float WorldModelRotation { get; set; }
        public float WorldModelVerticalTilt { get; set; }
        public float Tilt { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public string FileName { get; set; }

        public void NormalizeTilts()
        {
            WorldModelRotation = WorldModelRotation / 180.0f;
            WorldModelVerticalTilt = WorldModelVerticalTilt / 180.0f;
            Tilt = Tilt / 180.0f;
        }

        public ImageAdditionalData Copy()
        {
            return new ImageAdditionalData
            {
                WorldModelRotation = this.WorldModelRotation,
                WorldModelVerticalTilt = this.WorldModelVerticalTilt,
                Tilt = this.Tilt,
                Latitude = this.Latitude,
                Longitude = this.Longitude,
                FileName = this.FileName
            };
        }
    }
}
