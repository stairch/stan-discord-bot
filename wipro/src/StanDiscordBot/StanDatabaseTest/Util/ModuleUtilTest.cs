using StanDatabase.Util;

namespace StanDatabaseTest.Util
{
    [TestClass]
    public class ModuleUtilTest
    {
        [TestMethod]
        public void TestExtractModuleShortnameWithNumberElement()
        {
            string shortname = "I.BA_111_DIGITECH.16";
            string expected = "DIGITECH";
            Assert.AreEqual(expected, ModuleUtil.ExtractModuleShortname(shortname));
        }

        [TestMethod]
        public void TestExtractModuleShortnameWithNumberAtEnd()
        {
            string shortname = "I.MAWI_AFE2.22";
            string expected = "AFE2";
            Assert.AreEqual(expected, ModuleUtil.ExtractModuleShortname(shortname));
        }

        [TestMethod]
        public void TestExtractModuleShortnameWithNumberAtStart()
        {
            string shortname = "I.BA_3DMOD4RT.21";
            string expected = "3DMOD4RT";
            Assert.AreEqual(expected, ModuleUtil.ExtractModuleShortname(shortname));
        }

        [TestMethod]
        public void TestExtractModuleShortnamewWithPlus()
        {
            string shortname = "I.BA_BAA+INF.16";
            string expected = "BAA+INF";
            Assert.AreEqual(expected, ModuleUtil.ExtractModuleShortname(shortname));
        }

        [TestMethod]
        public void TestExtractModuleShortnameEnglish()
        {
            string shortname = "I.BA_ENGTCS.20";
            string expected = "ENGTCS";
            Assert.AreEqual(expected, ModuleUtil.ExtractModuleShortname(shortname));
        }

        [TestMethod]
        public void TestExtractModuleShortnameMultipleCourses()
        {
            // cut out "6a" since this is the specification when multiple courses are done
            string shortname = "I.BA_FM_6a.19";
            string expected = "FM";
            Assert.AreEqual(expected, ModuleUtil.ExtractModuleShortname(shortname));
        }

        [TestMethod]
        public void TestExtractModuleShortnameMultipleCourses2()
        {
            // cut out "K" since this is the specification when multiple courses are done
            string shortname = "I.BA_GPOR_K.18";
            string expected = "GPOR";
            Assert.AreEqual(expected, ModuleUtil.ExtractModuleShortname(shortname));
        }

        [TestMethod]
        public void TestExtractModuleShortnameMultipleCourses3()
        {
            // cut out "K" since this is the specification when multiple courses are done
            string shortname = "I.BA_HPP_1_3.18";
            string expected = "HPP";
            Assert.AreEqual(expected, ModuleUtil.ExtractModuleShortname(shortname));
        }

        [TestMethod]
        public void TestExtractModuleShortnameIsaAtEnd()
        {
            string shortname = "I.BA_APP_ISA.19";
            string expected = "APP";
            Assert.AreEqual(expected, ModuleUtil.ExtractModuleShortname(shortname));
        }

        [TestMethod]
        public void TestExtractModuleShortnameIsaAtEnd2()
        {
            string shortname = "I.MFD_ISA.22";
            string expected = "MFD";
            Assert.AreEqual(expected, ModuleUtil.ExtractModuleShortname(shortname));
        }

        [TestMethod]
        public void TestExtractModuleShortnameIsaAtStart()
        {
            string shortname = "I.ISA_IGL.17";
            string expected = "IGL";
            Assert.AreEqual(expected, ModuleUtil.ExtractModuleShortname(shortname));
        }

        /// <summary>
        /// EVA stands for "Ergänzenden Veranstaltungen"
        /// </summary>
        [TestMethod]
        public void TestExtractModuleShortnameEVA()
        {
            string shortname = "I.MSE_EVA_STW.19";
            string expected = "STW";
            Assert.AreEqual(expected, ModuleUtil.ExtractModuleShortname(shortname));
        }

        [TestMethod]
        public void TestExtractModuleShortnameEVA2()
        {
            string shortname = "I.MSE_EVA1.18";
            string expected = "EVA1";
            Assert.AreEqual(expected, ModuleUtil.ExtractModuleShortname(shortname));
        }
    }
}
