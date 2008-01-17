using System;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;


namespace ClearCanvas.Enterprise.Authentication {

    // This class is based on code found here:
    // http://msdn.microsoft.com/msdnmag/issues/03/08/SecurityBriefs/

    /// <summary>
    /// Password component
    /// </summary>
	public partial class Password
	{
        private const int saltLength = 6;

        /// <summary>
        /// Creates a new <see cref="Password"/> object from the specified clear-text password string,
        /// and assigns the specified expiry time.
        /// </summary>
        /// <param name="clearTextPassword"></param>
        /// <param name="expiryTime"></param>
        /// <returns></returns>
        public static Password CreatePassword(string clearTextPassword, DateTime? expiryTime)
        {
            Platform.CheckForNullReference(clearTextPassword, "clearTextPassword");

            string salt = CreateSalt();
            string hash = CalculateHash(salt, clearTextPassword);
            return new Password(salt, hash, expiryTime);
        }

        /// <summary>
        /// Verifies whether the specified password string matches this <see cref="Password"/> object.
        /// </summary>
        /// <param name="clearTextPassword"></param>
        /// <returns></returns>
        public bool Verify(string clearTextPassword)
        {
            Platform.CheckForNullReference(clearTextPassword, "clearTextPassword");

            string h = CalculateHash(_salt, clearTextPassword);
            return _saltedHash.Equals(h);
        }

        #region Utilities

        private static string CreateSalt()
        {
            byte[] r = CreateRandomBytes(saltLength);
            return Convert.ToBase64String(r);
        }

        private static byte[] CreateRandomBytes(int len)
        {
            byte[] r = new byte[len];
            new RNGCryptoServiceProvider().GetBytes(r);
            return r;
        }

        private static string CalculateHash(string salt, string password)
        {
            byte[] data = ToByteArray(salt + password);
            byte[] hash = CalculateHash(data);
            return Convert.ToBase64String(hash);
        }

        private static byte[] CalculateHash(byte[] data)
        {
            return new SHA1CryptoServiceProvider().ComputeHash(data);
        }

        private static byte[] ToByteArray(string s)
        {
            return System.Text.Encoding.UTF8.GetBytes(s);
        }

        #endregion


        /// <summary>
        /// Not used.
		/// </summary>
		private void CustomInitialize()
		{
		}
	}
}