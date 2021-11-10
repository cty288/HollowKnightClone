using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using MikroFramework.DatabaseKit.NHibernate;
using MikroNHibernateCore;
using UnityEditor;

namespace MikroFramework.NHibernate.Examples
{
    /// <summary>
    /// This class extends function for NHibernateTableManager<User>, which is the User Table¡£
    /// If you want more functions other than those in the NHibernateTableManager for a specific table,
    /// you can also create your own extension classes like the one below
    /// </summary>
    public static class UserTableManagerExtension {
        /// <summary>
        /// Search the User object from the database, given the username. (Null of not found)
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public static async Task<User> SearchUsername(this NHibernateTableManager<User> table, 
            string username) {
            return await table.SearchByFieldNameUniqueResult("Username", username);
        }

        /// <summary>
        /// Search the User object from the database, given the playfabid. (Null of not found)
        /// </summary>
        /// <param name="playfabid"></param>
        /// <returns></returns>
        public static async Task<User> SearchPlayfabid(this NHibernateTableManager<User> table,
            string playfabid) {
            return await table.SearchByFieldNameUniqueResult("Playfabid", playfabid);
        }

        /// <summary>
        /// Authenticate the username, playfabid, and password of a player. Add them to the database if not found.
        /// Return the added User database object. Return null if username and playfabid do not match the same user.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="playfabid"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static async Task<User> AuthenticateUsernamePlayfabid(this NHibernateTableManager<User> table,
            string username, string playfabid,string password) {
            if (await table.SearchUsername(username) == null && await table.SearchPlayfabid(playfabid) == null) {
                //new User
                await table.Add(new User() {Username = username, Playfabid = playfabid, Password = password, LastLoginTime = DateTime.Now});
                return await table.SearchUsername(username);
            }


            User oldUserSearchResult = await table.SearchByFieldNamesUniqueResult(new string[] {"Username", "Playfabid"}, new[] {username, playfabid});
            if (oldUserSearchResult == null) {
                return null;
            }

            oldUserSearchResult.Password = password;
            oldUserSearchResult.LastLoginTime=DateTime.Now;

            await table.Update(oldUserSearchResult);
            return oldUserSearchResult;
        }
    }

}
