using System;
using System.Data.SqlClient;

using MDUA.Framework;
using MDUA.Framework.Exceptions;
using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using MDUA.DataAccess;

namespace MDUA.BusinessLogic
{
	/// <summary>
    /// Business logic processing for RecruitmentEligible.
    /// </summary>    
	public partial class RecruitmentEligibleManager
	{
	
		/// <summary>
        /// Update RecruitmentEligible Object.
        /// Data manipulation processing for: new, deleted, updated RecruitmentEligible
        /// </summary>
        /// <param name="recruitmentEligibleObject"></param>
        /// <returns></returns>
        public bool Update(RecruitmentEligible recruitmentEligibleObject)
        {
			bool success = false;
			
			success = UpdateBase(recruitmentEligibleObject);
		
			return success;
        }
		
		/// <summary>
        /// Fill External Childs of RecruitmentEligible Object.
        /// </summary>
        /// <param name="recruitmentEligibleObject"></param>
        /// <returns></returns>
        public void FillChilds(RecruitmentEligible recruitmentEligibleObject)
        {
			///Fill external information of Childs of RecruitmentEligibleObject
        }
	}	
}