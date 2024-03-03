using MathNet.Spatial.Euclidean;
using System.Drawing;

namespace Seilroboter.Robot
{
    public interface IDrive
    {
        #region Propertys
        public string Model { get; set; }
        public Point3D Position { get; set; }
        public float RopeLength { get; set; }
        #endregion

       

    }
}