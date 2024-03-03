using MathNet.Spatial.Euclidean;

namespace Seilroboter.Robot
{
    public interface IRobot
    {
        #region Propertys

        public List<IDrive> Drives { get; set; }
        public Point3D Position { get; set; }

        #endregion

       

    }
}
