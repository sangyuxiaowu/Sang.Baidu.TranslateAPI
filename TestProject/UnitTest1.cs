using Sang.Baidu.TranslateAPI;

namespace TestProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task TestOk()
        {
            BaiduTranslator translator = new BaiduTranslator("2015063000000001", "????");
            var result = await translator.Translate("apple", "zh","en", "1435660288", "f89f9594663708c1605f3d736d01d2d4");
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.AreEqual("Æ»¹û", result.Trans_Result[0].Dst);
        }
        [TestMethod]
        public async Task TestErr()
        {
            BaiduTranslator translator = new BaiduTranslator("2015063000000001", "????");
            var result = await translator.Translate("apple", "zh");
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.Error_Code);
            Console.WriteLine(result.Error_Msg);
        }

        [TestMethod]
        public async Task TestErr2()
        {
            BaiduTranslator translator = new BaiduTranslator("2015063000000001", "????","http://baidu.fy");
            var result = await translator.Translate("apple", "zh");
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Success);
            Console.WriteLine(result.Error_Code);
            Assert.IsNotNull(result.Error_Code);
            Console.WriteLine(result.Error_Msg);
        }
    }
}