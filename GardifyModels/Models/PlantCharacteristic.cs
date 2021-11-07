namespace GardifyModels.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Linq;

    public partial class PlantCharacteristic : _BaseEntity
    {

        [Required]
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public PlantCharacteristicCategory Category { get; set; }
        [Required]
        [ForeignKey("Plant")]
        public int PlantId { get; set; }
        public Plant Plant { get; set; }
        public decimal Min { get; set; }
        public decimal? Max { get; set; }
        public bool Includes(IEnumerable<PlantCharacteristic> others)
        {
            return others.Any(c => Includes(c));
        }
        public bool Includes(PlantCharacteristic other)
        {
            if(CategoryId == other.CategoryId)
            {
                switch (other.Category.CharacteristicValueType)
                {
                    case ModelEnums.CharacteristicValueType.SingleNumber:
                    case ModelEnums.CharacteristicValueType.SingleMonth:
                    case ModelEnums.CharacteristicValueType.SingleLatinNumber:
                        if (other.Max != null)
                        {
                            if (Min <= other.Min && Max >= other.Max)
                            {
                                String s = "";
                            }
                            return Min <= other.Min && Max >= other.Max;
                        }
                        else
                        {
                            if (Min == other.Min)
                            {
                                String s = "";
                            }
                            return Min == other.Min;
                        }
                    case ModelEnums.CharacteristicValueType.NumberRange:
                    case ModelEnums.CharacteristicValueType.MonthRange:
                    case ModelEnums.CharacteristicValueType.LatinNumberRange:
                        if (Max != null)
                        {
                            if (Min <= other.Min && Max >= other.Max)
                            {
                                String s = "";
                            }
                            return Min <= other.Min && Max >= other.Max;
                        }
                        else
                        {
                            if (Min <= other.Min && Max >= other.Max)
                            {
                                String s = "";
                            }
                            return Min <= other.Min && Max >= other.Max;
                        }
                    default:
                        return false;
                }
            }
            return false;
        }
    }
    public class PlantCharacteristicSimplified
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public decimal Min { get; set; }
        public decimal? Max { get; set; }
        public int PlantId { get; set; }
        public PlantTagCategory Category { get; set; }
        public string CategoryTitle { get; set; }
        public int? PlantTagCategoryId { get; set; }
        public PlantCharacteristicSimplified(PlantCharacteristic plantCharacteristic)
        {
            Id = plantCharacteristic.Id;
            CategoryId = plantCharacteristic.CategoryId;
            Max = plantCharacteristic.Max;
            Min = plantCharacteristic.Min;
            PlantId = plantCharacteristic.PlantId;
            Category =  plantCharacteristic.Category.PlantTagCategory;
            CategoryTitle = plantCharacteristic.Category.Title;
            PlantTagCategoryId = plantCharacteristic.Category.PlantTagCategoryId;
        }
    }
}
