using FluentAssertions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Tests {
	public class LinqTests {
		[Fact]
		public void ClosureCanBeUsedForStreams() {
			const string plainText = "The quick brown fox jumps over the lazy dog.";
			const string password = "Lorem ipsum";

			string cipherText = from plainTextBytes in Encoding.UTF8.GetBytes(plainText).ToOne()
								let keySize = 128
								let rng = new RNGCryptoServiceProvider()
								let saltBytes = new byte[16].Also(it => rng.GetBytes(it))
								let ivBytes = new byte[16].Also(it => rng.GetBytes(it))
								let passwordDeriveBytes = new Rfc2898DeriveBytes(password, saltBytes, iterations: 1000)
								let passwordBytes = passwordDeriveBytes.GetBytes(keySize / 8)
								let rijndael = new RijndaelManaged {
									BlockSize = keySize,
									Mode = CipherMode.CBC,
									Padding = PaddingMode.PKCS7
								}
								let encryptor = rijndael.CreateEncryptor(passwordBytes, ivBytes)
								let memoryStream = new MemoryStream()
								let cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write)
								select cryptoStream.Let(cryptoStream => {
									cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
									cryptoStream.FlushFinalBlock();
									var cipherTextBytes = saltBytes.Concat(ivBytes).Concat(memoryStream.ToArray()).ToArray();
									return Convert.ToBase64String(cipherTextBytes);
 								});

			string decryptedText = from saltedCipherTextBytes in Convert.FromBase64String(cipherText).ToOne()
								   let keySize = 128
								   let saltBytes = saltedCipherTextBytes.Take(keySize / 8).ToArray()
								   let ivBytes = saltedCipherTextBytes.Skip(keySize / 8).Take(keySize / 8).ToArray()
								   let cipherTextBytes = saltedCipherTextBytes.Skip((keySize / 8) * 2).ToArray()
								   let passwordDeriveBytes = new Rfc2898DeriveBytes(password, saltBytes, iterations: 1000)
								   let passwordBytes = passwordDeriveBytes.GetBytes(keySize / 8)
								   let rijndael = new RijndaelManaged {
									   BlockSize = keySize,
									   Mode = CipherMode.CBC,
									   Padding = PaddingMode.PKCS7
								   }
								   let decryptor = rijndael.CreateDecryptor(passwordBytes, ivBytes)
								   let memoryStream = new MemoryStream(cipherTextBytes)
								   let cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read)
								   select cryptoStream.Let(cryptoStream => {
									   var plainTextBytes = new byte[cipherTextBytes.Length];
									   var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
									   return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
								   });

			decryptedText.Should().Be(plainText);
		}

		[Fact]
		public async void ClosureCanHaveMultipleAwaits() {
			string s1 = "HeLLo wOrld";
			string s2 = "hEllo World";

			bool areEqual = from normalizedS1 in (await ToUpperAsync(s1)).ToOne()
							join normalizedS2 in (await ToUpperAsync(s2)).ToOne() on 1 equals 1
							select normalizedS1 == normalizedS2;
			areEqual.Should().BeTrue();
		}

		private static Task<string> ToUpperAsync(string s) {
			return Task.FromResult(s.ToUpper());
		}
	}
}
