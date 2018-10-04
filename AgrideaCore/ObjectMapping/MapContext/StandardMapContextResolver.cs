
namespace Agridea.ObjectMapping
{
    public class StandardMapContextResolver : IMapContextResolver
    {
        private MapContext mapContext_;

        public StandardMapContextResolver()
        {
            mapContext_ = new MapContext();
        }
        public MapContext Context { get { return mapContext_; } }
    }
}
