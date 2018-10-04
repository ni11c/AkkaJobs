using System.Collections.Generic;

namespace Agridea.Acorda.Agis
{
    public interface ILocalData
    {
        string localFarmId { get; set; }

        string localPersonId { get; set; }
    }

    public static class BlwHelper
    {
        public static T SetLocalData<T>(this T instance, StructureModel model) where T : class, ILocalData
        {
            if (instance == null)
                return null;

            // todo: fix this once model is rewritten
            //instance.localFarmId = model.MainFarm.Ktidb.FormatToKtid(model);
            //instance.localPersonId = model.MainFarm.Person.Ktidp.FormatToKtid(model);
            return instance;
        }

        public static List<T> SetLocalData<T>(this List<T> list, StructureModel model) where T : class, ILocalData
        {
            foreach (var instance in list)
                instance.SetLocalData(model);

            return list;
        }
    }

    public partial class generalSurfaceDataTypeSurface : ILocalData { }

    public partial class animalDataTypeAnimalStock : ILocalData { }

    public partial class animalSummeringDataTypeAnimalStock : ILocalData { }

    public partial class slopeSurfaceDataTypeSlopeSurface : ILocalData { }

    public partial class grapeSurfaceDataTypeGrapeSurface : ILocalData { }

    public partial class networkSurfaceDataTypeNetworkSurface : ILocalData { }

    public partial class biodiversitySurfaceDataTypeBiodiversitySurface : ILocalData { }

    public partial class lowEmissionFertilizationType : ILocalData { }

    public partial class mildSoilTreatmentType : ILocalData { }

    public partial class precisePesticideApplicationType : ILocalData { }

    public partial class countrysideQualityDataTypeCountrysideQualityProject : ILocalData { }

    public partial class aquacultureDataTypeAquaticAnimalData : ILocalData { }

    public partial class apicultureDataTypeFarmApicultureData : ILocalData { }
}