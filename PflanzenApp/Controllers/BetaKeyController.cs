using Microsoft.AspNet.Identity;
using GardifyModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace PflanzenApp.Controllers
{
    public class BetaKeyController : _BaseController
    {
        #region DB
        [NonAction]
        public void DbAddBetaMember(LoginViewModel model)
        {
            BetaMembers member = new BetaMembers();
			if (User != null)
			{
				member.OnCreate(User.Identity.GetUserName());
			}
			else
			{
				member.OnCreate("SYSTEM");
			}
			member.Deleted = false;
            member.Activated = false;
            member.Beta = model.beta;
            member.Date = DateTime.Now;
            member.Description = "Added by BetaKeyController";
            member.Email = model.alternateEmail;
            member.Notification = model.notification;
            ctx.BetaMember.Add(member);
            ctx.SaveChanges();
        }
        [NonAction]
        public IEnumerable<BetaKeys> DbGetBetaKeys(Guid userid)
        {
            var keys = (from k in ctx.BetaKey
                        where k.OwnerId == userid
                        && k.Used == false
                        select k);
            return keys;
        }
        [NonAction]
        public bool DbCheckBetaKey(string betakey)
        {
            var key = (from k in ctx.BetaKey
                       where k.Key == betakey
                       && !k.Used
                       && !k.Deleted
                       select k).FirstOrDefault();
            if (key != null)
            {
				if(User != null)
				{
					key.OnEdit(User.Identity.GetUserName());
				} else
				{
					key.OnEdit("SYSTEM");
				}
                key.Used = true;
                key.UsedDate = DateTime.Now;
                ctx.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        [NonAction]
        public void DbAddBetaKeys(Guid guid, int keyCount = 5)
        {
            for (int i = 0; i < keyCount; i++) //TODO: Keine festen Werte, festlegen im Adminbereich
            {
                BetaKeys key = new BetaKeys();
                key.Key = DbGetRandomBetaKey();
                key.Activated = true;
                key.Used = false;
                key.UsedDate = null;
                key.OwnerId = guid;
				if (User != null)
				{
					key.OnCreate(User.Identity.GetUserName());
				}
				else
				{
					key.OnCreate("SYSTEM");
				}				
                key.Deleted = false;
                ctx.BetaKey.Add(key);
                ctx.SaveChanges();
            }
        }

        [NonAction]
        public string DbGetRandomBetaKey()
        {
            Guid rand = Guid.NewGuid();
            string randS = rand.ToString();
            return randS.Substring(0, 8);
        }
        #endregion
    }
}