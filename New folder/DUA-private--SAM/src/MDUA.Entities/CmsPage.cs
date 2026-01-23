using MDUA.Entities.Bases;
using MDUA.Entities.List;
using MDUA.Framework;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace MDUA.Entities
{
	public partial class CmsPage 
	{
        [NotMapped]
        public List<CmsAsset> CssAssets { get; set; } = new List<CmsAsset>();

        [NotMapped]
        public List<CmsAsset> JsAssets { get; set; } = new List<CmsAsset>();
        [NotMapped]
        public List<CmsAsset> ImageAssets { get; set; } = new List<CmsAsset>();
    }
}