using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace DefaultNamespace
{
    public enum PlateType
    {
        HARD,
        SOFT,
        OCEANIC
    }

    public class Plate
    {
        private int id;
        private Vector2 origin;
        private List<Vector2> regions = new List<Vector2>();
        private List<Vector2> edgeRing = new List<Vector2>();
        private List<Vector2> innerEdgeRing = new List<Vector2>();
        private Dictionary<Vector2, List<Vector2>> allPointsByRegions = new Dictionary<Vector2, List<Vector2>>();
        private Vector2 direction;
        private float force;
        private List<Plate> neighborPlates;
        private PlateType type;

        public Plate()
        {
        }
        
        public Dictionary<Vector2, List<Vector2>> AllPointsByRegions => allPointsByRegions;

        public Vector2 getOrigin()
        {
            return this.origin;
        }

        public void addRegion(Vector2 regionCenter)
        {
            regions.Add(regionCenter);
        }

        public int getId()
        {
            return id;
        }

        public List<Vector2> getAllPoints()
        {
            return regions;
        }

        public void setId(int id)
        {
            this.id = id;
        }

        public void setOrigin(Vector2 origin)
        {
            this.origin = origin;
        }

        public void setDirection(Vector2 direction)
        {
            this.direction = direction;
        }

        public void setForce(float force)
        {
            this.force = force;
        }

        public float getForce()
        {
            return force;
        }

        public void setType(PlateType type)
        {
            this.type = type;
        }

        public void setEdgeRing(List<Vector2> edgeRing)
        {
            this.edgeRing = edgeRing;
        }
        
        public List<Vector2> getEdgeRing()
        {
            return this.edgeRing;
        }
        
        public void setInnerEdgeRing(List<Vector2> InnerEdgeRing)
        {
            this.innerEdgeRing = innerEdgeRing;
        }
        
        public List<Vector2> getInnerEdgeRing()
        {
            return this.innerEdgeRing;
        }
        
        public void setNeighbors(List<Plate> plates)
        {
            this.neighborPlates = plates;
        }

        public List<Plate> getNeighbors()
        {
            return this.neighborPlates;
        }

        public Vector2 getDirection()
        {
            return this.direction;
        }
    }
}