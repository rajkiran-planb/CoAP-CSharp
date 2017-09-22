﻿using System;
using System.Text;
using PeterO.Cbor;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
using NUnit.Framework;
using Com.AugustCellars.COSE;

namespace Com.AugustCellars.CoAP.OSCOAP
{
    [TestClass]
    public class SecurityContext_Test
    {
        private static readonly byte[] _SenderId = Encoding.UTF8.GetBytes("client");
        private static readonly byte[] _RecipientId = Encoding.UTF8.GetBytes("server");
        private static readonly byte[] _Secret = new byte[] {01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x1A, 0x1B, 0x1C, 0x1D, 0x1E, 0x1F, 0x20, 0x21, 0x22, 0x23};
        private static readonly CBORObject _KeyAgreeAlg = AlgorithmValues.ECDH_SS_HKDF_256;
        private static readonly byte[] _Salt = Encoding.UTF8.GetBytes("Salt String");

        [TestMethod]
        public void Derive_CCM()
        {
            SecurityContext ctx = SecurityContext.DeriveContext(_Secret, _SenderId, _RecipientId, null, AlgorithmValues.AES_CCM_64_64_128, _KeyAgreeAlg);

            Assert.AreEqual(ctx.Sender.BaseIV, new byte[]{ 0x01, 0x53, 0xDD, 0xFE, 0xDE, 0x44, 0x19 });
            Assert.AreEqual(ctx.Sender.Key, new byte[] {0x21, 0x64, 0x42, 0xDA, 0x60, 0x3C, 0x51, 0x59, 0x2D, 0xF4, 0xC3, 0xD0, 0xCD, 0x1D, 0x0D, 0x48 });
            Assert.AreEqual(ctx.Recipient.BaseIV, new byte[]{ 0x20, 0x75, 0x0B, 0x95, 0xF9, 0x78, 0xC8 });
            Assert.AreEqual(ctx.Recipient.Key, new byte[]{ 0xD5, 0xCB, 0x37, 0x10, 0x37, 0x15, 0x34, 0xA1, 0xCA, 0x22, 0x4E, 0x19, 0xEB, 0x96, 0xE9, 0x6D });

            SecurityContext ctx2 = SecurityContext.DeriveContext(_Secret, _RecipientId, _SenderId, null, AlgorithmValues.AES_CCM_64_64_128, _KeyAgreeAlg);
            Assert.AreEqual(ctx.Sender.BaseIV, ctx2.Recipient.BaseIV);
            Assert.AreEqual(ctx.Sender.Key, ctx2.Recipient.Key);
        }

        [TestMethod]
        public void Derive_Salt()
        {
            SecurityContext ctx = SecurityContext.DeriveContext(_Secret, _SenderId, _RecipientId, _Salt, AlgorithmValues.AES_CCM_64_64_128, _KeyAgreeAlg);

            Assert.AreEqual(ctx.Sender.BaseIV, new byte[] { 0x35, 0x12, 0x7A, 0x79, 0x77, 0xAD, 0x8C });
            Assert.AreEqual(ctx.Sender.Key, new byte[] { 0xF4, 0x70, 0x23, 0x71, 0x3B, 0x40, 0xA2, 0x61, 0x17, 0xD4, 0xA8, 0x33, 0xF7, 0x70, 0xC3, 0xB0 });
            Assert.AreEqual(ctx.Recipient.BaseIV, new byte[] { 0x33, 0x86, 0xBA, 0x6E, 0x7E, 0x0C, 0x13 });
            Assert.AreEqual(ctx.Recipient.Key, new byte[] {0x09, 0xF6, 0x3F, 0xFB, 0x75, 0xAB, 0x1F, 0x10, 0x1B, 0x2D, 0x41, 0xA6, 0xB2, 0x2D, 0x42, 0x0E});
        }

        [TestMethod]
        public void Derive_Hash512()
        {
            SecurityContext ctx = SecurityContext.DeriveContext(_Secret, _SenderId, _RecipientId, _Salt, AlgorithmValues.AES_CCM_64_64_128, AlgorithmValues.ECDH_SS_HKDF_512);

            Assert.AreEqual(ctx.Sender.BaseIV, new byte[] { 0x5E, 0x99, 0xCA, 0x7A, 0xA3, 0xB1, 0x50 });
            Assert.AreEqual(ctx.Sender.Key, new byte[] { 0xF4, 0x5D, 0xB2, 0x0E, 0xEC, 0x35, 0x95, 0x7E, 0xC6, 0x40, 0x30, 0xF0, 0x0C, 0xE2, 0x7B, 0x7D });
            Assert.AreEqual(ctx.Recipient.BaseIV, new byte[] { 0x69, 0xA1, 0x79, 0xC5, 0xCD, 0x74, 0x43 });
            Assert.AreEqual(ctx.Recipient.Key, new byte[] { 0x79, 0x2C, 0x4F, 0xD9, 0xDE, 0x44, 0xE1, 0x9B, 0xBF, 0xD6, 0xD4, 0x01, 0x1B, 0xB1, 0xB9, 0xCC });
        }

        [TestMethod]
        public void Derive_GCM()
        {
            SecurityContext ctx = SecurityContext.DeriveContext(_Secret, _SenderId, _RecipientId, null, AlgorithmValues.AES_GCM_128, _KeyAgreeAlg);

            Assert.AreEqual(ctx.Sender.BaseIV, new byte[] { 0x6B, 0xE5, 0x0D, 0x26, 0x2D, 0xF4, 0x63 });
            Assert.AreEqual(ctx.Sender.Key, new byte[] { 0xAA, 0x43, 0x2E, 0xA7, 0xF4, 0xC0, 0xAF, 0x8E, 0x1B, 0x0D, 0x82, 0xD0, 0x13, 0x50, 0xC1, 0xCB });
            Assert.AreEqual(ctx.Recipient.BaseIV, new byte[] { 0xB3, 0x02, 0xED, 0xB7, 0xFB, 0xF7, 0x9E });
            Assert.AreEqual(ctx.Recipient.Key, new byte[] { 0x04, 0xCF, 0xD6, 0xF1, 0xE2, 0x64, 0xF4, 0x95, 0x7D, 0xC3, 0xE1, 0x6F, 0x32, 0x09, 0x11, 0x4E });
        }
    }
}
