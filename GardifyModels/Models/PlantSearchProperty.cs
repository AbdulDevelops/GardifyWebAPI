using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GardifyModels.Models
{
    public class PlantSearchPropertyItem
    {
        
        public int id { get; set; }
        public int plantId { get; set; }

        public string nameGerman { get; set; }
        public string nameLatin { get; set; }
        public string Familie { get; set; }
        public string Synonym { get; set; }
        public string TagProperty { get; set; }

        public string HeightProperty { get; set; }

        public string BloomProperty { get; set; }


        public string GroupProperty { get; set; }

    }

    public class PlantSearchPropertyEntry
    {

        public int id { get; set; }

        public string nameGerman { get; set; }
        public string nameLatin { get; set; }
        public string Familie { get; set; }
        public string Synonym { get; set; }

        public string TagProperty { get; set; }

        public string HeightProperty { get; set; }

        public string BloomProperty { get; set; }
        public string GroupProperty { get; set; }


    }

    public class PlantSearchPropertyView
    {
        private const string V = ", ";

        public PlantSearchPropertyView(PlantSearchPropertyItem model)
        {
            id = model.plantId;
            nameGerman = model.nameGerman;
            nameLatin = model.nameLatin;
            Familie = model.Familie;
            Synonym = model.Synonym;
            TagProperty = model.TagProperty;
            HeightProperty = model.HeightProperty;
            BloomProperty = model.BloomProperty;
            GroupProperty = model.GroupProperty;
        }
        public int id { get; set; }

        [ForeignKey("Plant")]

        public virtual Plant Plant { get; set; }
        public string nameGerman { get; set; }
        public byte[] nameGermanNormByte => nameGerman != null ? System.Text.Encoding.GetEncoding("ISO-8859-8").GetBytes(nameGerman) : null;

        public string nameGermanNorm => nameGerman != null ? System.Text.Encoding.UTF8.GetString(nameGermanNormByte) : "";

        public string nameLatin { get; set; }
        public string Familie { get; set; }
        public string Synonym { get; set; }

        public string TagProperty { get; set; }

        //public List<int> TagPropertyList => TagProperty != null ? TagProperty.Split(',').Where(t => t != "").Select(t => int.Parse(t.Trim())).ToList() : null;
        public string HeightProperty { get; set; }
        public double HeightPropertyMin => HeightProperty != null ? double.Parse( HeightProperty.Split(';')[0]) / 100 : 0.0;
        public double HeightPropertyMax => HeightProperty != null ? double.Parse(HeightProperty.Split(';')[1]) / 100 : 0.0;
        public string BloomProperty { get; set; }

        public double BloomPropertyMin => BloomProperty != null ? (double.Parse(BloomProperty.Split(';')[0]) / 100) : 0.0;
        public double BloomPropertyMax => BloomProperty != null ? double.Parse(BloomProperty.Split(';')[1]) / 100 : 0.0;
        public string GroupProperty { get; set; }
        //public int[] GroupPropertyArray => GroupProperty != null ? (GroupProperty.Split(V).Select(n => n != "" ? int.Parse(n) : 0));
        public int[] GroupPropertyArray => GroupProperty != null ? (GroupProperty.Split(',').Select(n => n.Trim() != "" ? int.Parse(n.Trim()) : 0).ToArray()) : null;


    }
}