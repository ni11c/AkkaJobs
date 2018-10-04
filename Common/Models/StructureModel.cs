using System;
using System.Collections.Generic;
using System.Linq;
using Agridea.Core;

namespace Agridea.Acorda.Agis
{
    public class StructureModel
    {
        public FarmModel MainFarm { get; set; }
        public IList<int> AllFarmIds { get; set; }
        public string CantonCode { get; set; }
        public decimal VergAndSchafNorm { get; set; }
        public int ConstRedForm { get; set; }
        public decimal ConstRedProf { get; set; }
        public decimal ConstPrimeArbreminus20 { get; set; }
        public object Administrator { get; set; }
        public IList<CbeObjetModel> CbeObjetList { get; set; }
        public IList<BdQualityModel> SurfaceQualityList { get; set; }
        public IList<BdQualityModel> TreeQualityList { get; set; }
        public IList<BdNetworkModel> SurfaceNetworkList { get; set; }
        public IList<BdNetworkModel> TreeNetworkList { get; set; }
        public IList<BdSummeringQualityModel> SummeringQualityList { get; set; }
        public IList<ParcelModel> ParcelList { get; set; }
        public IList<AnimalModel> AnimalList { get; set; }
        public IList<AnimalTvdModel> AnimalTvdList { get; set; }
        public ReductionProfessionnelleModel ReductionProfessionnelle { get; set; }
        public PaiementAnneePrecedenteModel PaiementAnneePrecedente { get; set; }
        public IList<PreviousYearAmountModel> PreviousYearAmountList { get; set; }
        public IList<ReductionModel> ReductionList { get; set; }
        public IList<ReductionCantonaleModel> ReductionCantonaleList { get; set; }
        public IList<EthoProgramModel> EthoProgramList { get; set; }
        public IList<AgriculturalZoneModel> AgriculturalZoneList { get; set; }
        public IList<CerTechniqueModel> CerTechniqueList { get; set; }
        public IList<ExtensoCategoryModel> ExtensoCategoryList { get; set; }
        public IList<AnimalHobbyModel> AnimalHobbyList { get; set; }
        public IList<ParcelHobbyModel> ParcelHobbyList { get; set; }
        public IList<EstiAnimalModel> EstiAnimalList { get; set; }
        public IList<EstiAnimalTvdModel> EstiAnimalTvdList { get; set; }
        public IList<CerMildSoilTreatmentModel> CerMildSoilTreatmentList { get; set; }
        public IList<CerLowEmissionFertilizationModel> CerLowEmissionFertilizationList { get; set; }
        public IList<CqpParticipationModel> CqpParticipationList { get; set; }
        public IList<CqpCountrysideQualityProjectModel> CqpCountrysideQualityProjectList { get; set; }
        public IList<FishModel> FishList { get; set; }
        public IList<BeesModel> BeesList { get; set; }

        public StructureModel()
        {
            CbeObjetList = new List<CbeObjetModel>();
            SurfaceQualityList = new List<BdQualityModel>();
            TreeQualityList = new List<BdQualityModel>();
            SummeringQualityList = new List<BdSummeringQualityModel>();
            SurfaceNetworkList = new List<BdNetworkModel>();
            TreeNetworkList = new List<BdNetworkModel>();
            ParcelList = new List<ParcelModel>();
            AnimalList = new List<AnimalModel>();
            AnimalTvdList = new List<AnimalTvdModel>();
            ReductionList = new List<ReductionModel>();
            EthoProgramList = new List<EthoProgramModel>();
            AnimalHobbyList = new List<AnimalHobbyModel>();
            ParcelHobbyList = new List<ParcelHobbyModel>();
            EstiAnimalList = new List<EstiAnimalModel>();
            EstiAnimalTvdList = new List<EstiAnimalTvdModel>();
            CerMildSoilTreatmentList = new List<CerMildSoilTreatmentModel>();
            CerLowEmissionFertilizationList = new List<CerLowEmissionFertilizationModel>();
            CqpParticipationList = new List<CqpParticipationModel>();
            CqpCountrysideQualityProjectList = new List<CqpCountrysideQualityProjectModel>();
            AgriculturalZoneList = new List<AgriculturalZoneModel>();
            CerTechniqueList = new List<CerTechniqueModel>();
            ExtensoCategoryList = new List<ExtensoCategoryModel>();
        }

        public List<animalDataTypeAnimalStock> BindAnimals()
        {
            var list = new List<animalDataTypeAnimalStock>();
            list.AddRange(AnimalList.Select(x => x.Bind().SetLocalData(this)));
            list.AddRange(AnimalTvdList.Select(x => x.Bind().SetLocalData(this)));
            list.AddRange(AnimalHobbyList.Select(x => x.Bind().SetLocalData(this)));
            return list;
        }

        #region Subclasses

        public class FarmModel
        {
            public int Id { get; set; }
            public int EstiParametreEstiModeGardeMoutonCode { get; set; }
        }

        public class CbeObjetModel
        {
        }

        public class BdQualityModel
        {
        }

        public class BdNetworkModel
        {
        }

        public class BdSummeringQualityModel
        {
        }

        public class ParcelModel
        {
        }

        public class AnimalModel
        {
            public int Id { get; set; }
            public int AverageQuantity { get; set; }
            public int DeclarationDayQuantity { get; set; }
            public double DeterminantUgbfg { get; set; }
            public int SummeringPreviousYearDuration { get; set; }
            public int SummeringPreviousYearQuantity { get; set; }
            public string FarmKtidb { get; set; }
            public int AnimalTypeCode { get; set; }
            public bool AnimalTypeNeedsPigsStablingMethod { get; set; }
            public bool AnimalTypeNeedsPoultryStablingMethod { get; set; }
            public bool AnimalTypeNeedsStablingMethod
            {
                get { return AnimalTypeNeedsPoultryStablingMethod || AnimalTypeNeedsPigsStablingMethod; }
            }
            public DateTime CreationDate { get; set; }
            public DateTime? ModificationDate { get; set; }
            public DateTime LatestChangeDate
            {
                get { return ModificationDate ?? CreationDate; }
            }
            public int PigsStablingMethodCode { get; set; }
            public int PoultryStablingMethodCode { get; set; }

            public animalDataTypeAnimalStock Bind()
            {
                if (AverageQuantity == 0
                    && DeclarationDayQuantity == 0
                    && SummeringPreviousYearDuration == 0
                    && SummeringPreviousYearQuantity == 0)
                    return null;

                var target = new animalDataTypeAnimalStock
                {
                    animalCategory = AnimalTypeCode.FormatCodeToItemEnum<animalCategoryType>(),
                    surveyFarmId = FarmKtidb,
                    creationDate = LatestChangeDate,
                };
                var selfReportedData = new selfReportedDataType
                {
                    averageQuantity = AverageQuantity.ToString(),
                    declarationDayQuantity = DeclarationDayQuantity.ToString(),
                    keepingTypeSpecified = AnimalTypeNeedsStablingMethod,
                    summeringDuration = SummeringPreviousYearDuration.ToString(),
                    summeringQuantity = SummeringPreviousYearQuantity.ToString(),
                };
                if (selfReportedData.keepingTypeSpecified)
                {
                    if ((AnimalTypeNeedsPigsStablingMethod && PigsStablingMethodCode == default(int)) ||
                        (AnimalTypeNeedsPoultryStablingMethod && PoultryStablingMethodCode == default(int)))
                    {
                        Log.Warning("Farm {0} needs {1} keeping type but has none", FarmKtidb,
                           AnimalTypeNeedsPigsStablingMethod ? "pigs" :
                           AnimalTypeNeedsPoultryStablingMethod ? "poultry" :
                           "no"
                       );
                    }

                    var keepingCode = AnimalTypeNeedsPigsStablingMethod ? PigsStablingMethodCode : PoultryStablingMethodCode;

                    selfReportedData.keepingType = keepingCode.FormatCodeToItemEnum(animalKeepingType.Item0, "0");
                }
                target.Items = new List<object>(new[] { selfReportedData });
                return target;
            }
        }

        public class AnimalTvdModel
        {
            public int Id { get; set; }
            public int DoDeclarationDayQuantity { get; set; }
            public int DoDurationSum { get; set; }
            public int DoDurationSumTotal { get; set; }
            public decimal DoLiveStockUnit { get; set; }
            public decimal DoLiveStockUnitTotal { get; set; }
            public decimal DoQuantityTotal { get; set; }
            public int DoSaDurationSum { get; set; }
            public decimal DoSaLiveStockUnit { get; set; }
            public int DoSiDurationSum { get; set; }
            public decimal DoSiLiveStockUnit { get; set; }
            public decimal DoSiNormedLiveStockUnit { get; set; }
            public int FtDeclarationDayQuantity { get; set; }
            public decimal FtLiveStockUnit { get; set; }
            public decimal FtLiveStockUnitTotal { get; set; }
            public decimal FtQuantity { get; set; }
            public decimal FtQuantityTotal { get; set; }
            public decimal FtSaLiveStockUnit { get; set; }
            public decimal FtSiLiveStockUnit { get; set; }
            public decimal FtSiNormedLiveStockUnit { get; set; }
            public string FarmKtidb { get; set; }
            public int AnimalTypeCode { get; set; }
            public DateTime CreationDate { get; set; }
            public DateTime? ModificationDate { get; set; }
            public DateTime LatestChangeDate
            {
                get { return ModificationDate ?? CreationDate; }
            }

            public animalDataTypeAnimalStock Bind()
            {
                var target = new animalDataTypeAnimalStock
                {
                    animalCategory = AnimalTypeCode.FormatCodeToItemEnum<animalCategoryType>(),
                    surveyFarmId = FarmKtidb,
                    creationDate = LatestChangeDate,
                };
                var atdData = new atdDataType
                {
                    atdDataOriginal = new atdDataOriginalType
                    {
                        durationSumTotal = DoDurationSumTotal.ToString(),
                        livestockUnitTotal = DoLiveStockUnitTotal,
                        quantityTotal = DoQuantityTotal,
                        surveyFarmDetails = new atdDataOriginalTypeSurveyFarmDetails
                        {
                            declarationDayQuantity = DoDeclarationDayQuantity.ToString(),
                            durationSum = DoDurationSum.ToString(),
                            livestockUnit = DoLiveStockUnit
                        },
                        summeringInlandDetails = new atdDataOriginalTypeSummeringInlandDetails
                        {
                            durationSum = DoSiDurationSum.ToString(),
                            livestockUnit = DoSiLiveStockUnit,
                            normedLivestockUnit = DoSiNormedLiveStockUnit
                        },
                        summeringAbroadDetails = new atdDataOriginalTypeSummeringAbroadDetails
                        {
                            durationSum = DoSaDurationSum.ToString(),
                            livestockUnit = DoSaLiveStockUnit
                        }
                    },
                    atdDataFinal = new atdDataFinalType
                    {
                        livestockUnitTotal = FtLiveStockUnitTotal,
                        quantityTotal = FtQuantityTotal,
                        surveyFarmDetails = new atdDataFinalTypeSurveyFarmDetails
                        {
                            declarationDayQuantity = FtDeclarationDayQuantity.ToString(),
                            quantity = FtQuantity,
                            livestockUnit = FtLiveStockUnit
                        },
                        summeringInlandDetails = new atdDataFinalTypeSummeringInlandDetails
                        {
                            livestockUnit = FtSiLiveStockUnit,
                            normedLivestockUnit = FtSiNormedLiveStockUnit
                        },
                        summeringAbroadDetails = new atdDataFinalTypeSummeringAbroadDetails
                        {
                            livestockUnit = FtSaLiveStockUnit
                        }
                    }
                };
                target.Items = new List<object>(new[] { atdData });
                return target;
            }
        }

        public class ReductionProfessionnelleModel
        {
        }

        public class PaiementAnneePrecedenteModel
        {
        }

        public class PreviousYearAmountModel
        {
        }

        public class ReductionModel
        {
        }

        public class ReductionCantonaleModel
        {
        }

        public class EthoProgramModel
        {
        }

        public class AgriculturalZoneModel
        {
        }

        public class CerTechniqueModel
        {
        }

        public class ExtensoCategoryModel
        {
        }

        public class AnimalHobbyModel
        {
            public int Id { get; set; }
            public int DeclarationDayQuantity { get; set; }
            public int SummeringPreviousYearQuantity { get; set; }
            public string FarmKtidb { get; set; }
            public int AnimalTypeCode { get; set; }
            public DateTime CreationDate { get; set; }
            public DateTime? ModificationDate { get; set; }
            public DateTime LatestChangeDate
            {
                get { return ModificationDate ?? CreationDate; }
            }

            public animalDataTypeAnimalStock Bind()
            {
                var target = new animalDataTypeAnimalStock
                {
                    animalCategory = AnimalTypeCode.CheckAnimalCodes().FormatCodeToItemEnum<animalCategoryType>(),
                    surveyFarmId = FarmKtidb,
                    creationDate = LatestChangeDate,
                };
                var selfReportedData = new selfReportedDataType
                {
                    averageQuantity = DeclarationDayQuantity.ToString(),
                    declarationDayQuantity = DeclarationDayQuantity.ToString(),
                    keepingTypeSpecified = false,
                    summeringDuration = "0",
                    summeringQuantity = SummeringPreviousYearQuantity.ToString()
                };

                target.Items = new List<object>(new[] { selfReportedData });
                return target;
            }
        }

        public class ParcelHobbyModel
        {
        }

        public class EstiAnimalModel
        {
            public int Id { get; set; }
            public int Quantity { get; set; }
            public int SummeringDuration { get; set; }
            public string FarmKtidb { get; set; }
            public string FarmKtidbAndName { get; set; }
            public string FarmPersonKtidp { get; set; }
            public int AnimalTypeCode { get; set; }
            public bool AnimalTypeIsSheep { get; set; }
            public DateTime CreationDate { get; set; }
            public DateTime? ModificationDate { get; set; }
            public DateTime LatestChangeDate
            {
                get { return ModificationDate ?? CreationDate; }
            }

            public animalSummeringDataTypeAnimalStock Bind(int sheepKeepingTypeCode)
            {
                var target = new animalSummeringDataTypeAnimalStock
                {
                    animalCategory = AnimalTypeCode.FormatCodeToItemEnum<animalCategoryType>(),
                    creationDate = LatestChangeDate,
                    localFarmId = FarmKtidb,
                    localPersonId = FarmPersonKtidp
                };
                var selfReportedData = new selfReportedSummeringDataType
                {
                    quantity = Quantity.ToString(),
                    summeringDuration = SummeringDuration.ToString(),
                    summeringTypeSpecified = sheepKeepingTypeCode != default(int)
                };
                if (sheepKeepingTypeCode != default(int))
                {
                    if (!AnimalTypeIsSheep && sheepKeepingTypeCode.In(1, 2, 3))
                    {
                        Log.Warning("EstiAnimal type {0} Farm {1}: summering type {2} is only available for sheep.", AnimalTypeCode, FarmKtidbAndName, sheepKeepingTypeCode);
                        sheepKeepingTypeCode = 0;
                    }
                    selfReportedData.summeringType = sheepKeepingTypeCode.FormatCodeToItemEnum<summeringTypeType>(codeFormat: "0");
                }

                target.Items = new List<object>(new[] { selfReportedData });
                return target;
            }
        }

        public class EstiAnimalTvdModel
        {
        }

        public class CerMildSoilTreatmentModel
        {
        }

        public class CerLowEmissionFertilizationModel
        {
        }

        public class CqpParticipationModel
        {
        }

        public class CqpCountrysideQualityProjectModel
        {
        }

        public class FishModel
        {
        }

        public class BeesModel
        {
        }

        #endregion
    }
}