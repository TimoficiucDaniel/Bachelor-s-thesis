using System.Numerics;

namespace DefaultNamespace
{
    public class PlateBuilder
    {
        private Plate plate;
        private object lockObject = new object();

        public PlateBuilder()
        {
            this.reset();
        }

        public void reset()
        {
            plate = new Plate();
        }

        public PlateBuilder setId(int id)
        {
            plate.setId(id);
            return this;
        }

        public PlateBuilder addRegion(Vector2 regionCenter)
        {
            lock (lockObject)
            {
                plate.addRegion(regionCenter);
            }

            return this;
        }

        public PlateBuilder setCenter(Vector2 origin)
        {
            plate.setOrigin(origin);
            return this;
        }

        public PlateBuilder setDirection(Vector2 direction)
        {
            plate.setDirection(direction);
            return this;
        }

        public PlateBuilder setForce(float force)
        {
            plate.setForce(force);
            return this;
        }

        public PlateBuilder setType(PlateType type)
        {
            plate.setType(type);
            return this;
        }

        public Plate build()
        {
            return plate;
        }
    }
}